using StaRTS.Main.Models.Commands.Squads.Responses;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatStartWarCommand : GameCommand<CheatStartWarRequest, GetSquadWarStatusResponse>
	{
		public const string ACTION = "cheats.guildWar.start";

		public CheatStartWarCommand(CheatStartWarRequest request) : base("cheats.guildWar.start", request, new GetSquadWarStatusResponse())
		{
		}
	}
}
