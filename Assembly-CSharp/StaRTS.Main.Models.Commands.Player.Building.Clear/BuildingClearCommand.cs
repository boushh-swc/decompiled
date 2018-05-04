using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Clear
{
	public class BuildingClearCommand : GameActionCommand<BuildingClearRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.clear";

		public BuildingClearCommand(BuildingClearRequest request) : base("player.building.clear", request, new DefaultResponse())
		{
		}
	}
}
