using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatResetRaidCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.raids.reset";

		public CheatResetRaidCommand(PlayerIdRequest request) : base("cheats.raids.reset", request, new DefaultResponse())
		{
		}
	}
}
