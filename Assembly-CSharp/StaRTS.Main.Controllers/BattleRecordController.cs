using StaRTS.DataStructures;
using StaRTS.Main.Controllers.Performance;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Controllers
{
	public class BattleRecordController : IPerformanceObserver, IEventObserver
	{
		private delegate void HandleEvent(object cookie);

		private EventManager eventManager;

		private BattleController battleController;

		private BattleRecord battleRecord;

		private Dictionary<EventId, BattleRecordController.HandleEvent> eventHandlers;

		public BattleRecord BattleRecord
		{
			get
			{
				return this.battleRecord;
			}
		}

		public BattleRecordController()
		{
			Service.BattleRecordController = this;
			this.eventManager = Service.EventManager;
			this.battleController = Service.BattleController;
			this.eventHandlers = new Dictionary<EventId, BattleRecordController.HandleEvent>();
			this.eventHandlers.Add(EventId.TroopDeployed, new BattleRecordController.HandleEvent(this.HandleTroopDeployedEvent));
			this.eventHandlers.Add(EventId.SpecialAttackDeployed, new BattleRecordController.HandleEvent(this.HandleSpecialAttackDeployedEvent));
			this.eventHandlers.Add(EventId.HeroDeployed, new BattleRecordController.HandleEvent(this.HandleHeroDeployedEvent));
			this.eventHandlers.Add(EventId.TroopAbilityActivate, new BattleRecordController.HandleEvent(this.HandleTroopAbilityActivateEvent));
			this.eventHandlers.Add(EventId.ChampionDeployed, new BattleRecordController.HandleEvent(this.HandleChampionDeployedEvent));
			this.eventHandlers.Add(EventId.SquadTroopsDeployedByPlayer, new BattleRecordController.HandleEvent(this.HandleSquadTroopsDeployedEvent));
			this.eventHandlers.Add(EventId.BattleCanceled, new BattleRecordController.HandleEvent(this.HandleBattleCanceledEvent));
			this.eventHandlers.Add(EventId.BattleEndProcessing, new BattleRecordController.HandleEvent(this.HandleBattleEndedEvent));
			this.eventHandlers.Add(EventId.EntityKilled, new BattleRecordController.HandleEvent(this.HandleEntityKilledEvent));
			this.EraseRecord();
		}

		private void EraseRecord()
		{
			this.battleRecord = new BattleRecord();
		}

		public void StartRecord()
		{
			this.EraseRecord();
			foreach (EventId current in this.eventHandlers.Keys)
			{
				this.eventManager.RegisterObserver(this, current, EventPriority.Default);
			}
			this.battleRecord.LowestFPS = 9999f;
			Service.PerformanceMonitor.RegisterFPSObserver(this);
			this.battleRecord.SimSeed = this.battleController.SimSeed;
			this.battleRecord.ViewTimePassedPreBattle = this.battleController.ViewTimePassedPreBattle;
			this.battleRecord.CombatEncounter = Service.CombatEncounterController.GetCurrentCombatEncounter();
			CurrentBattle currentBattle = this.battleController.GetCurrentBattle();
			this.battleRecord.RecordId = currentBattle.RecordID;
			this.battleRecord.BattleType = currentBattle.Type;
			this.battleRecord.AttackerDeploymentData = BattleDeploymentData.Copy(currentBattle.AttackerDeployableData);
			this.battleRecord.DefenderDeploymentData = BattleDeploymentData.Copy(currentBattle.DefenderDeployableData);
			this.battleRecord.DefenderGuildTroops = currentBattle.DefenderGuildTroopsAvailable;
			this.battleRecord.AttackerGuildTroops = currentBattle.AttackerGuildTroopsAvailable;
			this.battleRecord.LootCreditsAvailable = currentBattle.LootCreditsAvailable;
			this.battleRecord.LootMaterialsAvailable = currentBattle.LootMaterialsAvailable;
			this.battleRecord.LootContrabandAvailable = currentBattle.LootContrabandAvailable;
			this.battleRecord.BuildingLootCreditsMap = currentBattle.BuildingLootCreditsMap;
			this.battleRecord.BuildingLootMaterialsMap = currentBattle.BuildingLootMaterialsMap;
			this.battleRecord.BuildingLootContrabandMap = currentBattle.BuildingLootContrabandMap;
			this.battleRecord.DefenderChampions = currentBattle.DefenderChampionsAvailable;
			this.battleRecord.CmsVersion = Service.ContentManager.GetFileVersion("patches/base.json").ToString();
			this.battleRecord.BattleVersion = "30.0";
			this.battleRecord.PlanetId = currentBattle.PlanetId;
			this.battleRecord.BattleLength = currentBattle.TimeLeft;
			this.battleRecord.DisabledBuildings = currentBattle.DisabledBuildings;
			this.battleRecord.victoryConditionsUids = new List<string>();
			for (int i = 0; i < currentBattle.VictoryConditions.Count; i++)
			{
				this.battleRecord.victoryConditionsUids.Add(currentBattle.VictoryConditions[i].Uid);
			}
			if (currentBattle.FailureCondition != null)
			{
				this.battleRecord.failureConditionUid = currentBattle.FailureCondition.Uid;
			}
			else
			{
				this.battleRecord.failureConditionUid = string.Empty;
			}
			this.battleRecord.BattleAttributes.AddDeviceInfo();
			this.battleRecord.DefenseEncounterProfile = currentBattle.DefenseEncounterProfile;
			this.battleRecord.AttackerWarBuffs = currentBattle.AttackerWarBuffs;
			this.battleRecord.DefenderWarBuffs = currentBattle.DefenderWarBuffs;
			this.battleRecord.AttackerEquipment = currentBattle.AttackerEquipment;
			this.battleRecord.DefenderEquipment = currentBattle.DefenderEquipment;
		}

		public void EndRecord()
		{
			foreach (EventId current in this.eventHandlers.Keys)
			{
				this.eventManager.UnregisterObserver(this, current);
			}
		}

		private uint GetActionTime()
		{
			return this.battleController.Now;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			BattleRecordController.HandleEvent handleEvent;
			if (this.eventHandlers.TryGetValue(id, out handleEvent))
			{
				handleEvent(cookie);
			}
			return EatResponse.NotEaten;
		}

		public void HandleTroopDeployedEvent(object cookie)
		{
			SmartEntity smartEntity = (SmartEntity)cookie;
			TroopComponent troopComp = smartEntity.TroopComp;
			if (troopComp == null)
			{
				return;
			}
			TransformComponent transformComp = smartEntity.TransformComp;
			if (transformComp == null)
			{
				return;
			}
			TroopPlacedAction troopPlacedAction = new TroopPlacedAction();
			troopPlacedAction.Time = this.GetActionTime();
			troopPlacedAction.TroopId = troopComp.TroopType.Uid;
			troopPlacedAction.BoardX = transformComp.CenterGridX();
			troopPlacedAction.BoardZ = transformComp.CenterGridZ();
			TeamComponent teamComp = smartEntity.TeamComp;
			troopPlacedAction.TeamType = teamComp.TeamType;
			this.battleRecord.Add(troopPlacedAction);
		}

		private void HandleTroopAbilityActivateEvent(object cookie)
		{
			SmartEntity smartEntity = (SmartEntity)cookie;
			TroopAbilityVO abilityVO = smartEntity.TroopComp.AbilityVO;
			if (abilityVO != null && !abilityVO.Auto)
			{
				HeroAbilityAction heroAbilityAction = new HeroAbilityAction();
				heroAbilityAction.Time = this.GetActionTime();
				heroAbilityAction.TroopUid = smartEntity.TroopComp.TroopType.Uid;
				this.battleRecord.Add(heroAbilityAction);
			}
		}

		public void HandleSpecialAttackDeployedEvent(object cookie)
		{
			SpecialAttack specialAttack = (SpecialAttack)cookie;
			SpecialAttackDeployedAction specialAttackDeployedAction = new SpecialAttackDeployedAction();
			specialAttackDeployedAction.Time = this.GetActionTime();
			specialAttackDeployedAction.SpecialAttackId = specialAttack.VO.Uid;
			specialAttackDeployedAction.BoardX = specialAttack.TargetBoardX;
			specialAttackDeployedAction.BoardZ = specialAttack.TargetBoardZ;
			specialAttackDeployedAction.TeamType = specialAttack.TeamType;
			this.battleRecord.Add(specialAttackDeployedAction);
		}

		public void HandleSquadTroopsDeployedEvent(object cookie)
		{
			IntPosition intPosition = (IntPosition)cookie;
			SquadTroopPlacedAction squadTroopPlacedAction = new SquadTroopPlacedAction();
			squadTroopPlacedAction.Time = this.GetActionTime();
			squadTroopPlacedAction.BoardX = intPosition.x;
			squadTroopPlacedAction.BoardZ = intPosition.z;
			this.battleRecord.Add(squadTroopPlacedAction);
		}

		private void HandleBattleCanceledEvent(object cookie)
		{
			BattleCanceledAction battleCanceledAction = new BattleCanceledAction();
			battleCanceledAction.Time = this.GetActionTime();
			this.battleRecord.Add(battleCanceledAction);
		}

		private void HandleBattleEndedEvent(object cookie)
		{
			Service.PerformanceMonitor.UnregisterFPSObserver(this);
			CurrentBattle currentBattle = this.battleController.GetCurrentBattle();
			this.battleRecord.BattleAttributes.BattleEndedAt = this.battleController.Now;
			this.battleRecord.BattleAttributes.TimeLeft = currentBattle.TimeLeft;
			this.battleRecord.BattleAttributes.DamagePercentage = currentBattle.DamagePercent;
			this.battleRecord.BattleAttributes.LootCreditsEarned = currentBattle.LootCreditsEarned;
			this.battleRecord.BattleAttributes.LootMaterialsEarned = currentBattle.LootMaterialsEarned;
			this.battleRecord.BattleAttributes.LootContrabandEarned = currentBattle.LootContrabandEarned;
			this.eventManager.SendEvent(EventId.BattleEndRecorded, null);
			if ((float)GameConstants.FPS_THRESHOLD > this.battleRecord.LowestFPS)
			{
				GamePlayer worldOwner = GameUtils.GetWorldOwner();
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append("battle: ");
				stringBuilder.Append(this.battleRecord.RecordId);
				stringBuilder.Append("|");
				stringBuilder.Append(worldOwner.CurrentXPAmount);
				stringBuilder.Append("|");
				stringBuilder.Append(currentBattle.DefenderBaseScore);
				Service.BILoggingController.TrackLowFPS(stringBuilder.ToString());
			}
		}

		private void HandleHeroDeployedEvent(object cookie)
		{
			SmartEntity smartEntity = (SmartEntity)cookie;
			TroopComponent troopComp = smartEntity.TroopComp;
			if (troopComp == null)
			{
				return;
			}
			TransformComponent transformComp = smartEntity.TransformComp;
			if (transformComp == null)
			{
				return;
			}
			HeroDeployedAction heroDeployedAction = new HeroDeployedAction();
			heroDeployedAction.Time = this.GetActionTime();
			heroDeployedAction.TroopUid = troopComp.TroopType.Uid;
			heroDeployedAction.BoardX = transformComp.CenterGridX();
			heroDeployedAction.BoardZ = transformComp.CenterGridZ();
			TeamComponent teamComp = smartEntity.TeamComp;
			heroDeployedAction.TeamType = teamComp.TeamType;
			this.battleRecord.Add(heroDeployedAction);
		}

		private void HandleChampionDeployedEvent(object cookie)
		{
			SmartEntity smartEntity = (SmartEntity)cookie;
			TroopComponent troopComp = smartEntity.TroopComp;
			if (troopComp == null)
			{
				return;
			}
			TransformComponent transformComp = smartEntity.TransformComp;
			if (transformComp == null)
			{
				return;
			}
			ChampionDeployedAction championDeployedAction = new ChampionDeployedAction();
			championDeployedAction.Time = this.GetActionTime();
			championDeployedAction.TroopUid = troopComp.TroopType.Uid;
			championDeployedAction.BoardX = transformComp.CenterGridX();
			championDeployedAction.BoardZ = transformComp.CenterGridZ();
			TeamComponent teamComp = smartEntity.TeamComp;
			championDeployedAction.TeamType = teamComp.TeamType;
			this.battleRecord.Add(championDeployedAction);
		}

		private void HandleEntityKilledEvent(object cookie)
		{
			this.battleRecord.BattleAttributes.AddToDeathLog(cookie as SmartEntity, this.GetActionTime());
		}

		public void OnPerformanceFPS(float fps)
		{
			if (fps > 0f && this.battleRecord != null && fps < this.battleRecord.LowestFPS)
			{
				uint actionTime = this.GetActionTime();
				if (actionTime > 1000u)
				{
					this.battleRecord.LowestFPS = fps;
					this.battleRecord.LowestFPSTime = actionTime;
				}
			}
		}

		public void OnPerformanceMemUsed(uint memUsed)
		{
		}

		public void OnPerformanceFPeak(uint fpeak)
		{
		}

		public void OnPerformanceMemRsvd(uint memRsvd)
		{
		}

		public void OnPerformanceMemTexture(uint memTexture)
		{
		}

		public void OnPerformanceMemMesh(uint memMesh)
		{
		}

		public void OnPerformanceMemAudio(uint memAudio)
		{
		}

		public void OnPerformanceMemAnimation(uint memAnimation)
		{
		}

		public void OnPerformanceMemMaterials(uint memMaterials)
		{
		}

		public void OnPerformanceDeviceMemUsage(long memory)
		{
		}
	}
}
