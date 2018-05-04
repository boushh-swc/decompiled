using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class SimulateWarMatchMakingCommand : GameCommand<SimulateWarMatchMakingRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.guildWar.getTarget";

		public SimulateWarMatchMakingCommand(SimulateWarMatchMakingRequest request) : base("cheats.guildWar.getTarget", request, new DefaultResponse())
		{
		}
	}
}
