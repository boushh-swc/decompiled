using System;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidDefenseCompleteCommand : GameActionCommand<RaidDefenseCompleteRequest, RaidDefenseCompleteResponse>
	{
		public const string ACTION = "player.raids.complete";

		public RaidDefenseCompleteCommand(RaidDefenseCompleteRequest request) : base("player.raids.complete", request, new RaidDefenseCompleteResponse())
		{
		}
	}
}
