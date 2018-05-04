using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX
{
	public class LabelFader : IViewFrameTimeObserver
	{
		private float remainingTime;

		private UXLabel label;

		private UXFactory uxFactory;

		private float fadeTime;

		private LabelFaderCompleteDelegate onComplete;

		private UXElement objectToDestroy;

		private Color tempColor;

		public int LineCount
		{
			get;
			private set;
		}

		public UXLabel Label
		{
			get
			{
				return this.label;
			}
		}

		public LabelFader(UXLabel label, UXFactory uxFactory, float showTime, float fadeTime, LabelFaderCompleteDelegate onComplete, int lineCount, UXElement objectToDestroy)
		{
			this.remainingTime = showTime + fadeTime;
			this.label = label;
			this.uxFactory = uxFactory;
			this.fadeTime = fadeTime;
			this.onComplete = onComplete;
			this.objectToDestroy = objectToDestroy;
			this.LineCount = lineCount;
			if (fadeTime > 0f || showTime > 0f)
			{
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
		}

		public void OnViewFrameTime(float dt)
		{
			this.remainingTime -= dt;
			if (this.remainingTime <= 0f || this.label == null)
			{
				this.Destroy();
			}
			else if (this.remainingTime < this.fadeTime)
			{
				this.tempColor = this.label.TextColor;
				this.tempColor.a = this.remainingTime / this.fadeTime;
				this.label.TextColor = this.tempColor;
			}
		}

		public void MoveUp(int linesToMove)
		{
			Vector3 localPosition = this.label.LocalPosition;
			if (localPosition.y > (float)Screen.height * 0.5f)
			{
				this.Destroy();
			}
			else
			{
				localPosition.y += this.label.LineHeight * (float)linesToMove;
				this.label.LocalPosition = localPosition;
			}
		}

		public void Destroy()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			if (this.uxFactory != null)
			{
				this.uxFactory.DestroyElement(this.objectToDestroy);
			}
			if (this.onComplete != null)
			{
				this.onComplete(this);
			}
			this.onComplete = null;
			this.objectToDestroy = null;
			this.uxFactory = null;
			this.label = null;
		}
	}
}
