using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Player.Building.Contracts;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Upgrade
{
	public class BuildingUpgradeCommand : GameActionCommand<BuildingContractRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.upgrade";

		public BuildingUpgradeCommand(BuildingContractRequest request) : base("player.building.upgrade", request, new DefaultResponse())
		{
		}
	}
}
