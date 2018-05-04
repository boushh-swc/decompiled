using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Building.Contracts.Buyout
{
	public class BuildingContractBuyoutCommand : GameActionCommand<BuildingContractRequest, DefaultResponse>
	{
		public const string ACTION = "player.building.buyout";

		public BuildingContractBuyoutCommand(BuildingContractRequest request) : base("player.building.buyout", request, new DefaultResponse())
		{
		}
	}
}
