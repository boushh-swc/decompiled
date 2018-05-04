using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Animation.Anims;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class AnimUXScale : AbstractAnimVector
	{
		public UXElement Target
		{
			get;
			set;
		}

		public AnimUXScale(UXElement target, float duration, Vector3 endScale)
		{
			this.Target = target;
			base.Duration = duration;
			base.End = endScale;
		}

		public override void OnBegin()
		{
			base.Start = this.Target.LocalScale;
			base.Delta = base.End - base.Start;
		}

		public override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.Target.LocalScale = this.Vector;
		}

		public void SetEndScale(Vector3 endScale)
		{
			base.Delta = endScale - base.Start;
		}
	}
}
