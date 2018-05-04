using System;

namespace StaRTS.Main.Models
{
	public class RewardabilityResult
	{
		public bool CanAward
		{
			get;
			set;
		}

		public string Reason
		{
			get;
			set;
		}

		public RewardabilityResult()
		{
			this.CanAward = true;
		}
	}
}
