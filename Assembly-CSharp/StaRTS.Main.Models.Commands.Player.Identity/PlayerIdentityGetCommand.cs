using System;

namespace StaRTS.Main.Models.Commands.Player.Identity
{
	public class PlayerIdentityGetCommand : GameActionCommand<PlayerIdentityRequest, PlayerIdentityGetResponse>
	{
		public const string ACTION = "player.identity.get";

		public PlayerIdentityGetCommand(PlayerIdentityRequest request) : base("player.identity.get", request, new PlayerIdentityGetResponse())
		{
		}
	}
}
