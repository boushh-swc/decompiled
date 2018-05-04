using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.Player.Store
{
	public class BuyMultiResourceCommand : GameActionCommand<BuyMultiResourceRequest, DefaultResponse>
	{
		public const string ACTION = "player.store.multibuy";

		public BuyMultiResourceCommand(BuyMultiResourceRequest request) : base("player.store.multibuy", request, new DefaultResponse())
		{
		}
	}
}
