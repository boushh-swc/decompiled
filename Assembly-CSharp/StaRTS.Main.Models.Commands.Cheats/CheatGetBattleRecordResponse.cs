using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatGetBattleRecordResponse : AbstractResponse
	{
		public BattleRecord ReplayData
		{
			get;
			set;
		}

		public string BattleId
		{
			get;
			set;
		}

		public CheatGetBattleRecordResponse(string battleId)
		{
			this.BattleId = battleId;
		}

		public override ISerializable FromObject(object obj)
		{
			this.ReplayData = new BattleRecord();
			this.ReplayData.FromObject(obj);
			return this;
		}
	}
}
