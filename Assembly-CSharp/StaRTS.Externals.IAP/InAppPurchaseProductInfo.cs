using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace StaRTS.Externals.IAP
{
	public class InAppPurchaseProductInfo
	{
		private const string APP_STORE_ID = "appStoreId";

		private const string CURRENCY_CODE = "currencyCode";

		private const string FORMATTED_REAL_COST = "formattedRealCost";

		private const string NAME = "name";

		private const string PRICE_LOCALE = "priceLocale";

		private const string REAL_COST = "realCost";

		private const string RUBLE_SYMBOL = "₽";

		private const string OLD_RUBLE_ABBREV = "\u00a0руб.";

		public string AppStoreId
		{
			get;
			set;
		}

		public string FormattedRealCost
		{
			get;
			set;
		}

		public string Name
		{
			get;
			set;
		}

		public string RealCost
		{
			get;
			set;
		}

		public double Cost
		{
			get;
			set;
		}

		public string PriceLocale
		{
			get;
			set;
		}

		public string CurrencyCode
		{
			get;
			set;
		}

		public static InAppPurchaseProductInfo Parse(object value)
		{
			InAppPurchaseProductInfo inAppPurchaseProductInfo = new InAppPurchaseProductInfo();
			IDictionary<string, object> dictionary = value as Dictionary<string, object>;
			inAppPurchaseProductInfo.FormattedRealCost = (dictionary["formattedRealCost"] as string);
			string text = dictionary["realCost"] as string;
			text = Regex.Replace(text, "[^\\s,.0-9]", string.Empty);
			inAppPurchaseProductInfo.RealCost = text;
			inAppPurchaseProductInfo.Name = (dictionary["name"] as string);
			inAppPurchaseProductInfo.AppStoreId = (dictionary["appStoreId"] as string);
			if (dictionary.ContainsKey("currencyCode"))
			{
				inAppPurchaseProductInfo.CurrencyCode = (dictionary["currencyCode"] as string);
			}
			return inAppPurchaseProductInfo;
		}

		private static void SwapForOldRubleAbbrev(InAppPurchaseProductInfo iap)
		{
			if (iap == null || string.IsNullOrEmpty(iap.FormattedRealCost))
			{
				return;
			}
			if (iap.FormattedRealCost.Contains("₽"))
			{
				iap.FormattedRealCost = iap.FormattedRealCost.Replace("₽", "\u00a0руб.");
			}
		}

		public override string ToString()
		{
			StringBuilder stringBuilder = new StringBuilder(this.AppStoreId);
			stringBuilder.Append(":");
			stringBuilder.Append(this.Name);
			stringBuilder.Append(":");
			stringBuilder.Append(this.FormattedRealCost);
			stringBuilder.Append(":");
			stringBuilder.Append(this.RealCost);
			return stringBuilder.ToString();
		}
	}
}
