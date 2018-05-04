using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Utils.Tween
{
	public class RotateTween : IViewFrameTimeObserver
	{
		private Action<RotateTween> onComplete;

		private Action<RotateTween> onUpdate;

		private AnimationCurve animationCurve;

		private Transform transform;

		private float duration;

		private Quaternion startRotation;

		private Quaternion endRotation;

		private float time;

		private bool tweenComplete;

		public RotateTween(Transform transform, float duration, Quaternion startRotation, Quaternion endRotation, AnimationCurve animationCurve, Action<RotateTween> onComplete, Action<RotateTween> onUpdate)
		{
			this.transform = transform;
			this.duration = duration;
			this.startRotation = startRotation;
			this.endRotation = endRotation;
			this.animationCurve = animationCurve;
			this.onComplete = onComplete;
			this.onUpdate = onUpdate;
			this.time = 0f;
			this.tweenComplete = false;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.transform == null)
			{
				this.Destroy();
				return;
			}
			this.time += dt;
			float t = this.animationCurve.Evaluate(this.time / this.duration);
			this.transform.rotation = Quaternion.Slerp(this.startRotation, this.endRotation, t);
			if (this.onUpdate != null)
			{
				this.onUpdate(this);
			}
			if (this.time >= this.duration)
			{
				if (this.transform != null)
				{
					this.transform.rotation = this.endRotation;
				}
				if (this.onComplete != null)
				{
					this.onComplete(this);
				}
				this.Destroy();
			}
		}

		public void Destroy()
		{
			this.animationCurve = null;
			this.transform = null;
			this.tweenComplete = true;
			this.onComplete = null;
			this.onUpdate = null;
			if (Service.ViewTimeEngine != null)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}

		public bool IsTweenComplete()
		{
			return this.tweenComplete;
		}
	}
}
