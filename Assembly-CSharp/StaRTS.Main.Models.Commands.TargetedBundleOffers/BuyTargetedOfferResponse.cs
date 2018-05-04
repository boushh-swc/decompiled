using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class BuyTargetedOfferResponse : AbstractResponse
	{
		public string offerUID;

		public TargetedOfferSummary TargetedOffers;

		public CrateData CrateDataTO
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.TargetedOffers = new TargetedOfferSummary();
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("targetedOffer"))
			{
				this.offerUID = (string)dictionary["targetedOffer"];
			}
			if (dictionary.ContainsKey("targetedOfferResult"))
			{
				this.TargetedOffers.FromObject(dictionary["targetedOfferResult"]);
			}
			if (dictionary.ContainsKey("crateData"))
			{
				this.CrateDataTO = new CrateData();
				this.CrateDataTO.FromObject(dictionary["crateData"]);
			}
			return this;
		}
	}
}
