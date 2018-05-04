using StaRTS.FX;
using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class LightingEffectsController : IViewFrameTimeObserver
	{
		public AbstractLightingEffects LightingEffects
		{
			get;
			private set;
		}

		public LightingEffectsController()
		{
			Service.LightingEffectsController = this;
			if (GameConstants.TIME_OF_DAY_ENABLED)
			{
				this.RegisterForFrameUpdates();
			}
			this.LightingEffects = new TimeOfDayLightingEffects();
		}

		public void OnViewFrameTime(float dt)
		{
			this.UpdateLightingEffects(dt);
		}

		public Color GetCurrentLightingColor(LightingColorType type)
		{
			return this.LightingEffects.GetCurrentLightingColor(type);
		}

		public void RegisterForFrameUpdates()
		{
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void UnregisterForFrameUpdates()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		private void UpdateLightingEffects(float dt)
		{
			this.LightingEffects.UpdateEnvironmentLighting(dt);
		}
	}
}
