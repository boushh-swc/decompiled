using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Move;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class BuildingMover : IUserInputObserver, IEventObserver
	{
		private const float LIFT_DISTANCE_WORLD = 3f;

		private const int FINGER_ID = 0;

		private BuildingSelector buildingSelector;

		private bool lifted;

		private bool canOccupy;

		private Dictionary<Entity, int> prevValidBoardAnchorX;

		private Dictionary<Entity, int> prevValidBoardAnchorZ;

		private Dictionary<Entity, int> prevBoardAnchorX;

		private Dictionary<Entity, int> prevBoardAnchorZ;

		private Vector2 pressScreenPosition;

		private bool dragged;

		private SmartEntity pressedBuilding;

		private bool moved;

		private bool eatDrags;

		private BuildingController buildingController;

		private PlanetView worldView;

		private UserInputInhibitor inhibitor;

		public bool Enabled
		{
			set
			{
				if (value)
				{
					Service.UserInputManager.RegisterObserver(this, UserInputLayer.World);
				}
				else
				{
					Service.UserInputManager.UnregisterObserver(this, UserInputLayer.World);
				}
			}
		}

		public bool Lifted
		{
			get
			{
				return this.lifted;
			}
		}

		public BuildingMover(BuildingSelector buildingSelector)
		{
			this.buildingSelector = buildingSelector;
			this.lifted = false;
			this.canOccupy = false;
			this.prevValidBoardAnchorX = new Dictionary<Entity, int>();
			this.prevValidBoardAnchorZ = new Dictionary<Entity, int>();
			this.prevBoardAnchorX = new Dictionary<Entity, int>();
			this.prevBoardAnchorZ = new Dictionary<Entity, int>();
			this.pressScreenPosition = Vector2.zero;
			this.dragged = false;
			this.pressedBuilding = null;
			this.moved = false;
			this.buildingController = Service.BuildingController;
			this.worldView = Service.WorldInitializer.View;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.UserWantedEditBaseState, EventPriority.Default);
			this.inhibitor = Service.UserInputInhibitor;
		}

		public void OnStartPurchaseBuilding(SmartEntity buildingEntity, bool stampable)
		{
			this.EnsureLoweredLiftedBuilding();
			this.buildingSelector.EnsureDeselectSelectedBuilding();
			this.ResetOnPress(Vector2.zero, Vector3.zero);
			this.buildingSelector.SelectBuilding(buildingEntity, Vector3.zero);
			this.LiftSelectedBuilding(this.buildingSelector.SelectedBuilding, false);
			int cx = 0;
			int cz = 0;
			Service.CameraManager.MainCamera.GetLookatBoardCell(out cx, out cz);
			int boardX;
			int num;
			this.buildingController.FindStartingLocation(buildingEntity, out boardX, out num, cx, cz, stampable);
			float x;
			float z;
			EditBaseController.BuildingBoardToWorld(buildingEntity, boardX, num, out x, out z);
			Vector3 vector = new Vector3(x, 0f, z);
			this.MoveLiftedBuilding(buildingEntity, vector);
			UXController uXController = Service.UXController;
			uXController.MiscElementsManager.ShowConfirmGroup(buildingEntity, new MiscConfirmDelegate(this.OnConfirmPurchase));
			uXController.HUD.ToggleExitEditModeButton(false);
			vector.x = Units.BoardToWorldX(boardX);
			vector.z = Units.BoardToWorldX(num);
			this.worldView.PanToLocation(vector);
			this.LowerLiftedBuilding(DropKind.JustDrop, false, true, true, false);
		}

		public bool UnstashBuilding(SmartEntity buildingEntity, Position pos, bool stampable, bool panToBuilding, bool playLoweredSound)
		{
			this.EnsureLoweredLiftedBuilding();
			this.buildingSelector.EnsureDeselectSelectedBuilding();
			this.ResetOnPress(Vector2.zero, Vector3.zero);
			this.buildingSelector.SelectBuilding(buildingEntity, Vector3.zero);
			this.LiftSelectedBuilding(this.buildingSelector.SelectedBuilding, false);
			int x;
			int z;
			if (pos == null)
			{
				int cx = 0;
				int cz = 0;
				Service.CameraManager.MainCamera.GetLookatBoardCell(out cx, out cz);
				this.buildingController.FindStartingLocation(buildingEntity, out x, out z, cx, cz, stampable);
				UXController uXController = Service.UXController;
				if (stampable)
				{
					uXController.MiscElementsManager.ShowConfirmGroup(buildingEntity, new MiscConfirmDelegate(this.OnConfirmUnstashStamp));
				}
				else
				{
					this.buildingController.DisableUnstashStampingState();
				}
			}
			else
			{
				x = pos.X;
				z = pos.Z;
			}
			float x2;
			float z2;
			EditBaseController.BuildingBoardToWorld(buildingEntity, x, z, out x2, out z2);
			Vector3 vector = new Vector3(x2, 0f, z2);
			this.MoveLiftedBuilding(buildingEntity, vector);
			if (panToBuilding)
			{
				vector.x = Units.BoardToWorldX(x);
				vector.z = Units.BoardToWorldX(z);
				this.worldView.PanToLocation(vector);
			}
			BoardCell boardCell = Service.WorldController.AddBuildingToBoard(buildingEntity, x, z, true);
			if (boardCell != null)
			{
				this.LowerLiftedBuilding(DropKind.JustDrop, true, true, playLoweredSound, false);
			}
			else
			{
				this.lifted = false;
				this.moved = false;
			}
			return boardCell != null;
		}

		public bool CanPlaceBuilding(Entity buildingEntity, out int boardX, out int boardZ)
		{
			int cx = 0;
			int cz = 0;
			Service.CameraManager.MainCamera.GetLookatBoardCell(out cx, out cz);
			return this.buildingController.FindStartingLocation(buildingEntity, out boardX, out boardZ, cx, cz, false);
		}

		public void PlaceNewBuilding(Entity buildingEntity)
		{
			int num;
			int num2;
			bool flag = this.CanPlaceBuilding(buildingEntity, out num, out num2);
			if (flag)
			{
				Service.WorldController.AddBuildingHelper(buildingEntity, num, num2, true);
				BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
				Service.CurrentPlayer.Map.Buildings.Add(buildingComponent.BuildingTO);
				float x;
				float z;
				EditBaseController.BuildingBoardToWorld(buildingEntity, num, num2, out x, out z);
				Vector3 worldLocation = new Vector3(x, 0f, z);
				worldLocation.x = Units.BoardToWorldX(num);
				worldLocation.z = Units.BoardToWorldX(num2);
				this.worldView.PanToLocation(worldLocation);
			}
			else
			{
				Service.Logger.Warn("Unable to place building " + buildingEntity.ID);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.UserWantedEditBaseState)
			{
				if (id != EventId.BuildingViewReady)
				{
					if (id == EventId.GameStateChanged)
					{
						Type type = (Type)cookie;
						if (type == typeof(EditBaseState))
						{
							this.EnsureLoweredLiftedBuilding();
						}
					}
				}
				else
				{
					EntityViewParams entityViewParams = cookie as EntityViewParams;
					if (this.lifted && this.buildingSelector.IsPartOfSelection(entityViewParams.Entity))
					{
						this.LiftSelectedBuilding(entityViewParams.Entity, false, false);
					}
				}
			}
			else if (!(bool)cookie)
			{
				this.eatDrags = true;
			}
			return EatResponse.NotEaten;
		}

		public void DeselectBuildingBeforeCombiningMesh(out SmartEntity selectedBuilding, out Vector3 offset)
		{
			selectedBuilding = this.buildingSelector.SelectedBuilding;
			offset = this.buildingSelector.GrabPoint;
			if (selectedBuilding != null)
			{
				this.buildingSelector.DeselectSelectedBuilding();
			}
		}

		public void SelectBuildingAfterCombiningMesh(SmartEntity selectedBuilding, Vector3 offset)
		{
			if (selectedBuilding != null)
			{
				this.buildingSelector.SelectBuilding(selectedBuilding, offset, true);
			}
		}

		public void AutoLiftSelectedBuilding()
		{
			if (this.buildingSelector.SelectedBuilding != null && !this.lifted)
			{
				this.ClearPreviousAnchors();
				for (int i = 0; i < this.buildingSelector.AdditionalSelectedBuildings.Count; i++)
				{
					SmartEntity smartEntity = this.buildingSelector.AdditionalSelectedBuildings[i];
					this.LiftSelectedBuilding(smartEntity, false, false);
					Service.EventManager.SendEvent(EventId.UserLiftedBuilding, smartEntity);
				}
				this.LiftSelectedBuilding(this.buildingSelector.SelectedBuilding, true, false);
				this.UpdateWallConnectorsInSelection(false);
			}
		}

		private void SwitchAnchorBuildingInSelection(SmartEntity pressedBuilding, Vector3 offset)
		{
			List<SmartEntity> list = new List<SmartEntity>(this.buildingSelector.AdditionalSelectedBuildings);
			list.Add(this.buildingSelector.SelectedBuilding);
			list.Remove(pressedBuilding);
			this.buildingSelector.DeselectSelectedBuilding();
			this.buildingSelector.SelectBuilding(pressedBuilding, offset);
			for (int i = 0; i < list.Count; i++)
			{
				this.buildingSelector.AddBuildingToSelection(list[i]);
			}
			Service.UXController.HUD.ShowContextButtons(pressedBuilding);
		}

		private void DestroyBuilding(Entity building)
		{
			Service.EventManager.SendEvent(EventId.BuildingRemovedFromBoard, building);
			Service.CurrentPlayer.Map.OnRemoveBuildingFromMap();
			Service.EntityFactory.DestroyEntity(building, true, true);
		}

		private void ResetOnPress(Vector2 screenPosition, Vector3 groundPosition)
		{
			this.pressScreenPosition = screenPosition;
			this.dragged = false;
			this.pressedBuilding = null;
			this.moved = false;
			this.eatDrags = false;
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			this.ResetOnPress(screenPosition, groundPosition);
			SmartEntity buildingEntity = this.buildingSelector.GetBuildingEntity(target);
			if (buildingEntity == null)
			{
				return EatResponse.NotEaten;
			}
			Vector3 offset = groundPosition - target.transform.position;
			this.buildingSelector.GrabBuilding(offset);
			if (this.inhibitor.IsAllowable(buildingEntity))
			{
				this.pressedBuilding = buildingEntity;
			}
			if (this.pressedBuilding != null && this.buildingSelector.IsPartOfSelection(this.pressedBuilding))
			{
				if (this.pressedBuilding != this.buildingSelector.SelectedBuilding)
				{
					this.SwitchAnchorBuildingInSelection(this.pressedBuilding, offset);
				}
				if (!this.lifted && GameUtils.IsBuildingMovable(this.buildingSelector.SelectedBuilding))
				{
					this.ClearPreviousAnchors();
					for (int i = 0; i < this.buildingSelector.AdditionalSelectedBuildings.Count; i++)
					{
						SmartEntity smartEntity = this.buildingSelector.AdditionalSelectedBuildings[i];
						this.LiftSelectedBuilding(smartEntity, false, false);
						Service.EventManager.SendEvent(EventId.UserLiftedBuilding, smartEntity);
					}
					this.LiftSelectedBuilding(this.buildingSelector.SelectedBuilding, true, false);
					this.UpdateWallConnectorsInSelection(false);
				}
				return EatResponse.Eaten;
			}
			return EatResponse.NotEaten;
		}

		private void UpdateWallConnectorsInSelection(bool alreadyLifted)
		{
			if (this.buildingSelector.AdditionalSelectedBuildings.Count == 0)
			{
				return;
			}
			WallConnector wallConnector = Service.EntityViewManager.WallConnector;
			wallConnector.ConnectWallsInExclusiveSet(new List<SmartEntity>(this.buildingSelector.AdditionalSelectedBuildings)
			{
				this.buildingSelector.SelectedBuilding
			}, alreadyLifted);
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			if (!this.dragged && CameraUtils.HasDragged(screenPosition, this.pressScreenPosition))
			{
				this.dragged = true;
			}
			SmartEntity buildingEntity = this.buildingSelector.GetBuildingEntity(target);
			if (buildingEntity == null)
			{
				return (!this.eatDrags) ? EatResponse.NotEaten : EatResponse.Eaten;
			}
			if (this.lifted && this.buildingSelector.IsPartOfSelection(buildingEntity))
			{
				if (!this.moved)
				{
					this.moved = true;
					Service.UserInputManager.ReleaseSubordinates(this, UserInputLayer.World, 0);
				}
				for (int i = 0; i < this.buildingSelector.AdditionalSelectedBuildings.Count; i++)
				{
					this.MoveLiftedBuilding(this.buildingSelector.AdditionalSelectedBuildings[i], groundPosition, true);
				}
				this.MoveLiftedBuilding(this.buildingSelector.SelectedBuilding, groundPosition, true);
				this.canOccupy = this.EntireSelectionIsPlaceable();
				if (this.canOccupy)
				{
					this.UpdatePrevValidBoardAnchors();
				}
				return EatResponse.Eaten;
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnRelease(int id)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			bool isPurchasing = this.buildingController.IsPurchasing;
			bool unstashStampingEnabled = this.buildingController.UnstashStampingEnabled;
			if (this.dragged)
			{
				if (this.lifted && this.canOccupy)
				{
					this.LowerLiftedBuilding(DropKind.JustDrop, !isPurchasing, true, true, !isPurchasing);
				}
				else if (this.EntireSelectionOutsideBoard())
				{
					this.LowerLiftedBuilding(DropKind.JustDrop, !isPurchasing, true, true, !isPurchasing);
				}
			}
			else if (this.pressedBuilding == null && !isPurchasing && !unstashStampingEnabled)
			{
				if (this.lifted)
				{
					this.LowerLiftedBuilding(DropKind.JustDrop, true, true, true, !isPurchasing);
				}
				else if (this.buildingSelector.SelectedBuilding != null && !this.inhibitor.IsDenying())
				{
					this.buildingSelector.DeselectSelectedBuilding();
				}
			}
			else if (this.buildingSelector.SelectedBuilding == null && !isPurchasing && !unstashStampingEnabled)
			{
				if (this.inhibitor.IsAllowable(this.pressedBuilding))
				{
					this.buildingSelector.SelectBuilding(this.pressedBuilding, this.buildingSelector.GrabPoint);
					if (Service.BaseLayoutToolController.IsQuickStashModeEnabled && !Service.BaseLayoutToolController.IsBuildingClearable(this.pressedBuilding))
					{
						Service.EventManager.SendEvent(EventId.BuildingQuickStashed, this.pressedBuilding);
					}
				}
			}
			else if (!isPurchasing && !unstashStampingEnabled && !this.buildingSelector.IsPartOfSelection(this.pressedBuilding))
			{
				if (this.lifted)
				{
					this.LowerLiftedBuilding(DropKind.JustDrop, true, true, true, !isPurchasing);
				}
				if (this.buildingSelector.SelectedBuilding != null && !this.inhibitor.IsDenying())
				{
					this.buildingSelector.DeselectSelectedBuilding();
				}
				if (this.inhibitor.IsAllowable(this.pressedBuilding))
				{
					this.buildingSelector.SelectBuilding(this.pressedBuilding, this.buildingSelector.GrabPoint);
					if (Service.BaseLayoutToolController.IsQuickStashModeEnabled && !Service.BaseLayoutToolController.IsBuildingClearable(this.pressedBuilding))
					{
						Service.EventManager.SendEvent(EventId.BuildingQuickStashed, this.pressedBuilding);
					}
				}
			}
			else if (this.lifted)
			{
				if (!isPurchasing || this.canOccupy)
				{
					this.LowerLiftedBuilding(DropKind.JustDrop, !isPurchasing, true, true, !isPurchasing);
				}
				if (Service.BaseLayoutToolController.IsQuickStashModeEnabled && !Service.BaseLayoutToolController.IsBuildingClearable(this.pressedBuilding))
				{
					Service.EventManager.SendEvent(EventId.BuildingQuickStashed, this.pressedBuilding);
				}
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.NotEaten;
		}

		public void EnsureLoweredLiftedBuilding()
		{
			bool isPurchasing = this.buildingController.IsPurchasing;
			if (this.lifted || isPurchasing)
			{
				this.LowerLiftedBuilding((!isPurchasing) ? DropKind.JustDrop : DropKind.CancelPurchase, true, this.lifted, true, !isPurchasing);
			}
		}

		private void OnConfirmPurchase(bool accept)
		{
			this.LowerLiftedBuilding((!accept) ? DropKind.CancelPurchase : DropKind.ConfirmPurchase, true, this.lifted, true, true);
		}

		private void OnConfirmUnstashStamp(bool accept)
		{
			if (this.buildingSelector.SelectedBuilding != null)
			{
				string uid = this.buildingSelector.SelectedBuilding.Get<BuildingComponent>().BuildingType.Uid;
				BuildingController buildingController = Service.BuildingController;
				BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
				UXController uXController = Service.UXController;
				if (!accept)
				{
					this.EnsureLoweredLiftedBuilding();
					baseLayoutToolController.StashBuilding(this.buildingSelector.SelectedBuilding);
					buildingController.EnsureDeselectSelectedBuilding();
					this.buildingController.DisableUnstashStampingState();
					uXController.HUD.BaseLayoutToolView.RefreshStashedBuildingCount(uid);
					return;
				}
				baseLayoutToolController.StampUnstashBuildingByUID(uid);
				uXController.HUD.BaseLayoutToolView.RefreshStashedBuildingCount(uid);
				uXController.HUD.ShowContextButtons(buildingController.SelectedBuilding);
			}
		}

		public void RotateSelectedBuildings(Entity building)
		{
			this.AutoLiftSelectedBuilding();
			float num = Mathf.Sin(-1.57079637f);
			float num2 = Mathf.Cos(-1.57079637f);
			Vector3 grabPoint = this.buildingSelector.GrabPoint;
			float num3;
			float num4;
			EditBaseController.BuildingBoardToWorld(building, 0, 0, out num3, out num4);
			TransformComponent transformComponent = building.Get<TransformComponent>();
			float num5 = (float)transformComponent.X;
			float num6 = (float)transformComponent.Z;
			Vector3 worldGroundPosition = new Vector3(Units.BoardToWorldX(num5), 0f, Units.BoardToWorldX(num6));
			worldGroundPosition.x += num3 + grabPoint.x;
			worldGroundPosition.z += num4 + grabPoint.z;
			int count = this.buildingSelector.AdditionalSelectedBuildings.Count;
			this.MoveLiftedBuilding(building, worldGroundPosition, count > 0);
			for (int i = 0; i < count; i++)
			{
				Entity entity = this.buildingSelector.AdditionalSelectedBuildings[i];
				TransformComponent transformComponent2 = entity.Get<TransformComponent>();
				float num7 = (float)transformComponent2.X + (float)transformComponent2.BoardWidth / 2f - (float)transformComponent.BoardWidth / 2f;
				float num8 = (float)transformComponent2.Z + (float)transformComponent2.BoardDepth / 2f - (float)transformComponent.BoardWidth / 2f;
				float num9 = num7 - num5;
				float num10 = num8 - num6;
				float num11 = num5 + (num9 * num2 - num10 * num);
				float num12 = num6 + (num9 * num + num10 * num2);
				Vector3 worldGroundPosition2 = new Vector3(Units.BoardToWorldX(num11 - num9), 0f, Units.BoardToWorldZ(num12 - num10));
				worldGroundPosition2.x += num3 + grabPoint.x;
				worldGroundPosition2.z += num4 + grabPoint.z;
				this.MoveLiftedBuilding(entity, worldGroundPosition2, true);
			}
			this.canOccupy = this.EntireSelectionIsPlaceable();
			if (this.canOccupy)
			{
				this.UpdatePrevValidBoardAnchors();
				this.LowerLiftedBuilding(DropKind.JustDrop, true, true, true, true);
			}
			else
			{
				this.UpdateWallConnectorsInSelection(true);
			}
		}

		private void UpdatePrevValidBoardAnchors()
		{
			int count = this.buildingSelector.AdditionalSelectedBuildings.Count;
			SmartEntity smartEntity;
			for (int i = 0; i < count; i++)
			{
				smartEntity = this.buildingSelector.AdditionalSelectedBuildings[i];
				this.prevValidBoardAnchorX[smartEntity] = smartEntity.TransformComp.X;
				this.prevValidBoardAnchorZ[smartEntity] = smartEntity.TransformComp.Z;
			}
			smartEntity = this.buildingSelector.SelectedBuilding;
			this.prevValidBoardAnchorX[smartEntity] = smartEntity.TransformComp.X;
			this.prevValidBoardAnchorZ[smartEntity] = smartEntity.TransformComp.Z;
		}

		private void ClearPreviousAnchors()
		{
			this.prevValidBoardAnchorX.Clear();
			this.prevValidBoardAnchorZ.Clear();
			this.prevBoardAnchorX.Clear();
			this.prevBoardAnchorZ.Clear();
		}

		private void LiftSelectedBuilding(SmartEntity buildingInSelection, bool sendLiftedEvent)
		{
			this.LiftSelectedBuilding(buildingInSelection, sendLiftedEvent, true);
		}

		private void LiftSelectedBuilding(SmartEntity buildingInSelection, bool sendLiftedEvent, bool clearPreviousAnchorPos)
		{
			this.lifted = true;
			if (buildingInSelection == this.buildingSelector.SelectedBuilding)
			{
				Service.UXController.HUD.ShowContextButtons(buildingInSelection);
			}
			Vector3 grabPoint = this.buildingSelector.GrabPoint;
			BoardItemComponent boardItemComponent = buildingInSelection.Get<BoardItemComponent>();
			if (boardItemComponent.BoardItem.Filter == CollisionFilters.BUILDING)
			{
				boardItemComponent.BoardItem.Filter = CollisionFilters.BUILDING_GHOST;
			}
			else if (boardItemComponent.BoardItem.Filter == CollisionFilters.TRAP)
			{
				boardItemComponent.BoardItem.Filter = CollisionFilters.TRAP_GHOST;
			}
			else if (boardItemComponent.BoardItem.Filter == CollisionFilters.WALL)
			{
				boardItemComponent.BoardItem.Filter = CollisionFilters.WALL_GHOST;
			}
			else if (boardItemComponent.BoardItem.Filter == CollisionFilters.BLOCKER)
			{
				boardItemComponent.BoardItem.Filter = CollisionFilters.BLOCKER_GHOST;
			}
			else if (boardItemComponent.BoardItem.Filter == CollisionFilters.PLATFORM)
			{
				boardItemComponent.BoardItem.Filter = CollisionFilters.PLATFORM_GHOST;
			}
			this.canOccupy = true;
			if (clearPreviousAnchorPos)
			{
				this.ClearPreviousAnchors();
			}
			TransformComponent transformComp = this.buildingSelector.SelectedBuilding.TransformComp;
			Vector3 worldGroundPosition = new Vector3(Units.BoardToWorldX(transformComp.X), 0f, Units.BoardToWorldZ(transformComp.Z));
			if (sendLiftedEvent)
			{
				Service.EventManager.SendEvent(EventId.UserLiftedBuilding, buildingInSelection);
				Service.EventManager.SendEvent(EventId.UserLiftedBuildingAudio, buildingInSelection);
			}
			float num;
			float num2;
			EditBaseController.BuildingBoardToWorld(this.buildingSelector.SelectedBuilding, 0, 0, out num, out num2);
			worldGroundPosition.x += num + grabPoint.x;
			worldGroundPosition.z += num2 + grabPoint.z;
			this.MoveLiftedBuilding(buildingInSelection, worldGroundPosition);
			Service.EntityViewManager.SetCollider(buildingInSelection, false);
			this.buildingSelector.ApplySelectedEffect(buildingInSelection);
			Service.BuildingTooltipController.HideBuildingTooltip(buildingInSelection);
		}

		private void MoveLiftedBuilding(Entity building, Vector3 worldGroundPosition)
		{
			this.MoveLiftedBuilding(building, worldGroundPosition, false);
		}

		private void MoveLiftedBuilding(Entity building, Vector3 worldGroundPosition, bool isPartOfSelection)
		{
			Entity selectedBuilding = this.buildingSelector.SelectedBuilding;
			TransformComponent transformComponent = building.Get<TransformComponent>();
			TransformComponent transformComponent2 = selectedBuilding.Get<TransformComponent>();
			float num = (float)transformComponent2.X - (float)transformComponent.BoardWidth / 2f + (float)transformComponent2.BoardWidth / 2f;
			float num2 = (float)transformComponent2.Z - (float)transformComponent.BoardDepth / 2f + (float)transformComponent2.BoardDepth / 2f;
			float num3 = (float)transformComponent.X;
			float num4 = (float)transformComponent.Z;
			float num5 = Units.BoardToWorldX(num3 - num);
			float num6 = Units.BoardToWorldZ(num4 - num2);
			Vector3 grabPoint = this.buildingSelector.GrabPoint;
			float num7 = worldGroundPosition.x - grabPoint.x + num5;
			float num8 = worldGroundPosition.z - grabPoint.z + num6;
			float num9;
			float num10;
			EditBaseController.BuildingBoardToWorld(building, 0, 0, out num9, out num10);
			float num11 = num7 - num9;
			float num12 = num8 - num10;
			Units.SnapWorldToGridX(ref num11);
			Units.SnapWorldToGridZ(ref num12);
			int num13 = Units.WorldToBoardX(num11);
			int num14 = Units.WorldToBoardZ(num12);
			transformComponent.X = num13;
			transformComponent.Z = num14;
			int num15 = -2147483648;
			if (this.prevBoardAnchorX.ContainsKey(building))
			{
				num15 = this.prevBoardAnchorX[building];
			}
			int num16 = -2147483648;
			if (this.prevBoardAnchorZ.ContainsKey(building))
			{
				num16 = this.prevBoardAnchorZ[building];
			}
			if (num13 != num15 || num14 != num16)
			{
				this.canOccupy = this.EntireSelectionIsPlaceable();
				Service.UXController.MiscElementsManager.EnableConfirmGroupAcceptButton(this.canOccupy);
				FootprintMoveData cookie = new FootprintMoveData(building, num11, num12, this.canOccupy);
				Service.EventManager.SendEvent(EventId.UserMovedLiftedBuilding, cookie);
			}
			GameObjectViewComponent gameObjectViewComponent = building.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent != null)
			{
				gameObjectViewComponent.SetXYZ(num7, 3f, num8);
			}
			if (this.canOccupy && !isPartOfSelection)
			{
				this.prevValidBoardAnchorX[building] = num13;
				this.prevValidBoardAnchorZ[building] = num14;
			}
		}

		private void OnPayMeForCurrencyResult(object result, object cookie)
		{
			bool flag = GameUtils.HandleSoftCurrencyFlow(result, cookie);
			if (flag)
			{
				Entity selectedBuilding = this.buildingSelector.SelectedBuilding;
				BuildingTypeVO buildingType = selectedBuilding.Get<BuildingComponent>().BuildingType;
				if (buildingType.Type != BuildingType.DroidHut && PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
				{
					return;
				}
			}
			UXController uXController = Service.UXController;
			uXController.HUD.ToggleExitEditModeButton(true);
			this.LowerLiftedBuilding((!flag) ? DropKind.CancelPurchase : DropKind.ConfirmPurchase, true, this.lifted, true, true);
		}

		private void OnPayMeForDroidResult(object result, object cookie)
		{
			bool flag = result != null;
			UXController uXController = Service.UXController;
			uXController.HUD.ToggleExitEditModeButton(true);
			this.LowerLiftedBuilding((!flag) ? DropKind.CancelPurchase : DropKind.ConfirmPurchase, true, this.lifted, true, true);
		}

		private void OnPickPlanetResult(object result, object cookie)
		{
			bool flag = result != null;
			UXController uXController = Service.UXController;
			uXController.HUD.ToggleExitEditModeButton(true);
			string tag = string.Empty;
			PlanetVO planetVO = (PlanetVO)cookie;
			if (planetVO != null)
			{
				tag = planetVO.Uid;
				Service.SharedPlayerPrefs.SetPref("1stPlaName", LangUtils.GetPlanetDisplayNameKey(planetVO.Uid));
			}
			this.LowerLiftedBuildingHelper(this.buildingSelector.SelectedBuilding, (!flag) ? DropKind.CancelPurchase : DropKind.ConfirmPurchase, true, this.lifted, true, true, tag);
			Service.EventManager.SendEvent(EventId.BuildingPurchaseModeEnded, null);
		}

		private void LowerLiftedBuilding(DropKind dropKind, bool affectBoard, bool sendLoweredEvent, bool playLoweredSound, bool showContextButtons)
		{
			SmartEntity selectedBuilding = this.buildingSelector.SelectedBuilding;
			if (selectedBuilding == null)
			{
				this.lifted = false;
				this.moved = false;
				return;
			}
			UXController uXController = Service.UXController;
			if (dropKind != DropKind.JustDrop)
			{
				uXController.MiscElementsManager.HideConfirmGroup();
			}
			if (affectBoard && dropKind == DropKind.ConfirmPurchase)
			{
				BuildingTypeVO buildingType = selectedBuilding.BuildingComp.BuildingType;
				int credits = buildingType.Credits;
				int materials = buildingType.Materials;
				int contraband = buildingType.Contraband;
				string value = StringUtils.ToLowerCaseUnderscoreSeperated(buildingType.Type.ToString());
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append(value);
				stringBuilder.Append("|");
				stringBuilder.Append(buildingType.BuildingID);
				stringBuilder.Append("|");
				stringBuilder.Append(buildingType.Lvl);
				if (PayMeScreen.ShowIfNotEnoughCurrency(credits, materials, contraband, stringBuilder.ToString(), new OnScreenModalResult(this.OnPayMeForCurrencyResult)))
				{
					return;
				}
				if (buildingType.Type != BuildingType.DroidHut && PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
				{
					return;
				}
				if (buildingType.Type == BuildingType.NavigationCenter && buildingType.Lvl == 1)
				{
					Service.UXController.HUD.InitialNavigationCenterPlanetSelect(selectedBuilding, buildingType, new OnScreenModalResult(this.OnPickPlanetResult));
					return;
				}
			}
			BuildingTypeVO buildingType2 = selectedBuilding.Get<BuildingComponent>().BuildingType;
			if (buildingType2.Time == 0 && dropKind != DropKind.JustDrop)
			{
				showContextButtons = false;
			}
			if (dropKind != DropKind.JustDrop)
			{
				uXController.HUD.ToggleExitEditModeButton(true);
			}
			this.canOccupy = this.EntireSelectionIsPlaceable();
			if (!this.canOccupy)
			{
				this.LiftSelectedBuilding(selectedBuilding, false, false);
				int i = 0;
				int count = this.buildingSelector.AdditionalSelectedBuildings.Count;
				while (i < count)
				{
					this.LiftSelectedBuilding(this.buildingSelector.AdditionalSelectedBuildings[i], false, false);
					i++;
				}
			}
			bool flag = !this.LowerLiftedBuildingHelper(selectedBuilding, dropKind, affectBoard, sendLoweredEvent, playLoweredSound, showContextButtons, string.Empty);
			int j = 0;
			int count2 = this.buildingSelector.AdditionalSelectedBuildings.Count;
			while (j < count2)
			{
				this.LowerLiftedBuildingHelper(this.buildingSelector.AdditionalSelectedBuildings[j], dropKind, affectBoard, false, false, false, string.Empty);
				j++;
			}
			if (sendLoweredEvent)
			{
				for (int k = 0; k < this.buildingSelector.AdditionalSelectedBuildings.Count; k++)
				{
					Service.EventManager.SendEvent(EventId.UserLoweredBuilding, this.buildingSelector.AdditionalSelectedBuildings[k]);
				}
				Service.EventManager.SendEvent(EventId.UserLoweredBuilding, flag ? null : selectedBuilding);
			}
			if (affectBoard && dropKind == DropKind.JustDrop && this.ShouldSaveAfterEveryMove())
			{
				BuildingMultiMoveRequest buildingMultiMoveRequest = new BuildingMultiMoveRequest();
				buildingMultiMoveRequest.PositionMap = new PositionMap();
				Position position = new Position();
				position.X = selectedBuilding.Get<TransformComponent>().X;
				position.Z = selectedBuilding.Get<TransformComponent>().Z;
				buildingMultiMoveRequest.PositionMap.AddPosition(selectedBuilding.Get<BuildingComponent>().BuildingTO.Key, position);
				for (int l = 0; l < this.buildingSelector.AdditionalSelectedBuildings.Count; l++)
				{
					Entity entity = this.buildingSelector.AdditionalSelectedBuildings[l];
					position = new Position();
					position.X = entity.Get<TransformComponent>().X;
					position.Z = entity.Get<TransformComponent>().Z;
					buildingMultiMoveRequest.PositionMap.AddPosition(entity.Get<BuildingComponent>().BuildingTO.Key, position);
				}
				BuildingMultiMoveCommand command = new BuildingMultiMoveCommand(buildingMultiMoveRequest);
				Service.ServerAPI.Sync(command);
			}
			if (dropKind == DropKind.ConfirmPurchase || dropKind == DropKind.CancelPurchase)
			{
				Service.EventManager.SendEvent(EventId.BuildingPurchaseModeEnded, null);
			}
		}

		private bool ShouldSaveAfterEveryMove()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return !(currentState is BaseLayoutToolState) && !(currentState is WarBaseEditorState);
		}

		private bool EntireSelectionIsPlaceable()
		{
			bool flag = this.BuildingIsPlacable(this.buildingSelector.SelectedBuilding);
			if (!flag)
			{
				return flag;
			}
			int count = this.buildingSelector.AdditionalSelectedBuildings.Count;
			for (int i = 0; i < count; i++)
			{
				flag = this.BuildingIsPlacable(this.buildingSelector.AdditionalSelectedBuildings[i]);
				if (!flag)
				{
					break;
				}
			}
			return flag;
		}

		private bool EntireSelectionOutsideBoard()
		{
			if (this.buildingSelector.SelectedBuilding == null)
			{
				return false;
			}
			if (this.BuildingCanFit(this.buildingSelector.SelectedBuilding))
			{
				return false;
			}
			int i = 0;
			int count = this.buildingSelector.AdditionalSelectedBuildings.Count;
			while (i < count)
			{
				if (this.BuildingCanFit(this.buildingSelector.AdditionalSelectedBuildings[i]))
				{
					return false;
				}
				i++;
			}
			return true;
		}

		private bool BuildingIsPlacable(Entity building)
		{
			bool checkSkirt = building.Get<BuildingComponent>().BuildingType.Type != BuildingType.Blocker;
			BoardItemComponent boardItemComponent = building.Get<BoardItemComponent>();
			TransformComponent transformComponent = building.Get<TransformComponent>();
			return Service.BoardController.Board.CanOccupy(boardItemComponent.BoardItem, transformComponent.X, transformComponent.Z, checkSkirt);
		}

		private bool BuildingCanFit(Entity building)
		{
			bool result = false;
			if (building != null)
			{
				BoardItemComponent boardItemComponent = building.Get<BoardItemComponent>();
				TransformComponent transformComponent = building.Get<TransformComponent>();
				result = Service.BoardController.Board.FitsAt(boardItemComponent.BoardItem, transformComponent.X, transformComponent.Z, 1);
			}
			return result;
		}

		private bool LowerLiftedBuildingHelper(SmartEntity buildingInSelection, DropKind dropKind, bool affectBoard, bool sendLoweredEvent, bool playLoweredSound, bool showContextButtons, string tag)
		{
			this.lifted = false;
			this.moved = false;
			BuildingTypeVO buildingTypeVO = null;
			int num = 0;
			int num2 = 0;
			if ((this.canOccupy || dropKind != DropKind.JustDrop) && this.prevValidBoardAnchorX.ContainsKey(buildingInSelection) && this.prevValidBoardAnchorZ.ContainsKey(buildingInSelection))
			{
				num = this.prevValidBoardAnchorX[buildingInSelection];
				num2 = this.prevValidBoardAnchorZ[buildingInSelection];
			}
			else if (this.prevValidBoardAnchorX.ContainsKey(buildingInSelection) && this.prevValidBoardAnchorZ.ContainsKey(buildingInSelection))
			{
				num = this.prevValidBoardAnchorX[buildingInSelection];
				num2 = this.prevValidBoardAnchorZ[buildingInSelection];
			}
			else if (this.buildingSelector.AdditionalSelectedBuildings.Count == 0)
			{
				if (buildingInSelection != null)
				{
					Service.Logger.Warn("Something went wrong placing " + buildingInSelection.ToString() + " we should not be hitting this case where prevValidBoardAnchorX and prevValidBoardAnchorZ do not contain buildingInSelection");
				}
				else
				{
					Service.Logger.Warn("Something went wrong, we should not be hitting this case where prevValidBoardAnchorX and prevValidBoardAnchorZ do not contain buildingInSelection");
				}
			}
			if (affectBoard)
			{
				BoardCell boardCell = this.buildingController.OnLowerLiftedBuilding(buildingInSelection, num, num2, dropKind == DropKind.ConfirmPurchase, ref buildingTypeVO, tag);
				if (boardCell == null)
				{
					this.buildingSelector.DeselectSelectedBuilding();
					Service.EventManager.SendEvent(EventId.UserLoweredBuilding, buildingInSelection);
					this.DestroyBuilding(buildingInSelection);
					return false;
				}
				BoardItemComponent boardItemComponent = buildingInSelection.Get<BoardItemComponent>();
				if (boardItemComponent.BoardItem.Filter == CollisionFilters.BUILDING_GHOST)
				{
					boardItemComponent.BoardItem.Filter = CollisionFilters.BUILDING;
				}
				else if (boardItemComponent.BoardItem.Filter == CollisionFilters.TRAP_GHOST)
				{
					boardItemComponent.BoardItem.Filter = CollisionFilters.TRAP;
				}
				else if (boardItemComponent.BoardItem.Filter == CollisionFilters.WALL_GHOST)
				{
					boardItemComponent.BoardItem.Filter = CollisionFilters.WALL;
				}
				else if (boardItemComponent.BoardItem.Filter == CollisionFilters.BLOCKER_GHOST)
				{
					boardItemComponent.BoardItem.Filter = CollisionFilters.BLOCKER;
				}
				else if (boardItemComponent.BoardItem.Filter == CollisionFilters.PLATFORM_GHOST)
				{
					boardItemComponent.BoardItem.Filter = CollisionFilters.PLATFORM;
				}
				else
				{
					Service.Logger.ErrorFormat("LowerLiftedBuilding : Unexpected filter type {0}", new object[]
					{
						boardItemComponent.BoardItem.Filter
					});
					boardItemComponent.BoardItem.Filter = CollisionFilters.BUILDING;
				}
				num = boardCell.X;
				num2 = boardCell.Z;
			}
			else
			{
				float x;
				float z;
				EditBaseController.BuildingBoardToWorld(this.buildingSelector.SelectedBuilding, num, num2, out x, out z);
				Vector3 worldGroundPosition = new Vector3(x, 0f, z);
				worldGroundPosition.x += this.buildingSelector.GrabPoint.x;
				worldGroundPosition.z += this.buildingSelector.GrabPoint.z;
				this.MoveLiftedBuilding(this.buildingSelector.SelectedBuilding, worldGroundPosition);
			}
			GameObjectViewComponent gameObjectViewComponent = buildingInSelection.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent != null)
			{
				TransformComponent transformComponent = buildingInSelection.Get<TransformComponent>();
				gameObjectViewComponent.SetXYZ(Units.BoardToWorldX(transformComponent.CenterX()), 0f, Units.BoardToWorldZ(transformComponent.CenterZ()));
			}
			Service.EntityViewManager.SetCollider(buildingInSelection, true);
			if (sendLoweredEvent)
			{
				Service.EventManager.SendEvent(EventId.UserLoweredBuilding, buildingInSelection);
			}
			if (playLoweredSound)
			{
				Service.EventManager.SendEvent(EventId.UserLoweredBuildingAudio, buildingInSelection);
			}
			this.buildingSelector.ApplySelectedEffect(buildingInSelection);
			if (buildingTypeVO != null)
			{
				this.buildingController.StartPurchaseBuilding(buildingTypeVO, -1);
			}
			else if (showContextButtons)
			{
				Service.UXController.HUD.ShowContextButtons(buildingInSelection);
			}
			Service.BuildingTooltipController.EnsureBuildingTooltip(buildingInSelection);
			return true;
		}
	}
}
