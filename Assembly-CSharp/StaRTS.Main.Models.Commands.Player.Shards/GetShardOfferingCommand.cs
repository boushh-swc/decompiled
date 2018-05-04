using System;

namespace StaRTS.Main.Models.Commands.Player.Shards
{
	public class GetShardOfferingCommand : GameCommand<GetShardOfferingRequest, GetShardOfferingResponse>
	{
		public const string ACTION = "player.store.shard.offers.get";

		public GetShardOfferingCommand(GetShardOfferingRequest request) : base("player.store.shard.offers.get", request, new GetShardOfferingResponse())
		{
		}
	}
}
