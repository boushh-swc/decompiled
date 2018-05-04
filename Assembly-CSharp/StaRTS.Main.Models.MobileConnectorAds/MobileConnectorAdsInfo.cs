using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.MobileConnectorAds
{
	public class MobileConnectorAdsInfo : ISerializable
	{
		public int count;

		public DateTime nextAvailableDate;

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("count"))
			{
				this.count = Convert.ToInt32(dictionary["count"]);
			}
			if (dictionary.ContainsKey("nextAvailableDate"))
			{
				uint seconds = Convert.ToUInt32(dictionary["nextAvailableDate"]);
				this.nextAvailableDate = DateUtils.DateFromSeconds(seconds);
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize MobileConnectorAdsInfo");
			return string.Empty;
		}
	}
}
