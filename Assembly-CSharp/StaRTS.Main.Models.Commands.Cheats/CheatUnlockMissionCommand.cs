using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Commands.Missions;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatUnlockMissionCommand : GameCommand<MissionIdRequest, DefaultResponse>
	{
		public const string ACTION = "cheats.unlockSingleMission";

		public CheatUnlockMissionCommand(MissionIdRequest request) : base("cheats.unlockSingleMission", request, new DefaultResponse())
		{
		}
	}
}
