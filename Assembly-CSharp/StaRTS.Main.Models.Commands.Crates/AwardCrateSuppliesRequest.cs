using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class AwardCrateSuppliesRequest : PlayerIdChecksumRequest
	{
		private string crateUId;

		public AwardCrateSuppliesRequest(string crateUId)
		{
			this.crateUId = crateUId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("crateUid", this.crateUId);
			return startedSerializer.End().ToString();
		}
	}
}
