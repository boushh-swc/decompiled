using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class CheatAddCrateRequest : PlayerIdRequest
	{
		private string playerId;

		private string crateCMSId;

		private string context;

		private string planetId;

		private int hqLevel;

		private int expireInMin;

		public CheatAddCrateRequest(string crateCMSId, string context, string planetId, int hqLevel, int expireInMin)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			this.playerId = currentPlayer.PlayerId;
			this.crateCMSId = crateCMSId;
			this.context = context;
			this.planetId = planetId;
			this.hqLevel = hqLevel;
			this.expireInMin = expireInMin;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", this.playerId);
			serializer.AddString("crateId", this.crateCMSId);
			serializer.AddString("context", this.context);
			serializer.AddString("planet", this.planetId);
			serializer.AddString("hqLevel", this.hqLevel.ToString());
			serializer.AddString("expiresInMinutes", this.expireInMin.ToString());
			return serializer.End().ToString();
		}
	}
}
