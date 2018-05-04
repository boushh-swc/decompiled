using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatStartWarRequest : PlayerIdRequest
	{
		public string GuildId
		{
			get;
			private set;
		}

		public CheatStartWarRequest(string guildId)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.GuildId = guildId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("guildId", this.GuildId);
			return serializer.End().ToString();
		}
	}
}
