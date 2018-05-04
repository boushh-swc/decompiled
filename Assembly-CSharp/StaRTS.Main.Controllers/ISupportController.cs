using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public interface ISupportController
	{
		void UpdateCurrentContractsListFromServer(object serverData);

		void UpdateFrozenBuildingsListFromServer(object serverData);

		List<ContractTO> GetContractEventsThatHappenedOffline();

		void ReleaseContractEventsThatHappnedOffline();

		void DisableBuilding(SmartEntity building);

		int SortContractTOByEndTime(ContractTO a, ContractTO b);

		int SortByEndTime(Contract a, Contract b);

		Contract FindBuildingContract(string buildingKey);

		Contract FindFirstContractWithProductUid(string productUid);

		bool HasTroopContractForBuilding(string buildingKey);

		List<Contract> FindAllTroopContractsForBuilding(string buildingKey);

		List<Contract> FindAllContractsOfType(ContractType type);

		List<ContractTO> GetUninitializedContractData();

		Contract FindCurrentContract(string buildingKey);

		List<Contract> FindAllContractsThatConsumeDroids();

		bool IsContractValidForStorage(Contract contract);

		Contract StartHeroMobilization(TroopTypeVO hero, SmartEntity building);

		Contract StartChampionRepair(TroopTypeVO champion, SmartEntity building);

		Contract StartStarshipMobilization(SpecialAttackTypeVO starship, SmartEntity building);

		void StartClearingBuilding(SmartEntity building);

		void StartTurretCrossgrade(BuildingTypeVO swapBuildingInfo, SmartEntity turret);

		void InstantBuildingConstruct(BuildingTypeVO buildingType, SmartEntity selectedBuilding, int x, int z, string tag);

		void StartBuildingConstruct(BuildingTypeVO buildingType, SmartEntity selectedBuilding, int x, int z, string tag);

		void StartBuildingUpgrade(BuildingTypeVO nextUpgradeType, SmartEntity selectedBuilding, bool isInstant);

		void StartBuildingUpgrade(BuildingTypeVO nextUpgradeType, SmartEntity selectedBuilding, bool isInstant, string tag);

		void StartAllWallPartBuildingUpgrade(BuildingTypeVO nextUpgradeType, SmartEntity selectedBuilding, bool sendBackendCommand, bool sendBILog);

		bool StartTroopUpgrade(TroopTypeVO troop, SmartEntity building);

		bool StartStarshipUpgrade(SpecialAttackTypeVO starship, SmartEntity building);

		bool StartEquipmentUpgrade(EquipmentVO equipment, SmartEntity building);

		Contract StartTroopTrainContract(TroopTypeVO troop, SmartEntity building);

		void CancelTroopTrainContract(string productUid, SmartEntity building);

		void BuyoutAllTroopTrainContracts(SmartEntity entity);

		void BuyoutAllTroopTrainContracts(SmartEntity entity, bool alreadySpentCrystals);

		void CancelCurrentBuildingContract(Contract contract, SmartEntity building);

		bool FinishCurrentContract(SmartEntity entity, bool silent);

		bool FinishCurrentContract(SmartEntity entity, bool silent, bool sendBILog);

		void BuyOutCurrentBuildingContract(SmartEntity entity, bool sendBackendCommand);

		EatResponse OnEvent(EventId id, object cookie);

		bool IsBuildingFrozen(string buildingKey);

		void PauseBuilding(string buildingKey);

		void UnpauseAllBuildings();

		void UnfreezeAllBuildings(uint time);

		void SimulateCheckAllContractsWithCurrentTime();

		void GetEstimatedUpdatedContractListsForChecksum(bool simulateContractUpdate, out List<Contract> remainingContracts, out List<Contract> finishedContracts);

		void SyncCurrentPlayerInventoryWithServer(Dictionary<string, object> deployables);

		void CheatForceUpdateAllContracts();
	}
}
