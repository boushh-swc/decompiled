using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class SetFactionRequest : PlayerIdRequest
	{
		private string factionString;

		public SetFactionRequest(FactionType faction)
		{
			this.factionString = faction.ToString().ToLower();
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("faction", this.factionString);
			return serializer.End().ToString();
		}
	}
}
