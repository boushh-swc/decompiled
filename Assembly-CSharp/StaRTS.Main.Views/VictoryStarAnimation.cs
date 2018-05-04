using StaRTS.Main.Views.Animations;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils;
using StaRTS.Utils.Animation.Anims;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace StaRTS.Main.Views
{
	public class VictoryStarAnimation
	{
		private const float ANIMATION_TIME = 2f;

		private const float TWEEN_TIME = 0.5f;

		private const float SCREEN_SIZE_FACTOR = 0.5f;

		private const float VERTICAL_MOVEMENT = 100f;

		private int starNumber;

		private string message;

		private UXElement starAnimator;

		private UXElement starAnchor;

		private Vector3 originalLocation;

		private StarAnimationCompleteDelegate onComplete;

		[CompilerGenerated]
		private static Easing.EasingDelegate <>f__mg$cache0;

		public VictoryStarAnimation(int starNumber, string message)
		{
			this.starNumber = starNumber;
			this.message = message;
		}

		public void Start(StarAnimationCompleteDelegate onComplete)
		{
			this.onComplete = onComplete;
			HUD hUD = Service.UXController.HUD;
			UXLabel damageStarLabel = hUD.GetDamageStarLabel();
			damageStarLabel.Text = this.message;
			this.starAnimator = hUD.GetDamageStarAnimator();
			this.starAnchor = hUD.GetDamageStarAnchor();
			Animator component = this.starAnimator.Root.GetComponent<Animator>();
			if (component == null)
			{
				Service.Logger.WarnFormat("Unable to play star anim #{0}", new object[]
				{
					this.starNumber
				});
				if (onComplete != null)
				{
					onComplete(this.starNumber);
				}
			}
			else
			{
				this.starAnchor.Visible = true;
				this.starAnimator.Visible = true;
				component.enabled = true;
				component.SetTrigger("Show");
				Service.ViewTimerManager.CreateViewTimer(2f, false, new TimerDelegate(this.OnAnimationFinishedTimer), null);
			}
		}

		private void OnAnimationFinishedTimer(uint id, object cookie)
		{
			this.originalLocation = this.starAnimator.LocalPosition;
			AnimUXPosition animUXPosition = new AnimUXPosition(this.starAnimator, 0.5f, Vector3.right * (float)Screen.width * 0.5f + Vector3.up * 100f * this.starAnimator.UXCamera.Scale);
			Anim arg_83_0 = animUXPosition;
			if (VictoryStarAnimation.<>f__mg$cache0 == null)
			{
				VictoryStarAnimation.<>f__mg$cache0 = new Easing.EasingDelegate(Easing.SineEaseIn);
			}
			arg_83_0.EaseFunction = VictoryStarAnimation.<>f__mg$cache0;
			animUXPosition.OnCompleteCallback = new Action<Anim>(this.OnTweenFinished);
			Service.AnimController.Animate(animUXPosition);
		}

		private void OnTweenFinished(Anim anim)
		{
			Service.UXController.HUD.UpdateDamageStars(this.starNumber);
			this.starAnimator.LocalPosition = this.originalLocation;
			this.starAnimator.Visible = false;
			this.starAnchor.Visible = false;
			if (this.onComplete != null)
			{
				this.onComplete(this.starNumber);
			}
		}
	}
}
