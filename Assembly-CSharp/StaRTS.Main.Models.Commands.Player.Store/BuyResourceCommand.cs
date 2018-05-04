using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Store
{
	public class BuyResourceCommand : GameActionCommand<PlayerIdChecksumRequest, DefaultResponse>
	{
		public const string ACTION = "player.store.buy";

		public BuyResourceCommand(BuyResourceRequest request) : base("player.store.buy", request, new DefaultResponse())
		{
		}
	}
}
