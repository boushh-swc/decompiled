using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Move
{
	public class BuildingMoveCommand : GameActionCommand<BuildingMoveRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.move";

		public BuildingMoveCommand(BuildingMoveRequest request) : base("player.building.move", request, new DefaultResponse())
		{
		}
	}
}
