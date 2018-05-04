using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class InAppPurchaseTypeVO : IValueObject
	{
		public const string TYPE_AMAZON = "am";

		public const string TYPE_ANDROID = "a";

		public const string TYPE_FB_GAMEROOM = "fbgr";

		public const string TYPE_IOS = "i";

		public const string CURRENCY_TYPE_HARD = "hard";

		public const string CURRENCY_TYPE_UNIT = "unit";

		public const string CURRENCY_TYPE_PACK = "pack";

		public static int COLUMN_productId
		{
			get;
			private set;
		}

		public static int COLUMN_reward_empire
		{
			get;
			private set;
		}

		public static int COLUMN_reward_rebel
		{
			get;
			private set;
		}

		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_currencyType
		{
			get;
			private set;
		}

		public static int COLUMN_isPromo
		{
			get;
			private set;
		}

		public static int COLUMN_amount
		{
			get;
			private set;
		}

		public static int COLUMN_price
		{
			get;
			private set;
		}

		public static int COLUMN_order
		{
			get;
			private set;
		}

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_assetName_empire
		{
			get;
			private set;
		}

		public static int COLUMN_assetName_rebel
		{
			get;
			private set;
		}

		public static int COLUMN_topBadgeString
		{
			get;
			private set;
		}

		public static int COLUMN_bottomBadgeString
		{
			get;
			private set;
		}

		public static int COLUMN_redemptionString_empire
		{
			get;
			private set;
		}

		public static int COLUMN_redemptionString_rebel
		{
			get;
			private set;
		}

		public static int COLUMN_hideFromStore
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string ProductId
		{
			get;
			set;
		}

		public string RewardEmpire
		{
			get;
			set;
		}

		public string RewardRebel
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public string CurrencyType
		{
			get;
			set;
		}

		public bool IsPromo
		{
			get;
			set;
		}

		public int Amount
		{
			get;
			set;
		}

		public float Price
		{
			get;
			set;
		}

		public int Order
		{
			get;
			set;
		}

		public string CurrencyIconId
		{
			get;
			set;
		}

		public string TopBadgeString
		{
			get;
			set;
		}

		public string BottomBadgeString
		{
			get;
			set;
		}

		public string RedemptionStringEmpire
		{
			get;
			set;
		}

		public string RedemptionStringRebel
		{
			get;
			set;
		}

		public bool HideFromStore
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.ProductId = row.TryGetString(InAppPurchaseTypeVO.COLUMN_productId);
			this.RewardEmpire = row.TryGetString(InAppPurchaseTypeVO.COLUMN_reward_empire);
			this.RewardRebel = row.TryGetString(InAppPurchaseTypeVO.COLUMN_reward_rebel);
			this.Type = row.TryGetString(InAppPurchaseTypeVO.COLUMN_type);
			this.CurrencyType = row.TryGetString(InAppPurchaseTypeVO.COLUMN_currencyType);
			this.IsPromo = row.TryGetBool(InAppPurchaseTypeVO.COLUMN_isPromo);
			this.Amount = row.TryGetInt(InAppPurchaseTypeVO.COLUMN_amount);
			this.Price = row.TryGetFloat(InAppPurchaseTypeVO.COLUMN_price);
			this.Order = row.TryGetInt(InAppPurchaseTypeVO.COLUMN_order);
			this.CurrencyIconId = row.TryGetString(InAppPurchaseTypeVO.COLUMN_assetName);
			this.TopBadgeString = row.TryGetString(InAppPurchaseTypeVO.COLUMN_topBadgeString);
			this.BottomBadgeString = row.TryGetString(InAppPurchaseTypeVO.COLUMN_bottomBadgeString);
			this.RedemptionStringEmpire = row.TryGetString(InAppPurchaseTypeVO.COLUMN_redemptionString_empire);
			this.RedemptionStringRebel = row.TryGetString(InAppPurchaseTypeVO.COLUMN_redemptionString_rebel);
			this.HideFromStore = row.TryGetBool(InAppPurchaseTypeVO.COLUMN_hideFromStore);
		}
	}
}
