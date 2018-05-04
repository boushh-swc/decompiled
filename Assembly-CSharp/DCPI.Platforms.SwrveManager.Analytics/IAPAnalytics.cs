using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class IAPAnalytics : GameAnalytics
	{
		public enum IAPStore
		{
			APPLE = 0,
			GOOGLE = 1,
			UNKNOWN_STORE = 2
		}

		private string _productId;

		private float _cost;

		private int _quantity = 1;

		private string _local_currency;

		private IAPAnalytics.IAPStore _app_store;

		private string _durability;

		private int _level;

		private string _context;

		private string _type;

		private string _subtype;

		public string ProductId
		{
			get
			{
				return this._productId;
			}
		}

		public float Cost
		{
			get
			{
				return this._cost;
			}
		}

		public int Quantity
		{
			get
			{
				return this._quantity;
			}
		}

		public string LocalCurrency
		{
			get
			{
				return this._local_currency;
			}
		}

		public IAPAnalytics.IAPStore AppStore
		{
			get
			{
				return this._app_store;
			}
		}

		public string Durability
		{
			get
			{
				return this._durability;
			}
		}

		public int Level
		{
			get
			{
				return this._level;
			}
		}

		public string Context
		{
			get
			{
				return this._context;
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
		}

		public string Subtype
		{
			get
			{
				return this._subtype;
			}
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPAnalytics.IAPStore app_store, string durablity, int level)
		{
			this.InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, null, null, null);
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPAnalytics.IAPStore app_store, string durablity, int level, string context)
		{
			this.InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, context, null, null);
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPAnalytics.IAPStore app_store, string durablity, int level, string context, string type)
		{
			this.InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, context, type, null);
		}

		public IAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPAnalytics.IAPStore app_store, string durablity, int level, string context, string type, string subtype)
		{
			this.InitIAPAnalytics(productId, cost, quantity, local_currency, app_store, durablity, level, context, type, subtype);
		}

		private void InitIAPAnalytics(string productId, float cost, int quantity, string local_currency, IAPAnalytics.IAPStore app_store, string durablity, int level, string context, string type, string subtype)
		{
			this._productId = productId;
			this._cost = cost;
			this._quantity = quantity;
			this._local_currency = local_currency;
			this._app_store = app_store;
			this._durability = durablity;
			this._level = level;
			this._context = context;
			this._type = type;
			this._subtype = subtype;
		}

		public override string GetSwrveEvent()
		{
			return "IAP_custom";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["product_id"] = this._productId;
			dictionary["cost"] = this._cost;
			dictionary["quantity"] = this._quantity;
			dictionary["local_currency"] = this._local_currency;
			dictionary["app_store"] = this._app_store.ToString().ToLower();
			dictionary["durability"] = this._durability;
			if (this._level > -1)
			{
				dictionary["level"] = this._level;
			}
			if (!string.IsNullOrEmpty(this._context))
			{
				dictionary["context"] = this._context;
			}
			if (!string.IsNullOrEmpty(this._type))
			{
				dictionary["type"] = this._type;
			}
			if (!string.IsNullOrEmpty(this._subtype))
			{
				dictionary["subtype"] = this._subtype;
			}
			return dictionary;
		}
	}
}
