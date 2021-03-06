using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads
{
	public class SquadDonatedTroop
	{
		public string TroopUid
		{
			get;
			private set;
		}

		public Dictionary<string, int> SenderAmounts
		{
			get;
			private set;
		}

		public SquadDonatedTroop(string troopUid)
		{
			this.TroopUid = troopUid;
			this.SenderAmounts = new Dictionary<string, int>();
		}

		public void AddSenderAmount(string senderId, int amount)
		{
			if (this.SenderAmounts.ContainsKey(senderId))
			{
				Dictionary<string, int> senderAmounts;
				(senderAmounts = this.SenderAmounts)[senderId] = senderAmounts[senderId] + amount;
			}
			else
			{
				this.SenderAmounts.Add(senderId, amount);
			}
		}

		public int GetTotalAmount()
		{
			int num = 0;
			foreach (KeyValuePair<string, int> current in this.SenderAmounts)
			{
				num += current.Value;
			}
			return num;
		}
	}
}
