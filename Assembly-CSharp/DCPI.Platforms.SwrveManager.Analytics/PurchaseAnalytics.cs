using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class PurchaseAnalytics : GameAnalytics
	{
		private string _item;

		private float _cost;

		private string _currency;

		private string _durability;

		private int _quantity = -1;

		private int _level = -1;

		private string _context;

		private string _type;

		private string _subtype;

		private string _subtype2;

		public string Item
		{
			get
			{
				return this._item;
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

		public string Currency
		{
			get
			{
				return this._currency;
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

		public string Subtype2
		{
			get
			{
				return this._subtype2;
			}
		}

		public PurchaseAnalytics(string item, float cost, string currency, string durability)
		{
			this.InitPurchaseAnalytics(item, cost, -1, currency, durability, -1, null, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability)
		{
			this.InitPurchaseAnalytics(item, cost, quantity, currency, durability, -1, null, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level)
		{
			this.InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, null, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context)
		{
			this.InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, null, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type)
		{
			this.InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, type, null, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type, string subtype)
		{
			this.InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, type, subtype, null);
		}

		public PurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type, string subtype, string subtype2)
		{
			this.InitPurchaseAnalytics(item, cost, quantity, currency, durability, level, context, type, subtype, subtype2);
		}

		private void InitPurchaseAnalytics(string item, float cost, int quantity, string currency, string durability, int level, string context, string type, string subtype, string subtype2)
		{
			this._item = item;
			this._cost = cost;
			this._quantity = quantity;
			this._currency = currency;
			this._durability = durability;
			this._level = level;
			this._context = context;
			this._type = type;
			this._subtype = subtype;
			this._subtype2 = subtype2;
		}

		public override string GetSwrveEvent()
		{
			return "purchase_custom";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("item", this._item);
			dictionary.Add("cost", this._cost);
			if (this._quantity > -1)
			{
				dictionary.Add("quantity", this._quantity);
			}
			dictionary.Add("currency", this._currency);
			dictionary.Add("durability", this._durability);
			if (this._level > -1)
			{
				dictionary.Add("level", this._level);
			}
			if (!string.IsNullOrEmpty(this._context))
			{
				dictionary.Add("context", this._context);
			}
			if (!string.IsNullOrEmpty(this._type))
			{
				dictionary.Add("type", this._type);
			}
			if (!string.IsNullOrEmpty(this._subtype))
			{
				dictionary.Add("subtype", this._subtype);
			}
			if (!string.IsNullOrEmpty(this._subtype2))
			{
				dictionary.Add("subtype2", this._subtype2);
			}
			return dictionary;
		}
	}
}
