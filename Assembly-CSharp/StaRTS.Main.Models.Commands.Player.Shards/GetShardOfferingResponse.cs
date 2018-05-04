using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player.Shards
{
	public class GetShardOfferingResponse : AbstractResponse
	{
		public ShardShopData ShopData;

		public override ISerializable FromObject(object obj)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.ShopData = new ShardShopData();
			if (dictionary.ContainsKey("activeSeriesId"))
			{
				string text = dictionary["activeSeriesId"] as string;
				if (string.IsNullOrEmpty(text))
				{
					Service.Logger.Error("Shard Shop Active Series not found!");
					return this;
				}
				this.ShopData.ActiveSeries = staticDataController.Get<ShardShopSeriesVO>(text);
			}
			if (dictionary.ContainsKey("shardOffers"))
			{
				Dictionary<string, object> dictionary2 = dictionary["shardOffers"] as Dictionary<string, object>;
				foreach (string current in dictionary2.Keys)
				{
					CrateSupplyVO value = staticDataController.Get<CrateSupplyVO>(dictionary2[current] as string);
					this.ShopData.ShardOffers.Add(current, value);
				}
			}
			if (dictionary.ContainsKey("shardShopData"))
			{
				Dictionary<string, object> dictionary3 = dictionary["shardShopData"] as Dictionary<string, object>;
				if (dictionary3.ContainsKey("seriesId"))
				{
					this.ShopData.CurrentSeriesPeriod = (dictionary3["seriesId"] as string);
				}
				if (dictionary3.ContainsKey("offerExpiration"))
				{
					this.ShopData.Expiration = Convert.ToUInt32(dictionary3["offerExpiration"]);
				}
				if (dictionary3.ContainsKey("offsetMinutes"))
				{
					this.ShopData.OffsetMinutes = Convert.ToSingle(dictionary3["offsetMinutes"]);
				}
				for (int i = 0; i < 5; i++)
				{
					string shardSlotId = GameUtils.GetShardSlotId(i);
					if (dictionary3.ContainsKey(shardSlotId))
					{
						Dictionary<string, object> dictionary4 = dictionary3[shardSlotId] as Dictionary<string, object>;
						if (dictionary4 != null)
						{
							foreach (string current2 in dictionary4.Keys)
							{
								this.ShopData.Purchases[i].Add(current2, Convert.ToInt32(dictionary4[current2]));
							}
						}
					}
				}
			}
			return this;
		}
	}
}
