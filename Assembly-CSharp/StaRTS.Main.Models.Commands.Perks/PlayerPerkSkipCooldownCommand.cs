using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkSkipCooldownCommand : GameActionCommand<PlayerPerkSkipCooldownRequest, PlayerPerkSkipCooldownResponse>
	{
		public const string ACTION = "player.perks.skipCooldown";

		public PlayerPerkSkipCooldownCommand(PlayerPerkSkipCooldownRequest request) : base("player.perks.skipCooldown", request, new PlayerPerkSkipCooldownResponse())
		{
		}
	}
}
