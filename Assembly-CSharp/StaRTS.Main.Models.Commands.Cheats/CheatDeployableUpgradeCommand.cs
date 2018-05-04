using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatDeployableUpgradeCommand : GameCommand<CheatDeployableUpgradeRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.deployable.upgrade";

		public CheatDeployableUpgradeCommand(CheatDeployableUpgradeRequest request) : base("cheats.deployable.upgrade", request, new DefaultResponse())
		{
		}
	}
}
