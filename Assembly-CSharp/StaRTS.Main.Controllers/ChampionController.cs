using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.Main.Controllers.CombatTriggers;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class ChampionController : IEventObserver
	{
		private const uint EFFECTIVELY_NEVER_SECONDS = 1800u;

		private const string REPAIR_PURCHASE_CONTEXT_NAME = "repair";

		private const float CHAMPION_INITIAL_ROTATION = 180f;

		private TroopTypeVO repairChampionType;

		public ChampionController()
		{
			Service.ChampionController = this;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ExitEditMode, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingConstructed, EventPriority.AfterDefault);
			eventManager.RegisterObserver(this, EventId.BuildingLevelUpgraded);
			eventManager.RegisterObserver(this, EventId.BuildingStartedUpgrading);
			eventManager.RegisterObserver(this, EventId.BuildingCancelled);
			eventManager.RegisterObserver(this, EventId.TroopViewReady);
			eventManager.RegisterObserver(this, EventId.ChampionRepaired);
			eventManager.RegisterObserver(this, EventId.ExitBaseLayoutToolMode);
			eventManager.RegisterObserver(this, EventId.TargetedBundleChampionRedeemed);
		}

		private void CreateChampionsOnPlatforms()
		{
			NodeList<ChampionPlatformNode> championPlatformNodeList = Service.BuildingLookupController.ChampionPlatformNodeList;
			for (ChampionPlatformNode championPlatformNode = championPlatformNodeList.Head; championPlatformNode != null; championPlatformNode = championPlatformNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)championPlatformNode.Entity;
				BuildingComponent buildingComp = smartEntity.BuildingComp;
				if (!ContractUtils.IsBuildingConstructing(smartEntity))
				{
					BuildingTypeVO buildingType = buildingComp.BuildingType;
					TroopTypeVO championType = this.FindChampionTypeIfPlatform(buildingType);
					if (this.FindChampionEntity(championType) == null)
					{
						this.CreateChampionEntity(championType, smartEntity);
					}
				}
			}
		}

		public TroopTypeVO FindChampionTypeIfPlatform(BuildingTypeVO buildingType)
		{
			if (buildingType == null || buildingType.Type != BuildingType.ChampionPlatform)
			{
				return null;
			}
			string uid = buildingType.Uid;
			StaticDataController staticDataController = Service.StaticDataController;
			TroopTypeVO troopTypeVO = null;
			if (buildingType.LinkedUnit != null)
			{
				troopTypeVO = staticDataController.Get<TroopTypeVO>(buildingType.LinkedUnit);
			}
			if (troopTypeVO == null)
			{
				Service.Logger.Error("No champion found for platform builing " + uid);
			}
			return troopTypeVO;
		}

		private SmartEntity CreateChampionEntity(TroopTypeVO championType, SmartEntity building)
		{
			TransformComponent transformComp = building.TransformComp;
			SmartEntity smartEntity = Service.EntityFactory.CreateChampionEntity(championType, new IntPosition(transformComp.CenterGridX(), transformComp.CenterGridZ()));
			Service.BoardController.Board.AddChild(smartEntity.BoardItemComp.BoardItem, transformComp.X, transformComp.Z, null, false);
			Service.EntityController.AddEntity(smartEntity);
			return smartEntity;
		}

		private SmartEntity CreateDefensiveChampionEntityForBattle(TroopTypeVO championType, SmartEntity building)
		{
			TransformComponent transformComp = building.TransformComp;
			IntPosition boardPosition = new IntPosition(transformComp.CenterGridX(), transformComp.CenterGridZ());
			return Service.TroopController.SpawnChampion(championType, TeamType.Defender, boardPosition);
		}

		private void AddChampionToInventoryIfAlive(SmartEntity building, bool createEntity)
		{
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			TroopTypeVO troopTypeVO = this.FindChampionTypeIfPlatform(buildingType);
			if (troopTypeVO == null)
			{
				return;
			}
			if (createEntity)
			{
				this.CreateChampionEntity(troopTypeVO, building);
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			currentPlayer.AddChampionToInventoryIfAlive(troopTypeVO.Uid);
		}

		private void HandleChampionPlatformUpgradeStarted(SmartEntity building)
		{
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			TroopTypeVO troopTypeVO = this.FindChampionTypeIfPlatform(buildingType);
			if (troopTypeVO == null)
			{
				return;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			currentPlayer.RemoveChampionFromInventory(troopTypeVO.Uid);
		}

		public bool IsChampionAvailable(SmartEntity building)
		{
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			TroopTypeVO troopTypeVO = this.FindChampionTypeIfPlatform(buildingType);
			return troopTypeVO != null && this.PlayerHasChampion(troopTypeVO);
		}

		private bool PlayerHasChampion(TroopTypeVO championType)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			return currentPlayer.Inventory.Champion.GetItemAmount(championType.Uid) > 0;
		}

		private void UpgradeChampionToMatchPlatform(SmartEntity building)
		{
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			TroopTypeVO troopTypeVO = this.FindChampionTypeIfPlatform(buildingType);
			if (troopTypeVO == null)
			{
				return;
			}
			BuildingUpgradeCatalog buildingUpgradeCatalog = Service.BuildingUpgradeCatalog;
			BuildingTypeVO nextLevel = buildingUpgradeCatalog.GetNextLevel(buildingType);
			TroopTypeVO troopTypeVO2 = this.FindChampionTypeIfPlatform(nextLevel);
			if (troopTypeVO2 == null)
			{
				return;
			}
			this.AddChampionToInventoryIfAlive(building, false);
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			UnlockedLevelData.UpgradeTroopsOrStarshipsInventory(currentPlayer.Inventory.Champion, false, troopTypeVO2.UpgradeGroup, troopTypeVO2.Uid);
			this.DestroyChampionEntity(troopTypeVO);
			if (Service.GameStateMachine.CurrentState is HomeState)
			{
				this.CreateChampionEntity(troopTypeVO2, building);
			}
		}

		private void DestroyChampionEntity(TroopTypeVO championType)
		{
			SmartEntity smartEntity = this.FindChampionEntity(championType);
			if (smartEntity != null)
			{
				Service.EntityFactory.DestroyEntity(smartEntity, true, false);
			}
		}

		public void DestroyAllChampionEntities()
		{
			NodeList<ChampionPlatformNode> championPlatformNodeList = Service.BuildingLookupController.ChampionPlatformNodeList;
			for (ChampionPlatformNode championPlatformNode = championPlatformNodeList.Head; championPlatformNode != null; championPlatformNode = championPlatformNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)championPlatformNode.Entity;
				BuildingComponent buildingComp = smartEntity.BuildingComp;
				BuildingTypeVO buildingType = buildingComp.BuildingType;
				TroopTypeVO championType = this.FindChampionTypeIfPlatform(buildingType);
				this.DestroyChampionEntity(championType);
			}
		}

		public SmartEntity FindChampionEntity(TroopTypeVO championType)
		{
			NodeList<ChampionNode> nodeList = Service.EntityController.GetNodeList<ChampionNode>();
			for (ChampionNode championNode = nodeList.Head; championNode != null; championNode = championNode.Next)
			{
				if (championNode.ChampionComp.ChampionType == championType)
				{
					return (SmartEntity)championNode.Entity;
				}
			}
			return null;
		}

		private SmartEntity FindPlatformForChampion(TroopTypeVO champion)
		{
			NodeList<ChampionPlatformNode> nodeList = Service.EntityController.GetNodeList<ChampionPlatformNode>();
			for (ChampionPlatformNode championPlatformNode = nodeList.Head; championPlatformNode != null; championPlatformNode = championPlatformNode.Next)
			{
				if (championPlatformNode.BuildingComp.BuildingType.LinkedUnit == champion.Uid)
				{
					return (SmartEntity)championPlatformNode.Entity;
				}
			}
			Service.Logger.Error("No platform building found for champion " + champion.Uid);
			return null;
		}

		private void OnPayMeForCurrencyResult(object result, object cookie)
		{
			if (GameUtils.HandleSoftCurrencyFlow(result, cookie))
			{
				if (!PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
				{
					this.ConfirmRepair();
				}
			}
			else
			{
				this.repairChampionType = null;
			}
		}

		private void OnPayMeForDroidResult(object result, object cookie)
		{
			if (result != null)
			{
				this.ConfirmRepair();
			}
			else
			{
				this.repairChampionType = null;
			}
		}

		private void ConfirmRepair()
		{
			SmartEntity smartEntity = this.FindPlatformForChampion(this.repairChampionType);
			Service.ISupportController.StartChampionRepair(this.repairChampionType, smartEntity);
			if (this.repairChampionType.TrainingTime > 0)
			{
				Service.UXController.HUD.ShowContextButtons(smartEntity);
			}
			this.repairChampionType = null;
		}

		public void StartChampionRepair(SmartEntity building)
		{
			BuildingTypeVO buildingType = building.BuildingComp.BuildingType;
			this.repairChampionType = this.FindChampionTypeIfPlatform(buildingType);
			if (this.repairChampionType == null)
			{
				return;
			}
			int credits = this.repairChampionType.Credits;
			int materials = this.repairChampionType.Materials;
			int contraband = this.repairChampionType.Contraband;
			if (credits != 0 || materials != 0 || contraband != 0)
			{
				string purchaseContext = string.Format("{0}|{1}|{2}|{3}", new object[]
				{
					StringUtils.ToLowerCaseUnderscoreSeperated(this.repairChampionType.Type.ToString()),
					this.repairChampionType.TroopID,
					this.repairChampionType.Lvl,
					"repair"
				});
				if (PayMeScreen.ShowIfNotEnoughCurrency(credits, materials, contraband, purchaseContext, new OnScreenModalResult(this.OnPayMeForCurrencyResult)))
				{
					return;
				}
			}
			if (PayMeScreen.ShowIfNoFreeDroids(new OnScreenModalResult(this.OnPayMeForDroidResult), null))
			{
				return;
			}
			this.ConfirmRepair();
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			bool flag = currentState is HomeState;
			bool flag2 = currentState is WarBoardState;
			bool flag3 = currentState is ApplicationLoadState;
			switch (id)
			{
			case EventId.TroopViewReady:
				if (flag || flag2 || flag3)
				{
					EntityViewParams entityViewParams = cookie as EntityViewParams;
					SmartEntity entity = entityViewParams.Entity;
					if (entity.ChampionComp != null)
					{
						bool underRepair = this.IsChampionBroken(entity.ChampionComp);
						Service.EntityRenderController.UpdateChampionAnimationStateInHomeOrWarBoardMode(entity, underRepair);
					}
				}
				return EatResponse.NotEaten;
			case EventId.TroopLevelUpgraded:
			case EventId.StarshipLevelUpgraded:
			case EventId.BuildingSwapped:
				IL_4F:
				if (id != EventId.ExitEditMode && id != EventId.ExitBaseLayoutToolMode)
				{
					if (id == EventId.BuildingCancelled)
					{
						ContractEventData contractEventData = (ContractEventData)cookie;
						this.AddChampionToInventoryIfAlive((SmartEntity)contractEventData.Entity, false);
						return EatResponse.NotEaten;
					}
					if (id == EventId.BuildingStartedUpgrading)
					{
						this.HandleChampionPlatformUpgradeStarted((SmartEntity)cookie);
						return EatResponse.NotEaten;
					}
					if (id == EventId.ChampionRepaired)
					{
						if (flag && cookie != null)
						{
							ContractEventData contractEventData2 = cookie as ContractEventData;
							SmartEntity smartEntity = contractEventData2.Entity as SmartEntity;
							TroopTypeVO troopTypeVO = this.FindChampionTypeIfPlatform(smartEntity.BuildingComp.BuildingType);
							if (troopTypeVO != null)
							{
								SmartEntity entity2 = this.FindChampionEntity(troopTypeVO);
								Service.EntityRenderController.UpdateChampionAnimationStateInHomeOrWarBoardMode(entity2, false);
							}
						}
						return EatResponse.NotEaten;
					}
					if (id != EventId.WorldLoadComplete)
					{
						if (id != EventId.TargetedBundleChampionRedeemed)
						{
							return EatResponse.NotEaten;
						}
						SmartEntity building = (SmartEntity)cookie;
						Service.PlayerValuesController.RecalculateAll();
						this.AddChampionToInventoryIfAlive(building, flag);
						return EatResponse.NotEaten;
					}
				}
				this.CreateChampionsOnPlatforms();
				return EatResponse.NotEaten;
			case EventId.BuildingLevelUpgraded:
			{
				ContractEventData contractEventData = (ContractEventData)cookie;
				this.UpgradeChampionToMatchPlatform((SmartEntity)contractEventData.Entity);
				return EatResponse.NotEaten;
			}
			case EventId.BuildingConstructed:
			{
				ContractEventData contractEventData = (ContractEventData)cookie;
				this.AddChampionToInventoryIfAlive((SmartEntity)contractEventData.Entity, flag);
				if (flag)
				{
					Service.BuildingTooltipController.EnsureBuildingTooltip((SmartEntity)contractEventData.Entity);
				}
				return EatResponse.NotEaten;
			}
			}
			goto IL_4F;
		}

		private bool IsChampionBroken(ChampionComponent championComp)
		{
			TroopTypeVO championType = championComp.ChampionType;
			SmartEntity selectedBuilding = this.FindPlatformForChampion(championType);
			return !this.PlayerHasChampion(championType) && (!ContractUtils.IsBuildingUpgrading(selectedBuilding) && !ContractUtils.IsBuildingConstructing(selectedBuilding));
		}

		public void RegisterChampionPlatforms(CurrentBattle currentBattle)
		{
			SmartEntity smartEntity = null;
			TroopTypeVO troopTypeVO = null;
			bool flag = false;
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				flag = false;
				smartEntity = (SmartEntity)buildingNode.Entity;
				troopTypeVO = this.FindChampionTypeIfPlatform(smartEntity.BuildingComp.BuildingType);
				if (buildingNode.BuildingComp.BuildingType.Type == BuildingType.ChampionPlatform)
				{
					if (currentBattle.Type == BattleType.PveAttack || currentBattle.Type == BattleType.ClientBattle || currentBattle.Type == BattleType.PveBuffBase || currentBattle.Type == BattleType.PvpAttackSquadWar)
					{
						if (troopTypeVO != null && smartEntity != null)
						{
							this.AddDefensiveChampionToPlatfrom(smartEntity, troopTypeVO);
						}
					}
					else if (troopTypeVO != null && currentBattle.DisabledBuildings != null && !currentBattle.DisabledBuildings.Contains(buildingNode.BuildingComp.BuildingTO.Key))
					{
						if (currentBattle.IsPvP())
						{
							if (currentBattle.DefenderChampionsAvailable != null)
							{
								foreach (KeyValuePair<string, int> current in currentBattle.DefenderChampionsAvailable)
								{
									if (current.Key == troopTypeVO.Uid && current.Value > 0)
									{
										flag = true;
									}
								}
							}
						}
						else if (currentBattle.Type == BattleType.PveDefend && !ContractUtils.IsBuildingUpgrading(smartEntity) && !ContractUtils.IsChampionRepairing(smartEntity))
						{
							Inventory inventory = Service.CurrentPlayer.Inventory;
							bool flag2 = inventory.Champion.HasItem(troopTypeVO.Uid) && inventory.Champion.GetItemAmount(troopTypeVO.Uid) > 0;
							if (flag2)
							{
								flag = true;
							}
						}
						if (flag)
						{
							this.AddDefensiveChampionToPlatfrom(smartEntity, troopTypeVO);
						}
					}
				}
			}
		}

		public void AddDefensiveChampionToPlatfrom(SmartEntity championPlatform, TroopTypeVO champion)
		{
			SmartEntity smartEntity = this.CreateDefensiveChampionEntityForBattle(champion, championPlatform);
			if (smartEntity != null)
			{
				smartEntity.TransformComp.Rotation = 180f;
				Service.EventManager.SendEvent(EventId.AddDecalToTroop, smartEntity);
			}
			championPlatform.ChampionPlatformComp.DefensiveChampion = smartEntity;
			Service.CombatTriggerManager.RegisterTrigger(new DefendedBuildingCombatTrigger(championPlatform, CombatTriggerType.Area, false, champion, 1, true, 0u, 1800u));
		}
	}
}
