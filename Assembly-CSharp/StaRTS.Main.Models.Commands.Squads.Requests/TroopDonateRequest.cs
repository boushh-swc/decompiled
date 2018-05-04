using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class TroopDonateRequest : PlayerIdRequest
	{
		public Dictionary<string, int> Donations
		{
			get;
			set;
		}

		public string RecipientId
		{
			get;
			set;
		}

		public string RequestId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("recipientId", this.RecipientId);
			serializer.AddString("requestId", this.RequestId);
			serializer.AddDictionary<int>("troopsDonated", this.Donations);
			return serializer.End().ToString();
		}
	}
}
