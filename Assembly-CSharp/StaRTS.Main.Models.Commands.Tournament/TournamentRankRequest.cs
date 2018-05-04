using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Tournament
{
	public class TournamentRankRequest : PlayerIdRequest
	{
		public string PlanetId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("planetId", this.PlanetId);
			return serializer.End().ToString();
		}
	}
}
