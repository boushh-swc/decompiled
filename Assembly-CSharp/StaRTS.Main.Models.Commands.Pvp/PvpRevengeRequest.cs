using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpRevengeRequest : PlayerIdChecksumRequest
	{
		public string OpponentId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("opponentId", this.OpponentId);
			return startedSerializer.End().ToString();
		}
	}
}
