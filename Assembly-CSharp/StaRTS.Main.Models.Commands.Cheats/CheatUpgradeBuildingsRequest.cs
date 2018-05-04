using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatUpgradeBuildingsRequest : PlayerIdRequest
	{
		private List<string> buildingIds;

		public CheatUpgradeBuildingsRequest(List<string> buildingIds)
		{
			this.buildingIds = buildingIds;
			base.PlayerId = Service.CurrentPlayer.PlayerId;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddArrayOfPrimitives<string>("buildingIds", this.buildingIds);
			return serializer.End().ToString();
		}
	}
}
