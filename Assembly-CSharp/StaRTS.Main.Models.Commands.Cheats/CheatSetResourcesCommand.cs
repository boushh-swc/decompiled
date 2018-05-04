using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetResourcesCommand : GameCommand<CheatSetResourcesRequest, DefaultResponse>
	{
		private const string ACTION = "cheats.resources.set";

		public CheatSetResourcesCommand(CheatSetResourcesRequest request) : base("cheats.resources.set", request, new DefaultResponse())
		{
		}
	}
}
