using StaRTS.Main.Models.Commands.Equipment;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetEquipmentRequest : EquipmentIdRequest
	{
		private int level;

		public CheatSetEquipmentRequest(string equipmentID, int level) : base(equipmentID)
		{
			this.level = level;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("equipmentId", base.EquipmentID);
			startedSerializer.Add<int>("equipmentLevel", this.level);
			return startedSerializer.End().ToString();
		}
	}
}
