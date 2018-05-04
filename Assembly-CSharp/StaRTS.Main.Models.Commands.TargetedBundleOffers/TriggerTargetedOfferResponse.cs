using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class TriggerTargetedOfferResponse : AbstractResponse
	{
		public string OfferId
		{
			get;
			private set;
		}

		public uint TriggerDate
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("offerUid"))
			{
				this.OfferId = Convert.ToString(dictionary["offerUid"]);
			}
			if (dictionary.ContainsKey("triggerDate"))
			{
				this.TriggerDate = Convert.ToUInt32(dictionary["triggerDate"]);
			}
			return this;
		}
	}
}
