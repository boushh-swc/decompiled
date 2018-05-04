using StaRTS.Main.Models;
using System;

namespace StaRTS.Main.Views.UX.Tags
{
	public class CurrencyTag
	{
		public CurrencyType Currency
		{
			get;
			private set;
		}

		public int Amount
		{
			get;
			private set;
		}

		public int Crystals
		{
			get;
			private set;
		}

		public string PurchaseContext
		{
			get;
			private set;
		}

		public object Cookie
		{
			get;
			private set;
		}

		public CurrencyTag(CurrencyType currency, int amount, int crystals, string purchaseContext, object cookie)
		{
			this.Currency = currency;
			this.Amount = amount;
			this.Crystals = crystals;
			this.PurchaseContext = purchaseContext;
			this.Cookie = cookie;
		}
	}
}
