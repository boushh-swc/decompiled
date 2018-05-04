using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SquadWarClaimRewardRequest : PlayerIdRequest
	{
		private string warID;

		public SquadWarClaimRewardRequest(string warID)
		{
			this.warID = warID;
		}

		public override string ToJson()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			base.PlayerId = currentPlayer.PlayerId;
			Serializer serializer = Serializer.Start();
			serializer.AddString("warId", this.warID);
			serializer.AddString("playerId", base.PlayerId);
			return serializer.End().ToString();
		}
	}
}
