using StaRTS.Externals.Manimal.TransferObjects.Request;
using System;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class GetTargetedOffersCommand : GameCommand<PlayerIdRequest, GetTargetedOffersResponse>
	{
		public const string ACTION = "player.store.offers.get";

		public GetTargetedOffersCommand(PlayerIdRequest request) : base("player.store.offers.get", request, new GetTargetedOffersResponse())
		{
		}
	}
}
