using StaRTS.Main.Models;
using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views
{
	public class PvpCountdownView : IViewFrameTimeObserver
	{
		private float age;

		private float duration;

		private HUD hud;

		private Action onCountdownComplete;

		private bool running;

		public PvpCountdownView(Action onCountdownComplete)
		{
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			this.age = 0f;
			this.duration = GameConstants.PVP_MATCH_COUNTDOWN;
			this.hud = Service.UXController.HUD;
			this.hud.ShowCountdown(true);
			this.onCountdownComplete = onCountdownComplete;
			this.running = true;
		}

		public void OnViewFrameTime(float dt)
		{
			if (!this.running)
			{
				return;
			}
			this.age += dt;
			if (this.age > this.duration)
			{
				this.Destroy();
				if (this.onCountdownComplete != null)
				{
					this.onCountdownComplete();
				}
				return;
			}
			float remaining = this.duration - this.age;
			this.hud.SetCountdownTime(remaining, this.duration);
		}

		public void Destroy()
		{
			this.hud.ShowCountdown(false);
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public void Pause()
		{
			this.running = false;
		}

		public void Resume()
		{
			this.running = true;
		}
	}
}
