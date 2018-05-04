using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public class PlanetIdRequest : PlayerIdRequest
	{
		public string PlanetId
		{
			get;
			set;
		}

		public PlanetIdRequest(string planetId)
		{
			this.PlanetId = planetId;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
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
