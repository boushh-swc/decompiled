using StaRTS.DataStructures;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class BattlePlaybackController : IEventObserver
	{
		private Dictionary<string, TimerDelegate> actionCallbacks;

		private SimTimerManager timerManager;

		private EventManager eventManager;

		private BattleController battleController;

		private const int DEFAULT_PLAYBACK_SCALE = 1;

		private const int MAX_PLAYBACK_SCALE = 4;

		private uint currentPlaybackScale;

		private uint[] timers;

		private BattleAttributes recordBattleAttr;

		private BattleAttributes playbackBattleAttr;

		public uint CurrentPlaybackScale
		{
			get
			{
				return this.currentPlaybackScale;
			}
		}

		public BattleAttributes PlaybackBattleAttr
		{
			get
			{
				return this.playbackBattleAttr;
			}
		}

		public BattleAttributes RecordBattleAttr
		{
			get
			{
				return this.recordBattleAttr;
			}
		}

		public BattleRecord CurrentBattleRecord
		{
			get;
			private set;
		}

		public BattleEntry CurrentBattleEntry
		{
			get;
			private set;
		}

		public BattlePlaybackController()
		{
			Service.BattlePlaybackController = this;
			this.timerManager = Service.SimTimerManager;
			this.eventManager = Service.EventManager;
			this.battleController = Service.BattleController;
			this.CurrentBattleRecord = null;
			this.CurrentBattleEntry = null;
			this.SetDefaultPlaybackScale();
			this.actionCallbacks = new Dictionary<string, TimerDelegate>();
			this.actionCallbacks.Add("TroopPlaced", new TimerDelegate(this.TroopPlacedActionCallback));
			this.actionCallbacks.Add("HeroDeployed", new TimerDelegate(this.HeroDeployedActionCallback));
			this.actionCallbacks.Add("HeroAbilityActivated", new TimerDelegate(this.HeroAbilityActivateActionCallback));
			this.actionCallbacks.Add("ChampionDeployed", new TimerDelegate(this.ChampionDeployedActionCallback));
			this.actionCallbacks.Add("SpecialAttackDeployed", new TimerDelegate(this.SpecialAttackDeployedActionCallback));
			this.actionCallbacks.Add("SquadTroopPlaced", new TimerDelegate(this.SquadTroopPlacedActionCallback));
			this.actionCallbacks.Add("BattleCanceled", new TimerDelegate(this.BattleCanceledActionCallback));
			this.eventManager.RegisterObserver(this, EventId.BattleReplayRequested, EventPriority.Default);
		}

		public void InitPlayback(BattleRecord battleRecord, BattleEntry battleEntry)
		{
			this.CurrentBattleRecord = battleRecord;
			this.CurrentBattleEntry = battleEntry;
			BattleInitializationData battleInitializationData = BattleInitializationData.CreateFromReplay(this.CurrentBattleRecord, this.CurrentBattleEntry);
			this.battleController.InitializeCurrentBattle(battleInitializationData);
		}

		public void StartPlayback()
		{
			this.SetDefaultPlaybackScale();
			Service.UXController.HUD.UpdateCurrentReplaySpeedUI();
			Service.UXController.HUD.ShowReplayTimer();
			this.battleController.PrepareWorldForBattle();
			Service.EventManager.SendEvent(EventId.EquipmentBuffShaderRemove, GameConstants.EQUIPMENT_SHADER_DELAY_REPLAY);
			this.battleController.StartBattle();
			this.eventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.BattleEndProcessing, EventPriority.Default);
			this.playbackBattleAttr = new BattleAttributes();
			this.recordBattleAttr = this.CurrentBattleRecord.BattleAttributes;
			this.timers = new uint[this.CurrentBattleRecord.BattleActions.Count];
			for (int i = 0; i < this.CurrentBattleRecord.BattleActions.Count; i++)
			{
				IBattleAction battleAction = this.CurrentBattleRecord.BattleActions[i];
				TimerDelegate callback;
				if (!this.actionCallbacks.TryGetValue(battleAction.ActionId, out callback))
				{
					callback = new TimerDelegate(this.DefaultActionCallback);
				}
				this.timers[i] = this.timerManager.CreateSimTimer(battleAction.Time + 1u, false, callback, battleAction);
			}
		}

		public void EndPlayback()
		{
			this.SetDefaultPlaybackScale();
			if (this.timers != null)
			{
				int i = 0;
				int num = this.timers.Length;
				while (i < num)
				{
					this.timerManager.KillSimTimer(this.timers[i]);
					i++;
				}
			}
			this.ScalePlaybackTime();
			this.eventManager.UnregisterObserver(this, EventId.BattleEndProcessing);
			this.eventManager.UnregisterObserver(this, EventId.EntityKilled);
			this.eventManager.SendEvent(EventId.BattleReplayEnded, null);
			if (BattleAttributes.Equals(this.playbackBattleAttr, this.recordBattleAttr))
			{
				Service.Logger.Debug("Verified: Playback and Record matched.");
			}
			else
			{
				Service.Logger.DebugFormat("PLAYBACK_MISMATCH!!! DamagePercentage, CreditsEarned, MaterialsEarned, ContrabandEarned, DeathLogCount, BattleEndedAt", new object[0]);
				Service.Logger.DebugFormat("PLAYBACK_MISMATCH!!! We got: {0}, {1}, {2}, {3}, {4}, {5}", new object[]
				{
					this.playbackBattleAttr.DamagePercentage,
					this.playbackBattleAttr.LootCreditsEarned,
					this.playbackBattleAttr.LootMaterialsEarned,
					this.playbackBattleAttr.LootContrabandEarned,
					this.playbackBattleAttr.DeathLogCount,
					this.playbackBattleAttr.BattleEndedAt
				});
				Service.Logger.DebugFormat("PLAYBACK_MISMATCH!!! We expect: {0}, {1}, {2}, {3}, {4}, {5}", new object[]
				{
					this.recordBattleAttr.DamagePercentage,
					this.recordBattleAttr.LootCreditsEarned,
					this.recordBattleAttr.LootMaterialsEarned,
					this.recordBattleAttr.LootContrabandEarned,
					this.recordBattleAttr.DeathLogCount,
					this.recordBattleAttr.BattleEndedAt
				});
			}
			string str = this.playbackBattleAttr.ToJson();
			string str2 = this.recordBattleAttr.ToJson();
			Service.Logger.Debug("playbackBattleAttr: " + str);
			Service.Logger.Debug("recordBattleAttr: " + str2);
		}

		public void DiscardLastReplay()
		{
			this.CurrentBattleRecord = null;
			this.CurrentBattleEntry = null;
		}

		public void FastForward()
		{
			if (this.currentPlaybackScale == 4u)
			{
				this.SetDefaultPlaybackScale();
			}
			else
			{
				this.currentPlaybackScale += 1u;
			}
			this.ScalePlaybackTime();
		}

		public void Pause()
		{
			if (this.currentPlaybackScale == 0u)
			{
				this.SetDefaultPlaybackScale();
			}
			else
			{
				this.currentPlaybackScale = 0u;
			}
			this.ScalePlaybackTime();
		}

		private void SetDefaultPlaybackScale()
		{
			this.currentPlaybackScale = 1u;
		}

		private void ScalePlaybackTime()
		{
			Service.SimTimeEngine.ScaleTime(this.currentPlaybackScale);
			Service.SpecialAttackController.SetSpeed(this.currentPlaybackScale);
			Service.FXManager.SetSpeed(this.currentPlaybackScale);
			Service.EntityRenderController.SetSpeed(this.currentPlaybackScale);
			Service.EntityController.SetSpeed(this.currentPlaybackScale);
			Service.ProjectileViewManager.SetSpeed(this.currentPlaybackScale);
		}

		private void TroopPlacedActionCallback(uint id, object cookie)
		{
			TroopPlacedAction troopPlacedAction = cookie as TroopPlacedAction;
			StaticDataController staticDataController = Service.StaticDataController;
			TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(troopPlacedAction.TroopId);
			IntPosition boardPosition = new IntPosition(troopPlacedAction.BoardX, troopPlacedAction.BoardZ);
			Service.TroopController.SpawnTroop(troopTypeVO, troopPlacedAction.TeamType, boardPosition, (troopPlacedAction.TeamType != TeamType.Defender) ? TroopSpawnMode.Unleashed : TroopSpawnMode.LeashedToBuilding, true);
			this.battleController.OnTroopDeployed(troopTypeVO.Uid, troopPlacedAction.TeamType, boardPosition);
		}

		private void HeroDeployedActionCallback(uint id, object cookie)
		{
			HeroDeployedAction heroDeployedAction = cookie as HeroDeployedAction;
			StaticDataController staticDataController = Service.StaticDataController;
			TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(heroDeployedAction.TroopUid);
			IntPosition boardPosition = new IntPosition(heroDeployedAction.BoardX, heroDeployedAction.BoardZ);
			SmartEntity cookie2 = Service.TroopController.SpawnHero(troopTypeVO, heroDeployedAction.TeamType, boardPosition);
			Service.EventManager.SendEvent(EventId.AddDecalToTroop, cookie2);
			this.battleController.OnHeroDeployed(troopTypeVO.Uid, heroDeployedAction.TeamType, boardPosition);
		}

		private void HeroAbilityActivateActionCallback(uint id, object cookie)
		{
			HeroAbilityAction heroAbilityAction = cookie as HeroAbilityAction;
			StaticDataController staticDataController = Service.StaticDataController;
			TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(heroAbilityAction.TroopUid);
			Service.TroopAbilityController.ActivateHeroAbility(troopTypeVO.Uid);
		}

		private void ChampionDeployedActionCallback(uint id, object cookie)
		{
			ChampionDeployedAction championDeployedAction = cookie as ChampionDeployedAction;
			StaticDataController staticDataController = Service.StaticDataController;
			TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(championDeployedAction.TroopUid);
			IntPosition boardPosition = new IntPosition(championDeployedAction.BoardX, championDeployedAction.BoardZ);
			Service.TroopController.SpawnChampion(troopTypeVO, championDeployedAction.TeamType, boardPosition);
			this.battleController.OnChampionDeployed(troopTypeVO.Uid, championDeployedAction.TeamType, boardPosition);
		}

		private void SpecialAttackDeployedActionCallback(uint id, object cookie)
		{
			SpecialAttackDeployedAction specialAttackDeployedAction = cookie as SpecialAttackDeployedAction;
			StaticDataController staticDataController = Service.StaticDataController;
			SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(specialAttackDeployedAction.SpecialAttackId);
			Vector3 vector = new Vector3(Units.BoardToWorldX(specialAttackDeployedAction.BoardX), 0f, Units.BoardToWorldZ(specialAttackDeployedAction.BoardZ));
			Service.SpecialAttackController.DeploySpecialAttack(specialAttackTypeVO, specialAttackDeployedAction.TeamType, vector);
			IntPosition boardPosition = Units.WorldToBoardIntPosition(vector);
			this.battleController.OnSpecialAttackDeployed(specialAttackTypeVO.Uid, specialAttackDeployedAction.TeamType, boardPosition);
		}

		private void SquadTroopPlacedActionCallback(uint id, object cookie)
		{
			SquadTroopPlacedAction squadTroopPlacedAction = cookie as SquadTroopPlacedAction;
			IntPosition boardPos = new IntPosition(squadTroopPlacedAction.BoardX, squadTroopPlacedAction.BoardZ);
			Dictionary<string, int> attackerGuildTroopsAvailable = this.battleController.GetCurrentBattle().AttackerGuildTroopsAvailable;
			Service.SquadTroopAttackController.DeploySquadTroops(boardPos, attackerGuildTroopsAvailable);
		}

		private void BattleCanceledActionCallback(uint id, object cookie)
		{
			this.battleController.CancelBattle();
		}

		private void DefaultActionCallback(uint id, object cookie)
		{
		}

		private void ReplayLastBattleOrReplay()
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			ReplayMapDataLoader replayMapDataLoader = Service.ReplayMapDataLoader;
			BattleRecord battleRecord = Service.BattleRecordController.BattleRecord;
			BattleEntry battleEntry = currentBattle.Clone();
			replayMapDataLoader.InitializeFromData(battleEntry, battleRecord);
			BattlePlaybackState.GoToBattlePlaybackState(battleRecord, battleEntry, replayMapDataLoader);
			this.LogReplayViewed(battleEntry.RecordID, battleEntry.Defender.PlayerId, battleEntry.SharerPlayerId);
		}

		public void LogReplayViewed(string battleId, string defenderPlayerId, string sharerPlayerId)
		{
			string playerId = Service.CurrentPlayer.PlayerId;
			string text = (sharerPlayerId == null) ? playerId : sharerPlayerId;
			string message = (!(text == playerId)) ? "shared" : "personal";
			string action = (!(defenderPlayerId == text)) ? "attack" : "defense";
			Service.BILoggingController.TrackGameAction("watch_replay", action, message, battleId, 1);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			CurrentBattle currentBattle = this.battleController.GetCurrentBattle();
			if (id != EventId.BattleEndProcessing)
			{
				if (id != EventId.EntityKilled)
				{
					if (id == EventId.BattleReplayRequested)
					{
						if (currentBattle.IsReplay)
						{
							Service.PvpManager.ReplayBattle(currentBattle.RecordID, currentBattle.Defender, null);
						}
						else
						{
							this.ReplayLastBattleOrReplay();
						}
					}
				}
				else
				{
					this.playbackBattleAttr.AddToDeathLog(cookie as SmartEntity, this.battleController.Now);
				}
			}
			else
			{
				this.playbackBattleAttr.BattleEndedAt = this.battleController.Now;
				this.playbackBattleAttr.TimeLeft = currentBattle.TimeLeft;
				this.playbackBattleAttr.DamagePercentage = currentBattle.DamagePercent;
				this.playbackBattleAttr.LootCreditsEarned = currentBattle.LootCreditsEarned;
				this.playbackBattleAttr.LootMaterialsEarned = currentBattle.LootMaterialsEarned;
				this.playbackBattleAttr.LootContrabandEarned = currentBattle.LootContrabandEarned;
				this.playbackBattleAttr.AddDeviceInfo();
			}
			return EatResponse.NotEaten;
		}
	}
}
