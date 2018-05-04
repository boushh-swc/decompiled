using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class RecoverExternalAccountCommand : GameCommand<PlayerIdRequest, DefaultResponse>
	{
		public const string ACTION = "player.account.recover";

		public RecoverExternalAccountCommand(PlayerIdRequest request) : base("player.account.recover", request, new DefaultResponse())
		{
		}
	}
}
