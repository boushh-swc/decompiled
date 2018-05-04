using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Controls
{
	public class CountdownControl : IEventObserver, IViewClockTimeObserver
	{
		private UXLabel label;

		private UXSlider progressBar;

		private string localizedFormat;

		private Color originalLabelColor;

		private int deadline;

		private string deadlineText;

		private int totalTime;

		private KeyValuePair<int, Color> threshold;

		private int offset;

		public CountdownControl(UXLabel label, string localizedFormat)
		{
			this.init(label, localizedFormat);
		}

		public CountdownControl(UXLabel label, string localizedFormat, int deadline)
		{
			this.init(label, localizedFormat);
			this.SetDeadline(deadline);
		}

		public CountdownControl(UXSlider pBar, UXLabel label, string localizedFormat, int deadline, int totalTime, string deadlineText)
		{
			this.init(label, localizedFormat);
			if (pBar != null)
			{
				this.progressBar = pBar;
				this.progressBar.SendDestroyEvent = true;
			}
			this.totalTime = totalTime;
			this.deadlineText = deadlineText;
			this.SetDeadline(deadline);
		}

		private void init(UXLabel label, string localizedFormat)
		{
			this.localizedFormat = localizedFormat;
			this.label = label;
			this.originalLabelColor = label.TextColor;
			this.label.SendDestroyEvent = true;
			Service.EventManager.RegisterObserver(this, EventId.ElementDestroyed);
		}

		public void SetOffsetMinutes(int minutesOffset)
		{
			this.offset = minutesOffset * 60;
		}

		public void SetOffsetSeconds(int secondsOffset)
		{
			this.offset = secondsOffset;
		}

		public void OnViewClockTime(float dt)
		{
			int num = (int)(Service.ServerAPI.ServerTime + (uint)this.offset);
			int num2 = Math.Max(0, this.deadline - num);
			if (this.label != null)
			{
				string timeLabelFromSeconds = GameUtils.GetTimeLabelFromSeconds(num2);
				this.label.Text = string.Format(this.localizedFormat, timeLabelFromSeconds);
				this.label.TextColor = ((num2 >= this.threshold.Key) ? this.originalLabelColor : this.threshold.Value);
			}
			if (this.progressBar != null && this.totalTime > 0 && this.totalTime > num2)
			{
				this.progressBar.Value = 1f * (float)(this.totalTime - num2) / (float)this.totalTime;
			}
			if (num2 <= 0)
			{
				if (this.deadlineText != null)
				{
					this.label.Text = this.deadlineText;
				}
				this.Destroy();
			}
		}

		public void SetThreshold(int thresholdSeconds, Color color)
		{
			this.threshold = new KeyValuePair<int, Color>(thresholdSeconds, color);
		}

		public void SetDeadline(int deadline)
		{
			this.deadline = deadline;
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (this.label != null && cookie is UXLabel && (UXLabel)cookie == this.label)
			{
				this.Destroy();
			}
			else if (this.progressBar != null && cookie is UXSlider && (UXSlider)cookie == this.progressBar)
			{
				this.Destroy();
			}
			return EatResponse.NotEaten;
		}

		public void Destroy()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			Service.EventManager.UnregisterObserver(this, EventId.ElementDestroyed);
			this.label = null;
			this.progressBar = null;
			this.deadline = 0;
			this.totalTime = 0;
			this.localizedFormat = null;
		}
	}
}
