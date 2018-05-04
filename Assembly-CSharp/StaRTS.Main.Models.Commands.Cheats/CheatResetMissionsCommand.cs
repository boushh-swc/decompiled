using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatResetMissionsCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.pve.reset";

		public CheatResetMissionsCommand(PlayerIdRequest request) : base("cheats.pve.reset", request, new DefaultResponse())
		{
		}
	}
}
