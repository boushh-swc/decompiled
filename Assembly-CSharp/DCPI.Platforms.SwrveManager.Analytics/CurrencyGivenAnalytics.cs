using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class CurrencyGivenAnalytics : GameAnalytics
	{
		private string _given_currency;

		private float _given_amount;

		private int _level = -1;

		private string _context;

		private string _type;

		private string _subtype;

		private string _subtype2;

		private IDictionary<string, object> _custom;

		public string GivenCurrency
		{
			get
			{
				return this._given_currency;
			}
		}

		public float GivenAmount
		{
			get
			{
				return this._given_amount;
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

		public IDictionary<string, object> Custom
		{
			get
			{
				return this._custom;
			}
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, -1, null, null, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, level, null, null, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, null, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, null, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, subtype, null, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype, string subtype2)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, subtype, subtype2, null);
		}

		public CurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype, string subtype2, IDictionary<string, object> custom)
		{
			this.InitCurrencyGivenAnalytics(given_currency, given_amount, level, context, type, subtype, subtype2, custom);
		}

		private void InitCurrencyGivenAnalytics(string given_currency, float given_amount, int level, string context, string type, string subtype, string subtype2, IDictionary<string, object> custom)
		{
			this._given_currency = given_currency;
			this._given_amount = given_amount;
			this._level = level;
			this._context = context;
			this._type = type;
			this._subtype = subtype;
			this._subtype2 = subtype2;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "currency_given_custom";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("given_currency", this._given_currency);
			dictionary.Add("given_amount", this._given_amount);
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
			if (this._custom != null)
			{
				foreach (KeyValuePair<string, object> current in this._custom)
				{
					dictionary.Add(current.Key, current.Value);
				}
			}
			return dictionary;
		}
	}
}
