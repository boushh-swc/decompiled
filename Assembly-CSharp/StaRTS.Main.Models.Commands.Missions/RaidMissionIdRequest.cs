using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Missions
{
	public class RaidMissionIdRequest : PlayerIdChecksumRequest
	{
		public string RaidMissionUid
		{
			get;
			private set;
		}

		public RaidMissionIdRequest(string missionUid)
		{
			this.RaidMissionUid = missionUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("raidMissionId", this.RaidMissionUid);
			return startedSerializer.End().ToString();
		}
	}
}
