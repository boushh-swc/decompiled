using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.UserInput;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class BuildingSelector : IEventObserver, IUserInputObserver
	{
		private const int FINGER_ID = 0;

		public const float LIFT_TIMEOUT = 1f;

		public const float EDIT_MODE_TIMEOUT = 1f;

		private const float PULSATE_FREQUENCY = 1f;

		private Entity selectedBuilding;

		private List<Entity> additionalSelectedBuildings;

		private Vector3 grabPoint;

		private Vector2 pressScreenPosition;

		private bool dragged;

		private uint liftTimerId;

		private Entity liftingBuilding;

		private uint editModeTimerId;

		private DynamicRadiusView radiusView;

		private BuildingController buildingController;

		private LongPressView longPress;

		public bool Enabled
		{
			set
			{
				if (value)
				{
					this.EnsureRadiusView();
					Service.UserInputManager.RegisterObserver(this, UserInputLayer.World);
				}
				else
				{
					Service.UserInputManager.UnregisterObserver(this, UserInputLayer.World);
				}
			}
		}

		public Entity SelectedBuilding
		{
			get
			{
				return this.selectedBuilding;
			}
		}

		public List<Entity> AdditionalSelectedBuildings
		{
			get
			{
				return this.additionalSelectedBuildings;
			}
		}

		public Vector3 GrabPoint
		{
			get
			{
				return this.grabPoint;
			}
		}

		public BuildingSelector()
		{
			this.selectedBuilding = null;
			this.grabPoint = Vector3.zero;
			this.pressScreenPosition = Vector2.zero;
			this.dragged = false;
			this.liftTimerId = 0u;
			this.liftingBuilding = null;
			this.editModeTimerId = 0u;
			this.radiusView = null;
			this.buildingController = Service.BuildingController;
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			this.longPress = new LongPressView();
			this.additionalSelectedBuildings = new List<Entity>();
		}

		public bool IsPartOfSelection(Entity building)
		{
			return building == this.selectedBuilding || this.additionalSelectedBuildings.Contains(building);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.BuildingViewReady)
			{
				if (id == EventId.GameStateChanged)
				{
					Type type = (Type)cookie;
					IState currentState = Service.GameStateMachine.CurrentState;
					if ((type != typeof(HomeState) || !(currentState is EditBaseState)) && (type != typeof(EditBaseState) || !(currentState is HomeState)))
					{
						this.EnsureDeselectSelectedBuilding();
					}
					Entity entity = (!(currentState is HomeState) && !(currentState is EditBaseState)) ? null : this.selectedBuilding;
					Service.UXController.HUD.ShowContextButtons(entity);
				}
			}
			else
			{
				EntityViewParams entityViewParams = cookie as EntityViewParams;
				if (this.IsPartOfSelection(entityViewParams.Entity))
				{
					this.ApplySelectedEffect(entityViewParams.Entity);
				}
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnPress(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			this.pressScreenPosition = screenPosition;
			this.dragged = false;
			Entity buildingEntity = this.GetBuildingEntity(target);
			if (buildingEntity == null)
			{
				this.StartEditModeTimer(groundPosition);
				return EatResponse.NotEaten;
			}
			if (!Service.UserInputInhibitor.IsAllowable(buildingEntity))
			{
				return EatResponse.NotEaten;
			}
			Vector3 offset = groundPosition - target.transform.position;
			this.GrabBuilding(offset);
			this.StartLiftingBuilding(buildingEntity);
			return EatResponse.NotEaten;
		}

		public EatResponse OnDrag(int id, GameObject target, Vector2 screenPosition, Vector3 groundPosition)
		{
			if (id != 0)
			{
				this.CancelEditModeTimer();
				return EatResponse.NotEaten;
			}
			if (!this.dragged && CameraUtils.HasDragged(screenPosition, this.pressScreenPosition))
			{
				this.dragged = true;
				this.CancelEditModeTimer();
			}
			return EatResponse.NotEaten;
		}

		public EatResponse OnRelease(int id)
		{
			if (id != 0)
			{
				return EatResponse.NotEaten;
			}
			if (!this.dragged && !this.buildingController.IsPurchasing)
			{
				if (this.liftingBuilding == null)
				{
					this.CancelEditModeTimer();
					if (this.selectedBuilding != null && !Service.UserInputInhibitor.IsDenying())
					{
						this.DeselectSelectedBuilding();
					}
				}
				else
				{
					Entity entity = this.liftingBuilding;
					this.CancelLiftingBuilding();
					if (this.selectedBuilding != null && Service.UserInputInhibitor.IsAllowable(entity))
					{
						if (entity != this.selectedBuilding && !this.additionalSelectedBuildings.Contains(entity))
						{
							this.DeselectSelectedBuilding();
						}
						else if (!Service.ICurrencyController.TryCollectCurrencyOnSelection(entity))
						{
							Service.EventManager.SendEvent(EventId.BuildingSelected, this.selectedBuilding);
							Service.EventManager.SendEvent(EventId.BuildingSelectedSound, this.selectedBuilding);
						}
					}
					if (this.selectedBuilding == null)
					{
						if (!Service.ICurrencyController.TryCollectCurrencyOnSelection(entity) && this.CanSelectBuilding((SmartEntity)entity))
						{
							this.SelectBuilding(entity, this.grabPoint);
						}
					}
					else if (!Service.UserInputInhibitor.IsDenying())
					{
						this.DeselectSelectedBuilding();
					}
				}
			}
			return EatResponse.NotEaten;
		}

		private bool CanSelectBuilding(SmartEntity building)
		{
			bool result = false;
			if (building != null)
			{
				result = true;
				if (GameUtils.IsVisitingBase())
				{
					bool flag = building.TrapComp != null;
					if (flag)
					{
						result = false;
					}
				}
			}
			return result;
		}

		public EatResponse OnScroll(float delta, Vector2 screenPosition)
		{
			return EatResponse.NotEaten;
		}

		public Entity GetBuildingEntity(GameObject target)
		{
			if (target == null)
			{
				return null;
			}
			EntityRef component = target.GetComponent<EntityRef>();
			if (component == null)
			{
				return null;
			}
			if (component.Entity.Get<BuildingComponent>() == null)
			{
				return null;
			}
			return component.Entity;
		}

		public void SelectAdjacentWalls(Entity building)
		{
			WallConnector wallConnector = Service.EntityViewManager.WallConnector;
			List<Entity> wallChains = wallConnector.GetWallChains(building, -1, 0);
			List<Entity> wallChains2 = wallConnector.GetWallChains(building, 1, 0);
			List<Entity> wallChains3 = wallConnector.GetWallChains(building, 0, -1);
			List<Entity> wallChains4 = wallConnector.GetWallChains(building, 0, 1);
			List<Entity> list = new List<Entity>();
			if (wallChains.Count + wallChains2.Count > wallChains3.Count + wallChains4.Count)
			{
				list.AddRange(wallChains);
				list.AddRange(wallChains2);
			}
			else
			{
				list.AddRange(wallChains4);
				list.AddRange(wallChains3);
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.AddBuildingToSelection(list[i]);
			}
		}

		public void AddBuildingToSelection(Entity building)
		{
			if (this.selectedBuilding == null)
			{
				throw new Exception("Can't select additional buildings until a root building is selected!");
			}
			if (!this.additionalSelectedBuildings.Contains(building))
			{
				this.additionalSelectedBuildings.Add(building);
				this.ApplySelectedEffect(building);
				if (building.Has<SupportViewComponent>())
				{
					building.Get<SupportViewComponent>().UpdateSelected(true);
				}
			}
		}

		public void SelectBuilding(Entity building, Vector3 offset)
		{
			this.SelectBuilding(building, offset, false);
		}

		public void SelectBuilding(Entity building, Vector3 offset, bool silent)
		{
			if (this.selectedBuilding != null)
			{
				throw new Exception("Must deselect old building before selecting a new one");
			}
			this.selectedBuilding = building;
			this.GrabBuilding(offset);
			this.ApplySelectedEffect(this.selectedBuilding);
			Service.UXController.HUD.ShowContextButtons(this.selectedBuilding);
			if (this.selectedBuilding.Has<SupportViewComponent>())
			{
				this.selectedBuilding.Get<SupportViewComponent>().UpdateSelected(true);
			}
			Service.EventManager.SendEvent(EventId.BuildingSelected, this.selectedBuilding);
			if (!silent)
			{
				Service.EventManager.SendEvent(EventId.BuildingSelectedSound, this.selectedBuilding);
			}
		}

		public void GrabBuilding(Vector3 offset)
		{
			this.grabPoint = offset;
			this.grabPoint.y = 0f;
		}

		public void RemoveBuildingFromCurrentSelection(Entity building)
		{
			if (this.additionalSelectedBuildings.Contains(building))
			{
				this.additionalSelectedBuildings.Remove(building);
				this.ApplyDeselectedEffect(building);
				Service.EventManager.SendEvent(EventId.BuildingDeselected, building);
			}
			else if (this.selectedBuilding == building)
			{
				List<Entity> list = new List<Entity>();
				Entity entity = null;
				if (this.additionalSelectedBuildings.Count > 0)
				{
					entity = this.additionalSelectedBuildings[0];
					list.AddRange(this.additionalSelectedBuildings.GetRange(1, this.additionalSelectedBuildings.Count - 1));
				}
				this.DeselectSelectedBuilding();
				if (entity != null)
				{
					this.SelectBuilding(entity, Vector3.zero);
					for (int i = 0; i < list.Count; i++)
					{
						this.AddBuildingToSelection(list[i]);
					}
				}
			}
		}

		public void DeselectSelectedBuilding()
		{
			if (this.selectedBuilding == null)
			{
				throw new Exception("Must have a selected building in order to deselect it");
			}
			Service.UXController.HUD.ShowContextButtons(null);
			Service.EventManager.SendEvent(EventId.BuildingDeselected, this.selectedBuilding);
			Entity entity = this.selectedBuilding;
			this.selectedBuilding = null;
			this.DeselectGroup();
			this.ApplyDeselectedEffect(entity);
			if (entity.Has<SupportViewComponent>())
			{
				entity.Get<SupportViewComponent>().UpdateSelected(false);
			}
		}

		private void DeselectGroup()
		{
			for (int i = 0; i < this.additionalSelectedBuildings.Count; i++)
			{
				Entity entity = this.additionalSelectedBuildings[i];
				this.ApplyDeselectedEffect(entity);
				Service.EventManager.SendEvent(EventId.BuildingDeselected, entity);
				if (entity.Has<SupportViewComponent>())
				{
					entity.Get<SupportViewComponent>().UpdateSelected(false);
				}
			}
			this.additionalSelectedBuildings.Clear();
		}

		public void EnsureDeselectSelectedBuilding()
		{
			if (this.liftingBuilding != null)
			{
				this.CancelLiftingBuilding();
			}
			if (this.selectedBuilding != null)
			{
				this.DeselectSelectedBuilding();
			}
		}

		private void StartLiftingBuilding(Entity building)
		{
			if (!Service.UserInputInhibitor.IsDenying())
			{
				this.ApplyLiftingEffect(building);
				this.liftTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.OnLiftTimer), null);
			}
			this.liftingBuilding = building;
		}

		private void StartEditModeTimer(Vector3 groundPosition)
		{
			if (!Service.UserInputInhibitor.IsDenying() && !GameUtils.IsVisitingBase())
			{
				this.longPress.StartTimer(groundPosition);
				this.editModeTimerId = Service.ViewTimerManager.CreateViewTimer(1f, false, new TimerDelegate(this.OnEditModeTimer), null);
			}
		}

		public void CancelEditModeTimer()
		{
			if (this.editModeTimerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.editModeTimerId);
				this.editModeTimerId = 0u;
			}
			this.longPress.KillTimer();
			if (this.liftingBuilding != null)
			{
				this.CancelLiftingBuilding();
			}
		}

		private void CancelLiftingBuilding()
		{
			if (this.liftingBuilding == this.selectedBuilding)
			{
				this.ApplySelectedEffect(this.liftingBuilding);
			}
			else
			{
				this.ApplyDeselectedEffect(this.liftingBuilding);
			}
			Service.ViewTimerManager.KillViewTimer(this.liftTimerId);
			this.longPress.KillTimer();
			this.liftTimerId = 0u;
			this.liftingBuilding = null;
		}

		private void OnEditModeTimer(uint id, object cookie)
		{
			this.longPress.KillTimer();
			Service.EventManager.SendEvent(EventId.UserWantedEditBaseState, false);
		}

		private void OnLiftTimer(uint id, object cookie)
		{
			if (id == this.liftTimerId && this.liftingBuilding != null)
			{
				if (this.liftingBuilding != this.selectedBuilding)
				{
					if (this.selectedBuilding != null)
					{
						this.DeselectSelectedBuilding();
					}
					this.SelectBuilding(this.liftingBuilding, this.grabPoint);
				}
				else
				{
					this.ApplySelectedEffect(this.liftingBuilding);
				}
				Service.EventManager.SendEvent(EventId.UserWantedEditBaseState, true);
				this.liftTimerId = 0u;
				this.liftingBuilding = null;
			}
		}

		private void EnsureRadiusView()
		{
			if (this.radiusView == null)
			{
				this.radiusView = new DynamicRadiusView();
			}
		}

		public void RedrawRadiusView(Entity building)
		{
			this.EnsureRadiusView();
			this.radiusView.HideHighlight();
			this.radiusView.ShowHighlight(building);
		}

		public void ApplySelectedEffect(Entity building)
		{
			this.buildingController.HighlightBuilding(building);
			this.RedrawRadiusView(building);
			this.longPress.KillTimer();
		}

		private void ApplyDeselectedEffect(Entity building)
		{
			this.buildingController.ClearBuildingHighlight(building);
			this.buildingController.UpdateBuildingHighlightForPerks(building);
			this.EnsureRadiusView();
			this.radiusView.HideHighlight(building);
		}

		private void ApplyLiftingEffect(Entity building)
		{
			if (!GameUtils.IsVisitingBase())
			{
				this.longPress.StartTimer(building);
			}
		}
	}
}
