using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Elements
{
	public class UXTween
	{
		public UIPlayTween NGUIPlayTween
		{
			get;
			private set;
		}

		public bool Enable
		{
			get
			{
				return !(this.NGUIPlayTween == null) && this.NGUIPlayTween.enabled;
			}
			set
			{
				if (this.NGUIPlayTween != null)
				{
					this.NGUIPlayTween.enabled = value;
				}
			}
		}

		public void Init(GameObject uiRoot)
		{
			if (uiRoot != null)
			{
				this.NGUIPlayTween = uiRoot.GetComponent<UIPlayTween>();
			}
		}

		public void ResetUIPlayTweenTargetToBegining()
		{
			if (this.NGUIPlayTween != null && this.NGUIPlayTween.tweenTarget != null)
			{
				GameObject tweenTarget = this.NGUIPlayTween.tweenTarget;
				TweenPosition component = tweenTarget.GetComponent<TweenPosition>();
				if (component != null)
				{
					component.enabled = false;
					component.ResetToBeginning();
					component.Sample(0f, true);
					component.tweenFactor = 0f;
				}
				TweenAlpha component2 = tweenTarget.GetComponent<TweenAlpha>();
				if (component2 != null)
				{
					component2.ResetToBeginning();
					component2.Sample(0f, true);
					component2.tweenFactor = 0f;
					component2.enabled = false;
				}
			}
		}
	}
}
