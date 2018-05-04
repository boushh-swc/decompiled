using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Move
{
	public class BuildingMoveRequest : PlayerIdChecksumRequest
	{
		public string BuildingId
		{
			get;
			set;
		}

		public Position Position
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("buildingId", this.BuildingId);
			startedSerializer.AddObject<Position>("position", this.Position);
			return startedSerializer.End().ToString();
		}
	}
}
