using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
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

		void DisableBuilding(Entity building);

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

		Contract StartHeroMobilization(TroopTypeVO hero, Entity building);

		Contract StartChampionRepair(TroopTypeVO champion, Entity building);

		Contract StartStarshipMobilization(SpecialAttackTypeVO starship, Entity building);

		void StartClearingBuilding(Entity building);

		void StartTurretCrossgrade(BuildingTypeVO swapBuildingInfo, Entity turret);

		void InstantBuildingConstruct(BuildingTypeVO buildingType, Entity selectedBuilding, int x, int z, string tag);

		void StartBuildingConstruct(BuildingTypeVO buildingType, Entity selectedBuilding, int x, int z, string tag);

		void StartBuildingUpgrade(BuildingTypeVO nextUpgradeType, Entity selectedBuilding, bool isInstant);

		void StartBuildingUpgrade(BuildingTypeVO nextUpgradeType, Entity selectedBuilding, bool isInstant, string tag);

		void StartAllWallPartBuildingUpgrade(BuildingTypeVO nextUpgradeType, Entity selectedBuilding, bool sendBackendCommand, bool sendBILog);

		bool StartTroopUpgrade(TroopTypeVO troop, Entity building);

		bool StartStarshipUpgrade(SpecialAttackTypeVO starship, Entity building);

		bool StartEquipmentUpgrade(EquipmentVO equipment, Entity building);

		Contract StartTroopTrainContract(TroopTypeVO troop, Entity building);

		void CancelTroopTrainContract(string productUid, Entity building);

		void BuyoutAllTroopTrainContracts(Entity entity);

		void BuyoutAllTroopTrainContracts(Entity entity, bool alreadySpentCrystals);

		void CancelCurrentBuildingContract(Contract contract, Entity building);

		bool FinishCurrentContract(Entity entity, bool silent);

		bool FinishCurrentContract(Entity entity, bool silent, bool sendBILog);

		void BuyOutCurrentBuildingContract(Entity entity, bool sendBackendCommand);

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
