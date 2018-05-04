using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class TroopDonateResponse : AbstractResponse
	{
		public Dictionary<string, int> TroopsDonated
		{
			get;
			private set;
		}

		public int DonationCount
		{
			get;
			private set;
		}

		public int LastTrackedDonationTime
		{
			get;
			private set;
		}

		public int DonationCooldownEndTime
		{
			get;
			private set;
		}

		public bool ReputationAwarded
		{
			get;
			private set;
		}

		public TroopDonateResponse()
		{
			this.TroopsDonated = new Dictionary<string, int>();
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("troopsDonated"))
			{
				Dictionary<string, object> dictionary2 = dictionary["troopsDonated"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						this.TroopsDonated.Add(current.Key, Convert.ToInt32(current.Value));
					}
				}
			}
			if (dictionary.ContainsKey("troopDonationProgress"))
			{
				Dictionary<string, object> dictionary3 = dictionary["troopDonationProgress"] as Dictionary<string, object>;
				if (dictionary3 != null)
				{
					if (dictionary3.ContainsKey("donationCount"))
					{
						this.DonationCount = Convert.ToInt32(dictionary3["donationCount"]);
					}
					if (dictionary3.ContainsKey("lastTrackedDonationTime"))
					{
						this.LastTrackedDonationTime = Convert.ToInt32(dictionary3["lastTrackedDonationTime"]);
					}
					if (dictionary3.ContainsKey("repDonationCooldownEndTime"))
					{
						this.DonationCooldownEndTime = Convert.ToInt32(dictionary3["repDonationCooldownEndTime"]);
					}
				}
			}
			if (dictionary.ContainsKey("reputationAwarded"))
			{
				this.ReputationAwarded = (bool)dictionary["reputationAwarded"];
			}
			return this;
		}
	}
}
