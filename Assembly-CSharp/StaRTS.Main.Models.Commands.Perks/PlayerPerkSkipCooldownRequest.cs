using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkSkipCooldownRequest : PlayerIdChecksumRequest
	{
		private string perkId;

		public PlayerPerkSkipCooldownRequest(string perkId)
		{
			this.perkId = perkId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("perkId", this.perkId);
			return startedSerializer.End().ToString();
		}
	}
}
