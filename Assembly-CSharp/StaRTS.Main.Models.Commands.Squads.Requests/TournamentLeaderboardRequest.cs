using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class TournamentLeaderboardRequest : PlayerIdRequest
	{
		public string PlanetId
		{
			get;
			set;
		}

		public TournamentLeaderboardRequest(string planetId, string playerId)
		{
			this.PlanetId = planetId;
			base.PlayerId = playerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("planetUid", this.PlanetId);
			return serializer.End().ToString();
		}
	}
}
