using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class TargetedOfferSummary
	{
		public string AvailableOffer;

		public uint NextOfferAvailableAt;

		public uint GlobalCooldownExpiresAt;

		public void FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("availableOffer"))
			{
				this.AvailableOffer = (dictionary["availableOffer"] as string);
			}
			if (dictionary.ContainsKey("nextOfferAvailableAt"))
			{
				this.NextOfferAvailableAt = Convert.ToUInt32(dictionary["nextOfferAvailableAt"] as string);
			}
			if (dictionary.ContainsKey("globalCooldownExpiresAt"))
			{
				this.GlobalCooldownExpiresAt = Convert.ToUInt32(dictionary["globalCooldownExpiresAt"] as string);
			}
		}
	}
}
