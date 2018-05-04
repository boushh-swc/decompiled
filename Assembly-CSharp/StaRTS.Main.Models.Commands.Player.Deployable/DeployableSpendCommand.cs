using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable
{
	public class DeployableSpendCommand : GameCommand<DeployableSpendRequest, DefaultResponse>
	{
		public const string ACTION = "player.deployable.spend";

		public DeployableSpendCommand(DeployableSpendRequest request) : base("player.deployable.spend", request, new DefaultResponse())
		{
		}
	}
}
