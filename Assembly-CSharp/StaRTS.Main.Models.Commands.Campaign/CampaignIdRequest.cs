using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Campaign
{
	public class CampaignIdRequest : PlayerIdChecksumRequest
	{
		private string campaignUid;

		public CampaignIdRequest(string campaignUid)
		{
			this.campaignUid = campaignUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("campaignUid", this.campaignUid);
			return startedSerializer.End().ToString();
		}
	}
}
