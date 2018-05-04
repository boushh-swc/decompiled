using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public class PvpGetNextTargetRequest : PlayerIdRequest
	{
		public string planetId;

		public PvpGetNextTargetRequest()
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.planetId = Service.CurrentPlayer.PlanetId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("planetId", this.planetId);
			return serializer.End().ToString();
		}
	}
}
