using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class AwardCrateSuppliesCommand : GameActionCommand<AwardCrateSuppliesRequest, AwardCrateSuppliesResponse>
	{
		public const string ACTION = "player.crate.award";

		public AwardCrateSuppliesCommand(AwardCrateSuppliesRequest request) : base("player.crate.award", request, new AwardCrateSuppliesResponse())
		{
		}
	}
}
