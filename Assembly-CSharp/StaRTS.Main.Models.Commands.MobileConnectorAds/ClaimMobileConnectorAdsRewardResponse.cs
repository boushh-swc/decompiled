using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.MobileConnectorAds
{
	public class ClaimMobileConnectorAdsRewardResponse : AbstractResponse
	{
		public CrateData CrateDataTO
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("crateData"))
			{
				object obj2 = dictionary["crateData"];
				this.CrateDataTO = new CrateData();
				this.CrateDataTO.FromObject(obj2);
			}
			if (dictionary.ContainsKey("mcaInfo"))
			{
				object rawMCAInfo = dictionary["mcaInfo"];
				currentPlayer.UpdateMobileConnectorAdsInfo(rawMCAInfo);
			}
			return this;
		}
	}
}
