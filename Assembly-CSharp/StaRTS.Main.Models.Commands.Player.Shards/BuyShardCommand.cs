using System;

namespace StaRTS.Main.Models.Commands.Player.Shards
{
	public class BuyShardCommand : GameCommand<BuyShardRequest, GetShardOfferingResponse>
	{
		public const string ACTION = "player.store.shard.offers.buy";

		public BuyShardCommand(BuyShardRequest request) : base("player.store.shard.offers.buy", request, new GetShardOfferingResponse())
		{
		}
	}
}
