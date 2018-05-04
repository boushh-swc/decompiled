using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkActivateRequest : PlayerIdChecksumRequest
	{
		private string perkId;

		public PlayerPerkActivateRequest(string perkId)
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
