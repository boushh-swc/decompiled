using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class LoginCommand : GameCommand<LoginRequest, LoginResponse>
	{
		public const string ACTION = "player.login";

		public LoginCommand(LoginRequest request) : base("player.login", request, new LoginResponse())
		{
		}
	}
}
