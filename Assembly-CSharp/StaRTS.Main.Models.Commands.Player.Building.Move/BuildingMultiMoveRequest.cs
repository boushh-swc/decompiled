using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Move
{
	public class BuildingMultiMoveRequest : PlayerIdChecksumRequest
	{
		public PositionMap PositionMap
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddObject<PositionMap>("positions", this.PositionMap);
			return startedSerializer.End().ToString();
		}
	}
}
