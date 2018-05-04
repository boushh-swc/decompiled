using System;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidDefenseStartCommand : GameActionCommand<RaidDefenseStartRequest, RaidDefenseStartResponse>
	{
		public const string ACTION = "player.raids.start";

		public RaidDefenseStartCommand(RaidDefenseStartRequest request) : base("player.raids.start", request, new RaidDefenseStartResponse())
		{
		}
	}
}
