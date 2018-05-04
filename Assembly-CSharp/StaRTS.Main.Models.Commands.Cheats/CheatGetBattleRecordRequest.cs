using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatGetBattleRecordRequest : PlayerIdRequest
	{
		private string battleId;

		public CheatGetBattleRecordRequest(string battleId)
		{
			this.battleId = battleId;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("battleId", this.battleId);
			serializer.AddString("playerId", base.PlayerId);
			return serializer.End().ToString();
		}
	}
}
