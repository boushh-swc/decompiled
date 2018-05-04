using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Equipment
{
	public class EquipmentIdRequest : PlayerIdChecksumRequest
	{
		public string EquipmentID
		{
			get;
			private set;
		}

		public EquipmentIdRequest(string equipmentID)
		{
			this.EquipmentID = equipmentID;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("equipmentId", this.EquipmentID);
			return startedSerializer.End().ToString();
		}
	}
}
