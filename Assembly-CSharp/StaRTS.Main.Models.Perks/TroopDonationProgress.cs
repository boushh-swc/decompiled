using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Perks
{
	public class TroopDonationProgress : ISerializable
	{
		public int DonationCount
		{
			get;
			set;
		}

		public int LastTrackedDonationTime
		{
			get;
			set;
		}

		public int DonationCooldownEndTime
		{
			get;
			set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("donationCount"))
			{
				this.DonationCount = Convert.ToInt32(dictionary["donationCount"]);
			}
			if (dictionary.ContainsKey("lastTrackedDonationTime"))
			{
				this.LastTrackedDonationTime = Convert.ToInt32(dictionary["lastTrackedDonationTime"]);
			}
			if (dictionary.ContainsKey("repDonationCooldownEndTime"))
			{
				this.DonationCooldownEndTime = Convert.ToInt32(dictionary["repDonationCooldownEndTime"]);
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize TroopDonationInfo");
			return string.Empty;
		}
	}
}
