using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable
{
	public class DeployableBuyoutContractCommand : GameActionCommand<DeployableContractRequest, DefaultResponse>
	{
		public const string ACTION = "player.deployable.buyout";

		public DeployableBuyoutContractCommand(DeployableContractRequest request) : base("player.deployable.buyout", request, new DefaultResponse())
		{
		}
	}
}
