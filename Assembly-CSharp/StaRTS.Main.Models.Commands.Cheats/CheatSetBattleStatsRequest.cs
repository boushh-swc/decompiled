using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetBattleStatsRequest : PlayerIdRequest
	{
		private KeyValuePair<string, int> argument;

		public CheatSetBattleStatsRequest(KeyValuePair<string, int> argument)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.argument = argument;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<int>(this.argument.Key, this.argument.Value);
			return serializer.End().ToString();
		}
	}
}
