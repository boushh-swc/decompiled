using Net.RichardLord.Ash.Core;
using StaRTS.Externals.Manimal;
using StaRTS.Main.Controllers.Entities.Systems;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Collect;
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
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers
{
	public class CurrencyController : IEventObserver, ICurrencyController
	{
		private const int MIN_FOR_COLLECTION = 1;

		private const int INITIAL_CONTRABAND_SEED = 1;

		private EntityController entityController;

		public const string BUILD_MORE_CREDIT_STORAGE = "BUILD_MORE_CREDIT_STORAGE";

		public const string FULL_CREDIT_STORAGE = "FULL_CREDIT_STORAGE";

		public const string UPGRADE_CREDIT_STORAGE = "UPGRADE_CREDIT_STORAGE";

		public const string BUILD_MORE_MATERIAL_STORAGE = "BUILD_MORE_MATERIAL_STORAGE";

		public const string FULL_MATERIAL_STORAGE = "FULL_MATERIAL_STORAGE";

		public const string UPGRADE_MATERIAL_STORAGE = "UPGRADE_MATERIAL_STORAGE";

		public const string BUILD_MORE_CONTRABAND_STORAGE = "BUILD_MORE_CONTRABAND_STORAGE";

		public const string FULL_CONTRABAND_STORAGE = "FULL_CONTRABAND_STORAGE";

		public const string UPGRADE_CONTRABAND_STORAGE = "UPGRADE_CONTRABAND_STORAGE";

		public CurrencyController()
		{
			Service.ICurrencyController = this;
			this.entityController = Service.EntityController;
			Service.EventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.InventoryResourceUpdated, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.PlanetRelocateStarted, EventPriority.Default);
		}

		public bool TryCollectCurrencyOnSelection(SmartEntity entity)
		{
			if (!(Service.GameStateMachine.CurrentState is HomeState))
			{
				return false;
			}
			if (Service.PostBattleRepairController.IsEntityInRepair(entity))
			{
				return false;
			}
			if (ContractUtils.IsBuildingConstructing(entity) || ContractUtils.IsBuildingUpgrading(entity))
			{
				return false;
			}
			if (!this.IsGeneratorThresholdMet(entity))
			{
				return false;
			}
			if (!this.CanStoreCollectionAmountFromGenerator(entity))
			{
				BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
				CurrencyType currency = buildingComponent.BuildingType.Currency;
				this.HandleUnableToCollect(currency);
				return false;
			}
			this.CollectCurrency(entity);
			return true;
		}

		public void CollectCurrency(Entity buildingEntity)
		{
			if (buildingEntity == null)
			{
				return;
			}
			GeneratorComponent generatorComponent = buildingEntity.Get<GeneratorComponent>();
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			GeneratorViewComponent generatorViewComponent = buildingEntity.Get<GeneratorViewComponent>();
			if (buildingComponent == null || generatorComponent == null)
			{
				return;
			}
			Building buildingTO = buildingComponent.BuildingTO;
			int num = this.CollectCurrencyFromGenerator(buildingEntity, true);
			string contextId = string.Empty;
			CurrencyType currency = buildingComponent.BuildingType.Currency;
			if (currency != CurrencyType.Credits)
			{
				if (currency != CurrencyType.Materials)
				{
					if (currency == CurrencyType.Contraband)
					{
						contextId = "Contraband";
					}
				}
				else
				{
					contextId = "Materials";
				}
			}
			else
			{
				contextId = "Credits";
			}
			if (buildingTO.AccruedCurrency < 1)
			{
				Service.UXController.HUD.ToggleContextButton(contextId, false);
			}
			if (num > 0)
			{
				this.OnCollectCurrency(buildingEntity, buildingComponent, num);
			}
			else if (num == 0)
			{
				this.HandleUnableToCollect(currency);
			}
			if (buildingTO.CurrentStorage == 0)
			{
				generatorViewComponent.ShowCollectButton(false);
			}
		}

		public void HandleUnableToCollect(CurrencyType currencyType)
		{
			BuildingLookupController buildingLookupController = Service.BuildingLookupController;
			StaticDataController staticDataController = Service.StaticDataController;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			BuildingTypeVO buildingTypeVO = null;
			foreach (BuildingTypeVO current in staticDataController.GetAll<BuildingTypeVO>())
			{
				if (current.Faction == currentPlayer.Faction && current.Type == BuildingType.Storage && current.Currency == currencyType)
				{
					buildingTypeVO = current;
					break;
				}
			}
			int num = 0;
			if (buildingTypeVO != null)
			{
				num = buildingLookupController.GetBuildingMaxPurchaseQuantity(buildingTypeVO, 0);
			}
			int num2 = 0;
			int num3 = 0;
			int highestLevelHQ = buildingLookupController.GetHighestLevelHQ();
			for (StorageNode storageNode = buildingLookupController.StorageNodeList.Head; storageNode != null; storageNode = storageNode.Next)
			{
				if (storageNode.BuildingComp.BuildingType.Currency == currencyType)
				{
					num2++;
					if (storageNode.BuildingComp.BuildingType.Lvl == highestLevelHQ)
					{
						num3++;
					}
				}
			}
			bool flag = num2 < num;
			string instructions = string.Empty;
			if (currencyType != CurrencyType.Credits)
			{
				if (currencyType != CurrencyType.Materials)
				{
					if (currencyType == CurrencyType.Contraband)
					{
						if (flag)
						{
							instructions = Service.Lang.Get("BUILD_MORE_CONTRABAND_STORAGE", new object[0]);
						}
						else if (num3 == num2)
						{
							instructions = Service.Lang.Get("FULL_CONTRABAND_STORAGE", new object[0]);
						}
						else
						{
							instructions = Service.Lang.Get("UPGRADE_CONTRABAND_STORAGE", new object[0]);
						}
					}
				}
				else if (flag)
				{
					instructions = Service.Lang.Get("BUILD_MORE_MATERIAL_STORAGE", new object[0]);
				}
				else if (num3 == num2)
				{
					instructions = Service.Lang.Get("FULL_MATERIAL_STORAGE", new object[0]);
				}
				else
				{
					instructions = Service.Lang.Get("UPGRADE_MATERIAL_STORAGE", new object[0]);
				}
			}
			else if (flag)
			{
				instructions = Service.Lang.Get("BUILD_MORE_CREDIT_STORAGE", new object[0]);
			}
			else if (num3 == num2)
			{
				instructions = Service.Lang.Get("FULL_CREDIT_STORAGE", new object[0]);
			}
			else
			{
				instructions = Service.Lang.Get("UPGRADE_CREDIT_STORAGE", new object[0]);
			}
			Service.UXController.MiscElementsManager.ShowPlayerInstructions(instructions);
		}

		public void ForceCollectAccruedCurrencyForUpgrade(Entity buildingEntity)
		{
			if (buildingEntity == null)
			{
				return;
			}
			GeneratorComponent generatorComponent = buildingEntity.Get<GeneratorComponent>();
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			if (buildingComponent == null || generatorComponent == null)
			{
				return;
			}
			int num = this.CollectCurrencyFromGenerator(buildingEntity, false);
			if (num > 0)
			{
				this.OnCollectCurrency(buildingEntity, buildingComponent, num);
			}
		}

		private void OnCollectCurrency(Entity buildingEntity, BuildingComponent buildingComp, int amountCollected)
		{
			CurrencyCollectionTag currencyCollectionTag = new CurrencyCollectionTag();
			currencyCollectionTag.Building = buildingEntity;
			currencyCollectionTag.Type = buildingComp.BuildingType.Currency;
			currencyCollectionTag.Delta = amountCollected;
			Service.CurrencyEffects.PlayEffect(buildingEntity, currencyCollectionTag.Type, amountCollected);
			this.UpdateStorageEffectsAfterCollection(buildingEntity, buildingComp.BuildingType);
			GeneratorViewComponent generatorViewComp = ((SmartEntity)buildingEntity).GeneratorViewComp;
			if (generatorViewComp != null)
			{
				generatorViewComp.ShowAmountCollectedText(amountCollected, currencyCollectionTag.Type);
			}
			Service.EventManager.SendEvent(EventId.CurrencyCollected, currencyCollectionTag);
		}

		private bool GetTimePassed(Building buildingTO, out uint deltaTime, bool logErrors)
		{
			uint time = ServerTime.Time;
			if (time < buildingTO.LastCollectTime)
			{
				deltaTime = 0u;
				if (logErrors)
				{
					Service.Logger.ErrorFormat("Cannot collect from {0}, cur time {1} is less than last collect time {2}", new object[]
					{
						buildingTO.Key,
						time,
						buildingTO.LastCollectTime
					});
				}
				return false;
			}
			deltaTime = time - buildingTO.LastCollectTime;
			if (deltaTime > 2147483647u)
			{
				if (logErrors)
				{
					Service.Logger.ErrorFormat("Cannot collect from {0}, delta time {1} is too large", new object[]
					{
						buildingTO.Key,
						deltaTime
					});
				}
				return false;
			}
			return true;
		}

		private int CollectCurrencyFromGenerator(Entity buildingEntity, bool sendServerCommand)
		{
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			Building buildingTO = buildingComponent.BuildingTO;
			Inventory inventory = Service.CurrentPlayer.Inventory;
			int num = -1;
			int num2 = 0;
			uint secondsPassed;
			if (this.GetTimePassed(buildingTO, out secondsPassed, true))
			{
				num = this.CalculateAccruedCurrency(buildingTO.CurrentStorage, buildingComponent.BuildingType, secondsPassed);
				CurrencyType currency = buildingComponent.BuildingType.Currency;
				if (currency != CurrencyType.Credits)
				{
					if (currency != CurrencyType.Materials)
					{
						if (currency == CurrencyType.Contraband)
						{
							num2 = inventory.ModifyContraband(num);
						}
					}
					else
					{
						num2 = inventory.ModifyMaterials(num);
					}
				}
				else
				{
					num2 = inventory.ModifyCredits(num);
				}
			}
			ServerAPI serverAPI = Service.ServerAPI;
			buildingTO.LastCollectTime = serverAPI.ServerTime;
			buildingTO.CurrentStorage = num2;
			buildingTO.AccruedCurrency = num2;
			if (sendServerCommand)
			{
				serverAPI.Enqueue(new BuildingCollectCommand(new BuildingCollectRequest
				{
					BuildingId = buildingTO.Key
				}));
			}
			return num - num2;
		}

		public bool CanStoreCollectionAmountFromGenerator(Entity buildingEntity)
		{
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			Inventory inventory = Service.CurrentPlayer.Inventory;
			int num = 0;
			CurrencyType currency = buildingComponent.BuildingType.Currency;
			if (currency != CurrencyType.Credits)
			{
				if (currency != CurrencyType.Materials)
				{
					if (currency == CurrencyType.Contraband)
					{
						num = inventory.GetItemCapacity("contraband") - inventory.GetItemAmount("contraband");
					}
				}
				else
				{
					num = inventory.GetItemCapacity("materials") - inventory.GetItemAmount("materials");
				}
			}
			else
			{
				num = inventory.GetItemCapacity("credits") - inventory.GetItemAmount("credits");
			}
			return num > 0;
		}

		public bool IsGeneratorCollectable(Entity buildingEntity)
		{
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			return buildingComponent.BuildingType.Type == BuildingType.Resource && buildingComponent.BuildingTO.AccruedCurrency >= 1;
		}

		public bool IsGeneratorThresholdMet(Entity buildingEntity)
		{
			BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return false;
			}
			BuildingTypeVO buildingType = buildingComponent.BuildingType;
			return buildingType.Type == BuildingType.Resource && buildingComponent.BuildingTO.AccruedCurrency >= buildingType.CollectNotify;
		}

		public int CalculateTimeUntilAllGeneratorsFull()
		{
			NodeList<GeneratorViewNode> generatorViewNodeList = Service.BuildingLookupController.GeneratorViewNodeList;
			ISupportController iSupportController = Service.ISupportController;
			int num = 0;
			if (generatorViewNodeList != null)
			{
				for (GeneratorViewNode generatorViewNode = generatorViewNodeList.Head; generatorViewNode != null; generatorViewNode = generatorViewNode.Next)
				{
					if (generatorViewNode.BuildingComp != null && iSupportController.FindCurrentContract(generatorViewNode.BuildingComp.BuildingTO.Key) == null)
					{
						int num2 = this.CalculateGeneratorFillTimeRemaining(generatorViewNode.Entity);
						if (num2 > num)
						{
							num = num2;
						}
					}
				}
			}
			return num;
		}

		public int CalculateGeneratorFillTimeRemaining(Entity buildingEntity)
		{
			int result = 0;
			if (buildingEntity != null)
			{
				BuildingComponent buildingComponent = buildingEntity.Get<BuildingComponent>();
				BuildingTypeVO buildingType = buildingComponent.BuildingType;
				int accruedCurrency = buildingComponent.BuildingTO.AccruedCurrency;
				int storage = buildingType.Storage;
				int num = storage - accruedCurrency;
				result = Service.PerkManager.GetSecondsTillFullIncludingPerkAdjustedRate(buildingType, (float)num, ServerTime.Time);
			}
			return result;
		}

		public void UpdateGeneratorAccruedCurrency(SmartEntity entity)
		{
			Building buildingTO = entity.BuildingComp.BuildingTO;
			BuildingTypeVO buildingType = entity.BuildingComp.BuildingType;
			bool flag = Service.BuildingController.SelectedBuilding == entity;
			ISupportController iSupportController = Service.ISupportController;
			if (iSupportController.FindCurrentContract(entity.BuildingComp.BuildingTO.Key) == null)
			{
				int accruedCurrency = buildingTO.AccruedCurrency;
				buildingTO.AccruedCurrency = this.UpdateAccruedCurrencyForView(buildingTO, buildingType);
				entity.GeneratorViewComp.ShowCollectButton(buildingTO.AccruedCurrency >= buildingType.CollectNotify);
				if (buildingTO.AccruedCurrency >= buildingType.Storage && accruedCurrency < buildingType.Storage)
				{
					Service.EventManager.SendEvent(EventId.GeneratorJustFilled, entity);
				}
			}
			if (flag)
			{
				string contextId = null;
				CurrencyType currency = buildingType.Currency;
				if (currency != CurrencyType.Credits)
				{
					if (currency != CurrencyType.Materials)
					{
						if (currency == CurrencyType.Contraband)
						{
							contextId = "Contraband";
						}
					}
					else
					{
						contextId = "Materials";
					}
				}
				else
				{
					contextId = "Credits";
				}
				Service.UXController.HUD.ToggleContextButton(contextId, this.IsGeneratorCollectable(entity));
			}
		}

		public float CurrencyPerSecond(BuildingTypeVO type)
		{
			return (float)type.Produce / (float)type.CycleTime;
		}

		public int CurrencyPerHour(BuildingTypeVO type)
		{
			return (int)(3600f * this.CurrencyPerSecond(type));
		}

		public int CalculateAccruedCurrency(Building buildingTO, BuildingTypeVO type)
		{
			uint secondsPassed;
			return (!this.GetTimePassed(buildingTO, out secondsPassed, false)) ? buildingTO.AccruedCurrency : this.CalculateAccruedCurrency(buildingTO.CurrentStorage, type, secondsPassed);
		}

		private int CalculateAccruedCurrency(int currentStorage, BuildingTypeVO type, uint secondsPassed)
		{
			PerkManager perkManager = Service.PerkManager;
			uint time = ServerTime.Time;
			uint startTime = time - secondsPassed;
			int num = perkManager.GetAccruedCurrencyIncludingPerkAdjustedRate(type, startTime, time) + currentStorage;
			if (num > type.Storage)
			{
				num = type.Storage;
			}
			return num;
		}

		private int UpdateAccruedCurrencyForView(Building buildingTO, BuildingTypeVO type)
		{
			int result = buildingTO.AccruedCurrency;
			uint secondsPassed;
			if (this.GetTimePassed(buildingTO, out secondsPassed, false))
			{
				result = this.CalculateAccruedCurrency(buildingTO.CurrentStorage, type, secondsPassed);
			}
			return result;
		}

		public void UpdateGeneratorAfterFinishedContract(BuildingTypeVO buildingVO, Building buildingTO, uint contractFinishTime, bool isConstructionContract)
		{
			if (buildingVO.Type != BuildingType.Resource)
			{
				return;
			}
			buildingTO.LastCollectTime = contractFinishTime;
			if (isConstructionContract && buildingVO.Currency == CurrencyType.Contraband)
			{
				buildingTO.AccruedCurrency = 1;
				buildingTO.CurrentStorage = 1;
			}
		}

		private void UpdateStorageEffectsAfterCollection(Entity generatorEntity, BuildingTypeVO generatorVO)
		{
			StorageEffects storageEffects = Service.StorageEffects;
			storageEffects.UpdateFillState(generatorEntity, generatorVO);
			NodeList<StorageNode> nodeList = Service.EntityController.GetNodeList<StorageNode>();
			for (StorageNode storageNode = nodeList.Head; storageNode != null; storageNode = storageNode.Next)
			{
				BuildingTypeVO buildingType = storageNode.BuildingComp.BuildingType;
				if (buildingType.Currency == generatorVO.Currency)
				{
					storageEffects.UpdateFillState(storageNode.Entity, buildingType);
				}
			}
		}

		public void UpdateAllStorageEffects()
		{
			StorageEffects storageEffects = Service.StorageEffects;
			NodeList<StorageNode> nodeList = Service.EntityController.GetNodeList<StorageNode>();
			for (StorageNode storageNode = nodeList.Head; storageNode != null; storageNode = storageNode.Next)
			{
				storageEffects.UpdateFillState(storageNode.Entity, storageNode.BuildingComp.BuildingType);
			}
			NodeList<GeneratorNode> nodeList2 = Service.EntityController.GetNodeList<GeneratorNode>();
			for (GeneratorNode generatorNode = nodeList2.Head; generatorNode != null; generatorNode = generatorNode.Next)
			{
				storageEffects.UpdateFillState(generatorNode.Entity, generatorNode.BuildingComp.BuildingType);
			}
		}

		private void UpdateStorageEffectsOnBuildingChange(Entity entity, BuildingTypeVO buildingVO)
		{
			if (buildingVO.Type == BuildingType.Storage)
			{
				this.UpdateStorageEffectsOnStorages(buildingVO.Currency);
			}
			else if (buildingVO.Type == BuildingType.Resource)
			{
				Service.StorageEffects.UpdateFillState(entity, buildingVO);
			}
		}

		private void UpdateStorageEffectsOnStorages(CurrencyType currencyType)
		{
			StorageEffects storageEffects = Service.StorageEffects;
			NodeList<StorageNode> nodeList = Service.EntityController.GetNodeList<StorageNode>();
			for (StorageNode storageNode = nodeList.Head; storageNode != null; storageNode = storageNode.Next)
			{
				if (storageNode.BuildingComp.BuildingType.Currency == currencyType)
				{
					storageEffects.UpdateFillState(storageNode.Entity, storageNode.BuildingComp.BuildingType);
				}
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			Type previousStateType = Service.GameStateMachine.PreviousStateType;
			if (id != EventId.BuildingConstructed)
			{
				if (id != EventId.WorldLoadComplete)
				{
					if (id != EventId.GameStateChanged)
					{
						if (id != EventId.InventoryResourceUpdated)
						{
							if (id == EventId.PlanetRelocateStarted)
							{
								if (this.entityController.IsViewSystemSet<GeneratorSystem>())
								{
									this.entityController.RemoveViewSystem<GeneratorSystem>();
								}
							}
						}
						else if (currentState is HomeState || currentState is EditBaseState || currentState is ApplicationLoadState)
						{
							string a = cookie as string;
							if (a == "credits")
							{
								this.UpdateStorageEffectsOnStorages(CurrencyType.Credits);
							}
							else if (a == "materials")
							{
								this.UpdateStorageEffectsOnStorages(CurrencyType.Materials);
							}
							else if (a == "contraband")
							{
								this.UpdateStorageEffectsOnStorages(CurrencyType.Contraband);
							}
						}
					}
					else if (!(currentState is HomeState) && !(currentState is IntroCameraState) && !(currentState is GalaxyState))
					{
						if (this.entityController.IsViewSystemSet<GeneratorSystem>())
						{
							this.entityController.RemoveViewSystem<GeneratorSystem>();
						}
					}
					else if (previousStateType == typeof(EditBaseState))
					{
						this.entityController.AddViewSystem(new GeneratorSystem(), 2070, 65535);
						Service.CurrencyEffects.PlaceEffects();
					}
					else if ((previousStateType == typeof(BattleStartState) || previousStateType == typeof(WarBoardState)) && currentState is HomeState)
					{
						this.entityController.AddViewSystem(new GeneratorSystem(), 2070, 65535);
						Service.CurrencyEffects.PlaceEffects();
					}
				}
				else
				{
					Service.CurrencyEffects.Cleanup();
					if (currentState is ApplicationLoadState || currentState is HomeState)
					{
						if (!this.entityController.IsViewSystemSet<GeneratorSystem>())
						{
							this.entityController.AddViewSystem(new GeneratorSystem(), 2070, 65535);
						}
						Service.CurrencyEffects.InitializeEffects("setupTypeCollection");
					}
					else if (currentState is BattleStartState)
					{
						Service.CurrencyEffects.InitializeEffects("setupTypeLooting");
					}
				}
			}
			else
			{
				ContractEventData contractEventData = (ContractEventData)cookie;
				this.UpdateStorageEffectsOnBuildingChange(contractEventData.Entity, contractEventData.BuildingVO);
			}
			return EatResponse.NotEaten;
		}
	}
}
