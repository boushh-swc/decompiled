using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.SpecOps
{
	public class InventoryTransferRequest : PlayerIdChecksumRequest
	{
		private string uid;

		private int amount;

		public InventoryTransferRequest(string uid, int amount)
		{
			this.uid = uid;
			this.amount = amount;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("uid", this.uid);
			startedSerializer.Add<int>("count", this.amount);
			return startedSerializer.End().ToString();
		}
	}
}
