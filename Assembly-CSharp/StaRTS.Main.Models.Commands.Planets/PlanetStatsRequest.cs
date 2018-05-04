using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Planets
{
	public class PlanetStatsRequest : PlayerIdRequest
	{
		private List<string> planetUIDs;

		public PlanetStatsRequest()
		{
			this.planetUIDs = new List<string>();
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public void AddPlanetID(string planetUID)
		{
			this.planetUIDs.Add(planetUID);
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddArrayOfPrimitives<string>("planets", this.planetUIDs);
			return serializer.End().ToString();
		}
	}
}
