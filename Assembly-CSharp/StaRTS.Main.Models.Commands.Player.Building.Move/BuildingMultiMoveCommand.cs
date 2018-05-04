using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Move
{
	public class BuildingMultiMoveCommand : GameActionCommand<BuildingMultiMoveRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.multimove";

		public BuildingMultiMoveCommand(BuildingMultiMoveRequest request) : base("player.building.multimove", request, new DefaultResponse())
		{
		}
	}
}
