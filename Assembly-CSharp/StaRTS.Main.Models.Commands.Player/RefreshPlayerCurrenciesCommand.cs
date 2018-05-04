using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.Player
{
	public class RefreshPlayerCurrenciesCommand : GameCommand<PlayerIdRequest, ExternalCurrencySyncResponse>
	{
		public const string ACTION = "player.get";

		public RefreshPlayerCurrenciesCommand(PlayerIdRequest request) : base("player.get", request, new ExternalCurrencySyncResponse())
		{
		}
	}
}
