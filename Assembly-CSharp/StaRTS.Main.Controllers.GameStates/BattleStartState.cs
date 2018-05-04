using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.GameStates
{
	public class BattleStartState : IGameState, IEventObserver, IState
	{
		private TransitionCompleteDelegate onComplete;

		private BattleInitializationData battleData;

		private float viewTimeOnWorldLoad;

		protected BattleStartState()
		{
		}

		public static void GoToBattleStartState(BattleInitializationData data, TransitionCompleteDelegate onComplete)
		{
			BattleStartState state = new BattleStartState();
			BattleStartState.GoToBattleStartState(state, data, onComplete);
		}

		public static void GoToBattleStartState(BattleStartState state, BattleInitializationData data, TransitionCompleteDelegate onComplete)
		{
			state.Setup(data, onComplete);
			state.SetupTransition();
		}

		protected void Setup(BattleInitializationData data, TransitionCompleteDelegate onComplete)
		{
			this.battleData = data;
			this.onComplete = onComplete;
			Service.BattleController.InitializeCurrentBattle(data);
			Service.EventManager.SendEvent(EventId.BattleLoadStart, data);
		}

		private void SetupTransition()
		{
			Service.UXController.HUD.Visible = false;
			if (this.battleData.BattleType == BattleType.PveDefend)
			{
				List<IAssetVO> battleProjectileAssets = ProjectileUtils.GetBattleProjectileAssets(Service.CurrentPlayer.Map, this.battleData.BattleVO, this.battleData.AttackerDeployableData, null, null, null, null, this.battleData.AttackerEquipment, this.battleData.DefenderEquipment);
				Service.SpecialAttackController.PreloadSpecialAttackMiscAssets();
				Service.ProjectileViewManager.LoadProjectileAssetsAndCreatePools(battleProjectileAssets);
				Service.GameStateMachine.SetState(this);
				this.OnWorldLoadComplete();
				Service.EventManager.SendEvent(EventId.BattleLoadedForDefend, this.battleData);
				this.OnWorldTransitionComplete();
				return;
			}
			IMapDataLoader mapDataLoader;
			if (this.battleData.PvpTarget != null)
			{
				mapDataLoader = Service.PvpMapDataLoader;
				((PvpMapDataLoader)mapDataLoader).Initialize(this.battleData);
			}
			else if (this.battleData.MemberWarData != null)
			{
				mapDataLoader = Service.WarBaseMapDataLoader;
				((WarBaseMapDataLoader)mapDataLoader).Initialize(this.battleData);
			}
			else
			{
				mapDataLoader = Service.NpcMapDataLoader;
				bool isPveBuffBase = this.battleData.BattleType == BattleType.PveBuffBase;
				((NpcMapDataLoader)mapDataLoader).Initialize(this.battleData, isPveBuffBase);
			}
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.MapDataProcessingStart, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			WorldTransitioner worldTransitioner = Service.WorldTransitioner;
			if (worldTransitioner.IsSoftWiping())
			{
				worldTransitioner.ContinueWipe(this, mapDataLoader, new TransitionCompleteDelegate(this.OnWorldTransitionComplete));
			}
			else if (this.battleData.BattleType == BattleType.PvpAttackSquadWar || this.battleData.BattleType == BattleType.PveBuffBase)
			{
				worldTransitioner.StartTransition(new WarboardToWarbaseTransition(this, mapDataLoader, new TransitionCompleteDelegate(this.OnWorldTransitionComplete), false, false));
			}
			else
			{
				worldTransitioner.StartTransition(new WorldToWorldTransition(this, mapDataLoader, new TransitionCompleteDelegate(this.OnWorldTransitionComplete), false, false));
			}
		}

		public virtual void OnEnter()
		{
			Service.UXController.MiscElementsManager.RemoveGalaxyTournamentStatus();
			HudConfig hudConfig = null;
			BattleType battleType = this.battleData.BattleType;
			switch (battleType)
			{
			case BattleType.Pvp:
				hudConfig = new HudConfig(new string[]
				{
					"Currency",
					"OpponentInfo",
					"MedalInfo",
					"ButtonHome",
					"LabelBaseNameOpponent",
					"TroopsGrid",
					"LabelDeployInstructions",
					"LabelCurrencyValueOpponent"
				});
				if (!this.battleData.IsRevenge)
				{
					hudConfig.Add("ButtonNextBattle");
				}
				break;
			case BattleType.PveDefend:
				hudConfig = new HudConfig(new string[]
				{
					"Currency",
					"PlayerInfo",
					"LabelBaseNameOpponent",
					"LabelCurrencyValueOpponent",
					"TroopsGrid",
					"LabelDeployInstructions"
				});
				break;
			case BattleType.PveAttack:
			case BattleType.PveFue:
			case BattleType.ClientBattle:
				hudConfig = new HudConfig(new string[]
				{
					"Currency",
					"OpponentInfo",
					"ButtonHome",
					"LabelBaseNameOpponent",
					"TroopsGrid",
					"LabelDeployInstructions",
					"LabelCurrencyValueOpponent"
				});
				break;
			case BattleType.PveBuffBase:
			case BattleType.PvpAttackSquadWar:
			{
				hudConfig = new HudConfig(new string[]
				{
					"OpponentInfo",
					"LabelBaseNameOpponent",
					"WarAttack"
				});
				SquadWarManager warManager = Service.SquadController.WarManager;
				bool flag = this.battleData.Attacker.PlayerFaction == this.battleData.Defender.PlayerFaction;
				SquadWarStatusType currentStatus = warManager.GetCurrentStatus();
				if (!flag)
				{
					hudConfig.Add("BuffsYoursSquadWars");
				}
				if (battleType != BattleType.PveBuffBase)
				{
					hudConfig.Add("BuffsOpponentsSquadWars");
				}
				if (this.battleData.Attacker.GuildId != this.battleData.Defender.GuildId && !warManager.IsCurrentlyScoutingOwnedBuffBase())
				{
					hudConfig.Add("WarAttackOpponent");
					if (currentStatus == SquadWarStatusType.PhaseAction)
					{
						hudConfig.Add("TroopsGrid");
					}
				}
				if (currentStatus == SquadWarStatusType.PhasePrep)
				{
					if (battleType == BattleType.PvpAttackSquadWar)
					{
						Service.BuildingController.EnterSelectMode();
					}
					else
					{
						Service.BuildingController.ExitAllModes();
					}
				}
				break;
			}
			}
			if (hudConfig != null)
			{
				Service.UXController.HUD.ConfigureControls(hudConfig);
			}
		}

		private void OnMapProcessingStart()
		{
			Rand rand = Service.Rand;
			BattleController battleController = Service.BattleController;
			battleController.SimSeed = rand.RandomizeSimSeed();
		}

		private void OnWorldLoadComplete()
		{
			this.viewTimeOnWorldLoad = Time.realtimeSinceStartup;
			Service.ChampionController.DestroyAllChampionEntities();
		}

		private void OnWorldTransitionComplete()
		{
			Service.UXController.HUD.Visible = true;
			Service.EventManager.SendEvent(EventId.BattleLoadEnd, null);
			Service.BattleController.PrepareWorldForBattle();
			if (this.onComplete != null)
			{
				this.onComplete();
			}
		}

		public virtual void OnExit(IState nextState)
		{
			BattleController battleController = Service.BattleController;
			if (nextState is BattlePlayState)
			{
				float viewTimePassedPreBattle = Time.realtimeSinceStartup - this.viewTimeOnWorldLoad;
				battleController.ViewTimePassedPreBattle = viewTimePassedPreBattle;
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.MapDataProcessingStart:
				this.OnMapProcessingStart();
				Service.EventManager.UnregisterObserver(this, EventId.MapDataProcessingStart);
				break;
			case EventId.WorldLoadComplete:
				this.OnWorldLoadComplete();
				Service.EventManager.UnregisterObserver(this, EventId.WorldLoadComplete);
				break;
			}
			return EatResponse.NotEaten;
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
