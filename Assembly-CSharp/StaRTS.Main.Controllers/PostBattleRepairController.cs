using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.Entities.Systems;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class PostBattleRepairController : IEventObserver
	{
		private Dictionary<Entity, HealthViewComponent> entitiesInRepairMap;

		private bool worldLoadPending;

		private bool canRepair;

		private bool repairing;

		private EventManager eventManager;

		public PostBattleRepairController()
		{
			Service.PostBattleRepairController = this;
			this.eventManager = Service.EventManager;
			this.eventManager.RegisterObserver(this, EventId.GameStateAboutToChange, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.MapDataProcessingStart, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.EntityHealthViewRegenerated, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.MapDataProcessingStart:
				this.worldLoadPending = true;
				this.canRepair = this.CheckIfCanRepair();
				return EatResponse.NotEaten;
			case EventId.MapDataProcessingEnd:
			{
				IL_15:
				if (id == EventId.BuildingViewReady)
				{
					if (this.canRepair)
					{
						EntityViewParams entityViewParams = (EntityViewParams)cookie;
						this.OnEntityLoaded(entityViewParams.Entity);
					}
					return EatResponse.NotEaten;
				}
				if (id == EventId.GameStateAboutToChange)
				{
					this.UpdateEntityRepairs(cookie as IState);
					return EatResponse.NotEaten;
				}
				if (id != EventId.EntityHealthViewRegenerated)
				{
					return EatResponse.NotEaten;
				}
				HealthViewComponent healthViewComponent = (HealthViewComponent)cookie;
				if (healthViewComponent.Entity != null)
				{
					this.entitiesInRepairMap.Remove(healthViewComponent.Entity);
					this.OnHeathRepairStopped(healthViewComponent);
					this.CleanupEntityAfterRepair(healthViewComponent.Entity, healthViewComponent);
				}
				return EatResponse.NotEaten;
			}
			case EventId.WorldLoadComplete:
			{
				this.worldLoadPending = false;
				IState currentState = Service.GameStateMachine.CurrentState;
				this.UpdateEntityRepairs(currentState);
				return EatResponse.NotEaten;
			}
			}
			goto IL_15;
		}

		private bool CheckIfCanRepair()
		{
			if (!Service.WorldTransitioner.IsCurrentWorldHome())
			{
				return false;
			}
			if (Service.CurrentPlayer.DamagedBuildings == null)
			{
				return false;
			}
			IState currentState = Service.GameStateMachine.CurrentState;
			if (!(currentState is HomeState) && !(currentState is EditBaseState) && !(currentState is ApplicationLoadState))
			{
				return false;
			}
			if (currentState is ApplicationLoadState && Service.PvpManager.GetBattlesThatHappenOffline().Count == 0)
			{
				Service.CurrentPlayer.DamagedBuildings.Clear();
				return false;
			}
			return true;
		}

		private void OnEntityLoaded(Entity entity)
		{
			if (entity.Has<BuildingComponent>())
			{
				BuildingType type = entity.Get<BuildingComponent>().BuildingType.Type;
				if (type != BuildingType.Wall && type != BuildingType.Clearable && type != BuildingType.Trap)
				{
					int startingHealth = this.GetStartingHealth(entity);
					if (startingHealth >= 0)
					{
						this.AddEntityForRepair(entity, startingHealth);
					}
				}
			}
		}

		private int GetStartingHealth(Entity entity)
		{
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			Building buildingTO = buildingComponent.BuildingTO;
			int num = -1;
			Service.CurrentPlayer.DamagedBuildings.TryGetValue(buildingTO.Key, out num);
			if (num > 0)
			{
				float num2 = 1f - (float)num / 100f;
				HealthComponent healthComponent = entity.Get<HealthComponent>();
				return Mathf.FloorToInt((float)healthComponent.MaxHealth * num2);
			}
			return -1;
		}

		private void UpdateEntityRepairs(IState state)
		{
			if (this.worldLoadPending)
			{
				return;
			}
			if (this.repairing && !(state is HomeState) && !(state is EditBaseState) && !(state is GalaxyState) && !(state is ApplicationLoadState) && !(state is BaseLayoutToolState))
			{
				this.StopHealthRepairs();
				return;
			}
			if (this.canRepair && this.entitiesInRepairMap != null)
			{
				foreach (HealthViewComponent current in this.entitiesInRepairMap.Values)
				{
					if (this.repairing)
					{
						if (!current.IsInitialized && current.Entity != null)
						{
							current.SetupElements();
						}
					}
					else
					{
						this.StartHealthRepair(current);
					}
				}
			}
		}

		private void AddEntityForRepair(Entity entity, int startingHealth)
		{
			HealthViewComponent healthViewComponent = entity.Get<HealthViewComponent>();
			if (healthViewComponent == null)
			{
				healthViewComponent = new HealthViewComponent();
				entity.Add(healthViewComponent);
			}
			HealthComponent healthComponent = entity.Get<HealthComponent>();
			healthViewComponent.UpdateHealth(startingHealth, healthComponent.MaxHealth, false);
			healthComponent.Health = startingHealth;
			if (!this.worldLoadPending)
			{
				healthViewComponent.SetupElements();
			}
			Service.EntityController.GetViewSystem<HealthRenderSystem>().UpdateRubbleStateFromHealthView(healthViewComponent);
			if (this.entitiesInRepairMap == null)
			{
				this.entitiesInRepairMap = new Dictionary<Entity, HealthViewComponent>();
			}
			if (!this.entitiesInRepairMap.ContainsKey(entity))
			{
				this.entitiesInRepairMap.Add(entity, healthViewComponent);
			}
			this.StartHealthRepair(healthViewComponent);
		}

		private void StartHealthRepair(HealthViewComponent healthView)
		{
			this.repairing = true;
			healthView.AutoRegenerating = true;
			Entity entity = healthView.Entity;
			this.eventManager.SendEvent(EventId.EntityPostBattleRepairStarted, entity);
			SupportViewComponent supportViewComponent = entity.Get<SupportViewComponent>();
			if (supportViewComponent != null)
			{
				supportViewComponent.TeardownElements();
			}
			GeneratorViewComponent generatorViewComponent = entity.Get<GeneratorViewComponent>();
			if (generatorViewComponent != null)
			{
				generatorViewComponent.SetEnabled(false);
			}
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			Service.StorageEffects.UpdateFillState(entity, buildingComponent.BuildingType);
		}

		private void OnHeathRepairStopped(HealthViewComponent healthView)
		{
			healthView.TeardownElements();
			SmartEntity smartEntity = (SmartEntity)healthView.Entity;
			this.eventManager.SendEvent(EventId.EntityPostBattleRepairFinished, smartEntity);
			if (this.entitiesInRepairMap == null || this.entitiesInRepairMap.Count == 0)
			{
				this.eventManager.SendEvent(EventId.AllPostBattleRepairFinished, null);
			}
			Service.BuildingTooltipController.EnsureBuildingTooltip(smartEntity);
			if (Service.BuildingController.SelectedBuilding == smartEntity)
			{
				Service.UXController.HUD.ShowContextButtons(smartEntity);
			}
		}

		public void StopHealthRepairs()
		{
			if (this.entitiesInRepairMap != null)
			{
				foreach (KeyValuePair<Entity, HealthViewComponent> current in this.entitiesInRepairMap)
				{
					Entity key = current.Key;
					HealthViewComponent value = current.Value;
					value.AutoRegenerating = false;
					this.OnHeathRepairStopped(value);
					this.CleanupEntityAfterRepair(key, value);
				}
				this.entitiesInRepairMap.Clear();
			}
			this.repairing = false;
		}

		private void CleanupEntityAfterRepair(Entity entity, HealthViewComponent healthView)
		{
			if (entity == null)
			{
				return;
			}
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return;
			}
			float num = (healthView.MaxHealthAmount != 0) ? ((float)healthView.HealthAmount / (float)healthView.MaxHealthAmount) : 0f;
			if (num > 0.2f)
			{
				Service.FXManager.RemoveAttachedRubbleFromEntity(entity);
			}
			Service.StorageEffects.UpdateFillState(entity, buildingComponent.BuildingType);
			this.UpdateDamagePercentageFromHealth(entity, Mathf.FloorToInt(num * 100f));
		}

		public void PauseHealthRepairs(bool pause)
		{
			if (this.entitiesInRepairMap != null)
			{
				foreach (KeyValuePair<Entity, HealthViewComponent> current in this.entitiesInRepairMap)
				{
					current.Value.AutoRegenerating = !pause;
				}
			}
		}

		private void UpdateDamagePercentageFromHealth(Entity entity, int healthPercentage)
		{
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			Building buildingTO = buildingComponent.BuildingTO;
			Service.CurrentPlayer.DamagedBuildings[buildingTO.Key] = 100 - healthPercentage;
			SmartEntity smartEntity = (SmartEntity)entity;
			HealthComponent healthComp = smartEntity.HealthComp;
			if ((float)healthPercentage > 20f)
			{
				int health = Mathf.FloorToInt((float)healthComp.MaxHealth * ((float)healthPercentage / 100f));
				healthComp.Health = health;
			}
			else
			{
				healthComp.Health = 0;
				if (buildingComponent.BuildingType.Type == BuildingType.HQ)
				{
					healthComp.Health = Mathf.FloorToInt((float)healthComp.MaxHealth * 0.2f);
					Service.FXManager.RemoveAttachedRubbleFromEntity(entity);
				}
			}
		}

		public void RemoveExistingRepair(Entity entity)
		{
			HealthViewComponent healthView = null;
			if (this.entitiesInRepairMap.TryGetValue(entity, out healthView))
			{
				this.CleanupEntityAfterRepair(entity, healthView);
				this.entitiesInRepairMap.Remove(entity);
			}
		}

		public bool IsEntityInRepair(Entity entity)
		{
			bool result = false;
			if (this.repairing && this.entitiesInRepairMap != null)
			{
				result = this.entitiesInRepairMap.ContainsKey(entity);
			}
			return result;
		}

		public void ForceRepairOnAllBuildings(float healthPercentage)
		{
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				this.ForceRepairOnBuilding(buildingNode.Entity, healthPercentage);
			}
		}

		public void ForceRepairOnBuilding(Entity entity, float healthPercentage)
		{
			this.canRepair = true;
			float num = (float)entity.Get<HealthComponent>().MaxHealth;
			int num2 = Mathf.FloorToInt(num * healthPercentage);
			if (entity.Has<GameObjectViewComponent>())
			{
				this.AddEntityForRepair(entity, num2);
			}
			else
			{
				this.UpdateDamagePercentageFromHealth(entity, num2);
			}
		}
	}
}
