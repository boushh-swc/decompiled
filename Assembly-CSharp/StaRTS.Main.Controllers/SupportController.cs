using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Equipment;
using StaRTS.Main.Models.Commands.Player.Building.Clear;
using StaRTS.Main.Models.Commands.Player.Building.Construct;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using StaRTS.Main.Models.Commands.Player.Building.Contracts.Buyout;
using StaRTS.Main.Models.Commands.Player.Building.Contracts.Cancel;
using StaRTS.Main.Models.Commands.Player.Building.Swap;
using StaRTS.Main.Models.Commands.Player.Building.Upgrade;
using StaRTS.Main.Models.Commands.Player.Deployable;
using StaRTS.Main.Models.Commands.Player.Deployable.Upgrade.Start;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Controllers
{
	public class SupportController : IEventObserver, ISupportController, IViewFrameTimeObserver
	{
		private delegate int ProductDeliveryDelegate(Contract contract, Building buildingTO, BuildingTypeVO buildingVO);

		private delegate bool ContractValidationDelegate(Contract contract);

		private delegate void FinishContractOnServerDelegate(string buildingKey, string productUid, int amount);

		private Dictionary<DeliveryType, SupportController.ProductDeliveryDelegate> deliveryMethods;

		private Dictionary<DeliveryType, SupportController.ContractValidationDelegate> validationMethods;

		private Dictionary<DeliveryType, EventId> events;

		private EventManager eventManager;

		private ServerAPI serverAPI;

		private List<ContractTO> contractDataFromServer;

		private List<string> frozenBuildingsFromServer;

		private List<ContractTO> offlineCompletedContracts;

		private List<Contract> currentContracts;

		private MutableIterator mutableIterator;

		private Dictionary<string, SmartEntity> buildingKeyToEntities;

		private HashSet<string> frozenBuildings;

		private HashSet<string> pausedBuildings;

		private bool isIterating;

		private bool needSortCurrentContracts;

		private Dictionary<string, int> temporaryInventorySizeServerDeltas;

		private float accumulatedUpdateDt;

		public const float UPDATE_TIME_THRESHOLD = 0.1f;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache0;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache1;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache2;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache3;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache4;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache5;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache6;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache7;

		[CompilerGenerated]
		private static SupportController.ContractValidationDelegate <>f__mg$cache8;

		public SupportController()
		{
			Service.ISupportController = this;
			this.currentContracts = new List<Contract>();
			this.mutableIterator = new MutableIterator();
			this.frozenBuildings = new HashSet<string>();
			this.pausedBuildings = new HashSet<string>();
			this.buildingKeyToEntities = new Dictionary<string, SmartEntity>();
			this.temporaryInventorySizeServerDeltas = new Dictionary<string, int>();
			this.accumulatedUpdateDt = 0f;
			this.eventManager = Service.EventManager;
			this.serverAPI = Service.ServerAPI;
			this.events = new Dictionary<DeliveryType, EventId>
			{
				{
					DeliveryType.Infantry,
					EventId.TroopRecruited
				},
				{
					DeliveryType.Vehicle,
					EventId.TroopRecruited
				},
				{
					DeliveryType.Mercenary,
					EventId.TroopRecruited
				},
				{
					DeliveryType.Starship,
					EventId.StarshipMobilized
				},
				{
					DeliveryType.Hero,
					EventId.HeroMobilized
				},
				{
					DeliveryType.Champion,
					EventId.ChampionRepaired
				},
				{
					DeliveryType.Building,
					EventId.BuildingConstructed
				},
				{
					DeliveryType.UpgradeBuilding,
					EventId.BuildingLevelUpgraded
				},
				{
					DeliveryType.SwapBuilding,
					EventId.BuildingSwapped
				},
				{
					DeliveryType.UpgradeTroop,
					EventId.TroopLevelUpgraded
				},
				{
					DeliveryType.UpgradeStarship,
					EventId.StarshipLevelUpgraded
				},
				{
					DeliveryType.UpgradeEquipment,
					EventId.EquipmentUpgraded
				},
				{
					DeliveryType.ClearClearable,
					EventId.ClearableCleared
				}
			};
			this.deliveryMethods = new Dictionary<DeliveryType, SupportController.ProductDeliveryDelegate>
			{
				{
					DeliveryType.Infantry,
					new SupportController.ProductDeliveryDelegate(this.DeliverTroop)
				},
				{
					DeliveryType.Vehicle,
					new SupportController.ProductDeliveryDelegate(this.DeliverTroop)
				},
				{
					DeliveryType.Mercenary,
					new SupportController.ProductDeliveryDelegate(this.DeliverTroop)
				},
				{
					DeliveryType.Starship,
					new SupportController.ProductDeliveryDelegate(this.DeliverStarship)
				},
				{
					DeliveryType.Hero,
					new SupportController.ProductDeliveryDelegate(this.DeliverHero)
				},
				{
					DeliveryType.Champion,
					new SupportController.ProductDeliveryDelegate(this.DeliverChampion)
				},
				{
					DeliveryType.Building,
					new SupportController.ProductDeliveryDelegate(this.DeliverBuilding)
				},
				{
					DeliveryType.UpgradeBuilding,
					new SupportController.ProductDeliveryDelegate(this.DeliverUpgradeBuilding)
				},
				{
					DeliveryType.SwapBuilding,
					new SupportController.ProductDeliveryDelegate(this.DeliverSwapBuilding)
				},
				{
					DeliveryType.UpgradeTroop,
					new SupportController.ProductDeliveryDelegate(this.DeliverUpgradeTroopOrStarship)
				},
				{
					DeliveryType.UpgradeStarship,
					new SupportController.ProductDeliveryDelegate(this.DeliverUpgradeTroopOrStarship)
				},
				{
					DeliveryType.UpgradeEquipment,
					new SupportController.ProductDeliveryDelegate(this.DeliverUpgradeEquipment)
				},
				{
					DeliveryType.ClearClearable,
					new SupportController.ProductDeliveryDelegate(this.ClearClearable)
				}
			};
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> dictionary = new Dictionary<DeliveryType, SupportController.ContractValidationDelegate>();
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_228_0 = dictionary;
			DeliveryType arg_228_1 = DeliveryType.Infantry;
			if (SupportController.<>f__mg$cache0 == null)
			{
				SupportController.<>f__mg$cache0 = new SupportController.ContractValidationDelegate(ContractUtils.IsArmyContractValid);
			}
			arg_228_0.Add(arg_228_1, SupportController.<>f__mg$cache0);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_24C_0 = dictionary;
			DeliveryType arg_24C_1 = DeliveryType.Vehicle;
			if (SupportController.<>f__mg$cache1 == null)
			{
				SupportController.<>f__mg$cache1 = new SupportController.ContractValidationDelegate(ContractUtils.IsArmyContractValid);
			}
			arg_24C_0.Add(arg_24C_1, SupportController.<>f__mg$cache1);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_271_0 = dictionary;
			DeliveryType arg_271_1 = DeliveryType.Mercenary;
			if (SupportController.<>f__mg$cache2 == null)
			{
				SupportController.<>f__mg$cache2 = new SupportController.ContractValidationDelegate(ContractUtils.IsArmyContractValid);
			}
			arg_271_0.Add(arg_271_1, SupportController.<>f__mg$cache2);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_295_0 = dictionary;
			DeliveryType arg_295_1 = DeliveryType.Starship;
			if (SupportController.<>f__mg$cache3 == null)
			{
				SupportController.<>f__mg$cache3 = new SupportController.ContractValidationDelegate(ContractUtils.IsArmyContractValid);
			}
			arg_295_0.Add(arg_295_1, SupportController.<>f__mg$cache3);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_2B9_0 = dictionary;
			DeliveryType arg_2B9_1 = DeliveryType.Hero;
			if (SupportController.<>f__mg$cache4 == null)
			{
				SupportController.<>f__mg$cache4 = new SupportController.ContractValidationDelegate(ContractUtils.IsArmyContractValid);
			}
			arg_2B9_0.Add(arg_2B9_1, SupportController.<>f__mg$cache4);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_2DD_0 = dictionary;
			DeliveryType arg_2DD_1 = DeliveryType.Champion;
			if (SupportController.<>f__mg$cache5 == null)
			{
				SupportController.<>f__mg$cache5 = new SupportController.ContractValidationDelegate(ContractUtils.IsArmyContractValid);
			}
			arg_2DD_0.Add(arg_2DD_1, SupportController.<>f__mg$cache5);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_302_0 = dictionary;
			DeliveryType arg_302_1 = DeliveryType.UpgradeTroop;
			if (SupportController.<>f__mg$cache6 == null)
			{
				SupportController.<>f__mg$cache6 = new SupportController.ContractValidationDelegate(ContractUtils.IsUpgradeTroopContractValid);
			}
			arg_302_0.Add(arg_302_1, SupportController.<>f__mg$cache6);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_327_0 = dictionary;
			DeliveryType arg_327_1 = DeliveryType.UpgradeStarship;
			if (SupportController.<>f__mg$cache7 == null)
			{
				SupportController.<>f__mg$cache7 = new SupportController.ContractValidationDelegate(ContractUtils.IsUpgradeStarshipContractValid);
			}
			arg_327_0.Add(arg_327_1, SupportController.<>f__mg$cache7);
			Dictionary<DeliveryType, SupportController.ContractValidationDelegate> arg_34C_0 = dictionary;
			DeliveryType arg_34C_1 = DeliveryType.UpgradeEquipment;
			if (SupportController.<>f__mg$cache8 == null)
			{
				SupportController.<>f__mg$cache8 = new SupportController.ContractValidationDelegate(ContractUtils.IsUpgradeEquipmentContractValid);
			}
			arg_34C_0.Add(arg_34C_1, SupportController.<>f__mg$cache8);
			this.validationMethods = dictionary;
			this.eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Notification);
			this.eventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.ContractsCompletedWhileOffline, EventPriority.Default);
		}

		public void UpdateCurrentContractsListFromServer(object serverData)
		{
			this.contractDataFromServer = new List<ContractTO>();
			List<object> list = serverData as List<object>;
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				ContractTO item = new ContractTO().FromObject(list[i]) as ContractTO;
				this.contractDataFromServer.Add(item);
				i++;
			}
		}

		public void UpdateFrozenBuildingsListFromServer(object serverData)
		{
			this.frozenBuildingsFromServer = new List<string>();
			List<object> list = serverData as List<object>;
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.frozenBuildingsFromServer.Add(list[i] as string);
				i++;
			}
		}

		private void InitializeContracts()
		{
			if (this.contractDataFromServer == null || this.currentContracts.Count > 0)
			{
				return;
			}
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			int i = 0;
			int count = this.contractDataFromServer.Count;
			while (i < count)
			{
				ContractTO contractTO = this.contractDataFromServer[i];
				BuildingTypeVO building = null;
				for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
				{
					if (buildingNode.BuildingComp.BuildingTO.Key == contractTO.BuildingKey)
					{
						building = buildingNode.BuildingComp.BuildingType;
						break;
					}
				}
				DeliveryType deliveryType;
				int totalTime;
				if (ContractUtils.IsBuildingType(contractTO.ContractType))
				{
					deliveryType = ContractUtils.GetDeliveryTypeForBuildingContract(contractTO.Uid, building);
					totalTime = ContractUtils.GetBuildingContractTotalTime(contractTO.Uid, deliveryType);
				}
				else
				{
					deliveryType = ContractUtils.GetDeliveryTypeForTroopContract(building);
					totalTime = ContractUtils.GetTroopContractTotalTime(contractTO.Uid, deliveryType, contractTO.PerkIds);
				}
				Contract contract = new Contract(contractTO.Uid, deliveryType, totalTime, 0.0, contractTO.Tag);
				contract.ContractTO = contractTO;
				contract.UpdateRemainingTime();
				this.currentContracts.Add(contract);
				i++;
			}
			if (this.frozenBuildingsFromServer != null)
			{
				for (BuildingNode buildingNode2 = nodeList.Head; buildingNode2 != null; buildingNode2 = buildingNode2.Next)
				{
					string key = buildingNode2.BuildingComp.BuildingTO.Key;
					int num = this.frozenBuildingsFromServer.IndexOf(key);
					if (num != -1)
					{
						if (this.FindCurrentContract(key) != null)
						{
							this.FreezeBuilding(key);
						}
						this.frozenBuildingsFromServer.RemoveAt(num);
					}
				}
			}
			this.frozenBuildingsFromServer = null;
			this.contractDataFromServer = null;
			this.SortCurrentContracts();
		}

		public List<ContractTO> GetContractEventsThatHappenedOffline()
		{
			return this.offlineCompletedContracts;
		}

		public List<ContractTO> GetUninitializedContractData()
		{
			return this.contractDataFromServer;
		}

		public void ReleaseContractEventsThatHappnedOffline()
		{
			if (this.offlineCompletedContracts != null)
			{
				int i = 0;
				int count = this.offlineCompletedContracts.Count;
				while (i < count)
				{
					ContractTO cookie = this.offlineCompletedContracts[i];
					Service.EventManager.SendEvent(EventId.ContractCompletedForStoryAction, cookie);
					i++;
				}
			}
			this.offlineCompletedContracts = null;
		}

		private void SendContractContinuedEvents()
		{
			NodeList<SupportNode> nodeList = Service.EntityController.GetNodeList<SupportNode>();
			for (SupportNode supportNode = nodeList.Head; supportNode != null; supportNode = supportNode.Next)
			{
				Contract contract = this.FindCurrentContract(supportNode.BuildingComp.BuildingTO.Key);
				if (contract != null)
				{
					switch (contract.DeliveryType)
					{
					case DeliveryType.Champion:
					case DeliveryType.Building:
					case DeliveryType.UpgradeBuilding:
					case DeliveryType.SwapBuilding:
						this.DisableBuilding((SmartEntity)supportNode.Entity);
						break;
					}
					ContractEventData cookie = new ContractEventData(contract, (SmartEntity)supportNode.Entity, true);
					this.eventManager.SendEvent(EventId.ContractContinued, cookie);
				}
			}
		}

		public void DisableBuilding(SmartEntity building)
		{
			Service.EventManager.SendEvent(EventId.ShowScaffolding, building);
			if (building.ShooterComp != null)
			{
				building.Remove<ShooterComponent>();
			}
			if (building.TurretShooterComp != null)
			{
				building.Remove<TurretShooterComponent>();
			}
			if (building.ShieldGeneratorComp != null)
			{
				Service.ShieldController.StopAllEffects();
				Service.ShieldController.RecalculateFlagStampsForShieldBorder(building, false);
				Service.ShieldController.RemoveShieldEffect(building);
				Service.EntityFactory.DestroyEntity(building.ShieldGeneratorComp.ShieldBorderEntity, true, true);
				building.Remove<ShieldGeneratorComponent>();
				Service.EventManager.SendEvent(EventId.ShieldDisabled, null);
			}
		}

		private void EnableBuilding(SmartEntity building)
		{
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			if (buildingType.Type == BuildingType.Turret)
			{
				TurretTypeVO turretType = Service.StaticDataController.Get<TurretTypeVO>(buildingType.TurretUid);
				Service.EntityFactory.AddTurretComponentsToEntity(building, turretType);
			}
			if (buildingType.Type == BuildingType.ShieldGenerator)
			{
				Service.EntityFactory.AddShieldComponentsToEntity(building, buildingType);
				Service.ShieldController.RecalculateFlagStampsForShieldBorder(building, true);
				Service.ShieldController.InitializeEffects(building);
			}
		}

		public int SortContractTOByEndTime(ContractTO a, ContractTO b)
		{
			if (a == b)
			{
				return 0;
			}
			uint endTime = a.EndTime;
			uint endTime2 = b.EndTime;
			if (endTime > endTime2)
			{
				return 1;
			}
			if (endTime != endTime2)
			{
				return -1;
			}
			long num = GameUtils.StringHash(a.BuildingKey);
			long num2 = GameUtils.StringHash(b.BuildingKey);
			if (num > num2)
			{
				return 1;
			}
			if (num < num2)
			{
				return -1;
			}
			bool flag = ContractUtils.IsTroopType(a.ContractType);
			bool flag2 = ContractUtils.IsTroopType(b.ContractType);
			if (flag && !flag2)
			{
				return 1;
			}
			if (!flag && flag2)
			{
				return -1;
			}
			num = GameUtils.StringHash(a.Uid);
			num2 = GameUtils.StringHash(b.Uid);
			return (num <= num2) ? ((num != num2) ? -1 : 0) : 1;
		}

		public int SortByEndTime(Contract a, Contract b)
		{
			if (a == b)
			{
				return 0;
			}
			return this.SortContractTOByEndTime(a.ContractTO, b.ContractTO);
		}

		public Contract FindBuildingContract(string buildingKey)
		{
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (this.currentContracts[i].ContractTO.BuildingKey == buildingKey && ContractUtils.IsBuildingType(this.currentContracts[i].ContractTO.ContractType))
				{
					return this.currentContracts[i];
				}
				i++;
			}
			return null;
		}

		public bool HasTroopContractForBuilding(string buildingKey)
		{
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (this.currentContracts[i].ContractTO.BuildingKey == buildingKey && ContractUtils.IsTroopType(this.currentContracts[i].ContractTO.ContractType))
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public List<Contract> FindAllTroopContractsForBuilding(string buildingKey)
		{
			List<Contract> list = new List<Contract>();
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (this.currentContracts[i].ContractTO.BuildingKey == buildingKey && ContractUtils.IsTroopType(this.currentContracts[i].ContractTO.ContractType))
				{
					list.Add(this.currentContracts[i]);
				}
				i++;
			}
			return list;
		}

		public List<Contract> FindAllContractsOfType(ContractType type)
		{
			List<Contract> list = new List<Contract>();
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (this.currentContracts[i].ContractTO.ContractType == type)
				{
					list.Add(this.currentContracts[i]);
				}
				i++;
			}
			return list;
		}

		public Contract FindCurrentContract(string buildingKey)
		{
			List<Contract> list = null;
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (this.currentContracts[i].ContractTO.BuildingKey == buildingKey)
				{
					if (list == null)
					{
						list = new List<Contract>();
					}
					list.Add(this.currentContracts[i]);
				}
				i++;
			}
			Contract result = null;
			if (list != null && list.Count > 0)
			{
				result = list[0];
				int j = 1;
				int count2 = list.Count;
				while (j < count2)
				{
					Contract contract = list[j];
					if (ContractUtils.IsBuildingType(contract.ContractTO.ContractType))
					{
						result = contract;
						break;
					}
					j++;
				}
			}
			return result;
		}

		public Contract FindFirstContractWithProductUid(string productUid)
		{
			if (string.IsNullOrEmpty(productUid))
			{
				return null;
			}
			for (int i = 0; i < this.currentContracts.Count; i++)
			{
				if (this.currentContracts[i].ProductUid.Equals(productUid))
				{
					return this.currentContracts[i];
				}
			}
			return null;
		}

		private Contract FindLastContractById(string buildingKey, string productUid)
		{
			for (int i = this.currentContracts.Count - 1; i >= 0; i--)
			{
				if (this.currentContracts[i].ContractTO.BuildingKey == buildingKey && this.currentContracts[i].ContractTO.Uid == productUid)
				{
					return this.currentContracts[i];
				}
			}
			return null;
		}

		private Contract FindLastTroopContractForBuilding(string buildingKey, string productUid)
		{
			Contract contract = null;
			Contract contract2 = null;
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (this.currentContracts[i].ContractTO.BuildingKey == buildingKey)
				{
					contract2 = this.currentContracts[i];
					if (this.currentContracts[i].ContractTO.Uid == productUid && (contract == null || this.currentContracts[i].ContractTO.EndTime > contract.ContractTO.EndTime))
					{
						contract = this.currentContracts[i];
					}
				}
				i++;
			}
			if (contract == null)
			{
				contract = contract2;
			}
			return contract;
		}

		private Contract FindFirstTroopContractForBuilding(string buildingKey)
		{
			Contract result = null;
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				Contract contract = this.currentContracts[i];
				if (contract.ContractTO.BuildingKey == buildingKey && ContractUtils.IsTroopType(contract.ContractTO.ContractType))
				{
					result = contract;
					break;
				}
				i++;
			}
			return result;
		}

		public List<Contract> FindAllContractsThatConsumeDroids()
		{
			List<Contract> list = new List<Contract>();
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				if (ContractUtils.ContractTypeConsumesDroid(this.currentContracts[i].ContractTO.ContractType))
				{
					list.Add(this.currentContracts[i]);
				}
				i++;
			}
			return list;
		}

		public bool IsContractValidForStorage(Contract contract)
		{
			return !this.validationMethods.ContainsKey(contract.DeliveryType) || this.validationMethods[contract.DeliveryType](contract);
		}

		private int DeliverProducts(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			if (this.deliveryMethods.ContainsKey(contract.DeliveryType))
			{
				return this.deliveryMethods[contract.DeliveryType](contract, buildingTO, buildingVO);
			}
			return 0;
		}

		private int DeliverTroop(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(contract.ProductUid);
			int level = currentPlayer.UnlockedLevels.Troops.GetLevel(troopTypeVO.UpgradeGroup);
			TroopTypeVO byLevel = Service.TroopUpgradeCatalog.GetByLevel(troopTypeVO, level);
			this.DeliverUnit(currentPlayer.Inventory.Troop, byLevel);
			return 0;
		}

		private int DeliverStarship(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			SpecialAttackTypeVO specialAttackTypeVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(contract.ProductUid);
			int level = currentPlayer.UnlockedLevels.Starships.GetLevel(specialAttackTypeVO.UpgradeGroup);
			SpecialAttackTypeVO byLevel = Service.StarshipUpgradeCatalog.GetByLevel(specialAttackTypeVO, level);
			this.DeliverUnit(currentPlayer.Inventory.SpecialAttack, byLevel);
			return 0;
		}

		private int DeliverHero(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			TroopTypeVO troopTypeVO = Service.StaticDataController.Get<TroopTypeVO>(contract.ProductUid);
			int level = currentPlayer.UnlockedLevels.Troops.GetLevel(troopTypeVO.UpgradeGroup);
			TroopTypeVO byLevel = Service.TroopUpgradeCatalog.GetByLevel(troopTypeVO, level);
			this.DeliverUnit(currentPlayer.Inventory.Hero, byLevel);
			return 0;
		}

		private int DeliverChampion(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			Service.CurrentPlayer.OnChampionRepaired(contract.ProductUid);
			return 0;
		}

		private void DeliverUnit(InventoryStorage inventoryStorage, IDeployableVO deployableVO)
		{
			if (this.temporaryInventorySizeServerDeltas.ContainsKey(inventoryStorage.Key) && this.temporaryInventorySizeServerDeltas[inventoryStorage.Key] < 0)
			{
				Dictionary<string, int> dictionary;
				string key;
				(dictionary = this.temporaryInventorySizeServerDeltas)[key = inventoryStorage.Key] = dictionary[key] + deployableVO.Size;
			}
			else
			{
				inventoryStorage.ModifyItemAmount(deployableVO.Uid, 1);
			}
		}

		private int DeliverBuilding(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			this.OnBuildingContractDelivered(contract, buildingTO, buildingVO);
			return 0;
		}

		private int DeliverUpgradeBuilding(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			buildingTO.Uid = contract.ProductUid;
			this.OnBuildingContractDelivered(contract, buildingTO, buildingVO);
			return 0;
		}

		private int DeliverSwapBuilding(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			buildingTO.Uid = contract.ProductUid;
			this.OnBuildingContractDelivered(contract, buildingTO, buildingVO);
			return 0;
		}

		private void OnBuildingContractDelivered(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			BuildingType type = buildingVO.Type;
			switch (type)
			{
			case BuildingType.FleetCommand:
			case BuildingType.HeroMobilizer:
			case BuildingType.ChampionPlatform:
			case BuildingType.Starport:
				this.UnfreezeAllBuildings(ServerTime.Time);
				return;
			case BuildingType.Housing:
			case BuildingType.Squad:
			{
				IL_27:
				if (type != BuildingType.Resource)
				{
					return;
				}
				bool isConstructionContract = contract.DeliveryType == DeliveryType.Building;
				Service.ICurrencyController.UpdateGeneratorAfterFinishedContract(buildingVO, buildingTO, contract.ContractTO.EndTime, isConstructionContract);
				return;
			}
			}
			goto IL_27;
		}

		private int DeliverUpgradeTroopOrStarship(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			Service.CurrentPlayer.UnlockedLevels.UpgradeTroopsOrStarships(contract);
			return 1;
		}

		private int DeliverUpgradeEquipment(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			Service.CurrentPlayer.UnlockedLevels.UpgradeEquipmentLevel(contract);
			return 1;
		}

		private int ClearClearable(Contract contract, Building buildingTO, BuildingTypeVO buildingVO)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			int currentStorage = buildingTO.CurrentStorage;
			if (currentStorage > 0)
			{
				currentPlayer.Inventory.ModifyCrystals(currentStorage);
				BuildingTypeVO buildingInfo = Service.StaticDataController.Get<BuildingTypeVO>(contract.ProductUid);
				Lang lang = Service.Lang;
				string instructions;
				if (currentStorage == 1)
				{
					instructions = lang.Get("CLEARABLE_FOUND", new object[]
					{
						currentStorage,
						lang.Get("CRYSTAL", new object[0]),
						LangUtils.GetClearableDisplayName(buildingInfo)
					});
				}
				else
				{
					instructions = lang.Get("CLEARABLE_FOUND", new object[]
					{
						currentStorage,
						lang.Get("CRYSTALS", new object[0]),
						LangUtils.GetClearableDisplayName(buildingInfo)
					});
				}
				Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions);
			}
			currentPlayer.Map.Buildings.Remove(buildingTO);
			return 0;
		}

		public Contract StartHeroMobilization(TroopTypeVO hero, SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			Contract result = this.StartTroopContract(hero.Uid, DeliveryType.Hero, hero.Size, building);
			this.SpendCurrencyForDeployableContract(hero, building);
			DeployableContractRequest request = new DeployableContractRequest(buildingComp.BuildingTO.Key, hero.Uid, 1);
			this.serverAPI.Enqueue(new DeployableStartContractCommand(request));
			return result;
		}

		public Contract StartChampionRepair(TroopTypeVO champion, SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			Contract result = this.StartTroopContract(champion.Uid, DeliveryType.Champion, champion.Size, building);
			this.SpendCurrencyForDeployableContract(champion, building);
			DeployableContractRequest request = new DeployableContractRequest(buildingComp.BuildingTO.Key, champion.Uid, 1);
			this.serverAPI.Enqueue(new DeployableStartContractCommand(request));
			this.eventManager.SendEvent(EventId.ChampionStartedRepairing, building);
			return result;
		}

		public Contract StartStarshipMobilization(SpecialAttackTypeVO starship, SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			Contract result = this.StartTroopContract(starship.Uid, DeliveryType.Starship, starship.Size, building);
			this.SpendCurrencyForDeployableContract(starship, building);
			DeployableContractRequest request = new DeployableContractRequest(buildingComp.BuildingTO.Key, starship.Uid, 1);
			this.serverAPI.Enqueue(new DeployableStartContractCommand(request));
			return result;
		}

		public void StartClearingBuilding(SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			BuildingTypeVO buildingType = buildingComp.BuildingType;
			if (!this.StartBuildingContract(buildingType.Uid, DeliveryType.ClearClearable, buildingType.Time, building))
			{
				return;
			}
			GameUtils.SpendCurrency(buildingType.Credits, buildingType.Materials, buildingType.Contraband, true);
			this.eventManager.SendEvent(EventId.ClearableStarted, building);
			BuildingClearCommand command = new BuildingClearCommand(new BuildingClearRequest
			{
				InstanceId = buildingComp.BuildingTO.Key,
				PayWithHardCurrency = false
			});
			this.serverAPI.Enqueue(command);
		}

		public void StartTurretCrossgrade(BuildingTypeVO swapBuildingInfo, SmartEntity turret)
		{
			if (!this.StartBuildingContract(swapBuildingInfo.Uid, DeliveryType.SwapBuilding, swapBuildingInfo.SwapTime, turret))
			{
				return;
			}
			int swapCredits = swapBuildingInfo.SwapCredits;
			int swapMaterials = swapBuildingInfo.SwapMaterials;
			int swapContraband = swapBuildingInfo.SwapContraband;
			GameUtils.SpendCurrency(swapCredits, swapMaterials, swapContraband, true);
			BuildingSwapCommand command = new BuildingSwapCommand(new BuildingSwapRequest
			{
				goingToBuildingUid = swapBuildingInfo.Uid,
				InstanceId = turret.BuildingComp.BuildingTO.Key
			});
			this.serverAPI.Enqueue(command);
			this.eventManager.SendEvent(EventId.BuildingStartedUpgrading, turret);
		}

		public void InstantBuildingConstruct(BuildingTypeVO buildingType, SmartEntity selectedBuilding, int x, int z, string tag)
		{
			BuildingComponent buildingComp = selectedBuilding.BuildingComp;
			this.serverAPI.Enqueue(new BuildingConstructCommand(new BuildingConstructRequest(buildingComp.BuildingTO.Key, buildingComp.BuildingTO.Uid, new Position
			{
				X = x,
				Z = z
			}, false, true, tag)));
			Contract contract = new Contract(buildingType.Uid, DeliveryType.Building, 0, 0.0, tag);
			ContractEventData cookie = new ContractEventData(contract, selectedBuilding, false);
			Service.EventManager.SendEvent(EventId.BuildingConstructed, cookie);
		}

		public void StartBuildingConstruct(BuildingTypeVO buildingType, SmartEntity selectedBuilding, int x, int z, string tag)
		{
			BuildingComponent buildingComp = selectedBuilding.BuildingComp;
			if (buildingType.Type == BuildingType.Resource)
			{
				buildingComp.BuildingTO.LastCollectTime = ServerTime.Time;
			}
			this.StartBuildingContract(buildingType.Uid, DeliveryType.Building, buildingType.Time, selectedBuilding);
			this.serverAPI.Enqueue(new BuildingConstructCommand(new BuildingConstructRequest(buildingComp.BuildingTO.Key, buildingComp.BuildingTO.Uid, new Position
			{
				X = x,
				Z = z
			}, false, buildingComp.BuildingType.Time == 0, tag)));
		}

		public void StartBuildingUpgrade(BuildingTypeVO nextUpgradeType, SmartEntity selectedBuilding, bool isInstant)
		{
			this.StartBuildingUpgrade(nextUpgradeType, selectedBuilding, isInstant, string.Empty);
		}

		public void StartBuildingUpgrade(BuildingTypeVO nextUpgradeType, SmartEntity selectedBuilding, bool isInstant, string tag)
		{
			int totalTime = (!isInstant) ? nextUpgradeType.Time : 0;
			if (!this.StartBuildingContract(nextUpgradeType.Uid, DeliveryType.UpgradeBuilding, totalTime, selectedBuilding, true, tag))
			{
				return;
			}
			Service.ICurrencyController.ForceCollectAccruedCurrencyForUpgrade(selectedBuilding);
			if (!isInstant)
			{
				int credits = nextUpgradeType.Credits;
				int materials = nextUpgradeType.Materials;
				int contraband = nextUpgradeType.Contraband;
				GameUtils.SpendCurrency(credits, materials, contraband, true);
			}
			this.eventManager.SendEvent(EventId.BuildingStartedUpgrading, selectedBuilding);
			if (!isInstant)
			{
				BuildingComponent buildingComp = selectedBuilding.BuildingComp;
				this.serverAPI.Enqueue(new BuildingUpgradeCommand(new BuildingContractRequest(buildingComp.BuildingTO.Key, false, tag)));
			}
		}

		public void StartAllWallPartBuildingUpgrade(BuildingTypeVO nextUpgradeType, SmartEntity selectedBuilding, bool sendBackendCommand, bool sendBILog)
		{
			if (!this.StartBuildingContract(nextUpgradeType.Uid, DeliveryType.UpgradeBuilding, nextUpgradeType.Time, selectedBuilding, sendBILog))
			{
				return;
			}
			if (sendBackendCommand)
			{
				int credits = nextUpgradeType.Credits;
				int materials = nextUpgradeType.Materials;
				int contraband = nextUpgradeType.Contraband;
				GameUtils.SpendCurrency(credits, materials, contraband, false);
			}
			BuildingComponent buildingComp = selectedBuilding.BuildingComp;
			if (sendBackendCommand)
			{
				this.serverAPI.Enqueue(new BuildingUpgradeCommand(new BuildingContractRequest(buildingComp.BuildingTO.Key, false, string.Empty)));
			}
		}

		public bool StartTroopUpgrade(TroopTypeVO troop, SmartEntity building)
		{
			this.StartBuildingContract(troop.Uid, DeliveryType.UpgradeTroop, troop.UpgradeTime, building);
			int upgradeCredits = troop.UpgradeCredits;
			int upgradeMaterials = troop.UpgradeMaterials;
			int upgradeContraband = troop.UpgradeContraband;
			GameUtils.SpendCurrency(upgradeCredits, upgradeMaterials, upgradeContraband, true);
			if (!string.IsNullOrEmpty(troop.UpgradeShardUid))
			{
				Service.DeployableShardUnlockController.SpendDeployableShard(troop.UpgradeShardUid, troop.UpgradeShardCount);
			}
			this.StartDeployableUpgradeOnServer(troop.Uid, building.BuildingComp.BuildingTO.Key);
			return true;
		}

		public bool StartStarshipUpgrade(SpecialAttackTypeVO starship, SmartEntity building)
		{
			this.StartBuildingContract(starship.Uid, DeliveryType.UpgradeStarship, starship.UpgradeTime, building);
			int upgradeCredits = starship.UpgradeCredits;
			int upgradeMaterials = starship.UpgradeMaterials;
			int upgradeContraband = starship.UpgradeContraband;
			GameUtils.SpendCurrency(upgradeCredits, upgradeMaterials, upgradeContraband, true);
			if (!string.IsNullOrEmpty(starship.UpgradeShardUid))
			{
				Service.DeployableShardUnlockController.SpendDeployableShard(starship.UpgradeShardUid, starship.UpgradeShardCount);
			}
			this.StartDeployableUpgradeOnServer(starship.Uid, building.BuildingComp.BuildingTO.Key);
			return true;
		}

		private void StartDeployableUpgradeOnServer(string deployableUid, string buildingInstanceId)
		{
			this.serverAPI.Enqueue(new DeployableUpgradeStartCommand(new DeployableUpgradeStartRequest
			{
				BuildingId = buildingInstanceId,
				TroopUid = deployableUid
			}));
		}

		public bool StartEquipmentUpgrade(EquipmentVO equipment, SmartEntity building)
		{
			this.StartBuildingContract(equipment.Uid, DeliveryType.UpgradeEquipment, equipment.UpgradeTime, building);
			Service.ArmoryController.ChargeEquipmentUpgradeCost(equipment);
			EquipmentUpgradeStartRequest request = new EquipmentUpgradeStartRequest(building.BuildingComp.BuildingTO.Key, equipment.Uid);
			EquipmentUpgradeStartCommand equipmentUpgradeStartCommand = new EquipmentUpgradeStartCommand(request);
			equipmentUpgradeStartCommand.Context = equipment.EquipmentID;
			equipmentUpgradeStartCommand.AddFailureCallback(new AbstractCommand<EquipmentUpgradeStartRequest, DefaultResponse>.OnFailureCallback(this.OnEquipmentUpgradeFailure));
			this.serverAPI.Enqueue(equipmentUpgradeStartCommand);
			return true;
		}

		private void OnEquipmentUpgradeFailure(uint status, object cookie)
		{
			Service.Logger.ErrorFormat("StartEquipmentUpgrade equipmentID '{0}' failed!", new object[]
			{
				cookie as string
			});
		}

		public Contract StartTroopTrainContract(TroopTypeVO troop, SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			DeliveryType deliveryType = DeliveryType.Invalid;
			TroopType type = troop.Type;
			if (type != TroopType.Infantry)
			{
				if (type != TroopType.Vehicle)
				{
					if (type == TroopType.Mercenary)
					{
						deliveryType = DeliveryType.Mercenary;
					}
				}
				else
				{
					deliveryType = DeliveryType.Vehicle;
				}
			}
			else
			{
				deliveryType = DeliveryType.Infantry;
			}
			if (deliveryType == DeliveryType.Invalid)
			{
				Service.Logger.ErrorFormat("Error adding bad troop train contract: {0}", new object[]
				{
					troop.Uid
				});
				return null;
			}
			Contract contract = this.StartTroopContract(troop.Uid, deliveryType, troop.Size, building);
			if (contract == null)
			{
				Service.Logger.ErrorFormat("Error Adding contract {0} at {1}", new object[]
				{
					troop.Uid,
					buildingComp.BuildingTO.Key
				});
				return null;
			}
			this.SpendCurrencyForDeployableContract(troop, building);
			DeployableContractRequest request = new DeployableContractRequest(buildingComp.BuildingTO.Key, troop.Uid, 1);
			this.serverAPI.Enqueue(new DeployableStartContractCommand(request));
			return contract;
		}

		public void CancelTroopTrainContract(string productUid, SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			string key = buildingComp.BuildingTO.Key;
			Contract contract = this.FindLastContractById(key, productUid);
			if (contract == null)
			{
				Service.Logger.ErrorFormat("No contract for {0} at {1}", new object[]
				{
					productUid,
					key
				});
				return;
			}
			this.SimulateCheckAllContractsWithCurrentTime();
			this.CancelContract(building, contract);
			uint time = ServerTime.Time;
			int num = 0;
			if (!this.frozenBuildings.Contains(key))
			{
				uint endTime = contract.ContractTO.EndTime;
				if (endTime - (uint)contract.TotalTime <= time)
				{
					num = GameUtils.GetTimeDifferenceSafe(endTime, time);
				}
				else
				{
					num = contract.TotalTime;
				}
				if (num > 0)
				{
					this.ShiftTroopContractTimesAfterThis(key, -num, endTime);
				}
			}
			if (num > 0 || this.IsBuildingFrozen(key))
			{
				this.RefundContractCost(contract);
				this.UnfreezeAllBuildings(time);
				DeployableContractRequest request = new DeployableContractRequest(key, productUid, 1);
				this.serverAPI.Enqueue(new DeployableCancelContractCommand(request));
			}
			else
			{
				this.UnfreezeAllBuildings(time);
			}
			ContractEventData cookie = new ContractEventData(contract, building, false, false);
			this.eventManager.SendEvent(EventId.TroopCancelled, cookie);
		}

		public void BuyoutAllTroopTrainContracts(SmartEntity entity)
		{
			this.BuyoutAllTroopTrainContracts(entity, false);
		}

		public void BuyoutAllTroopTrainContracts(SmartEntity entity, bool alreadySpentCrystals)
		{
			BuildingComponent buildingComp = entity.BuildingComp;
			List<Contract> list = new List<Contract>(this.FindAllTroopContractsForBuilding(buildingComp.BuildingTO.Key));
			int num = 0;
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				Contract contract = null;
				int num2 = 0;
				bool flag = false;
				if (i == list.Count - 1 || list[i].ProductUid != list[i + 1].ProductUid)
				{
					contract = list[num];
					num2 = i + 1 - num;
					int num3;
					if (num == 0)
					{
						num3 = contract.GetRemainingTimeForSim();
					}
					else
					{
						num3 = contract.TotalTime;
					}
					int num4 = contract.TotalTime * (num2 - 1) + num3;
					if (num4 > 0)
					{
						int num5 = GameUtils.SecondsToCrystals(num4);
						if (!alreadySpentCrystals && !GameUtils.SpendCrystals(num5))
						{
							return;
						}
						int currencyAmount = -num5;
						string itemType = "unit";
						string productUid = contract.ProductUid;
						int itemCount = num2;
						string type = (!Service.CurrentPlayer.CampaignProgress.FueInProgress) ? "speed_up_unit" : "FUE_speed_up_unit";
						string subType = "consumable";
						Service.DMOAnalyticsController.LogInAppCurrencyAction(currencyAmount, itemType, productUid, itemCount, type, subType);
						flag = true;
					}
					num = i + 1;
				}
				bool silent = i != 0;
				if (flag)
				{
					this.FinishCurrentContract(entity, silent, 0, new SupportController.FinishContractOnServerDelegate(this.BuyoutDeployableContractOnServer), buildingComp.BuildingTO.Key, contract.ProductUid, num2);
				}
				else
				{
					this.FinishCurrentContract(entity, silent);
				}
				this.UpdateTooltips(entity);
				i++;
			}
		}

		private void BuyoutDeployableContractOnServer(string buildingKey, string productUid, int amount)
		{
			DeployableContractRequest request = new DeployableContractRequest(buildingKey, productUid, amount);
			this.serverAPI.Enqueue(new DeployableBuyoutContractCommand(request));
		}

		private bool StartBuildingContract(string productUid, DeliveryType contractType, int totalTime, SmartEntity building)
		{
			return this.StartBuildingContract(productUid, contractType, totalTime, building, true);
		}

		private bool StartBuildingContract(string productUid, DeliveryType contractType, int totalTime, SmartEntity building, bool sendBILog)
		{
			return this.StartBuildingContract(productUid, contractType, totalTime, building, sendBILog, string.Empty);
		}

		private bool StartBuildingContract(string productUid, DeliveryType contractType, int totalTime, SmartEntity building, bool sendBILog, string tag)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			string key = buildingComp.BuildingTO.Key;
			Contract contract = this.FindBuildingContract(key);
			uint time = ServerTime.Time;
			if (contract != null)
			{
				return false;
			}
			contract = new Contract(productUid, contractType, totalTime, this.serverAPI.ServerTimePrecise, tag);
			contract.ContractTO = new ContractTO();
			contract.ContractTO.BuildingKey = key;
			contract.ContractTO.Uid = productUid;
			contract.ContractTO.ContractType = ContractUtils.GetContractType(contractType);
			contract.ContractTO.EndTime = time + (uint)totalTime;
			contract.ContractTO.Tag = tag;
			int num = totalTime;
			if (this.IsBuildingFrozen(key))
			{
				Contract contract2 = this.FindFirstTroopContractForBuilding(key);
				if (contract2 != null)
				{
					uint endTime = contract2.ContractTO.EndTime;
					if (time > endTime)
					{
						uint num2 = time - endTime;
						num += (int)num2;
					}
				}
			}
			if (buildingComp.BuildingType.Type != BuildingType.Wall)
			{
				this.ShiftTroopContractTimes(key, num);
			}
			this.AddContract(contract, building);
			ContractEventData cookie = new ContractEventData(contract, building, false, sendBILog);
			this.eventManager.SendEvent(EventId.ContractAdded, cookie);
			this.eventManager.SendEvent(EventId.ContractStarted, cookie);
			this.UpdateTooltips(building);
			if (buildingComp.BuildingType.Type != BuildingType.Wall)
			{
				DeliveryType deliveryType = contract.DeliveryType;
				if (deliveryType == DeliveryType.Building || deliveryType == DeliveryType.UpgradeBuilding || deliveryType == DeliveryType.SwapBuilding)
				{
					this.DisableBuilding(building);
				}
			}
			return true;
		}

		private Contract StartTroopContract(string productUid, DeliveryType contractType, int productSize, SmartEntity building)
		{
			BuildingComponent buildingComp = building.BuildingComp;
			Contract contract = this.FindBuildingContract(buildingComp.BuildingTO.Key);
			if (contract != null)
			{
				return null;
			}
			if (productSize > 0 && !ContractUtils.HasCapacityForTroop(building, productSize))
			{
				return null;
			}
			List<string> playerActivePerkIds = Service.PerkManager.GetPlayerActivePerkIds();
			int troopContractTotalTime = ContractUtils.GetTroopContractTotalTime(productUid, contractType, playerActivePerkIds);
			Contract contract2 = new Contract(productUid, contractType, troopContractTotalTime, this.serverAPI.ServerTimePrecise);
			contract2.ContractTO = new ContractTO();
			contract2.ContractTO.BuildingKey = buildingComp.BuildingTO.Key;
			contract2.ContractTO.Uid = productUid;
			contract2.ContractTO.ContractType = ContractUtils.GetContractType(contractType);
			contract2.ContractTO.PerkIds = playerActivePerkIds;
			uint time = ServerTime.Time;
			Contract contract3 = this.FindLastTroopContractForBuilding(buildingComp.BuildingTO.Key, productUid);
			if (contract3 != null)
			{
				this.ShiftTroopContractTimesAfterThis(buildingComp.BuildingTO.Key, contract2.TotalTime, contract3.ContractTO.EndTime);
				contract2.ContractTO.EndTime = contract3.ContractTO.EndTime + (uint)contract2.TotalTime;
			}
			else
			{
				contract2.ContractTO.EndTime = time + (uint)contract2.TotalTime;
			}
			this.AddContract(contract2, building);
			ContractEventData cookie = new ContractEventData(contract2, building, false);
			this.eventManager.SendEvent(EventId.ContractAdded, cookie);
			this.eventManager.SendEvent(EventId.ContractBacklogUpdated, cookie);
			this.eventManager.SendEvent(EventId.ContractStarted, cookie);
			this.UpdateTooltips(building);
			return contract2;
		}

		private void UpdateTooltips(SmartEntity building)
		{
			Service.BuildingTooltipController.EnsureBuildingTooltip(building);
			Service.UXController.HUD.UpdateDroidCount();
		}

		public void CancelCurrentBuildingContract(Contract contract, SmartEntity building)
		{
			if (contract.GetRemainingTimeForSim() <= 0)
			{
				return;
			}
			this.RefundContractCost(contract);
			this.CancelContract(building, contract);
			uint serverTime = this.serverAPI.ServerTime;
			BuildingComponent buildingComp = building.BuildingComp;
			string key = buildingComp.BuildingTO.Key;
			int timeDifferenceSafe = GameUtils.GetTimeDifferenceSafe(serverTime, contract.ContractTO.EndTime);
			this.ShiftTroopContractTimes(key, timeDifferenceSafe);
			contract.ContractTO.EndTime = serverTime;
			this.SortCurrentContracts();
			if (buildingComp.BuildingType.Type == BuildingType.Resource)
			{
				buildingComp.BuildingTO.LastCollectTime = serverTime;
			}
			this.serverAPI.Enqueue(new BuildingContractCancelCommand(new BuildingContractRequest(key, true, string.Empty)));
			ContractEventData cookie = new ContractEventData(contract, building, false, false);
			this.eventManager.SendEvent(EventId.BuildingCancelled, cookie);
		}

		private bool IsRefundableContractTypeForBuilding(ContractType contractType)
		{
			return contractType == ContractType.Upgrade || contractType == ContractType.Clear || contractType == ContractType.Research;
		}

		private void RefundContractCost(Contract contract)
		{
			string uid = contract.ContractTO.Uid;
			string buildingKey = contract.ContractTO.BuildingKey;
			List<string> perkIds = contract.ContractTO.PerkIds;
			int num = 0;
			int num2 = 0;
			int num3 = 0;
			SmartEntity smartEntity = null;
			ContractType contractType = ContractUtils.GetContractType(contract.DeliveryType);
			float multiplier;
			if (this.IsRefundableContractTypeForBuilding(contractType))
			{
				multiplier = (float)GameConstants.CONTRACT_REFUND_PERCENTAGE_BUILDINGS / 100f;
			}
			else
			{
				if (!ContractUtils.IsTroopType(contractType))
				{
					Service.Logger.Error("Attempted to refund unsupported contract type: " + contractType.ToString());
					return;
				}
				multiplier = (float)GameConstants.CONTRACT_REFUND_PERCENTAGE_TROOPS / 100f;
			}
			this.buildingKeyToEntities.TryGetValue(buildingKey, out smartEntity);
			BuildingTypeVO buildingVO = null;
			if (smartEntity != null)
			{
				buildingVO = smartEntity.BuildingComp.BuildingType;
			}
			ContractUtils.CalculateContractCost(uid, contract.DeliveryType, buildingVO, perkIds, out num, out num2, out num3);
			GameUtils.MultiplyCurrency(multiplier, ref num, ref num2, ref num3);
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (num > 0)
			{
				currentPlayer.Inventory.ModifyCredits(num);
			}
			if (num2 > 0)
			{
				currentPlayer.Inventory.ModifyMaterials(num2);
			}
			if (num3 > 0)
			{
				currentPlayer.Inventory.ModifyContraband(num3);
			}
		}

		private Contract CancelContract(SmartEntity entity, Contract contract)
		{
			ContractEventData cookie = new ContractEventData(contract, entity, false);
			this.eventManager.SendEvent(EventId.ContractCanceled, cookie);
			this.eventManager.SendEvent(EventId.ContractBacklogUpdated, cookie);
			this.RemoveContract(entity, contract, true);
			this.UpdateTooltips(entity);
			return contract;
		}

		public bool FinishCurrentContract(SmartEntity entity, bool silent, bool sendBILog)
		{
			return this.FinishCurrentContract(entity, silent, 0, null, null, null, 0, sendBILog);
		}

		public bool FinishCurrentContract(SmartEntity entity, bool silent)
		{
			return this.FinishCurrentContract(entity, silent, 0, null, null, null, 0, true);
		}

		private bool FinishCurrentContract(SmartEntity entity, bool silent, int troopContractShiftTime, SupportController.FinishContractOnServerDelegate serverDelegate, string buildingKey, string productUid, int amount)
		{
			return this.FinishCurrentContract(entity, silent, troopContractShiftTime, serverDelegate, buildingKey, productUid, amount, true);
		}

		private bool FinishCurrentContract(SmartEntity entity, bool silent, int troopContractShiftTime, SupportController.FinishContractOnServerDelegate serverDelegate, string buildingKey, string productUid, int amount, bool sendBILog)
		{
			if (entity == null)
			{
				Service.Logger.Error("entity is null in FinishCurrentContract");
				return false;
			}
			BuildingComponent buildingComp = entity.BuildingComp;
			if (buildingComp == null)
			{
				GameUtils.LogComponentsAsError("buildingComp is null in FinishCurrentContract", entity);
				return false;
			}
			Building buildingTO = buildingComp.BuildingTO;
			if (buildingTO == null)
			{
				Service.Logger.Error("buildingTO is null in FinishCurrentContract");
				return false;
			}
			string key = buildingTO.Key;
			if (key == null)
			{
				Service.Logger.ErrorFormat("buildingTOKey is null in FinishCurrentContract", new object[0]);
				return false;
			}
			Contract contract = this.FindCurrentContract(key);
			ContractEventData cookie = new ContractEventData(contract, entity, silent, sendBILog);
			if (buildingComp.BuildingType.Type != BuildingType.Wall && !this.IsContractValidForStorage(contract))
			{
				this.eventManager.SendEvent(EventId.ContractInvalidForStorage, cookie);
				this.eventManager.SendEvent(EventId.ContractStopped, entity);
				this.FreezeBuilding(key);
				return false;
			}
			if (troopContractShiftTime != 0)
			{
				this.ShiftTroopContractTimes(key, troopContractShiftTime);
			}
			this.RemoveContract(entity, contract, false);
			this.DeliverProducts(contract, buildingTO, buildingComp.BuildingType);
			this.UpdateTooltips(entity);
			if (serverDelegate != null)
			{
				serverDelegate(buildingKey, productUid, amount);
			}
			if (this.events.ContainsKey(contract.DeliveryType))
			{
				this.eventManager.SendEvent(this.events[contract.DeliveryType], cookie);
			}
			EventId eventId = EventId.ContractCompletedForStoryAction;
			if (eventId != EventId.Nop)
			{
				this.eventManager.SendEvent(eventId, contract.ContractTO);
			}
			this.eventManager.SendEvent(EventId.ContractCompleted, cookie);
			this.eventManager.SendEvent(EventId.ContractBacklogUpdated, cookie);
			string productUid2 = contract.ProductUid;
			if (this.IsDeployableUpgradeContract(contract) && Service.DeployableShardUnlockController.IsUIDForAShardUpgradableDeployable(productUid2))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				IDeployableVO optional = staticDataController.GetOptional<TroopTypeVO>(productUid2);
				if (optional == null)
				{
					optional = staticDataController.GetOptional<SpecialAttackTypeVO>(productUid2);
				}
				Service.EventManager.SendEvent(EventId.ShardUnitUpgraded, optional);
			}
			return true;
		}

		private bool IsDeployableUpgradeContract(Contract contract)
		{
			return contract.DeliveryType == DeliveryType.UpgradeStarship || contract.DeliveryType == DeliveryType.UpgradeTroop;
		}

		public void BuyOutCurrentBuildingContract(SmartEntity entity, bool sendBackendCommand)
		{
			BuildingComponent buildingComp = entity.BuildingComp;
			Contract contract = this.FindCurrentContract(buildingComp.BuildingTO.Key);
			if (contract == null)
			{
				return;
			}
			if (contract.GetRemainingTimeForSim() <= 0)
			{
				return;
			}
			bool flag = contract.DeliveryType == DeliveryType.Champion;
			uint time = ServerTime.Time;
			int troopContractShiftTime = (!flag) ? GameUtils.GetTimeDifferenceSafe(time, contract.ContractTO.EndTime) : 0;
			contract.ContractTO.EndTime = time;
			SupportController.FinishContractOnServerDelegate serverDelegate = null;
			string productUid = (!flag) ? buildingComp.BuildingTO.Uid : contract.ProductUid;
			if (sendBackendCommand)
			{
				if (contract.DeliveryType == DeliveryType.Champion)
				{
					serverDelegate = new SupportController.FinishContractOnServerDelegate(this.BuyoutDeployableContractOnServer);
				}
				else
				{
					serverDelegate = new SupportController.FinishContractOnServerDelegate(this.BuyoutBuildingOnServer);
				}
			}
			if ((contract.DeliveryType == DeliveryType.Building || contract.DeliveryType == DeliveryType.UpgradeBuilding) && buildingComp.BuildingType.Type == BuildingType.NavigationCenter)
			{
				Service.CurrentPlayer.AddUnlockedPlanet(contract.Tag);
			}
			this.FinishCurrentContract(entity, false, troopContractShiftTime, serverDelegate, buildingComp.BuildingTO.Key, productUid, 1);
		}

		private void BuyoutBuildingOnServer(string buildingKey, string productUid, int amount)
		{
			this.serverAPI.Enqueue(new BuildingContractBuyoutCommand(new BuildingContractRequest(buildingKey, true, string.Empty)));
		}

		private void SortCurrentContracts()
		{
			if (!this.isIterating)
			{
				this.currentContracts.Sort(new Comparison<Contract>(this.SortByEndTime));
				this.needSortCurrentContracts = false;
			}
			else
			{
				this.needSortCurrentContracts = true;
			}
		}

		private void IterationBegin()
		{
			this.mutableIterator.Init(this.currentContracts);
			this.isIterating = true;
		}

		private void IterationEnd()
		{
			this.mutableIterator.Reset();
			this.isIterating = false;
			if (this.needSortCurrentContracts)
			{
				this.SortCurrentContracts();
			}
		}

		private void RemoveContract(SmartEntity buildingEntity, Contract contract, bool isCanceling)
		{
			string buildingKey = contract.ContractTO.BuildingKey;
			int num = this.currentContracts.IndexOf(contract);
			if (num >= 0)
			{
				this.currentContracts.RemoveAt(num);
				this.mutableIterator.OnRemove(num);
			}
			this.SortCurrentContracts();
			Contract contract2 = this.FindCurrentContract(buildingKey);
			if (contract2 != null)
			{
				this.eventManager.SendEvent(EventId.ContractStarted, new ContractEventData(contract2, buildingEntity, false));
			}
			else
			{
				SupportViewComponent supportViewComp = buildingEntity.SupportViewComp;
				if (supportViewComp != null)
				{
					supportViewComp.TeardownElements();
				}
				this.eventManager.SendEvent(EventId.ContractStopped, buildingEntity);
			}
			DeliveryType deliveryType = contract.DeliveryType;
			if (deliveryType != DeliveryType.Building)
			{
				if (deliveryType == DeliveryType.UpgradeBuilding)
				{
					if (isCanceling)
					{
						this.EnableBuilding(buildingEntity);
					}
				}
			}
			else
			{
				this.EnableBuilding(buildingEntity);
			}
		}

		private void ShiftTroopContractTimes(string buildingKey, int shiftTime)
		{
			List<Contract> list = this.FindAllTroopContractsForBuilding(buildingKey);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				list[i].ContractTO.EndTime = GameUtils.GetModifiedTimeSafe(list[i].ContractTO.EndTime, shiftTime);
				i++;
			}
		}

		private void ShiftTroopContractTimesAfterThis(string buildingKey, int shiftTime, uint leastEndTime)
		{
			List<Contract> list = this.FindAllTroopContractsForBuilding(buildingKey);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				if (list[i].ContractTO.EndTime > leastEndTime)
				{
					list[i].ContractTO.EndTime = GameUtils.GetModifiedTimeSafe(list[i].ContractTO.EndTime, shiftTime);
				}
				i++;
			}
		}

		public void OnViewFrameTime(float dt)
		{
			this.accumulatedUpdateDt += dt;
			if (this.accumulatedUpdateDt >= 0.1f)
			{
				this.UpdateAllContracts(this.serverAPI.ServerTime, this.serverAPI.ServerTimePrecise);
				this.accumulatedUpdateDt = 0f;
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			IGameState gameState = Service.GameStateMachine.CurrentState as IGameState;
			if (id != EventId.WorldLoadComplete)
			{
				if (id != EventId.GameStateChanged)
				{
					if (id == EventId.ContractsCompletedWhileOffline)
					{
						this.offlineCompletedContracts = (cookie as List<ContractTO>);
					}
				}
				else if (!gameState.CanUpdateHomeContracts())
				{
					Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
				}
			}
			else if (gameState is ApplicationLoadState || gameState is HomeState || gameState is WarBoardState)
			{
				this.InitializeContracts();
				this.SendContractContinuedEvents();
				Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
			}
			return EatResponse.NotEaten;
		}

		private void AddContract(Contract contract, SmartEntity entity)
		{
			this.currentContracts.Add(contract);
			this.SortCurrentContracts();
		}

		private void UpdatingBuildingKeyToEntity()
		{
			this.buildingKeyToEntities.Clear();
			NodeList<SupportNode> nodeList = Service.EntityController.GetNodeList<SupportNode>();
			for (SupportNode supportNode = nodeList.Head; supportNode != null; supportNode = supportNode.Next)
			{
				string key = supportNode.BuildingComp.BuildingTO.Key;
				if (!this.buildingKeyToEntities.ContainsKey(key))
				{
					this.buildingKeyToEntities.Add(key, (SmartEntity)supportNode.Entity);
				}
				else
				{
					Service.Logger.Error("UpdatingBuildingKeyToEntity has duplicates");
				}
			}
		}

		private void UpdateAllContracts(uint now, double nowPrecise)
		{
			this.UpdatingBuildingKeyToEntity();
			this.IterationBegin();
			while (this.mutableIterator.Active())
			{
				Contract contract = this.currentContracts[this.mutableIterator.Index];
				ContractTO contractTO = contract.ContractTO;
				if (contractTO == null)
				{
					Service.Logger.Error("UpdateAllContracts: Null ContractTo found.");
				}
				else
				{
					string buildingKey = contractTO.BuildingKey;
					if (buildingKey == null)
					{
						Service.Logger.Error("UpdateAllContracts: Null buildingKey found.");
					}
					else if (nowPrecise - contract.LastUpdateTime >= 1.0)
					{
						contract.LastUpdateTime += 1.0;
						if (ContractUtils.IsBuildingType(contractTO.ContractType) || !this.IsBuildingFrozen(buildingKey))
						{
							SmartEntity smartEntity;
							this.buildingKeyToEntities.TryGetValue(buildingKey, out smartEntity);
							if (smartEntity != null)
							{
								contract.UpdateRemainingTime();
								if (!this.pausedBuildings.Contains(buildingKey))
								{
									if (contract.IsReadyToBeFinished())
									{
										this.FinishCurrentContract(smartEntity, false);
									}
								}
							}
							else
							{
								Service.Logger.Error("UpdateAllContracts: Null entity found.");
							}
						}
					}
				}
				this.mutableIterator.Next();
			}
			this.IterationEnd();
		}

		public void PauseBuilding(string buildingKey)
		{
			if (!this.pausedBuildings.Contains(buildingKey))
			{
				this.pausedBuildings.Add(buildingKey);
			}
		}

		public void UnpauseAllBuildings()
		{
			this.pausedBuildings.Clear();
			this.SortCurrentContracts();
		}

		public bool IsBuildingFrozen(string buildingKey)
		{
			return this.frozenBuildings.Contains(buildingKey);
		}

		private void FreezeBuilding(string buildingKey)
		{
			if (!this.frozenBuildings.Contains(buildingKey))
			{
				this.frozenBuildings.Add(buildingKey);
			}
		}

		public void UnfreezeAllBuildings(uint time)
		{
			Dictionary<string, uint> dictionary = null;
			HashSet<string> hashSet = null;
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				Contract contract = this.currentContracts[i];
				string buildingKey = contract.ContractTO.BuildingKey;
				if (hashSet == null || !hashSet.Contains(buildingKey))
				{
					if (this.frozenBuildings.Contains(buildingKey) && !contract.DoNotShiftTimesForUnfreeze)
					{
						if (ContractUtils.IsBuildingType(contract.ContractTO.ContractType))
						{
							if (dictionary == null)
							{
								dictionary = new Dictionary<string, uint>();
							}
							dictionary.Add(buildingKey, contract.ContractTO.EndTime);
						}
						else
						{
							if (hashSet == null)
							{
								hashSet = new HashSet<string>();
							}
							hashSet.Add(buildingKey);
							int num = 0;
							uint endTime = contract.ContractTO.EndTime;
							if (dictionary != null && dictionary.ContainsKey(buildingKey))
							{
								uint timeB = dictionary[buildingKey];
								num = GameUtils.GetTimeDifferenceSafe(endTime, timeB);
							}
							else if (time > contract.ContractTO.EndTime)
							{
								num = GameUtils.GetTimeDifferenceSafe(time, endTime);
							}
							if (num != 0)
							{
								this.ShiftTroopContractTimes(buildingKey, num);
							}
						}
					}
				}
				i++;
			}
			this.frozenBuildings.Clear();
			this.SortCurrentContracts();
		}

		private bool ShouldBeFrozen(Contract contract, Dictionary<string, int> inventorySizeOffsets, bool isTroopContract)
		{
			bool result = false;
			if (isTroopContract)
			{
				if (!ContractUtils.IsArmyContractValid(contract, inventorySizeOffsets))
				{
					result = true;
				}
			}
			else if (!this.IsContractValidForStorage(contract))
			{
				result = true;
			}
			return result;
		}

		public void SimulateCheckAllContractsWithCurrentTime()
		{
			List<Contract> list = null;
			this.SimulateCheckAllContractsWithCurrentTime(true, true, out list);
		}

		private void SimulateCheckAllContractsWithCurrentTime(bool updateFreezeState, bool simulateTroopContractUpdate, out List<Contract> finishedContracts)
		{
			finishedContracts = null;
			uint time = ServerTime.Time;
			Dictionary<string, int> dictionary = new Dictionary<string, int>();
			HashSet<string> hashSet = null;
			foreach (KeyValuePair<string, int> current in this.temporaryInventorySizeServerDeltas)
			{
				dictionary.Add(current.Key, current.Value);
			}
			int i = 0;
			int count = this.currentContracts.Count;
			while (i < count)
			{
				Contract contract = this.currentContracts[i];
				string buildingKey = contract.ContractTO.BuildingKey;
				bool flag = ContractUtils.IsTroopType(contract.ContractTO.ContractType);
				if (!flag || simulateTroopContractUpdate)
				{
					if (hashSet == null || !hashSet.Contains(buildingKey) || !flag)
					{
						if (time >= contract.ContractTO.EndTime)
						{
							if (this.ShouldBeFrozen(contract, dictionary, flag))
							{
								if (updateFreezeState)
								{
									this.FreezeBuilding(buildingKey);
								}
								if (hashSet == null)
								{
									hashSet = new HashSet<string>();
								}
								hashSet.Add(buildingKey);
							}
							else
							{
								if (updateFreezeState)
								{
									contract.DoNotShiftTimesForUnfreeze = true;
								}
								if (flag)
								{
									InventoryStorage inventoryStorage;
									int num;
									ContractUtils.GetArmyContractStorageAndSize(contract, out inventoryStorage, out num);
									if (dictionary.ContainsKey(inventoryStorage.Key))
									{
										Dictionary<string, int> dictionary2;
										string key;
										(dictionary2 = dictionary)[key = inventoryStorage.Key] = dictionary2[key] + num;
									}
									else
									{
										dictionary.Add(inventoryStorage.Key, num);
									}
								}
								if (contract.ContractTO.ContractType == ContractType.Upgrade && contract.TotalTime == 0)
								{
									BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(contract.ProductUid);
									if (buildingTypeVO.Type == BuildingType.Wall)
									{
										goto IL_1DE;
									}
								}
								if (finishedContracts == null)
								{
									finishedContracts = new List<Contract>();
								}
								finishedContracts.Add(contract);
							}
						}
					}
				}
				IL_1DE:
				i++;
			}
		}

		public void GetEstimatedUpdatedContractListsForChecksum(bool simulateTroopContractUpdate, out List<Contract> remainingContracts, out List<Contract> finishedContracts)
		{
			remainingContracts = null;
			finishedContracts = null;
			if (this.currentContracts != null)
			{
				this.SimulateCheckAllContractsWithCurrentTime(false, simulateTroopContractUpdate, out finishedContracts);
				remainingContracts = new List<Contract>(this.currentContracts);
				if (finishedContracts != null)
				{
					int i = 0;
					int count = finishedContracts.Count;
					while (i < count)
					{
						remainingContracts.Remove(finishedContracts[i]);
						i++;
					}
				}
			}
		}

		public void SyncCurrentPlayerInventoryWithServer(Dictionary<string, object> deployables)
		{
			if (deployables != null)
			{
				Inventory inventory = Service.CurrentPlayer.Inventory;
				this.SyncInventory(inventory.Troop, "troop", deployables);
				this.SyncInventory(inventory.SpecialAttack, "specialAttack", deployables);
				this.SyncInventory(inventory.Hero, "hero", deployables);
				this.SyncInventory(inventory.Champion, "champion", deployables);
			}
		}

		private void SyncInventory(InventoryStorage storage, string storageKey, Dictionary<string, object> deployables)
		{
			if (storage == null)
			{
				return;
			}
			if (deployables.ContainsKey(storageKey))
			{
				Dictionary<string, object> dictionary = deployables[storageKey] as Dictionary<string, object>;
				if (dictionary == null)
				{
					return;
				}
				int totalStorageAmount = storage.GetTotalStorageAmount();
				foreach (string current in storage.GetInternalStorage().Keys)
				{
					storage.ClearItemAmount(current);
				}
				foreach (string current2 in dictionary.Keys)
				{
					if (!storage.HasItem(current2))
					{
						InventoryEntry inventoryEntry = new InventoryEntry();
						inventoryEntry.FromObject(dictionary[current2]);
						storage.CreateInventoryItem(current2, inventoryEntry);
					}
					else
					{
						Dictionary<string, object> dictionary2 = dictionary[current2] as Dictionary<string, object>;
						if (dictionary2 != null)
						{
							int delta = Convert.ToInt32(dictionary2["amount"]);
							storage.ModifyItemAmount(current2, delta);
						}
					}
				}
				int totalStorageAmount2 = storage.GetTotalStorageAmount();
				int num = totalStorageAmount - totalStorageAmount2;
				if (this.temporaryInventorySizeServerDeltas.ContainsKey(storage.Key))
				{
					Dictionary<string, int> dictionary3;
					string key;
					(dictionary3 = this.temporaryInventorySizeServerDeltas)[key = storage.Key] = dictionary3[key] + num;
				}
				else
				{
					this.temporaryInventorySizeServerDeltas.Add(storage.Key, num);
				}
			}
		}

		public void CheatForceUpdateAllContracts()
		{
			this.UpdateAllContracts(this.serverAPI.ServerTime, this.serverAPI.ServerTimePrecise);
		}

		private void SpendCurrencyForDeployableContract(IDeployableVO deployableVO, SmartEntity building)
		{
			int credits = deployableVO.Credits;
			int materials = deployableVO.Materials;
			int contraband = deployableVO.Contraband;
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			float contractCostMultiplier = Service.PerkManager.GetContractCostMultiplier(buildingType);
			GameUtils.SpendCurrencyWithMultiplier(credits, materials, contraband, contractCostMultiplier, false);
		}
	}
}
