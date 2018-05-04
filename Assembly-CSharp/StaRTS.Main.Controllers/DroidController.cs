using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Pathfinding;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class DroidController : IEventObserver
	{
		private string DROID_UID = "civilianWorkerDroid01";

		private EntityController entityController;

		public DroidController()
		{
			this.entityController = Service.EntityController;
			Service.DroidController = this;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.DroidViewReady, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.InventoryResourceUpdated, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ClearableStarted, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ClearableCleared, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ChampionRepaired, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingPurchaseSuccess, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingLevelUpgraded, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingStartedUpgrading, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ChampionStartedRepairing, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingSwapped, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingCancelled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.TroopCancelled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BattleLoadStart, EventPriority.Default);
		}

		public Entity GetDroidHut()
		{
			NodeList<DroidHutNode> nodeList = this.entityController.GetNodeList<DroidHutNode>();
			DroidHutNode head = nodeList.Head;
			return (head == null) ? null : head.Entity;
		}

		private Entity CreateDroid(CivilianTypeVO droidType)
		{
			Entity droidHut = this.GetDroidHut();
			if (droidHut == null)
			{
				return null;
			}
			TransformComponent transformComponent = droidHut.Get<TransformComponent>();
			IntPosition position = new IntPosition(transformComponent.X - 1, transformComponent.Z - 1);
			Entity entity = Service.EntityFactory.CreateDroidEntity(droidType, position);
			BoardItemComponent boardItemComponent = entity.Get<BoardItemComponent>();
			Service.BoardController.Board.AddChild(boardItemComponent.BoardItem, position.x, position.z, null, false);
			Service.EntityController.AddEntity(entity);
			return entity;
		}

		public void DestroyDroid(DroidNode droid)
		{
			Service.EntityFactory.DestroyEntity(droid.Entity, true, false);
		}

		private void DestroyAllDroids()
		{
			List<DroidNode> list = new List<DroidNode>();
			NodeList<DroidNode> nodeList = Service.EntityController.GetNodeList<DroidNode>();
			for (DroidNode droidNode = nodeList.Head; droidNode != null; droidNode = droidNode.Next)
			{
				list.Add(droidNode);
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				this.DestroyDroid(list[i]);
			}
		}

		public void HideAllNonClearableDroids()
		{
			List<DroidNode> list = new List<DroidNode>();
			NodeList<DroidNode> nodeList = Service.EntityController.GetNodeList<DroidNode>();
			for (DroidNode droidNode = nodeList.Head; droidNode != null; droidNode = droidNode.Next)
			{
				BuildingType type = droidNode.Droid.Target.Get<BuildingComponent>().BuildingType.Type;
				if (type != BuildingType.Clearable)
				{
					list.Add(droidNode);
				}
			}
			for (int i = list.Count - 1; i >= 0; i--)
			{
				list[i].Entity.Get<GameObjectViewComponent>().MainGameObject.SetActive(false);
			}
		}

		public void ShowAllDroids()
		{
			NodeList<DroidNode> nodeList = Service.EntityController.GetNodeList<DroidNode>();
			for (DroidNode droidNode = nodeList.Head; droidNode != null; droidNode = droidNode.Next)
			{
				droidNode.Entity.Get<GameObjectViewComponent>().MainGameObject.SetActive(true);
				this.AssignDroidPath(droidNode);
			}
		}

		private Entity FindIdleDroid(CivilianTypeVO droidType)
		{
			Entity droidHut = this.GetDroidHut();
			NodeList<DroidNode> nodeList = Service.EntityController.GetNodeList<DroidNode>();
			for (DroidNode droidNode = nodeList.Head; droidNode != null; droidNode = droidNode.Next)
			{
				if (droidNode.Droid.Target == droidHut || droidNode.Droid.Target == null)
				{
					return droidNode.Entity;
				}
			}
			return this.CreateDroid(droidType);
		}

		public bool UpdateDroidTransform(DroidNode droid, float dt)
		{
			if (droid != null && droid.IsValid() && droid.Entity != null)
			{
				PathingComponent pathingComponent = droid.Entity.Get<PathingComponent>();
				if (pathingComponent != null)
				{
					if ((ulong)pathingComponent.TimeOnSegment > (ulong)((long)pathingComponent.TimeToMove) && this.ShouldCalculatePath(droid))
					{
						droid.State.CurState = EntityState.Moving;
						this.AssignDroidPath(droid);
						return false;
					}
					return pathingComponent.CurrentPath == null;
				}
			}
			return false;
		}

		private bool ShouldCalculatePath(DroidNode node)
		{
			TransformComponent transformComponent = node.Droid.Target.Get<TransformComponent>();
			if (transformComponent == null)
			{
				return false;
			}
			PathingComponent pathingComponent = node.Entity.Get<PathingComponent>();
			if (pathingComponent == null)
			{
				return false;
			}
			Path currentPath = pathingComponent.CurrentPath;
			if (currentPath == null)
			{
				return false;
			}
			if (currentPath.TurnCount == 0)
			{
				return false;
			}
			BoardCell turn = currentPath.GetTurn(currentPath.TurnCount - 1);
			int num = turn.X - transformComponent.X;
			int num2 = turn.Z - transformComponent.Z;
			return num < -1 || num > transformComponent.BoardWidth || num2 < -1 || num2 > transformComponent.BoardDepth;
		}

		public void AssignDroidPath(DroidNode droid)
		{
			if (droid != null && droid.IsValid())
			{
				int x = droid.Transform.X;
				int z = droid.Transform.Z;
				BoardController boardController = Service.BoardController;
				BoardCell startCell = boardController.Board.GetCellAt(x, z);
				SmartEntity smartEntity = (SmartEntity)droid.Droid.Target;
				TransformComponent transformComp = smartEntity.TransformComp;
				if (transformComp == null)
				{
					return;
				}
				int num = transformComp.X - 1;
				int num2 = transformComp.Z - 1;
				int num3 = Service.Rand.ViewRangeInt(0, transformComp.BoardWidth + transformComp.BoardDepth + 1);
				if (num3 <= transformComp.BoardWidth)
				{
					num += num3;
				}
				else
				{
					num2 += num3 - transformComp.BoardWidth;
				}
				BoardCell cellAt = boardController.Board.GetCellAt(num, num2);
				if (!droid.Droid.AnimateTravel)
				{
					startCell = cellAt;
				}
				if (cellAt != null)
				{
					Service.PathingManager.StartPathingWorkerOrPatrol((SmartEntity)droid.Entity, smartEntity, startCell, cellAt, droid.Size.Width, true);
				}
				droid.Droid.AnimateTravel = true;
			}
		}

		private void AssignWorkToDroid(Entity building, bool hasScaffolding, bool forceCreate, bool animateTravel)
		{
			CivilianTypeVO droidType = Service.StaticDataController.Get<CivilianTypeVO>(this.DROID_UID);
			Entity entity = (!forceCreate) ? this.FindIdleDroid(droidType) : this.CreateDroid(droidType);
			if (entity == null || building == null)
			{
				return;
			}
			BuildingComponent buildingComponent = building.Get<BuildingComponent>();
			if (buildingComponent == null || buildingComponent.BuildingType.Type == BuildingType.Wall)
			{
				return;
			}
			entity.Get<DroidComponent>().AnimateTravel = animateTravel;
			entity.Get<DroidComponent>().Target = building;
		}

		private void RemoveWorkFromDroid(Entity building, bool hasScaffolding)
		{
			NodeList<DroidNode> nodeList = Service.EntityController.GetNodeList<DroidNode>();
			for (DroidNode droidNode = nodeList.Head; droidNode != null; droidNode = droidNode.Next)
			{
				if (droidNode.Droid.Target == building)
				{
					droidNode.Droid.AnimateTravel = true;
					droidNode.Droid.Target = this.GetDroidHut();
					droidNode.State.CurState = EntityState.Moving;
					this.AssignDroidPath(droidNode);
					break;
				}
			}
		}

		private void InitializeDroids()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int num = currentPlayer.CurrentDroidsAmount;
			NodeList<SupportViewNode> nodeList = Service.EntityController.GetNodeList<SupportViewNode>();
			ISupportController iSupportController = Service.ISupportController;
			for (SupportViewNode supportViewNode = nodeList.Head; supportViewNode != null; supportViewNode = supportViewNode.Next)
			{
				Contract contract = iSupportController.FindCurrentContract(supportViewNode.Entity.Get<BuildingComponent>().BuildingTO.Key);
				if (contract != null)
				{
					DeliveryType deliveryType = contract.DeliveryType;
					if (deliveryType == DeliveryType.SwapBuilding || deliveryType == DeliveryType.UpgradeBuilding || deliveryType == DeliveryType.Building)
					{
						this.AssignWorkToDroid(supportViewNode.Entity, true, true, false);
						num--;
					}
					else if (deliveryType == DeliveryType.ClearClearable || deliveryType == DeliveryType.Champion)
					{
						this.AssignWorkToDroid(supportViewNode.Entity, false, true, false);
						num--;
					}
				}
			}
		}

		private void PrepareDroid(Entity droidEntity)
		{
			Entity droidHut = this.GetDroidHut();
			if (droidHut == null)
			{
				Service.EntityFactory.DestroyEntity(droidEntity, true, false);
				return;
			}
			DroidComponent droidComponent = droidEntity.Get<DroidComponent>();
			if (droidComponent.Target == null)
			{
				droidComponent.Target = droidHut;
			}
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.DroidViewReady:
			{
				EntityViewParams entityViewParams = cookie as EntityViewParams;
				this.PrepareDroid(entityViewParams.Entity);
				return EatResponse.NotEaten;
			}
			case EventId.BuildingViewReady:
			case EventId.BuildingViewFailed:
			{
				IL_20:
				switch (id)
				{
				case EventId.BuildingPurchaseSuccess:
				case EventId.BuildingStartedUpgrading:
					goto IL_137;
				case EventId.BuildingPurchaseModeStarted:
				case EventId.BuildingPurchaseModeEnded:
				{
					IL_3A:
					switch (id)
					{
					case EventId.BuildingLevelUpgraded:
					case EventId.BuildingSwapped:
					case EventId.BuildingConstructed:
						break;
					default:
						if (id != EventId.ClearableCleared)
						{
							if (id == EventId.ClearableStarted || id == EventId.ChampionStartedRepairing)
							{
								goto IL_137;
							}
							if (id != EventId.ChampionRepaired)
							{
								if (id == EventId.WorldLoadComplete)
								{
									this.InitializeDroids();
									return EatResponse.NotEaten;
								}
								if (id == EventId.BattleLoadStart)
								{
									this.DestroyAllDroids();
									return EatResponse.NotEaten;
								}
								if (id != EventId.InventoryResourceUpdated)
								{
									return EatResponse.NotEaten;
								}
								if (object.Equals(cookie, "droids"))
								{
									this.AssignWorkToDroid(this.GetDroidHut(), false, true, true);
								}
								return EatResponse.NotEaten;
							}
						}
						break;
					}
					ContractEventData contractEventData = cookie as ContractEventData;
					this.RemoveWorkFromDroid(contractEventData.Entity, id != EventId.ClearableCleared);
					return EatResponse.NotEaten;
				}
				}
				goto IL_3A;
				IL_137:
				Entity building = cookie as Entity;
				this.AssignWorkToDroid(building, false, false, true);
				return EatResponse.NotEaten;
			}
			case EventId.BuildingCancelled:
			case EventId.TroopCancelled:
			{
				ContractEventData contractEventData2 = (ContractEventData)cookie;
				ContractType contractType = ContractUtils.GetContractType(contractEventData2.Contract.DeliveryType);
				if (ContractUtils.ContractTypeConsumesDroid(contractType))
				{
					SmartEntity smartEntity = (SmartEntity)contractEventData2.Entity;
					BuildingComponent buildingComp = smartEntity.BuildingComp;
					this.RemoveWorkFromDroid(smartEntity, buildingComp.BuildingType.Type != BuildingType.Clearable);
				}
				return EatResponse.NotEaten;
			}
			}
			goto IL_20;
		}
	}
}
