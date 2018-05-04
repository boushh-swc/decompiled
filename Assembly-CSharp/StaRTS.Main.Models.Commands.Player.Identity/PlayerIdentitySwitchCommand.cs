using System;

namespace StaRTS.Main.Models.Commands.Player.Identity
{
	public class PlayerIdentitySwitchCommand : GameActionCommand<PlayerIdentityRequest, PlayerIdentitySwitchResponse>
	{
		public const string ACTION = "player.identity.switch";

		public PlayerIdentitySwitchCommand(PlayerIdentityRequest request) : base("player.identity.switch", request, new PlayerIdentitySwitchResponse())
		{
		}
	}
}
