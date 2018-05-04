using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Views.UX.Screens
{
	public class ShardShopViewTO
	{
		public const string STATE_LOCKED_SOLD_OUT = "soldOut";

		public const string STATE_LOCKED_MAXED_OUT = "maxedOut";

		public const string STATE_LOCKED_REQ_CANTINA = "requiresCantina";

		public const string STATE_LOCKED_REQ_ARMORY = "requiresArmory";

		public const string STATE_UNLOCKED = "unlocked";

		public bool CanAffordAll;

		public bool CanAffordSingle;

		public CostVO CostOfAllShards;

		public CostVO CostOfNextShard;

		public string ItemName;

		public int Quality;

		public int RemainingShardsForSale;

		public int SlotIndex;

		public CrateSupplyVO SupplyVO;

		public int UpgradeShardsEarned;

		public int UpgradeShardsRequired;

		public int PlayerHQLevel;

		public string State;

		public bool Upgradeable;

		public bool ValueEquals(ShardShopViewTO shardShopVto)
		{
			return shardShopVto != null && this.CanAffordAll == shardShopVto.CanAffordAll && this.CanAffordSingle == shardShopVto.CanAffordSingle && !(this.ItemName != shardShopVto.ItemName) && this.Quality == shardShopVto.Quality && this.RemainingShardsForSale == shardShopVto.RemainingShardsForSale && this.SlotIndex == shardShopVto.SlotIndex && this.UpgradeShardsEarned == shardShopVto.UpgradeShardsEarned && this.UpgradeShardsRequired == shardShopVto.UpgradeShardsRequired && this.PlayerHQLevel == shardShopVto.PlayerHQLevel && !(this.State != shardShopVto.State) && this.Upgradeable == shardShopVto.Upgradeable;
		}
	}
}
