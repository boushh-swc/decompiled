using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Animation.Anims;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.Animations
{
	public class AnimUXPosition : AbstractAnimVector
	{
		public UXElement Target
		{
			get;
			set;
		}

		public AnimUXPosition(UXElement target, float duration, Vector3 endPos)
		{
			this.Target = target;
			base.Duration = duration;
			base.End = endPos;
		}

		public override void OnBegin()
		{
			base.Start = this.Target.LocalPosition;
			base.Delta = base.End - base.Start;
		}

		public override void OnUpdate(float dt)
		{
			base.OnUpdate(dt);
			this.Target.LocalPosition = this.Vector;
		}

		public void SetEndPos(Vector3 endPos)
		{
			base.Delta = endPos - base.Start;
		}
	}
}
