using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Contracts.Cancel
{
	public class BuildingContractCancelCommand : GameActionCommand<BuildingContractRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.cancel";

		public BuildingContractCancelCommand(BuildingContractRequest request) : base("player.building.cancel", request, new DefaultResponse())
		{
		}
	}
}
