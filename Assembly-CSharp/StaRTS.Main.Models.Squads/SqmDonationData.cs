using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads
{
	public class SqmDonationData
	{
		public string RequestId;

		public string RecipientId;

		public Dictionary<string, int> Donations;

		public int DonationCount;
	}
}
