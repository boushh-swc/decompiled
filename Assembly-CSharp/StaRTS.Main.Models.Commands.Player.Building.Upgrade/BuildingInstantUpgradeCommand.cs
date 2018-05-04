using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Upgrade
{
	public class BuildingInstantUpgradeCommand : GameActionCommand<BuildingInstantUpgradeRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.instantUpgrade";

		public BuildingInstantUpgradeCommand(BuildingInstantUpgradeRequest request) : base("player.building.instantUpgrade", request, new DefaultResponse())
		{
		}
	}
}
