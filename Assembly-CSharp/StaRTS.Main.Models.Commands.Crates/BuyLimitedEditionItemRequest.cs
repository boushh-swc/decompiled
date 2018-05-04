using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class BuyLimitedEditionItemRequest : PlayerIdChecksumRequest
	{
		private string leiUid;

		public BuyLimitedEditionItemRequest(string leiUid)
		{
			this.leiUid = leiUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("leUid", this.leiUid);
			return startedSerializer.End().ToString();
		}
	}
}
