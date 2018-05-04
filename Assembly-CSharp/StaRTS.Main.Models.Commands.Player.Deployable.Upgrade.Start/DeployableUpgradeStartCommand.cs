using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Deployable.Upgrade.Start
{
	public class DeployableUpgradeStartCommand : GameActionCommand<DeployableUpgradeStartRequest, DefaultResponse>
	{
		public const string ACTION = "player.deployable.upgrade.start";

		public DeployableUpgradeStartCommand(DeployableUpgradeStartRequest request) : base("player.deployable.upgrade.start", request, new DefaultResponse())
		{
		}
	}
}
