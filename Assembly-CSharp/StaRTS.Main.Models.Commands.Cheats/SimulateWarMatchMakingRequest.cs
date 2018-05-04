using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class SimulateWarMatchMakingRequest : PlayerIdRequest
	{
		private string quildId;

		public SimulateWarMatchMakingRequest(string squadId)
		{
			this.quildId = squadId;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("guildId", this.quildId);
			return serializer.End().ToString();
		}
	}
}
