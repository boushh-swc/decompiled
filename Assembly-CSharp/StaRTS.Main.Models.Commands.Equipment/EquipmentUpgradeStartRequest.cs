using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Equipment
{
	public class EquipmentUpgradeStartRequest : PlayerIdChecksumRequest
	{
		public string BuildingId
		{
			get;
			set;
		}

		public string EquipmentUid
		{
			get;
			set;
		}

		public EquipmentUpgradeStartRequest(string buildingId, string equipmentUid)
		{
			this.BuildingId = buildingId;
			this.EquipmentUid = equipmentUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("buildingId", this.BuildingId);
			startedSerializer.AddString("equipmentUid", this.EquipmentUid);
			return startedSerializer.End().ToString();
		}
	}
}
