using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSquadWarTurnsRequest : PlayerIdRequest
	{
		private int turns;

		public CheatSquadWarTurnsRequest(int turns)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.turns = turns;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<int>("turns", this.turns);
			return serializer.End().ToString();
		}
	}
}
