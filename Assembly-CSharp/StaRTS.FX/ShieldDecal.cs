using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class ShieldDecal : IViewFrameTimeObserver
	{
		private const float FADE_TIME = 1.5f;

		private const float FADE_DELAY = 3f;

		private const float FADE_OUT_TIME = 0.75f;

		private float fadeTime;

		private float fadeDelay;

		private Material material;

		private bool fading;

		private float fadingTime;

		private Color initialColor;

		private bool fadeIn;

		private Color fadeColor;

		public ShieldDecal(Material m, bool startInvisible)
		{
			this.material = m;
			this.fadingTime = 0f;
			this.fadeIn = startInvisible;
			this.StartFade();
		}

		private void StartFade()
		{
			if (this.fadeIn)
			{
				this.initialColor = this.material.GetColor("_TintColor");
				this.material.SetColor("_TintColor", Color.black);
				this.fadeTime = 1.5f;
				this.fadeDelay = 3f;
			}
			else
			{
				this.fadeDelay = 0f;
				this.fadeTime = 0.75f;
			}
			this.fadingTime = 0f;
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnViewFrameTime(float dt)
		{
			bool flag = this.fadingTime >= this.fadeTime + this.fadeDelay;
			if (flag)
			{
				this.fadingTime = this.fadeTime + this.fadeDelay;
			}
			if (this.material != null && this.fadingTime > this.fadeDelay)
			{
				float t = (this.fadingTime - this.fadeDelay) / this.fadeTime;
				if (this.fadeIn)
				{
					this.fadeColor = Color.Lerp(Color.black, this.initialColor, t);
				}
				else
				{
					this.fadeColor = Color.Lerp(this.initialColor, Color.black, t);
				}
				this.material.SetColor("_TintColor", this.fadeColor);
			}
			this.fadingTime += dt;
			if (flag)
			{
				Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			}
		}
	}
}
