using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatPointsRequest : PlayerIdRequest
	{
		private int amount;

		private string campaignUid;

		public CheatPointsRequest(int points)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.amount = points;
			this.campaignUid = null;
		}

		public CheatPointsRequest(int points, string campaignUid) : this(points)
		{
			this.campaignUid = campaignUid;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<int>("amount", this.amount);
			if (this.campaignUid != null)
			{
				serializer.AddString("campaignUid", this.campaignUid);
			}
			return serializer.End().ToString();
		}
	}
}
