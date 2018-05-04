using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Contracts
{
	public class BuildingUpgradeAllWallsRequest : PlayerIdChecksumRequest
	{
		public string BuildingUid
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("buildingUid", this.BuildingUid);
			return startedSerializer.End().ToString();
		}
	}
}
