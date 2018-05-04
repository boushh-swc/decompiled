using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatUnlockAllMissionsCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.unlockAllMissions";

		public CheatUnlockAllMissionsCommand(PlayerIdRequest request) : base("cheats.unlockAllMissions", request, new DefaultResponse())
		{
		}
	}
}
