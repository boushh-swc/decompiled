using Net.RichardLord.Ash.Core;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers.Entities.Systems;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Controllers
{
	public class EntityController : Game, ISimTimeObserver, IViewFrameTimeObserver
	{
		private float viewSpeed;

		public EntityViewManager View
		{
			get;
			protected set;
		}

		public EntityController()
		{
			Service.EntityController = this;
			new DroidController();
			new ChampionController();
			this.View = new EntityViewManager();
			this.viewSpeed = 1f;
			base.AddSimSystem(new TrackingSystem(), 1050, 21845);
			base.AddSimSystem(new MovementSystem(), 1060, 43690);
			base.AddViewSystem(new EntityRenderSystem(), 2030, SystemSchedulingPatterns.ENTITY_RENDER);
			base.AddViewSystem(new TrackingRenderSystem(), 2050, SystemSchedulingPatterns.TRACKING_RENDER);
			base.AddViewSystem(new HealthRenderSystem(), 2060, SystemSchedulingPatterns.HEALTH_RENDER);
			base.AddViewSystem(new TransportSystem(), 2090, 65535);
			base.AddViewSystem(new DroidSystem(), 2020, 65535);
			Service.SimTimeEngine.RegisterSimTimeObserver(this);
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
		}

		public void OnSimTime(uint dt)
		{
			base.UpdateSimSystems(dt);
		}

		public void OnViewFrameTime(float dt)
		{
			base.UpdateViewSystems(dt * this.viewSpeed);
		}

		public void SetSpeed(float speed)
		{
			this.viewSpeed = speed;
		}
	}
}
