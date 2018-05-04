using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.TargetedBundleOffers
{
	public class ReserveTargetedOfferIDRequest : PlayerIdRequest
	{
		public string OfferId
		{
			get;
			set;
		}

		public string ProductId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("offerUid", this.OfferId);
			serializer.AddString("offerProductId", this.ProductId);
			return serializer.End().ToString();
		}
	}
}
