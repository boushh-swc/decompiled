using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.MobileConnectorAds
{
	public class ClaimMobileConnectorAdsRewardRequest : PlayerIdRequest
	{
		private string adUnitID;

		public ClaimMobileConnectorAdsRewardRequest(string adUnitID)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.adUnitID = adUnitID;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.AddString("auid", this.adUnitID);
			return serializer.End().ToString();
		}
	}
}
