using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Upgrade
{
	public class BuildingUpgradeAllWallsCommand : GameActionCommand<BuildingUpgradeAllWallsRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.upgradeAll";

		public BuildingUpgradeAllWallsCommand(BuildingUpgradeAllWallsRequest request) : base("player.building.upgradeAll", request, new DefaultResponse())
		{
		}
	}
}
