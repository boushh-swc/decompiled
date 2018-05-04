using System;
using UnityEngine;

namespace StaRTS.Utils.Animation.Anims
{
	public class AnimScale : AbstractAnimVector
	{
		public Transform Target
		{
			get;
			set;
		}

		public AnimScale(Transform target, float duration, Vector3 endScale)
		{
			this.Target = target;
			base.Duration = duration;
			base.End = endScale;
		}

		public AnimScale(GameObject target, float duration, Vector3 endScale) : this(target.transform, duration, endScale)
		{
		}

		public override void OnBegin()
		{
			base.Start = this.Target.localScale;
			base.Delta = base.End - base.Start;
		}

		public override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.Target.localScale = this.Vector;
		}

		public void SetEndPos(Vector3 endScale)
		{
			base.Delta = endScale - base.Start;
		}
	}
}
