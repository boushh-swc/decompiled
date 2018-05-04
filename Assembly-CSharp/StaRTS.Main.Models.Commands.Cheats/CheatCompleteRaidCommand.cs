using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Commands.Player.Raids;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatCompleteRaidCommand : GameCommand<PlayerIdRequest, RaidDefenseCompleteResponse>
	{
		public const string ACTION = "cheats.raids.complete";

		public CheatCompleteRaidCommand(PlayerIdRequest request) : base("cheats.raids.complete", request, new RaidDefenseCompleteResponse())
		{
		}
	}
}
