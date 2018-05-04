using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Planets
{
	public class RelocatePlanetRequest : PlayerIdChecksumRequest
	{
		private string planetUID;

		private bool payWithHardCurrency;

		public RelocatePlanetRequest(string planetUID, bool pay)
		{
			this.planetUID = planetUID;
			this.payWithHardCurrency = pay;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("playerId", base.PlayerId);
			startedSerializer.AddString("planet", this.planetUID);
			startedSerializer.AddBool("payWithHardCurrency", this.payWithHardCurrency);
			return startedSerializer.End().ToString();
		}
	}
}
