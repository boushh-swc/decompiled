using StaRTS.Main.Models.Commands.Equipment;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatEarnEquipmentShardRequest : EquipmentIdRequest
	{
		public int ShardsToInvest
		{
			get;
			private set;
		}

		public CheatEarnEquipmentShardRequest(string equipmentID, int shardsToInvest) : base(equipmentID)
		{
			this.ShardsToInvest = shardsToInvest;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("equipmentId", base.EquipmentID);
			startedSerializer.Add<int>("shardsToInvest", this.ShardsToInvest);
			return startedSerializer.End().ToString();
		}
	}
}
