using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Construct
{
	public class BuildingConstructCommand : GameActionCommand<BuildingConstructRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.construct";

		public BuildingConstructCommand(BuildingConstructRequest request) : base("player.building.construct", request, new DefaultResponse())
		{
		}
	}
}
