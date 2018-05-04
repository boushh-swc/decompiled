using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands
{
	public class GetReplayResponse : AbstractResponse
	{
		public BattleEntry EntryData
		{
			get;
			set;
		}

		public BattleRecord ReplayData
		{
			get;
			set;
		}

		public Dictionary<string, object> Response
		{
			get;
			set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.Response = (obj as Dictionary<string, object>);
			if (this.Response.ContainsKey("battleLog"))
			{
				this.EntryData = new BattleEntry();
				this.EntryData.FromObject(this.Response["battleLog"]);
				this.EntryData.SetupExpendedTroops();
			}
			if (this.Response.ContainsKey("replayData"))
			{
				this.ReplayData = new BattleRecord();
				this.ReplayData.FromObject(this.Response["replayData"]);
			}
			if (this.ReplayData.BattleType == BattleType.Pvp)
			{
				this.EntryData.AllowReplay = true;
			}
			return this;
		}

		public BattleRecord GetOriginalReplayRecord()
		{
			if (this.Response != null && this.Response.ContainsKey("replayData"))
			{
				this.ReplayData = new BattleRecord();
				this.ReplayData.FromObject(this.Response["replayData"]);
				return this.ReplayData;
			}
			return null;
		}
	}
}
