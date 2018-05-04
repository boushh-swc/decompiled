using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class BuyTargetedOfferRequest : PlayerIdChecksumRequest
	{
		private string offerUid;

		public BuyTargetedOfferRequest(string offerUid)
		{
			this.offerUid = offerUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("offerUid", this.offerUid);
			return startedSerializer.End().ToString();
		}
	}
}
