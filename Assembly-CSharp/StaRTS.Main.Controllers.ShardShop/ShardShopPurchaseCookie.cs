using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Controllers.ShardShop
{
	public class ShardShopPurchaseCookie
	{
		public object Cookie;

		public int SlotIndex
		{
			get;
			private set;
		}

		public int Quantity
		{
			get;
			private set;
		}

		public Action<int, bool> DeferredSuccessCallback
		{
			get;
			private set;
		}

		public Action<object> ServerCallback
		{
			get;
			private set;
		}

		public CostVO Cost
		{
			get;
			private set;
		}

		public ShardShopPurchaseCookie(int slotIndex, int quantity, Action<int, bool> deferredSuccessCallback, Action<object> serverCallback, object cookie, CostVO cost)
		{
			this.SlotIndex = slotIndex;
			this.Quantity = quantity;
			this.DeferredSuccessCallback = deferredSuccessCallback;
			this.ServerCallback = serverCallback;
			this.Cookie = cookie;
			this.Cost = cost;
		}
	}
}
