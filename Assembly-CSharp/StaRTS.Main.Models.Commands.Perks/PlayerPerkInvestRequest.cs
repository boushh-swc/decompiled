using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkInvestRequest : PlayerIdChecksumRequest
	{
		private string perkId;

		private int repToInvest;

		public PlayerPerkInvestRequest(string perkId, int amount)
		{
			this.perkId = perkId;
			this.repToInvest = amount;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("perkId", this.perkId);
			startedSerializer.AddString("repToInvest", this.repToInvest.ToString());
			return startedSerializer.End().ToString();
		}
	}
}
