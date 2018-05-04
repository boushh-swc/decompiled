using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ScreenTransition : IViewFrameTimeObserver
	{
		private const int WAIT_FRAMES = 2;

		private string animationToPlay;

		private Animation animation;

		private OnScreenTransitionComplete onComplete;

		private int waitFrames;

		private bool disabledColliders;

		public ScreenTransition(Animation a)
		{
			this.animation = a;
		}

		public void PlayTransition(string transitionName, OnScreenTransitionComplete onTransitionComplete, bool delay)
		{
			this.AnimationComplete();
			this.onComplete = onTransitionComplete;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.animationToPlay = transitionName;
			if (delay)
			{
				this.waitFrames = 2;
			}
			else
			{
				this.waitFrames = 0;
				this.AnimationPlay();
			}
		}

		public void Destroy()
		{
			this.AnimationComplete();
			ScreenTransition.ForceAlpha(this.animation, 0f);
		}

		public static void ForceAlpha(Animation animation, float alpha)
		{
			animation.gameObject.GetComponent<UIPanel>().alpha = alpha;
			animation.gameObject.GetComponent<AnimatedAlpha>().alpha = alpha;
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.waitFrames == 0)
			{
				if (!this.animation.isPlaying)
				{
					this.AnimationComplete();
				}
			}
			else if (--this.waitFrames == 0)
			{
				this.AnimationPlay();
			}
		}

		private void AnimationComplete()
		{
			this.AnimationStop();
			if (Service.ViewTimeEngine != null)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
			else if (Service.Logger != null)
			{
				Service.Logger.Error("ScreenTransition.AnimationComplete: ViewTimeEngine is not set in Service");
			}
			if (this.disabledColliders)
			{
				if (Service.CameraManager != null)
				{
					if (!Service.CameraManager.WipeCamera.IsRendering())
					{
						Service.CameraManager.UXCamera.ReceiveEvents = true;
					}
				}
				else if (Service.Logger != null)
				{
					Service.Logger.Error("ScreenTransition.AnimationComplete: CameraManager is not set in Service");
				}
				this.disabledColliders = false;
			}
			if (this.onComplete != null)
			{
				OnScreenTransitionComplete onScreenTransitionComplete = this.onComplete;
				this.onComplete = null;
				onScreenTransitionComplete();
			}
		}

		private void AnimationPlay()
		{
			this.AnimationStop();
			if (!this.disabledColliders)
			{
				Service.CameraManager.UXCamera.ReceiveEvents = false;
				this.disabledColliders = true;
			}
			this.animation.Play(this.animationToPlay);
			this.animationToPlay = null;
		}

		private void AnimationStop()
		{
			if (this.animation.isPlaying)
			{
				this.animation.Stop();
			}
		}
	}
}
