using System;

namespace StaRTS.Main.Models.Squads
{
	public class SqmRequestData
	{
		public string Text;

		public bool PayToSkip;

		public int StartingAvailableCapacity;

		public int TotalCapacity;

		public int TroopDonationLimit;

		public int CurrentDonationSize;

		public int CurrentPlayerDonationCount;

		public int ResendCrystalCost;

		public string WarId;

		public bool IsWarRequest
		{
			get
			{
				return !string.IsNullOrEmpty(this.WarId);
			}
		}
	}
}
