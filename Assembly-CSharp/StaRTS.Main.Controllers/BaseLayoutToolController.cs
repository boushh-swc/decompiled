using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Move;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class BaseLayoutToolController : IEventObserver
	{
		private PositionMap lastSavedMap;

		public Dictionary<string, List<SmartEntity>> stashedBuildingMap;

		private const string NO_VALID_POSITION_FOR_UNSTASH = "NO_VALID_POSITION_FOR_UNSTASH";

		public bool IsBaseLayoutModeActive
		{
			get;
			private set;
		}

		public bool IsSavingBaseLayout
		{
			get;
			private set;
		}

		public bool ShouldRevertMap
		{
			get;
			private set;
		}

		public bool IsQuickStashModeEnabled
		{
			get;
			set;
		}

		public BaseLayoutToolController()
		{
			Service.BaseLayoutToolController = this;
			this.ShouldRevertMap = false;
			this.IsQuickStashModeEnabled = false;
			this.IsSavingBaseLayout = false;
		}

		public void EnterBaseLayoutTool()
		{
			this.IsBaseLayoutModeActive = true;
			this.IsQuickStashModeEnabled = false;
			Service.EventManager.RegisterObserver(this, EventId.BuildingQuickStashed, EventPriority.AfterDefault);
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady);
			Service.EventManager.RegisterObserver(this, EventId.UserLoweredBuilding, EventPriority.AfterDefault);
		}

		public void ExitBaseLayoutTool()
		{
			this.IsBaseLayoutModeActive = false;
			Service.EventManager.UnregisterObserver(this, EventId.BuildingQuickStashed);
			Service.EventManager.UnregisterObserver(this, EventId.BuildingViewReady);
			Service.EventManager.UnregisterObserver(this, EventId.UserLoweredBuilding);
			Service.BuildingController.DisableUnstashStampingState();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.BuildingQuickStashed)
			{
				if (id != EventId.UserLoweredBuilding)
				{
					if (id == EventId.BuildingViewReady)
					{
						EntityViewParams entityViewParams = (EntityViewParams)cookie;
						if (this.IsBuildingStashed(entityViewParams.Entity))
						{
							GameObjectViewComponent gameObjectViewComp = entityViewParams.Entity.GameObjectViewComp;
							TransformComponent transformComp = entityViewParams.Entity.TransformComp;
							gameObjectViewComp.SetXYZ(Units.BoardToWorldX(transformComp.CenterX()), -1000f, Units.BoardToWorldZ(transformComp.CenterZ()));
						}
					}
				}
				else
				{
					Entity entity = (Entity)cookie;
					if (entity != null)
					{
						Building buildingTO = entity.Get<BuildingComponent>().BuildingTO;
						Position position = this.lastSavedMap.GetPosition(buildingTO.Key);
						if ((position != null && this.HasBuildingMoved(buildingTO, position)) || (Service.GameStateMachine.CurrentState is WarBaseEditorState && position == null))
						{
							this.ShouldRevertMap = true;
						}
					}
				}
			}
			else if (this.IsQuickStashModeEnabled)
			{
				SmartEntity smartEntity = (SmartEntity)cookie;
				this.StashBuilding(smartEntity);
				string uid = smartEntity.BuildingComp.BuildingTO.Uid;
				Service.BuildingController.EnsureDeselectSelectedBuilding();
				Service.UXController.HUD.BaseLayoutToolView.RefreshStashedBuildingCount(uid);
			}
			return EatResponse.NotEaten;
		}

		public int GetBuildingLastSavedX(string buildingKey)
		{
			return this.lastSavedMap.GetPosition(buildingKey).X;
		}

		public int GetBuildingLastSavedZ(string buildingKey)
		{
			return this.lastSavedMap.GetPosition(buildingKey).Z;
		}

		public bool IsBuildingStashed(SmartEntity buildingEntity)
		{
			GameObjectViewComponent gameObjectViewComp = buildingEntity.GameObjectViewComp;
			if (gameObjectViewComp != null && gameObjectViewComp.MainTransform.position.y < 0f)
			{
				return true;
			}
			if (this.stashedBuildingMap != null)
			{
				if (buildingEntity.BuildingComp != null)
				{
					return false;
				}
				string uid = buildingEntity.BuildingComp.BuildingType.Uid;
				if (this.stashedBuildingMap.ContainsKey(uid) && this.stashedBuildingMap[uid].Contains(buildingEntity))
				{
					return true;
				}
			}
			return false;
		}

		public bool IsStashedBuildingListEmpty()
		{
			if (this.stashedBuildingMap == null)
			{
				return true;
			}
			foreach (KeyValuePair<string, List<SmartEntity>> current in this.stashedBuildingMap)
			{
				List<SmartEntity> value = current.Value;
				if (value.Count > 0)
				{
					return false;
				}
			}
			return true;
		}

		public bool IsListOutOfGivenBuilding(string buildingUID)
		{
			return this.stashedBuildingMap == null || !this.stashedBuildingMap.ContainsKey(buildingUID) || this.stashedBuildingMap[buildingUID].Count < 1;
		}

		public void PauseContractsOnAllBuildings()
		{
			List<SmartEntity> buildingListByType = Service.BuildingLookupController.GetBuildingListByType(BuildingType.Any);
			int i = 0;
			int count = buildingListByType.Count;
			while (i < count)
			{
				if (!this.IsBuildingClearable(buildingListByType[i]))
				{
					BuildingComponent buildingComp = buildingListByType[i].BuildingComp;
					Service.ISupportController.PauseBuilding(buildingComp.BuildingTO.Key);
				}
				i++;
			}
		}

		public void ResumeContractsOnAllBuildings()
		{
			Service.ISupportController.UnpauseAllBuildings();
		}

		public void StashAllBuildings()
		{
			Service.BuildingController.EnsureLoweredLiftedBuilding();
			Service.BuildingController.EnsureDeselectSelectedBuilding();
			List<SmartEntity> buildingListByType = Service.BuildingLookupController.GetBuildingListByType(BuildingType.Any);
			int i = 0;
			int count = buildingListByType.Count;
			while (i < count)
			{
				SmartEntity buildingEntity = buildingListByType[i];
				if (!this.IsBuildingClearable(buildingEntity))
				{
					if (!this.IsBuildingStashed(buildingEntity))
					{
						this.StashBuilding(buildingEntity);
					}
				}
				i++;
			}
			this.ShouldRevertMap = true;
		}

		private void StashAllMovedBuildings()
		{
			List<SmartEntity> buildingListByType = Service.BuildingLookupController.GetBuildingListByType(BuildingType.Any);
			int i = 0;
			int count = buildingListByType.Count;
			while (i < count)
			{
				SmartEntity smartEntity = buildingListByType[i];
				BuildingComponent buildingComp = smartEntity.BuildingComp;
				Building buildingTO = buildingComp.BuildingTO;
				if (buildingComp.BuildingType.Type != BuildingType.Clearable)
				{
					if (!this.IsBuildingStashed(smartEntity))
					{
						string key = buildingTO.Key;
						Position position = this.lastSavedMap.GetPosition(key);
						if (position == null)
						{
							Service.Logger.Error("BLT: Old Building position for " + key + " not found!");
						}
						else if (this.HasBuildingMoved(buildingTO, position))
						{
							this.StashBuilding(smartEntity);
						}
					}
				}
				i++;
			}
		}

		public void UpdateLastSavedMap()
		{
			List<Building> buildings = this.GetCurrentPlayerMap().Buildings;
			if (this.lastSavedMap == null)
			{
				this.lastSavedMap = new PositionMap();
			}
			else
			{
				this.lastSavedMap.ClearAllPositions();
			}
			int i = 0;
			int count = buildings.Count;
			while (i < count)
			{
				Position position = new Position();
				position.X = buildings[i].X;
				position.Z = buildings[i].Z;
				this.lastSavedMap.AddPosition(buildings[i].Key, position);
				i++;
			}
			this.ShouldRevertMap = false;
		}

		private void SaveBaseLayout(PositionMap diffMap)
		{
			if (Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				Service.WarBaseEditController.SaveWarBaseMap(diffMap);
			}
			else
			{
				BuildingMultiMoveCommand command = new BuildingMultiMoveCommand(new BuildingMultiMoveRequest
				{
					PositionMap = diffMap
				});
				Service.ServerAPI.Enqueue(command);
			}
		}

		public Map GetCurrentPlayerMap()
		{
			WarBaseEditController warBaseEditController = Service.WarBaseEditController;
			if (warBaseEditController.mapData != null && Service.GameStateMachine.CurrentState is WarBaseEditorState)
			{
				return warBaseEditController.mapData;
			}
			return Service.CurrentPlayer.Map;
		}

		public void SaveMap()
		{
			if (!this.IsStashedBuildingListEmpty())
			{
				Service.Logger.Warn("BLT: We can't save the map as there are still stashed buildings");
				return;
			}
			PositionMap positionMap = new PositionMap();
			this.IsSavingBaseLayout = true;
			bool flag = false;
			List<Building> buildings = this.GetCurrentPlayerMap().Buildings;
			int i = 0;
			int count = buildings.Count;
			while (i < count)
			{
				string key = buildings[i].Key;
				Position position = this.lastSavedMap.GetPosition(key);
				if (!(Service.GameStateMachine.CurrentState is WarBaseEditorState) && position == null)
				{
					Service.Logger.Error("BLT: Old Building position for " + key + " not found!");
				}
				else if (position == null || this.HasBuildingMoved(buildings[i], position))
				{
					positionMap.AddPosition(key, new Position
					{
						X = buildings[i].X,
						Z = buildings[i].Z
					});
					flag = true;
				}
				i++;
			}
			if (flag)
			{
				this.SaveBaseLayout(positionMap);
				this.UpdateLastSavedMap();
				this.ClearStashedBuildings();
			}
			this.ShouldRevertMap = false;
			this.IsSavingBaseLayout = false;
		}

		public void RevertToPreviousMapLayout()
		{
			if (this.ShouldRevertMap)
			{
				this.StashAllMovedBuildings();
			}
			if (this.stashedBuildingMap == null)
			{
				return;
			}
			foreach (KeyValuePair<string, List<SmartEntity>> current in this.stashedBuildingMap)
			{
				List<SmartEntity> value = current.Value;
				while (value.Count > 0)
				{
					this.UnstashBuildingByUID(current.Key, true, false, false, false);
				}
			}
			Service.EventManager.SendEvent(EventId.UserLoweredBuildingAudio, null);
			this.ClearStashedBuildings();
		}

		public void ClearStashedBuildings()
		{
			if (this.stashedBuildingMap != null)
			{
				this.stashedBuildingMap.Clear();
				this.stashedBuildingMap = null;
			}
		}

		public void StashBuilding(SmartEntity buildingEntity)
		{
			this.StashBuilding(buildingEntity, true);
		}

		public void StashBuilding(SmartEntity buildingEntity, bool allowRevert)
		{
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			if (buildingComponent.BuildingType.Type == BuildingType.Clearable)
			{
				Service.Logger.Warn("BLT: Can't stash clearable: " + buildingComponent.BuildingTO.Key + ":" + buildingComponent.BuildingTO.Uid);
				return;
			}
			string uid = buildingComponent.BuildingTO.Uid;
			if (this.stashedBuildingMap == null)
			{
				this.stashedBuildingMap = new Dictionary<string, List<SmartEntity>>();
			}
			if (!this.stashedBuildingMap.ContainsKey(uid))
			{
				this.stashedBuildingMap.Add(uid, new List<SmartEntity>());
			}
			List<SmartEntity> list = this.stashedBuildingMap[uid];
			if (!list.Contains(buildingEntity))
			{
				list.Add(buildingEntity);
			}
			GameObjectViewComponent gameObjectViewComp = buildingEntity.GameObjectViewComp;
			if (gameObjectViewComp != null)
			{
				TransformComponent transformComp = buildingEntity.TransformComp;
				gameObjectViewComp.SetXYZ(Units.BoardToWorldX(transformComp.CenterX()), -1000f, Units.BoardToWorldZ(transformComp.CenterZ()));
			}
			if (buildingEntity.HealthViewComp != null)
			{
				HealthViewComponent healthViewComponent = buildingEntity.Get<HealthViewComponent>();
				healthViewComponent.TeardownElements();
			}
			Service.EventManager.SendEvent(EventId.UserStashedBuilding, buildingEntity);
			Service.BoardController.RemoveEntity(buildingEntity, true);
			Service.EventManager.SendEvent(EventId.EntityDestroyed, buildingEntity.ID);
			Service.EventManager.SendEvent(EventId.BuildingMovedOnBoard, buildingEntity);
			Service.BuildingController.DisableUnstashStampingState();
			if (allowRevert)
			{
				this.ShouldRevertMap = true;
			}
		}

		public void UnstashBuildingByUID(string buildingUID, bool returnToOriginalPosition, bool stampable, bool panToBuilding, bool playLoweredSound)
		{
			if (this.stashedBuildingMap == null)
			{
				return;
			}
			if (!this.stashedBuildingMap.ContainsKey(buildingUID) || this.stashedBuildingMap[buildingUID].Count < 1)
			{
				Service.Logger.Error("Can't unstash! No buildings of : " + buildingUID + " currently stashed");
				return;
			}
			List<SmartEntity> list = this.stashedBuildingMap[buildingUID];
			SmartEntity smartEntity = list[0];
			BuildingComponent buildingComp = smartEntity.BuildingComp;
			bool flag = false;
			if (stampable && this.IsBuildingStampable(smartEntity))
			{
				flag = true;
			}
			Position pos = null;
			if (returnToOriginalPosition)
			{
				pos = this.lastSavedMap.GetPosition(buildingComp.BuildingTO.Key);
				if (flag)
				{
					flag = false;
					Service.Logger.Warn("No stamping while reverting!!");
				}
			}
			BuildingController buildingController = Service.BuildingController;
			GameObjectViewComponent gameObjectViewComponent = smartEntity.Get<GameObjectViewComponent>();
			TransformComponent transformComponent = smartEntity.Get<TransformComponent>();
			if (gameObjectViewComponent != null)
			{
				gameObjectViewComponent.SetXYZ(Units.BoardToWorldX(transformComponent.CenterX()), 0f, Units.BoardToWorldZ(transformComponent.CenterZ()));
			}
			if (smartEntity.Has<HealthViewComponent>())
			{
				HealthViewComponent healthViewComponent = smartEntity.Get<HealthViewComponent>();
				healthViewComponent.SetupElements();
			}
			List<SmartEntity> list2 = this.stashedBuildingMap[buildingUID];
			if (list2.Contains(smartEntity))
			{
				list2.Remove(smartEntity);
				if (!buildingController.PositionUnstashedBuilding(smartEntity, pos, flag, panToBuilding, playLoweredSound))
				{
					Service.Logger.ErrorFormat("Unable to place building from stash.  Building {0} {1}", new object[]
					{
						smartEntity.Get<BuildingComponent>().BuildingTO.Key,
						smartEntity.Get<BuildingComponent>().BuildingType.Uid
					});
					Service.UXController.MiscElementsManager.ShowPlayerInstructionsError(Service.Lang.Get("NO_VALID_POSITION_FOR_UNSTASH", new object[0]));
					this.StashBuilding(smartEntity);
				}
			}
		}

		public void StampUnstashBuildingByUID(string buildingUID)
		{
			BuildingController buildingController = Service.BuildingController;
			Entity selectedBuilding = buildingController.SelectedBuilding;
			BoardCell currentCell = selectedBuilding.Get<BoardItemComponent>().BoardItem.CurrentCell;
			buildingController.SaveLastStampLocation(currentCell.X, currentCell.Z);
			if (this.IsListOutOfGivenBuilding(buildingUID))
			{
				buildingController.DisableUnstashStampingState();
			}
			else
			{
				this.UnstashBuildingByUID(buildingUID, false, true, true, true);
			}
		}

		private bool HasBuildingMoved(Building building, Position oldPosition)
		{
			return oldPosition.X != building.X || oldPosition.Z != building.Z;
		}

		public bool IsBuildingClearable(Entity buildingEntity)
		{
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			return buildingComponent.BuildingType.Type == BuildingType.Clearable;
		}

		public bool IsBuildingStampable(Entity buildingEntity)
		{
			return buildingEntity.Get<BuildingComponent>().BuildingType.Type == BuildingType.Wall;
		}

		public bool IsActive()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return currentState is BaseLayoutToolState || currentState is WarBaseEditorState;
		}

		public bool ShouldChecksumLastSaveData()
		{
			bool flag = this.IsBaseLayoutModeActive && !this.IsSavingBaseLayout;
			return flag && !(Service.GameStateMachine.CurrentState is WarBaseEditorState);
		}
	}
}
