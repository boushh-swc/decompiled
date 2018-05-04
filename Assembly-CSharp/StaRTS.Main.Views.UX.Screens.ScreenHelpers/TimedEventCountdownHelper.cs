using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views.UX.Screens.ScreenHelpers
{
	public class TimedEventCountdownHelper : IViewClockTimeObserver
	{
		private const string CAMPAIGN_BEGINS = "CAMPAIGN_BEGINS_IN";

		private const string CAMPAIGN_ENDS = "CAMPAIGN_ENDS_IN";

		private const string REWARD_DURATION = "REWARD_DURATION";

		private UXLabel label;

		private Lang lang;

		private string timerColor;

		public ITimedEventVO Campaign
		{
			get;
			set;
		}

		public string TimerColor
		{
			get
			{
				return this.timerColor;
			}
			set
			{
				this.timerColor = value;
				if (this.Campaign != null)
				{
					this.UpdateTimeRemaining();
				}
			}
		}

		public TimedEventCountdownHelper(UXLabel label, ITimedEventVO campaign)
		{
			this.label = label;
			this.Campaign = campaign;
			this.lang = Service.Lang;
			Service.ViewTimeEngine.RegisterClockTimeObserver(this, 1f);
			if (this.Campaign != null)
			{
				this.UpdateTimeRemaining();
			}
		}

		public void OnViewClockTime(float dt)
		{
			if (this.Campaign == null)
			{
				return;
			}
			this.UpdateTimeRemaining();
		}

		private void UpdateTimeRemaining()
		{
			TimedEventState state = TimedEventUtils.GetState(this.Campaign);
			string text = string.Empty;
			if (state != TimedEventState.Upcoming)
			{
				if (state != TimedEventState.Live)
				{
					if (state == TimedEventState.Closing)
					{
						int num = TimedEventUtils.GetStoreSecondsRemaining(this.Campaign);
						string text2 = LangUtils.FormatTime((long)num);
						text = this.lang.Get("REWARD_DURATION", new object[]
						{
							text2
						});
					}
				}
				else
				{
					int num = TimedEventUtils.GetSecondsRemaining(this.Campaign);
					string text2 = LangUtils.FormatTime((long)num);
					text = this.lang.Get("CAMPAIGN_ENDS_IN", new object[]
					{
						text2
					});
				}
			}
			else
			{
				int num = TimedEventUtils.GetSecondsRemaining(this.Campaign);
				string text2 = LangUtils.FormatTime((long)num);
				text = this.lang.Get("CAMPAIGN_BEGINS_IN", new object[]
				{
					text2
				});
			}
			if (!string.IsNullOrEmpty(text))
			{
				this.label.Text = UXUtils.WrapTextInColor(text, this.timerColor);
			}
		}

		public void Destroy()
		{
			Service.ViewTimeEngine.UnregisterClockTimeObserver(this);
			this.label = null;
			this.Campaign = null;
			this.lang = null;
		}
	}
}
