using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSquadWarTurnsCommand : GameCommand<CheatSquadWarTurnsRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.guildWar.setTurns";

		public CheatSquadWarTurnsCommand(CheatSquadWarTurnsRequest request) : base("cheats.guildWar.setTurns", request, new DefaultResponse())
		{
		}
	}
}
