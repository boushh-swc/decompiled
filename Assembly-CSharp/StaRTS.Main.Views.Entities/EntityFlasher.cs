using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.Entities
{
	public class EntityFlasher : IViewFrameTimeObserver, IEventObserver
	{
		private List<FlashingEntity> flashingEntities;

		public EntityFlasher()
		{
			this.flashingEntities = new List<FlashingEntity>();
		}

		public void AddFlashing(SmartEntity entity, float flashDuration, float flashDelay)
		{
			GameObjectViewComponent gameObjectViewComp = entity.GameObjectViewComp;
			if (gameObjectViewComp == null)
			{
				return;
			}
			if (gameObjectViewComp.IsFlashing)
			{
				return;
			}
			gameObjectViewComp.IsFlashing = true;
			if (this.flashingEntities.Count == 0)
			{
				this.RegisterObservers();
			}
			FlashingEntity item = new FlashingEntity(entity, gameObjectViewComp.MainGameObject, flashDuration, flashDelay);
			this.flashingEntities.Add(item);
		}

		public void RemoveFlashing(SmartEntity entity)
		{
			bool flag = false;
			for (int i = this.flashingEntities.Count - 1; i >= 0; i--)
			{
				FlashingEntity flashingEntity = this.flashingEntities[i];
				if (flashingEntity.Entity == entity)
				{
					flag = true;
					this.StopFlashing(i);
				}
			}
			if (flag)
			{
				this.TrySendShaderResetOnEntityEvent();
			}
		}

		private void StopFlashing(int i)
		{
			FlashingEntity flashingEntity = this.flashingEntities[i];
			flashingEntity.Complete();
			this.flashingEntities.RemoveAt(i);
			if (this.flashingEntities.Count == 0)
			{
				this.UnregisterObservers();
			}
			GameObjectViewComponent gameObjectViewComp = flashingEntity.Entity.GameObjectViewComp;
			if (gameObjectViewComp != null)
			{
				gameObjectViewComp.IsFlashing = false;
			}
		}

		public void RemoveFlashingFromAllEntities()
		{
			if (this.flashingEntities.Count == 0)
			{
				return;
			}
			int count = this.flashingEntities.Count;
			for (int i = 0; i < count; i++)
			{
				FlashingEntity flashingEntity = this.flashingEntities[i];
				flashingEntity.Complete();
			}
			this.TrySendShaderResetOnEntityEvent();
			this.flashingEntities.Clear();
			this.UnregisterObservers();
		}

		private void TrySendShaderResetOnEntityEvent()
		{
			if (GameConstants.SEND_RESET_EVENT_ON_ENTITY_STOP_FLASHING)
			{
				Service.EventManager.SendEvent(EventId.ShaderResetOnEntity, null);
			}
		}

		private void RegisterObservers()
		{
			Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			Service.EventManager.RegisterObserver(this, EventId.ViewObjectsPurged, EventPriority.Default);
		}

		private void UnregisterObservers()
		{
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			Service.EventManager.UnregisterObserver(this, EventId.ViewObjectsPurged);
		}

		public void OnViewFrameTime(float dt)
		{
			bool flag = false;
			for (int i = this.flashingEntities.Count - 1; i >= 0; i--)
			{
				FlashingEntity flashingEntity = this.flashingEntities[i];
				if (flashingEntity.Flash(dt))
				{
					flag = true;
					this.StopFlashing(i);
				}
			}
			if (flag)
			{
				this.TrySendShaderResetOnEntityEvent();
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ViewObjectsPurged)
			{
				this.RemoveFlashing((SmartEntity)cookie);
			}
			return EatResponse.NotEaten;
		}
	}
}
