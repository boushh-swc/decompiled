using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class PlayerLeaderboardRequest : PlayerIdRequest
	{
		public string PlanetId
		{
			get;
			set;
		}

		public PlayerLeaderboardRequest(string planetId, string playerId)
		{
			this.PlanetId = planetId;
			base.PlayerId = playerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			if (!string.IsNullOrEmpty(this.PlanetId))
			{
				serializer.AddString("planetUid", this.PlanetId);
			}
			return serializer.End().ToString();
		}
	}
}
