using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSaveBattleRecordRequest : PlayerIdRequest
	{
		private string battleRecordId;

		private BattleRecord replayData;

		public CheatSaveBattleRecordRequest(string battleRecordId, BattleRecord replayData)
		{
			this.battleRecordId = battleRecordId;
			this.replayData = replayData;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("battleId", this.battleRecordId);
			serializer.AddObject<BattleRecord>("replayData", this.replayData);
			serializer.AddString("playerId", base.PlayerId);
			return serializer.End().ToString();
		}
	}
}
