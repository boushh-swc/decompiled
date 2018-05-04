using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Player.Deployable;
using System;

namespace StaRTS.Main.Models.Commands.Squads
{
	public class SquadWarSpendDeployablesCommand : SquadGameCommand<DeployableSpendRequest, DefaultResponse>
	{
		public const string ACTION = "guild.war.deployable.spend";

		public SquadWarSpendDeployablesCommand(DeployableSpendRequest request) : base("guild.war.deployable.spend", request, new DefaultResponse())
		{
		}
	}
}
