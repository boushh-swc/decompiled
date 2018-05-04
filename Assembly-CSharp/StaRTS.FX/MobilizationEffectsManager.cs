using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.FX
{
	public class MobilizationEffectsManager : IEventObserver
	{
		private const string NAV_CENTER_FX_UID = "GalacticNavHologram";

		private Dictionary<uint, BuildingHoloEffect> effectsByEntityId;

		public MobilizationEffectsManager()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ContractStarted, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractContinued, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractCanceled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.HeroMobilized, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.StarshipMobilized, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.HeroMobilizedFromPrize, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.StarshipMobilizedFromPrize, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingCancelled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingReplaced, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.WorldReset, EventPriority.Default);
			Service.MobilizationEffectsManager = this;
		}

		private void CreateEffect(Entity building, string unitUid, bool isStarship, bool isNavCenter)
		{
			if (this.effectsByEntityId == null)
			{
				this.effectsByEntityId = new Dictionary<uint, BuildingHoloEffect>();
			}
			if (!this.effectsByEntityId.ContainsKey(building.ID))
			{
				BuildingHoloEffect buildingHoloEffect = new BuildingHoloEffect(building, unitUid, isStarship, isNavCenter);
				this.effectsByEntityId.Add(building.ID, buildingHoloEffect);
			}
			else
			{
				BuildingHoloEffect buildingHoloEffect = this.effectsByEntityId[building.ID];
				buildingHoloEffect.CreateMobilizationHolo(unitUid, isStarship, isNavCenter);
			}
		}

		private void RemoveEffectByEntityId(uint entityId)
		{
			if (this.effectsByEntityId != null && this.effectsByEntityId.ContainsKey(entityId))
			{
				BuildingHoloEffect buildingHoloEffect = this.effectsByEntityId[entityId];
				buildingHoloEffect.Cleanup();
				this.effectsByEntityId.Remove(entityId);
			}
		}

		private void AddAllEffects(bool checkContracts)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			NodeList<TacticalCommandNode> tacticalCommandNodeList = buildingLookupController.TacticalCommandNodeList;
			for (TacticalCommandNode tacticalCommandNode = tacticalCommandNodeList.Head; tacticalCommandNode != null; tacticalCommandNode = tacticalCommandNode.Next)
			{
				this.UpdateEffectsForBuilding(tacticalCommandNode.BuildingComp, false, checkContracts);
			}
			NodeList<FleetCommandNode> fleetCommandNodeList = buildingLookupController.FleetCommandNodeList;
			for (FleetCommandNode fleetCommandNode = fleetCommandNodeList.Head; fleetCommandNode != null; fleetCommandNode = fleetCommandNode.Next)
			{
				this.UpdateEffectsForBuilding(fleetCommandNode.BuildingComp, true, checkContracts);
			}
			this.AddNavigationCenterHolo();
		}

		private void AddNavigationCenterHolo(Entity building)
		{
			if (building != null && building.Has<GameObjectViewComponent>())
			{
				this.CreateEffect(building, "GalacticNavHologram", false, true);
			}
			else
			{
				Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			}
		}

		private void AddNavigationCenterHolo()
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			NodeList<NavigationCenterNode> navigationCenterNodeList = buildingLookupController.NavigationCenterNodeList;
			for (NavigationCenterNode navigationCenterNode = navigationCenterNodeList.Head; navigationCenterNode != null; navigationCenterNode = navigationCenterNode.Next)
			{
				bool flag = ContractUtils.IsBuildingUpgrading(navigationCenterNode.Entity);
				if (!ContractUtils.IsBuildingConstructing(navigationCenterNode.Entity) && !flag)
				{
					this.CreateEffect(navigationCenterNode.BuildingComp.Entity, "GalacticNavHologram", false, true);
				}
			}
		}

		private void UpdateEffectsForBuilding(BuildingComponent buildingComp, bool isStarship, bool checkContracts)
		{
			string mobilizedUnit = this.GetMobilizedUnit(isStarship);
			if (mobilizedUnit != null)
			{
				this.CreateEffect(buildingComp.Entity, mobilizedUnit, isStarship, false);
			}
			else if (checkContracts && Service.ISupportController.HasTroopContractForBuilding(buildingComp.BuildingTO.Key))
			{
				this.CreateEffect(buildingComp.Entity, null, isStarship, false);
			}
		}

		public void UpdataAllEffects()
		{
			this.RemoveAllEffects();
			this.AddAllEffects(true);
		}

		private void RemoveAllEffects()
		{
			if (this.effectsByEntityId != null)
			{
				foreach (BuildingHoloEffect current in this.effectsByEntityId.Values)
				{
					current.Cleanup();
				}
				this.effectsByEntityId.Clear();
			}
		}

		public void TransferEffects(Entity oldBuilding, Entity newBuilding)
		{
			if (this.effectsByEntityId != null && this.effectsByEntityId.ContainsKey(oldBuilding.ID) && !this.effectsByEntityId.ContainsKey(newBuilding.ID))
			{
				BuildingHoloEffect buildingHoloEffect = this.effectsByEntityId[oldBuilding.ID];
				buildingHoloEffect.TransferEffect(newBuilding);
				this.effectsByEntityId.Remove(oldBuilding.ID);
				this.effectsByEntityId.Add(newBuilding.ID, buildingHoloEffect);
				if (buildingHoloEffect.WaitingForBuildingView)
				{
					Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
				}
			}
		}

		private void OnContractStarted(ContractEventData data)
		{
			DeliveryType deliveryType = data.Contract.DeliveryType;
			if (deliveryType == DeliveryType.Hero || deliveryType == DeliveryType.Starship)
			{
				this.CreateEffect(data.Entity, null, false, false);
			}
			else if (deliveryType == DeliveryType.UpgradeBuilding && data.BuildingVO.Type == BuildingType.NavigationCenter)
			{
				this.RemoveEffectByEntityId(data.Entity.ID);
			}
		}

		private void OnContractCanceled(ContractEventData data)
		{
			if (this.effectsByEntityId == null || !this.effectsByEntityId.ContainsKey(data.Entity.ID))
			{
				return;
			}
			DeliveryType deliveryType = data.Contract.DeliveryType;
			if (deliveryType == DeliveryType.Hero || deliveryType == DeliveryType.Starship)
			{
				if (this.GetMobilizedUnit(deliveryType == DeliveryType.Starship) != null)
				{
					return;
				}
				List<Contract> list = Service.ISupportController.FindAllTroopContractsForBuilding(data.BuildingKey);
				if (list.Count <= 1)
				{
					this.RemoveEffectByEntityId(data.Entity.ID);
				}
			}
		}

		private void OnUnitMobilized(ContractEventData data)
		{
			DeliveryType deliveryType = data.Contract.DeliveryType;
			if (deliveryType == DeliveryType.Hero || deliveryType == DeliveryType.Starship)
			{
				this.CreateEffect(data.Entity, data.Contract.ProductUid, deliveryType == DeliveryType.Starship, false);
			}
		}

		private string GetMobilizedUnit(bool isStarship)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			if (!isStarship)
			{
				List<TroopTypeVO> list = null;
				IEnumerable<KeyValuePair<string, InventoryEntry>> allHeroes = Service.CurrentPlayer.GetAllHeroes();
				foreach (KeyValuePair<string, InventoryEntry> current in allHeroes)
				{
					if (current.Value.Amount > 0)
					{
						if (list == null)
						{
							list = new List<TroopTypeVO>();
						}
						list.Add(staticDataController.Get<TroopTypeVO>(current.Key));
					}
				}
				if (list != null)
				{
					list.Sort(new Comparison<TroopTypeVO>(this.CompareTroops));
					return list[0].Uid;
				}
			}
			else
			{
				List<SpecialAttackTypeVO> list2 = null;
				IEnumerable<KeyValuePair<string, InventoryEntry>> allSpecialAttacks = Service.CurrentPlayer.GetAllSpecialAttacks();
				foreach (KeyValuePair<string, InventoryEntry> current2 in allSpecialAttacks)
				{
					if (current2.Value.Amount > 0)
					{
						if (list2 == null)
						{
							list2 = new List<SpecialAttackTypeVO>();
						}
						list2.Add(staticDataController.Get<SpecialAttackTypeVO>(current2.Key));
					}
				}
				if (list2 != null)
				{
					list2.Sort(new Comparison<SpecialAttackTypeVO>(this.CompareStarships));
					return list2[0].Uid;
				}
			}
			return null;
		}

		private int CompareTroops(TroopTypeVO a, TroopTypeVO b)
		{
			return b.Order.CompareTo(a.Order);
		}

		private int CompareStarships(SpecialAttackTypeVO a, SpecialAttackTypeVO b)
		{
			return b.Order.CompareTo(a.Order);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.StarshipMobilized:
			case EventId.HeroMobilized:
				this.OnUnitMobilized((ContractEventData)cookie);
				break;
			case EventId.StarshipMobilizedFromPrize:
			{
				Entity entity = Service.BuildingLookupController.FleetCommandNodeList.Head.Entity;
				this.CreateEffect(entity, (string)cookie, true, false);
				break;
			}
			case EventId.HeroMobilizedFromPrize:
			{
				Entity entity2 = Service.BuildingLookupController.TacticalCommandNodeList.Head.Entity;
				this.CreateEffect(entity2, (string)cookie, false, false);
				break;
			}
			default:
				switch (id)
				{
				case EventId.WorldLoadComplete:
				{
					IState currentState = Service.GameStateMachine.CurrentState;
					if (currentState is ApplicationLoadState || currentState is HomeState || currentState is WarBoardState)
					{
						this.AddAllEffects(false);
					}
					return EatResponse.NotEaten;
				}
				case EventId.WorldInTransitionComplete:
				case EventId.WorldOutTransitionComplete:
				{
					IL_37:
					switch (id)
					{
					case EventId.BuildingViewReady:
					{
						EntityViewParams entityViewParams = (EntityViewParams)cookie;
						if (this.effectsByEntityId != null && this.effectsByEntityId.ContainsKey(entityViewParams.Entity.ID))
						{
							BuildingHoloEffect buildingHoloEffect = this.effectsByEntityId[entityViewParams.Entity.ID];
							if (buildingHoloEffect.WaitingForBuildingView)
							{
								buildingHoloEffect.UpdateEffect();
							}
						}
						if (this.effectsByEntityId != null && entityViewParams.Entity.Has<NavigationCenterComponent>())
						{
							this.AddNavigationCenterHolo(entityViewParams.Entity);
						}
						return EatResponse.NotEaten;
					}
					case EventId.BuildingViewFailed:
						IL_4C:
						if (id == EventId.BuildingConstructed)
						{
							goto IL_10C;
						}
						if (id == EventId.BuildingReplaced)
						{
							Entity entity3 = cookie as Entity;
							if (entity3.Has<NavigationCenterComponent>())
							{
								this.AddNavigationCenterHolo(entity3);
							}
							return EatResponse.NotEaten;
						}
						if (id == EventId.ContractStarted || id == EventId.ContractContinued)
						{
							this.OnContractStarted((ContractEventData)cookie);
							return EatResponse.NotEaten;
						}
						if (id != EventId.ContractCanceled)
						{
							return EatResponse.NotEaten;
						}
						this.OnContractCanceled((ContractEventData)cookie);
						return EatResponse.NotEaten;
					case EventId.BuildingCancelled:
						goto IL_10C;
					}
					goto IL_4C;
					IL_10C:
					ContractEventData contractEventData = (ContractEventData)cookie;
					if (contractEventData.BuildingVO.Type == BuildingType.NavigationCenter)
					{
						this.AddNavigationCenterHolo();
					}
					return EatResponse.NotEaten;
				}
				case EventId.WorldReset:
					this.RemoveAllEffects();
					return EatResponse.NotEaten;
				}
				goto IL_37;
			}
			return EatResponse.NotEaten;
		}
	}
}
