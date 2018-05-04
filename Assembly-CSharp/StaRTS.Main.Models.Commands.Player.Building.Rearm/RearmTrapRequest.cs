using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Building.Rearm
{
	public class RearmTrapRequest : PlayerIdChecksumRequest
	{
		public List<string> BuildingIds
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddArrayOfPrimitives<string>("buildingIds", this.BuildingIds);
			return startedSerializer.End().ToString();
		}
	}
}
