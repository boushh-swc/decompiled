using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class BattlePlaybackState : IGameState, IEventObserver, IState
	{
		private BattleRecord battleRecord;

		private BattleEntry battleEntry;

		protected BattlePlaybackState()
		{
		}

		public static void GoToBattlePlaybackState(BattleRecord battleRecord, BattleEntry battleEntry, IMapDataLoader mapDataLoader)
		{
			BattlePlaybackState battlePlaybackState = new BattlePlaybackState();
			Service.BattlePlaybackController.InitPlayback(battleRecord, battleEntry);
			battlePlaybackState.Setup(battleRecord, battleEntry, mapDataLoader);
		}

		public static void GoToBattlePlaybackState(ReplayMapDataLoader mapDataLoader)
		{
			BattlePlaybackState battlePlaybackState = new BattlePlaybackState();
			battlePlaybackState.Setup(null, null, mapDataLoader);
			Service.EventManager.RegisterObserver(battlePlaybackState, EventId.BattleRecordRetrieved, EventPriority.Default);
		}

		protected void Setup(BattleRecord battleRecord, BattleEntry battleEntry, IMapDataLoader mapDataLoader)
		{
			this.battleRecord = battleRecord;
			this.battleEntry = battleEntry;
			if (battleRecord != null)
			{
				Service.EventManager.SendEvent(EventId.BattleReplaySetup, battleRecord);
			}
			Service.EventManager.RegisterObserver(this, EventId.MapDataProcessingStart, EventPriority.Default);
			Service.WorldTransitioner.StartTransition(new WorldToWorldTransition(this, mapDataLoader, new TransitionCompleteDelegate(this.OnWorldTransitionComplete), false, true));
		}

		public void OnEnter()
		{
			Service.UXController.HUD.Visible = true;
			Service.UXController.HUD.ConfigureControls(new HudConfig(new string[]
			{
				"Currency",
				"OpponentInfo",
				"ButtonHome",
				"DamageStars",
				"LabelBaseNameOpponent",
				"LabelCurrencyValueOpponent",
				"ReplayControls"
			}));
		}

		public void OnExit(IState nextState)
		{
			Service.BattleController.EndBattleRightAway();
			Service.BattlePlaybackController.EndPlayback();
		}

		private void OnMapProcessingStart()
		{
			Service.Rand.SimSeed = this.battleRecord.SimSeed;
		}

		private void OnWorldTransitionComplete()
		{
			Service.BattlePlaybackController.StartPlayback();
			Service.ChampionController.DestroyAllChampionEntities();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.MapDataProcessingStart)
			{
				if (id == EventId.BattleRecordRetrieved)
				{
					GetReplayResponse getReplayResponse = (GetReplayResponse)cookie;
					this.battleRecord = getReplayResponse.ReplayData;
					this.battleEntry = getReplayResponse.EntryData;
					Service.BattlePlaybackController.InitPlayback(this.battleRecord, this.battleEntry);
				}
			}
			else
			{
				this.OnMapProcessingStart();
				Service.EventManager.UnregisterObserver(this, EventId.MapDataProcessingStart);
			}
			return EatResponse.NotEaten;
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
