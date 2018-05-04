using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class GetReplayRequest : PlayerIdRequest
	{
		private string battleId;

		private string participantId;

		public GetReplayRequest(string playerId, string battleId, string participantId)
		{
			base.PlayerId = playerId;
			this.battleId = battleId;
			this.participantId = participantId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("battleId", this.battleId);
			serializer.AddString("participantId", this.participantId);
			return serializer.End().ToString();
		}
	}
}
