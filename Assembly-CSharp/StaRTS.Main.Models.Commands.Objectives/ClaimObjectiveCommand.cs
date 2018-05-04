using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Crates;
using System;

namespace StaRTS.Main.Models.Commands.Objectives
{
	public class ClaimObjectiveCommand : GameCommand<PlayerIdRequest, CrateDataResponse>
	{
		public const string ACTION = "player.objective.claim";

		public ClaimObjectiveCommand(PlayerIdRequest request) : base("player.objective.claim", request, new CrateDataResponse())
		{
		}
	}
}
