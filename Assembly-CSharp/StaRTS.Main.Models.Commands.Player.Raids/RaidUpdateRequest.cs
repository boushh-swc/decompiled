using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidUpdateRequest : PlayerIdChecksumRequest
	{
		private string planetId;

		public RaidUpdateRequest(string planetId)
		{
			this.planetId = planetId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("planetId", this.planetId);
			return startedSerializer.End().ToString();
		}
	}
}
