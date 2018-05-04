using StaRTS.Main.Models.ValueObjects;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class ShardShopData
	{
		public ShardShopSeriesVO ActiveSeries;

		public string CurrentSeriesPeriod;

		public uint Expiration;

		public Dictionary<string, CrateSupplyVO> ShardOffers;

		public List<Dictionary<string, int>> Purchases;

		public float OffsetMinutes;

		public ShardShopData()
		{
			this.ShardOffers = new Dictionary<string, CrateSupplyVO>();
			this.Purchases = new List<Dictionary<string, int>>();
			for (int i = 0; i < 5; i++)
			{
				this.Purchases.Add(new Dictionary<string, int>());
			}
		}

		public ShardShopData Copy()
		{
			ShardShopData shardShopData = new ShardShopData();
			shardShopData.ActiveSeries = this.ActiveSeries;
			shardShopData.CurrentSeriesPeriod = this.CurrentSeriesPeriod;
			shardShopData.Expiration = this.Expiration;
			shardShopData.ShardOffers = new Dictionary<string, CrateSupplyVO>();
			foreach (string current in this.ShardOffers.Keys)
			{
				shardShopData.ShardOffers.Add(current, this.ShardOffers[current]);
			}
			shardShopData.Purchases = new List<Dictionary<string, int>>();
			for (int i = 0; i < this.Purchases.Count; i++)
			{
				Dictionary<string, int> dictionary = new Dictionary<string, int>();
				foreach (string current2 in this.Purchases[i].Keys)
				{
					dictionary.Add(current2, this.Purchases[i][current2]);
				}
				shardShopData.Purchases.Add(dictionary);
			}
			shardShopData.OffsetMinutes = this.OffsetMinutes;
			return shardShopData;
		}

		public bool HasOfferChanged(ShardShopData other)
		{
			if (other == null)
			{
				return true;
			}
			if (other.ActiveSeries != this.ActiveSeries)
			{
				return true;
			}
			if (!other.CurrentSeriesPeriod.Equals(this.CurrentSeriesPeriod))
			{
				return true;
			}
			if (other.OffsetMinutes != this.OffsetMinutes)
			{
				return true;
			}
			if (this.ShardOffers.Count != other.ShardOffers.Count)
			{
				return true;
			}
			foreach (string current in this.ShardOffers.Keys)
			{
				if (!other.ShardOffers.ContainsKey(current))
				{
					bool result = true;
					return result;
				}
				if (this.ShardOffers[current] != other.ShardOffers[current])
				{
					bool result = true;
					return result;
				}
			}
			return false;
		}
	}
}
