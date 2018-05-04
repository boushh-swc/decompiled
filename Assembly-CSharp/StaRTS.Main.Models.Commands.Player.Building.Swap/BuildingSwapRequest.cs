using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Swap
{
	public class BuildingSwapRequest : PlayerIdChecksumRequest
	{
		public string goingToBuildingUid
		{
			get;
			set;
		}

		public string InstanceId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("buildingId", this.InstanceId);
			startedSerializer.AddString("buildingUid", this.goingToBuildingUid);
			return startedSerializer.End().ToString();
		}
	}
}
