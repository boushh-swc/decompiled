using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class GetTargetedOffersResponse : AbstractResponse
	{
		public TargetedOfferSummary TargetedOffers;

		public List<TargetedBundleVO> TargetedBundleVOs
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.TargetedOffers = new TargetedOfferSummary();
			this.TargetedOffers.FromObject(obj);
			return this;
		}
	}
}
