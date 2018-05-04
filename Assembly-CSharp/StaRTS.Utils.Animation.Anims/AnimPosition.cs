using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Utils.Animation.Anims
{
	public class AnimPosition : AbstractAnimVector
	{
		public Transform Target
		{
			get;
			set;
		}

		public AnimPosition(Transform target, float duration, Vector3 endPos)
		{
			this.Target = target;
			base.Duration = duration;
			base.End = endPos;
		}

		public AnimPosition(GameObject target, float duration, Vector3 endPos) : this(target.transform, duration, endPos)
		{
		}

		public override void OnBegin()
		{
			base.Start = this.Target.localPosition;
			base.Delta = base.End - base.Start;
		}

		public override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			if (this.Target == null)
			{
				Service.AnimController.CompleteAndRemoveAnim(this);
				return;
			}
			this.Target.localPosition = this.Vector;
		}
	}
}
