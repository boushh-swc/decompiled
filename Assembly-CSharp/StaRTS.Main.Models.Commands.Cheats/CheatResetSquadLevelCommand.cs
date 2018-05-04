using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatResetSquadLevelCommand : GameCommand<CheatResetSquadLevelRequest, DefaultResponse>
	{
		private const string ACTION = "cheats.player.resetGuildAdv";

		public CheatResetSquadLevelCommand(CheatResetSquadLevelRequest request) : base("cheats.player.resetGuildAdv", request, new DefaultResponse())
		{
		}
	}
}
