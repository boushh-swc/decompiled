using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Swap
{
	public class BuildingSwapCommand : GameActionCommand<BuildingSwapRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.swap";

		public BuildingSwapCommand(BuildingSwapRequest request) : base("player.building.swap", request, new DefaultResponse())
		{
		}
	}
}
