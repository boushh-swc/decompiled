using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SetPlayerNameRequest : PlayerIdRequest
	{
		private string name;

		public SetPlayerNameRequest(string nameRequested)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.name = nameRequested;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("playerName", this.name);
			return serializer.End().ToString();
		}
	}
}
