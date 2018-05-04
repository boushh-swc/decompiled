using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Move
{
	public class WarBaseSaveRequest : PlayerIdRequest
	{
		public PositionMap PositionMap
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddObject<PositionMap>("positions", this.PositionMap);
			return serializer.End().ToString();
		}
	}
}
