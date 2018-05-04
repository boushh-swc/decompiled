using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSquadWarTimeTravelCommand : GameCommand<CheatSquadWarTimeTravelRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.guildWar.timeTravel";

		public CheatSquadWarTimeTravelCommand(CheatSquadWarTimeTravelRequest request) : base("cheats.guildWar.timeTravel", request, new DefaultResponse())
		{
		}
	}
}
