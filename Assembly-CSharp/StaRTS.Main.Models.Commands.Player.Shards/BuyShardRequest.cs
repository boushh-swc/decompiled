using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Shards
{
	public class BuyShardRequest : PlayerIdChecksumRequest
	{
		private string poolId;

		private int quantity;

		public BuyShardRequest(string poolId, int quantity)
		{
			this.poolId = poolId;
			this.quantity = quantity;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("poolSlotId", this.poolId);
			startedSerializer.Add<int>("count", this.quantity);
			return startedSerializer.End().ToString();
		}
	}
}
