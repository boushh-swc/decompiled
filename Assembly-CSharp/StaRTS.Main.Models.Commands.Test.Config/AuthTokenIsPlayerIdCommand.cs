using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Test.Config
{
	public class AuthTokenIsPlayerIdCommand : GameCommand<DefaultRequest, AuthTokenIsPlayerIdResponse>
	{
		public const string ACTION = "test.authtoken.isPlayerId";

		public AuthTokenIsPlayerIdCommand(AuthTokenIsPlayerIdResponse response) : base("test.authtoken.isPlayerId", new DefaultRequest(), response)
		{
		}
	}
}
