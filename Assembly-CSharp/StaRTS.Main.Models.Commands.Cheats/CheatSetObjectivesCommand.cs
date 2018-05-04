using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetObjectivesCommand : GameCommand<CheatSetObjectivesRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.objectives.set";

		public CheatSetObjectivesCommand(CheatSetObjectivesRequest request) : base("cheats.objectives.set", request, new DefaultResponse())
		{
		}
	}
}
