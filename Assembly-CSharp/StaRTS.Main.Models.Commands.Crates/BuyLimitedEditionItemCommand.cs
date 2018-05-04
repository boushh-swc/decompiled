using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class BuyLimitedEditionItemCommand : GameActionCommand<BuyLimitedEditionItemRequest, CrateDataResponse>
	{
		public const string ACTION = "player.store.crate.buyLE";

		public BuyLimitedEditionItemCommand(BuyLimitedEditionItemRequest request) : base("player.store.crate.buyLE", request, new CrateDataResponse())
		{
		}
	}
}
