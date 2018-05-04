using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetSquadLevelRequest : PlayerIdRequest
	{
		private int levelToSet;

		public CheatSetSquadLevelRequest(int levelToSet)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.levelToSet = levelToSet;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("level", this.levelToSet.ToString());
			return serializer.End().ToString();
		}
	}
}
