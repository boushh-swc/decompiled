using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Player.Account.External
{
	public class GetExternalAccountsCommand : GameCommand<PlayerIdRequest, GetExternalAccountsResponse>
	{
		public const string ACTION = "player.account.external.get";

		public GetExternalAccountsCommand(PlayerIdRequest request) : base("player.account.external.get", request, new GetExternalAccountsResponse())
		{
		}
	}
}
