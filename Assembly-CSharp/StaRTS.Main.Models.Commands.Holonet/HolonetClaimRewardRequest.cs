using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Holonet
{
	public class HolonetClaimRewardRequest : PlayerIdChecksumRequest
	{
		private string limitedTimeRewardUid;

		private string contextUid;

		public HolonetClaimRewardRequest(string limitedTimeRewardUid, string contextUid)
		{
			this.limitedTimeRewardUid = limitedTimeRewardUid;
			this.contextUid = contextUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("uid", this.limitedTimeRewardUid);
			startedSerializer.AddString("rewardContext", this.contextUid);
			return startedSerializer.End().ToString();
		}
	}
}
