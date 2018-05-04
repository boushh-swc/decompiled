using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Commands;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class ReplayMapDataLoader : IMapDataLoader
	{
		private MapLoadedDelegate OnMapLoaded;

		private BattleParticipant defender;

		private string replayOwnerPlayerId;

		private GetReplayResponse replayResponseData;

		private const WorldType worldType = WorldType.Replay;

		public ReplayMapDataLoader()
		{
			Service.ReplayMapDataLoader = this;
		}

		public ReplayMapDataLoader Initialize(BattleParticipant defender, string replayOwnerPlayerId)
		{
			this.defender = defender;
			this.replayOwnerPlayerId = replayOwnerPlayerId;
			this.replayResponseData = null;
			return this;
		}

		public void SetReplayResponseData(GetReplayResponse replayResponseData)
		{
			this.replayResponseData = replayResponseData;
		}

		public ReplayMapDataLoader InitializeFromData(BattleEntry battleEntry, BattleRecord battleRecord)
		{
			this.replayResponseData = new GetReplayResponse();
			this.replayResponseData.EntryData = battleEntry;
			this.replayResponseData.ReplayData = battleRecord;
			this.defender = battleEntry.Defender;
			this.replayOwnerPlayerId = null;
			return this;
		}

		public void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail)
		{
			BattleRecord battleRecord = this.replayResponseData.GetOriginalReplayRecord();
			if (battleRecord == null)
			{
				battleRecord = this.replayResponseData.ReplayData;
			}
			onMapLoaded(battleRecord.CombatEncounter.map);
		}

		public void OnReplayLoaded(GetReplayResponse response, object cookie)
		{
			ProcessingScreen.Hide();
			this.replayResponseData = response;
			if (this.replayResponseData == null)
			{
				this.OnReplayLoadFailed(2110u, null);
				return;
			}
			BattlePlaybackState.GoToBattlePlaybackState(this);
			BattleRecord replayData = response.ReplayData;
			BattleEntry entryData = response.EntryData;
			replayData.RecordId = entryData.RecordID;
			entryData.SharerPlayerId = this.replayOwnerPlayerId;
			bool flag = Service.CurrentPlayer.PlayerId == entryData.AttackerID || this.replayOwnerPlayerId == entryData.AttackerID;
			entryData.Won = ((!flag) ? (entryData.EarnedStars == 0) : (entryData.EarnedStars > 0));
			Service.EventManager.SendEvent(EventId.BattleRecordRetrieved, this.replayResponseData);
		}

		public void OnReplayLoadFailed(uint status, object cookie)
		{
			ProcessingScreen.Hide();
			if (!(Service.GameStateMachine.CurrentState is HomeState))
			{
				HomeState.GoToHomeState(null, false);
			}
			if (status == 2110u)
			{
				string message = Service.Lang.Get("REPLAY_DATA_NOT_FOUND", new object[0]);
				AlertScreen.ShowModal(false, null, message, null, null);
			}
		}

		public List<IAssetVO> GetPreloads()
		{
			BattleRecord battleRecord = null;
			if (this.replayResponseData != null)
			{
				battleRecord = this.replayResponseData.ReplayData;
			}
			return MapDataLoaderUtils.GetBattleRecordPreloads(battleRecord);
		}

		public List<IAssetVO> GetProjectilePreloads(Map map)
		{
			BattleRecord replayData = this.replayResponseData.ReplayData;
			return ProjectileUtils.GetBattleRecordProjectileAssets(map, replayData, replayData.AttackerWarBuffs, replayData.DefenderWarBuffs, replayData.AttackerEquipment, replayData.DefenderEquipment);
		}

		public WorldType GetWorldType()
		{
			return WorldType.Replay;
		}

		public string GetWorldName()
		{
			return this.defender.PlayerName;
		}

		public string GetFactionAssetName()
		{
			return UXUtils.GetIconNameFromFactionType(this.defender.PlayerFaction);
		}

		public PlanetVO GetPlanetData()
		{
			if (this.replayResponseData != null && this.replayResponseData.ReplayData != null)
			{
				return Service.StaticDataController.Get<PlanetVO>(this.replayResponseData.ReplayData.PlanetId);
			}
			return null;
		}
	}
}
