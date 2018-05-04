using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MeshCombiner;
using StaRTS.Utils.Pooling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.CombineMesh
{
	public class CombineMeshManager : IEventObserver
	{
		private bool isSomeMeshesCombined;

		private bool isStartupTasksCompleted;

		private GameObject combinedMeshesGameObject;

		private GameObjectPool meshCombinerGameObjectPool;

		private Dictionary<BuildingType, MeshCombiner> meshCombiners;

		private AbstractCombineMeshHelper homeBaseCobmbineMeshHelper;

		private AbstractCombineMeshHelper battleBaseCombineMeshHelper;

		public CombineMeshManager()
		{
			Service.CombineMeshManager = this;
			EventManager eventManager = Service.EventManager;
			this.meshCombinerGameObjectPool = MeshCombiner.CreateMeshCombinerObjectPool();
			this.combinedMeshesGameObject = new GameObject("CombinedMeshes");
			this.homeBaseCobmbineMeshHelper = new HomeBaseCombineMeshHelper();
			this.battleBaseCombineMeshHelper = new BattleBaseCombineMeshHelper();
			HashSet<BuildingType> hashSet = new HashSet<BuildingType>();
			hashSet.UnionWith(this.homeBaseCobmbineMeshHelper.GetEligibleBuildingTypes());
			hashSet.UnionWith(this.battleBaseCombineMeshHelper.GetEligibleBuildingTypes());
			this.meshCombiners = new Dictionary<BuildingType, MeshCombiner>();
			foreach (BuildingType current in hashSet)
			{
				this.meshCombiners.Add(current, new MeshCombiner(this.meshCombinerGameObjectPool, this.combinedMeshesGameObject, current.ToString()));
			}
			eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.MeshCombineAfterOthers);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.MeshCombineAfterOthers);
			eventManager.RegisterObserver(this, EventId.ShaderResetOnEntity, EventPriority.MeshCombineAfterOthers);
			eventManager.RegisterObserver(this, EventId.StartupTasksCompleted, EventPriority.MeshCombineAfterOthers);
			eventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.BeforeDefault);
			eventManager.RegisterObserver(this, EventId.BuildingLevelUpgraded, EventPriority.BeforeDefault);
			eventManager.RegisterObserver(this, EventId.BuildingSwapped, EventPriority.BeforeDefault);
			eventManager.RegisterObserver(this, EventId.ClearableCleared, EventPriority.AfterDefault);
			eventManager.RegisterObserver(this, EventId.WorldReset, EventPriority.BeforeDefault);
			eventManager.RegisterObserver(this, EventId.PostBuildingEntityKilled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingViewFailed, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.BuildingLevelUpgraded:
			case EventId.BuildingSwapped:
				if (!this.IsFueInProgress() && this.IsCurrentWorldHome())
				{
					SmartEntity entity = ((ContractEventData)cookie).Entity;
					this.GetCurrentCombineMeshHelper().BuildingObjectDestroyed(entity, this.meshCombiners, false);
				}
				break;
			case EventId.BuildingConstructed:
				break;
			default:
			{
				if (id != EventId.BuildingViewReady && id != EventId.BuildingViewFailed)
				{
					switch (id)
					{
					case EventId.ClearableCleared:
						if (!this.IsFueInProgress())
						{
							this.GetCurrentCombineMeshHelper().BuildingObjectDestroyed(BuildingType.Clearable, this.meshCombiners, true);
						}
						return EatResponse.NotEaten;
					case EventId.ClearableStarted:
						IL_38:
						switch (id)
						{
						case EventId.WorldLoadComplete:
							if ((!this.IsCurrentWorldHome() || this.isStartupTasksCompleted) && !this.IsCurrentWorldUserWarBase())
							{
								this.CombineAllMeshTypes();
							}
							return EatResponse.NotEaten;
						case EventId.WorldInTransitionComplete:
						case EventId.WorldOutTransitionComplete:
							IL_51:
							if (id == EventId.PostBuildingEntityKilled)
							{
								BuildingType buildingType = (BuildingType)cookie;
								if (!this.IsFueInProgress())
								{
									this.GetCurrentCombineMeshHelper().BuildingObjectDestroyed(buildingType, this.meshCombiners, true);
								}
								return EatResponse.NotEaten;
							}
							if (id == EventId.GameStateChanged)
							{
								IState currentState = Service.GameStateMachine.CurrentState;
								Type previousStateType = (Type)cookie;
								if (currentState is HomeState)
								{
									if (!this.isStartupTasksCompleted)
									{
										this.CombineAllMeshTypes();
									}
									else if (this.IsPreviousStateEditMode(previousStateType))
									{
										this.CombineAllMeshTypes();
									}
								}
								else if (this.DidJustTransitionFromHomeToEditState(previousStateType, currentState))
								{
									this.UncombineAllMeshTypes(false);
								}
								return EatResponse.NotEaten;
							}
							if (id != EventId.ShaderResetOnEntity)
							{
								return EatResponse.NotEaten;
							}
							if ((!this.IsCurrentWorldHome() || this.isStartupTasksCompleted) && !this.IsCurrentWorldUserWarBase())
							{
								this.CombineAllMeshTypes();
							}
							return EatResponse.NotEaten;
						case EventId.WorldReset:
							if (this.isSomeMeshesCombined)
							{
								this.UncombineAllMeshTypes(true);
							}
							return EatResponse.NotEaten;
						}
						goto IL_51;
					case EventId.StartupTasksCompleted:
						this.isStartupTasksCompleted = true;
						return EatResponse.NotEaten;
					}
					goto IL_38;
				}
				SmartEntity entity2 = ((EntityViewParams)cookie).Entity;
				if (!this.IsFueInProgress() && this.isStartupTasksCompleted && Service.WorldTransitioner.IsEverythingLoaded() && this.IsCurrentWorldHome())
				{
					this.GetCurrentCombineMeshHelper().BuildingObjectAdded(entity2, this.meshCombiners);
				}
				break;
			}
			}
			return EatResponse.NotEaten;
		}

		private bool IsPreviousStateEditMode(Type previousStateType)
		{
			return previousStateType == typeof(EditBaseState);
		}

		private bool DidJustTransitionFromHomeToEditState(Type previousStateType, IState currentState)
		{
			return previousStateType == typeof(HomeState) && currentState is EditBaseState;
		}

		public bool IsRendererCombined(SmartEntity entity, Renderer renderer)
		{
			if (entity.BuildingComp != null)
			{
				BuildingType buildingTypeFromBuilding = this.GetBuildingTypeFromBuilding(entity);
				MeshCombiner meshCombiner;
				if (this.meshCombiners.TryGetValue(buildingTypeFromBuilding, out meshCombiner))
				{
					return meshCombiner.IsRendererCombined(renderer);
				}
			}
			return false;
		}

		private void CombineAllMeshTypes()
		{
			if (!this.IsFueInProgress())
			{
				this.isSomeMeshesCombined = true;
				this.GetCurrentCombineMeshHelper().CombineAllMeshTypes(this.meshCombiners);
			}
		}

		private void UncombineAllMeshTypes(bool resetPool)
		{
			if (!this.IsFueInProgress())
			{
				this.isSomeMeshesCombined = false;
				this.GetCurrentCombineMeshHelper().UncombineAllMeshTypes(this.meshCombiners);
				if (resetPool)
				{
					this.meshCombinerGameObjectPool.ClearOutPool();
				}
			}
		}

		private bool IsFueInProgress()
		{
			return Service.CurrentPlayer.CampaignProgress.FueInProgress;
		}

		private bool IsCurrentWorldHome()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return Service.WorldTransitioner.IsCurrentWorldHome();
		}

		private bool IsCurrentWorldUserWarBase()
		{
			return Service.WorldTransitioner.IsCurrentWorldUserWarBase();
		}

		private AbstractCombineMeshHelper GetCurrentCombineMeshHelper()
		{
			if (this.IsCurrentWorldHome())
			{
				return this.homeBaseCobmbineMeshHelper;
			}
			return this.battleBaseCombineMeshHelper;
		}

		private BuildingType GetBuildingTypeFromBuilding(SmartEntity smartEntity)
		{
			return smartEntity.BuildingComp.BuildingType.Type;
		}
	}
}
