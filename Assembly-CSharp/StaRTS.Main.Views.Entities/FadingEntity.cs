using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Views.Entities
{
	public class FadingEntity : AbstractFadingView
	{
		private Entity entity;

		public Entity Entity
		{
			get
			{
				return this.entity;
			}
		}

		public FadingEntity(Entity entity, float delay, float fadeTime, FadingDelegate onStart, FadingDelegate onComplete) : base(entity, delay, fadeTime, onStart, onComplete)
		{
			this.entity = entity;
			GameObjectViewComponent gameObjectViewComponent = entity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent != null)
			{
				MeterShaderComponent meter = entity.Get<MeterShaderComponent>();
				base.InitData(gameObjectViewComponent.MainGameObject, meter);
			}
		}
	}
}
