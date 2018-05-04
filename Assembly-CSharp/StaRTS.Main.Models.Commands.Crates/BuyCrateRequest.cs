using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class BuyCrateRequest : PlayerIdChecksumRequest
	{
		private string crateId;

		public BuyCrateRequest(string crateId)
		{
			this.crateId = crateId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("crateId", this.crateId);
			return startedSerializer.End().ToString();
		}
	}
}
