using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable
{
	public class DeployableCancelContractCommand : GameActionCommand<DeployableContractRequest, DefaultResponse>
	{
		public const string ACTION = "player.deployable.cancel";

		public DeployableCancelContractCommand(DeployableContractRequest request) : base("player.deployable.cancel", request, new DefaultResponse())
		{
		}
	}
}
