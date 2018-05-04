using StaRTS.Externals.Manimal.TransferObjects.Response;
using System;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class ReserveTargetedOfferCommand : GameCommand<ReserveTargetedOfferIDRequest, DefaultResponse>
	{
		public const string ACTION = "player.store.offers.reserve";

		public ReserveTargetedOfferCommand(ReserveTargetedOfferIDRequest request) : base("player.store.offers.reserve", request, new DefaultResponse())
		{
		}
	}
}
