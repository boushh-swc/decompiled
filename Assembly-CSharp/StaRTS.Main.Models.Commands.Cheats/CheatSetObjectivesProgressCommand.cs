using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetObjectivesProgressCommand : GameCommand<CheatSetObjectivesProgressRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.objectiveProgress.set";

		public CheatSetObjectivesProgressCommand(CheatSetObjectivesProgressRequest request) : base("cheats.objectiveProgress.set", request, new DefaultResponse())
		{
		}
	}
}
