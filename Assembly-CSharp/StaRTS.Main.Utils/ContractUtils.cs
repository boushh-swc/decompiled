using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public static class ContractUtils
	{
		public static DeliveryType GetDeliveryTypeForBuildingContract(string buildingContractUid, BuildingTypeVO building)
		{
			if (building.Type == BuildingType.Clearable)
			{
				return DeliveryType.ClearClearable;
			}
			if (building.Type == BuildingType.TroopResearch)
			{
				TroopTypeVO optional = Service.StaticDataController.GetOptional<TroopTypeVO>(buildingContractUid);
				if (optional != null)
				{
					return DeliveryType.UpgradeTroop;
				}
				SpecialAttackTypeVO optional2 = Service.StaticDataController.GetOptional<SpecialAttackTypeVO>(buildingContractUid);
				if (optional2 != null)
				{
					return DeliveryType.UpgradeStarship;
				}
				EquipmentVO optional3 = Service.StaticDataController.GetOptional<EquipmentVO>(buildingContractUid);
				if (optional3 != null)
				{
					return DeliveryType.UpgradeEquipment;
				}
			}
			BuildingTypeVO optional4 = Service.StaticDataController.GetOptional<BuildingTypeVO>(buildingContractUid);
			if (optional4 == null || optional4.Type != building.Type)
			{
				return DeliveryType.Invalid;
			}
			if (optional4.BuildingID != building.BuildingID)
			{
				return DeliveryType.SwapBuilding;
			}
			if (optional4.Lvl == 1)
			{
				return DeliveryType.Building;
			}
			return DeliveryType.UpgradeBuilding;
		}

		public static DeliveryType GetDeliveryTypeForTroopContract(BuildingTypeVO building)
		{
			BuildingType type = building.Type;
			switch (type)
			{
			case BuildingType.Barracks:
				return DeliveryType.Infantry;
			case BuildingType.Factory:
				return DeliveryType.Vehicle;
			case BuildingType.FleetCommand:
				return DeliveryType.Starship;
			case BuildingType.HeroMobilizer:
				return DeliveryType.Hero;
			case BuildingType.ChampionPlatform:
				return DeliveryType.Champion;
			default:
				if (type != BuildingType.Cantina)
				{
					return DeliveryType.Invalid;
				}
				return DeliveryType.Mercenary;
			}
		}

		public static int GetBuildingContractTotalTime(string buildingContractUid, DeliveryType type)
		{
			if (type == DeliveryType.UpgradeTroop)
			{
				TroopTypeVO optional = Service.StaticDataController.GetOptional<TroopTypeVO>(buildingContractUid);
				return optional.UpgradeTime;
			}
			if (type == DeliveryType.UpgradeStarship)
			{
				SpecialAttackTypeVO optional2 = Service.StaticDataController.GetOptional<SpecialAttackTypeVO>(buildingContractUid);
				return optional2.UpgradeTime;
			}
			if (type == DeliveryType.UpgradeEquipment)
			{
				EquipmentVO optional3 = Service.StaticDataController.GetOptional<EquipmentVO>(buildingContractUid);
				return optional3.UpgradeTime;
			}
			BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(buildingContractUid);
			switch (type)
			{
			case DeliveryType.Building:
			case DeliveryType.UpgradeBuilding:
			case DeliveryType.ClearClearable:
				return buildingTypeVO.Time;
			case DeliveryType.SwapBuilding:
				return buildingTypeVO.SwapTime;
			}
			return 0;
		}

		public static int GetTroopContractTotalTime(string productUid, DeliveryType type, List<string> perkIds)
		{
			int num = 0;
			StaticDataController staticDataController = Service.StaticDataController;
			IUpgradeableVO upgradeableVO = null;
			switch (type)
			{
			case DeliveryType.Infantry:
			case DeliveryType.Vehicle:
			case DeliveryType.Hero:
			case DeliveryType.Champion:
			case DeliveryType.Mercenary:
			{
				TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(productUid);
				num = troopTypeVO.TrainingTime;
				upgradeableVO = troopTypeVO;
				break;
			}
			case DeliveryType.Starship:
			case DeliveryType.UpgradeStarship:
			{
				SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(productUid);
				if (type == DeliveryType.UpgradeStarship)
				{
					num = specialAttackTypeVO.UpgradeTime;
				}
				else
				{
					num = specialAttackTypeVO.TrainingTime;
				}
				upgradeableVO = specialAttackTypeVO;
				break;
			}
			}
			ContractType contractType = ContractUtils.GetContractType(type);
			if (upgradeableVO != null && perkIds != null && perkIds.Count > 0 && ContractUtils.IsTroopType(contractType))
			{
				PerkManager perkManager = Service.PerkManager;
				float contractTimeReductionMultiplierForPerks = perkManager.GetContractTimeReductionMultiplierForPerks(upgradeableVO, perkIds);
				num = Mathf.FloorToInt((float)num * contractTimeReductionMultiplierForPerks);
			}
			return num;
		}

		public static DeliveryType GetTroopContractTypeByBuilding(BuildingTypeVO building)
		{
			BuildingType type = building.Type;
			switch (type)
			{
			case BuildingType.Barracks:
				return DeliveryType.Infantry;
			case BuildingType.Factory:
				return DeliveryType.Vehicle;
			case BuildingType.FleetCommand:
				return DeliveryType.Starship;
			case BuildingType.HeroMobilizer:
				return DeliveryType.Hero;
			case BuildingType.ChampionPlatform:
				return DeliveryType.Champion;
			default:
				if (type != BuildingType.Cantina)
				{
					return DeliveryType.Invalid;
				}
				return DeliveryType.Mercenary;
			}
		}

		public static ContractType GetContractType(DeliveryType deliveryType)
		{
			switch (deliveryType)
			{
			case DeliveryType.Invalid:
				return ContractType.Invalid;
			case DeliveryType.Infantry:
			case DeliveryType.Vehicle:
			case DeliveryType.Mercenary:
				return ContractType.Troop;
			case DeliveryType.Starship:
				return ContractType.SpecialAttack;
			case DeliveryType.Hero:
				return ContractType.Hero;
			case DeliveryType.Champion:
				return ContractType.Champion;
			case DeliveryType.Building:
				return ContractType.Build;
			case DeliveryType.UpgradeBuilding:
			case DeliveryType.SwapBuilding:
				return ContractType.Upgrade;
			case DeliveryType.UpgradeTroop:
			case DeliveryType.UpgradeStarship:
			case DeliveryType.UpgradeEquipment:
				return ContractType.Research;
			case DeliveryType.ClearClearable:
				return ContractType.Clear;
			default:
				Service.Logger.ErrorFormat("Unhandled case for Contract Type mapping: {0}", new object[]
				{
					deliveryType
				});
				return ContractType.Invalid;
			}
		}

		public static bool IsTroopType(ContractType contractType)
		{
			return contractType == ContractType.Hero || contractType == ContractType.Champion || contractType == ContractType.SpecialAttack || contractType == ContractType.Troop;
		}

		public static bool IsBuildingType(ContractType contractType)
		{
			return !ContractUtils.IsTroopType(contractType);
		}

		public static bool ContractTypeConsumesDroid(ContractType contractType)
		{
			return contractType == ContractType.Champion || (contractType != ContractType.Research && ContractUtils.IsBuildingType(contractType));
		}

		public static int CalculateRemainingTimeOfAllTroopContracts(Entity entity)
		{
			int num = 0;
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			List<Contract> list = Service.ISupportController.FindAllTroopContractsForBuilding(buildingComponent.BuildingTO.Key);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				if (i == 0)
				{
					num += list[i].GetRemainingTimeForSim();
				}
				else
				{
					num += list[i].TotalTime;
				}
				i++;
			}
			return num;
		}

		public static int CalculateNumTroopsQueued(Entity entity)
		{
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			List<Contract> list = Service.ISupportController.FindAllTroopContractsForBuilding(buildingComponent.BuildingTO.Key);
			return list.Count;
		}

		public static int CalculateSpaceOccupiedByQueuedTroops(Entity entity)
		{
			int num = 0;
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			List<Contract> list = Service.ISupportController.FindAllTroopContractsForBuilding(buildingComponent.BuildingTO.Key);
			bool flag = false;
			if (buildingComponent.BuildingType.Type == BuildingType.FleetCommand)
			{
				flag = true;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			for (int i = 0; i < list.Count; i++)
			{
				string productUid = list[i].ProductUid;
				int size;
				if (flag)
				{
					size = staticDataController.Get<SpecialAttackTypeVO>(productUid).Size;
				}
				else
				{
					size = staticDataController.Get<TroopTypeVO>(productUid).Size;
				}
				num += size;
			}
			return num;
		}

		public static bool HasCapacityForTroop(Entity entity, int size)
		{
			int num = ContractUtils.CalculateSpaceOccupiedByQueuedTroops(entity);
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			return size <= buildingComponent.BuildingType.Storage - num;
		}

		public static void CalculateContractCost(string productUid, DeliveryType deliveryType, BuildingTypeVO buildingVO, List<string> contractPerkIds, out int credits, out int materials, out int contraband)
		{
			credits = 0;
			materials = 0;
			contraband = 0;
			switch (deliveryType)
			{
			case DeliveryType.Infantry:
			case DeliveryType.Vehicle:
			case DeliveryType.Hero:
			case DeliveryType.Champion:
			case DeliveryType.Mercenary:
			{
				TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(productUid);
				credits = troopTypeVO.Credits;
				materials = troopTypeVO.Materials;
				contraband = troopTypeVO.Contraband;
				goto IL_1B5;
			}
			case DeliveryType.Starship:
			{
				SpecialAttackTypeVO specialAttackTypeVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(productUid);
				credits = specialAttackTypeVO.Credits;
				materials = specialAttackTypeVO.Materials;
				contraband = specialAttackTypeVO.Contraband;
				goto IL_1B5;
			}
			case DeliveryType.UpgradeBuilding:
			{
				BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(productUid);
				credits = buildingTypeVO.UpgradeCredits;
				materials = buildingTypeVO.UpgradeMaterials;
				contraband = buildingTypeVO.UpgradeContraband;
				goto IL_1B5;
			}
			case DeliveryType.SwapBuilding:
			{
				BuildingTypeVO buildingTypeVO2 = Service.StaticDataController.Get<BuildingTypeVO>(productUid);
				credits = buildingTypeVO2.SwapCredits;
				materials = buildingTypeVO2.SwapMaterials;
				contraband = buildingTypeVO2.SwapContraband;
				goto IL_1B5;
			}
			case DeliveryType.UpgradeTroop:
			{
				TroopTypeVO troopTypeVO2 = Service.StaticDataController.Get<TroopTypeVO>(productUid);
				credits = troopTypeVO2.UpgradeCredits;
				materials = troopTypeVO2.UpgradeMaterials;
				contraband = troopTypeVO2.UpgradeContraband;
				goto IL_1B5;
			}
			case DeliveryType.UpgradeStarship:
			{
				SpecialAttackTypeVO specialAttackTypeVO2 = Service.StaticDataController.Get<SpecialAttackTypeVO>(productUid);
				credits = specialAttackTypeVO2.UpgradeCredits;
				materials = specialAttackTypeVO2.UpgradeMaterials;
				contraband = specialAttackTypeVO2.UpgradeContraband;
				goto IL_1B5;
			}
			case DeliveryType.UpgradeEquipment:
				goto IL_1B5;
			case DeliveryType.ClearClearable:
			{
				BuildingTypeVO buildingTypeVO3 = Service.StaticDataController.Get<BuildingTypeVO>(productUid);
				credits = buildingTypeVO3.Credits;
				materials = buildingTypeVO3.Materials;
				contraband = buildingTypeVO3.Contraband;
				goto IL_1B5;
			}
			}
			Service.Logger.Error("DeliveryType has no cost: " + deliveryType);
			IL_1B5:
			ContractType contractType = ContractUtils.GetContractType(deliveryType);
			if (ContractUtils.IsTroopType(contractType) && buildingVO != null && contractPerkIds != null && contractPerkIds.Count > 0)
			{
				PerkManager perkManager = Service.PerkManager;
				float contractCostMultiplierForPerks = perkManager.GetContractCostMultiplierForPerks(buildingVO, contractPerkIds);
				GameUtils.MultiplyCurrency(contractCostMultiplierForPerks, ref credits, ref materials, ref contraband);
			}
		}

		public static bool IsBuildingClearing(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingClearing: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingClearing: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			return contract != null && !contract.IsReadyToBeFinished() && contract.DeliveryType == DeliveryType.ClearClearable;
		}

		public static bool IsBuildingConstructing(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingConstructing: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingConstructing: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			return ContractUtils.IsBuildingConstructing(buildingComponent.BuildingTO.Key);
		}

		public static bool IsBuildingConstructing(string buildingKey)
		{
			Contract contract = Service.ISupportController.FindCurrentContract(buildingKey);
			return contract != null && !contract.IsReadyToBeFinished() && contract.DeliveryType == DeliveryType.Building;
		}

		public static bool IsChampionRepairing(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsChampionRepairing: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsChampionRepairing: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			if (buildingComponent.BuildingType.Type != BuildingType.ChampionPlatform)
			{
				return false;
			}
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			return contract != null && !contract.IsReadyToBeFinished() && contract.DeliveryType == DeliveryType.Champion;
		}

		public static bool IsBuildingUpgrading(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingUpgrading: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingUpgrading: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			return contract != null && !contract.IsReadyToBeFinished() && contract.DeliveryType == DeliveryType.UpgradeBuilding;
		}

		public static bool CanCancelDeployableContract(Entity selectedBuilding)
		{
			if (!ContractUtils.IsArmyUpgrading(selectedBuilding))
			{
				return false;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			string productUid = contract.ProductUid;
			IDeployableVO optional = staticDataController.GetOptional<TroopTypeVO>(productUid);
			string value = null;
			if (optional != null)
			{
				value = optional.UpgradeShardUid;
			}
			else
			{
				optional = staticDataController.GetOptional<SpecialAttackTypeVO>(productUid);
				if (optional != null)
				{
					value = optional.UpgradeShardUid;
				}
				else
				{
					Service.Logger.Error("CanCancelDeployableContract: Unsupported deployable type, not troop or special attack " + productUid);
				}
			}
			return string.IsNullOrEmpty(value);
		}

		public static bool IsArmyUpgrading(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsArmyUpgrading: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsArmyUpgrading: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			return contract != null && !contract.IsReadyToBeFinished() && (contract.DeliveryType == DeliveryType.UpgradeTroop || contract.DeliveryType == DeliveryType.UpgradeStarship);
		}

		public static bool IsEquipmentUpgrading(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsEquipmentUpgrading: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsEquipmentUpgrading: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			return contract != null && !contract.IsReadyToBeFinished() && contract.DeliveryType == DeliveryType.UpgradeEquipment;
		}

		public static bool IsBuildingSwapping(Entity selectedBuilding)
		{
			if (selectedBuilding == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingSwapping: SelectedBuilding = null");
				return false;
			}
			if (selectedBuilding.Get<BuildingComponent>() == null)
			{
				Service.Logger.Error("ContractUtils.IsBuildingSwapping: selectedBuilding.BuildingComponent = null");
				return false;
			}
			BuildingComponent buildingComponent = selectedBuilding.Get<BuildingComponent>();
			Contract contract = Service.ISupportController.FindCurrentContract(buildingComponent.BuildingTO.Key);
			return contract != null && !contract.IsReadyToBeFinished() && contract.DeliveryType == DeliveryType.SwapBuilding;
		}

		public static void GetArmyContractStorageAndSize(Contract contract, out InventoryStorage storage, out int size)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			StaticDataController staticDataController = Service.StaticDataController;
			switch (contract.DeliveryType)
			{
			case DeliveryType.Starship:
				storage = currentPlayer.Inventory.SpecialAttack;
				size = staticDataController.Get<SpecialAttackTypeVO>(contract.ProductUid).Size;
				break;
			case DeliveryType.Hero:
				storage = currentPlayer.Inventory.Hero;
				size = staticDataController.Get<TroopTypeVO>(contract.ProductUid).Size;
				break;
			case DeliveryType.Champion:
				storage = currentPlayer.Inventory.Champion;
				size = staticDataController.Get<TroopTypeVO>(contract.ProductUid).Size;
				break;
			default:
				storage = currentPlayer.Inventory.Troop;
				size = staticDataController.Get<TroopTypeVO>(contract.ProductUid).Size;
				break;
			}
		}

		public static bool IsArmyContractValid(Contract contract, Dictionary<string, int> inventorySizeOffsets)
		{
			InventoryStorage inventoryStorage;
			int num;
			ContractUtils.GetArmyContractStorageAndSize(contract, out inventoryStorage, out num);
			int num2 = 0;
			if (inventorySizeOffsets != null && inventorySizeOffsets.ContainsKey(inventoryStorage.Key))
			{
				num2 = inventorySizeOffsets[inventoryStorage.Key];
			}
			return inventoryStorage.GetTotalStorageCapacity() == -1 || inventoryStorage.GetTotalStorageAmount() + num + num2 <= inventoryStorage.GetTotalStorageCapacity();
		}

		public static bool IsArmyContractValid(Contract contract)
		{
			return ContractUtils.IsArmyContractValid(contract, null);
		}

		public static bool IsUpgradeTroopContractValid(Contract contract)
		{
			TroopTypeVO upgradeable = Service.StaticDataController.Get<TroopTypeVO>(contract.ProductUid);
			TroopUpgradeCatalog troopUpgradeCatalog = Service.TroopUpgradeCatalog;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return troopUpgradeCatalog.CanUpgradeTo(currentPlayer.UnlockedLevels.Troops, upgradeable);
		}

		public static bool IsUpgradeStarshipContractValid(Contract contract)
		{
			SpecialAttackTypeVO upgradeable = Service.StaticDataController.Get<SpecialAttackTypeVO>(contract.ProductUid);
			StarshipUpgradeCatalog starshipUpgradeCatalog = Service.StarshipUpgradeCatalog;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return starshipUpgradeCatalog.CanUpgradeTo(currentPlayer.UnlockedLevels.Starships, upgradeable);
		}

		public static bool IsUpgradeEquipmentContractValid(Contract contract)
		{
			EquipmentVO upgradeable = Service.StaticDataController.Get<EquipmentVO>(contract.ProductUid);
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return equipmentUpgradeCatalog.CanUpgradeTo(currentPlayer.UnlockedLevels.Equipment, upgradeable);
		}

		private static Contract FindBuildingToFinish()
		{
			Contract result = null;
			List<Contract> list = Service.ISupportController.FindAllContractsThatConsumeDroids();
			if (list.Count > 0)
			{
				result = list[0];
			}
			return result;
		}

		public static int MinimumCostToFinish()
		{
			Contract contract = ContractUtils.FindBuildingToFinish();
			return ContractUtils.GetCrystalCostToFinishContract(contract);
		}

		public static int GetCrystalCostToFinishContract(Contract contract)
		{
			int result = 0;
			if (contract != null)
			{
				int remainingTimeForSim = contract.GetRemainingTimeForSim();
				if (remainingTimeForSim > 0)
				{
					result = GameUtils.SecondsToCrystals(remainingTimeForSim);
				}
			}
			return result;
		}

		public static bool InstantFreeupDroid()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int num = ContractUtils.CalculateDroidsInUse();
			if (num >= currentPlayer.CurrentDroidsAmount)
			{
				Contract contract = ContractUtils.FindBuildingToFinish();
				if (contract != null)
				{
					int crystalCostToFinishContract = ContractUtils.GetCrystalCostToFinishContract(contract);
					if (GameUtils.SpendCrystals(crystalCostToFinishContract))
					{
						NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
						for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
						{
							if (buildingNode.BuildingComp.BuildingTO.Key == contract.ContractTO.BuildingKey)
							{
								Service.EventManager.SendEvent(EventId.InitiatedBuyout, null);
								Service.ISupportController.BuyOutCurrentBuildingContract(buildingNode.Entity, true);
								return true;
							}
						}
					}
				}
			}
			return false;
		}

		public static int CalculateDroidsInUse()
		{
			List<Contract> list = Service.ISupportController.FindAllContractsThatConsumeDroids();
			return list.Count;
		}

		public static bool HasExistingHeroContract(string heroUpgradeGroup)
		{
			bool result = false;
			StaticDataController staticDataController = Service.StaticDataController;
			ISupportController iSupportController = Service.ISupportController;
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			BuildingComponent buildingComp = buildingLookupController.TacticalCommandNodeList.Head.BuildingComp;
			string key = buildingComp.BuildingTO.Key;
			List<Contract> list = iSupportController.FindAllTroopContractsForBuilding(key);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(list[i].ProductUid);
				if (troopTypeVO.UpgradeGroup == heroUpgradeGroup)
				{
					result = true;
					break;
				}
				i++;
			}
			return result;
		}
	}
}
