using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable.Upgrade.Start
{
	public class DeployableUpgradeStartRequest : PlayerIdChecksumRequest
	{
		public string BuildingId
		{
			get;
			set;
		}

		public string TroopUid
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("playerId", base.PlayerId);
			startedSerializer.AddString("buildingId", this.BuildingId);
			startedSerializer.AddString("troopUid", this.TroopUid);
			return startedSerializer.End().ToString();
		}
	}
}
