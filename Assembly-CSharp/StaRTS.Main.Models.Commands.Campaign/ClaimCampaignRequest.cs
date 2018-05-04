using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Campaign
{
	public class ClaimCampaignRequest : PlayerIdChecksumRequest
	{
		private string campaignUid;

		private string lastMissionCompletedUid;

		public ClaimCampaignRequest(string campaignUid, string lastMissionCompletedUid)
		{
			this.campaignUid = campaignUid;
			this.lastMissionCompletedUid = lastMissionCompletedUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("campaignUid", this.campaignUid);
			startedSerializer.AddString("missionUid", this.lastMissionCompletedUid);
			return startedSerializer.End().ToString();
		}
	}
}
