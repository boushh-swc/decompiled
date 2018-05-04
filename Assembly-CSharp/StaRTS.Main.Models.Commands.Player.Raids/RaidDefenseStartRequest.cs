using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidDefenseStartRequest : PlayerIdChecksumRequest
	{
		private string planetId;

		private string raidMissionId;

		public RaidDefenseStartRequest(string planetId, string raidMissionId)
		{
			this.planetId = planetId;
			this.raidMissionId = raidMissionId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("planetId", this.planetId);
			startedSerializer.AddString("raidMissionId", this.raidMissionId);
			return startedSerializer.End().ToString();
		}
	}
}
