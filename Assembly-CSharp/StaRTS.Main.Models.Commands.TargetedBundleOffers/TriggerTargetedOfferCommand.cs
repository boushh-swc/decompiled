using System;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class TriggerTargetedOfferCommand : GameCommand<TargetedOfferIDRequest, TriggerTargetedOfferResponse>
	{
		public const string ACTION = "player.store.offers.trigger";

		public TriggerTargetedOfferCommand(TargetedOfferIDRequest request) : base("player.store.offers.trigger", request, new TriggerTargetedOfferResponse())
		{
		}
	}
}
