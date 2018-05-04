using System;
using UnityEngine;

namespace StaRTS.Utils.Animation.Anims
{
	public class AnimColor : Anim
	{
		private Color start;

		private Color end;

		private Color delta;

		private Material material;

		public AnimColor(Material m, float duration, Color startColor, Color endColor)
		{
			this.material = m;
			this.start = startColor;
			this.end = endColor;
			base.Duration = duration;
		}

		public override void OnBegin()
		{
			this.delta = new Color(this.end.r - this.start.r, this.end.g - this.start.g, this.end.b - this.start.b, this.end.a - this.start.a);
		}

		public override void OnUpdate(float dt)
		{
			this.material.color = new Color(this.EaseFunction(base.Age, this.start.r, this.delta.r, base.Duration), this.EaseFunction(base.Age, this.start.g, this.delta.g, base.Duration), this.EaseFunction(base.Age, this.start.b, this.delta.b, base.Duration), this.EaseFunction(base.Age, this.start.a, this.delta.a, base.Duration));
		}
	}
}
