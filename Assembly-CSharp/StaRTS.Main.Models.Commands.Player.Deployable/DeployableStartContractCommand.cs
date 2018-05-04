using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable
{
	public class DeployableStartContractCommand : GameActionCommand<DeployableContractRequest, DefaultResponse>
	{
		public const string ACTION = "player.deployable.train";

		public DeployableStartContractCommand(DeployableContractRequest request) : base("player.deployable.train", request, new DefaultResponse())
		{
		}
	}
}
