using System;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidUpdateCommand : GameCommand<RaidUpdateRequest, RaidUpdateResponse>
	{
		public const string ACTION = "player.raids.update";

		public RaidUpdateCommand(RaidUpdateRequest request) : base("player.raids.update", request, new RaidUpdateResponse())
		{
		}
	}
}
