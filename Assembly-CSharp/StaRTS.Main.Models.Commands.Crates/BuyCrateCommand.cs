using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class BuyCrateCommand : GameActionCommand<BuyCrateRequest, CrateDataResponse>
	{
		public const string ACTION = "player.store.crate.buy";

		public BuyCrateCommand(BuyCrateRequest request) : base("player.store.crate.buy", request, new CrateDataResponse())
		{
		}
	}
}
