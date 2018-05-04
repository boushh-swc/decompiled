using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class UnregisterExternalAccountCommand : GameCommand<UnregisterExternalAccountRequest, DefaultResponse>
	{
		public const string ACTION = "player.account.external.unregister";

		public UnregisterExternalAccountCommand(UnregisterExternalAccountRequest request) : base("player.account.external.unregister", request, new DefaultResponse())
		{
		}
	}
}
