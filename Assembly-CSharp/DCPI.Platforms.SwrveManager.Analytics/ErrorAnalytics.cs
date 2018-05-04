using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class ErrorAnalytics : GameAnalytics
	{
		private string _reason;

		private string _type;

		private string _context;

		private IDictionary<string, object> _custom;

		public string Reason
		{
			get
			{
				return this._reason;
			}
		}

		public string Type
		{
			get
			{
				return this._type;
			}
		}

		public string Context
		{
			get
			{
				return this._context;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return this._custom;
			}
		}

		public ErrorAnalytics(string reason)
		{
			this.InitErrorActionAnalytics(reason, null, null, null);
		}

		public ErrorAnalytics(string reason, string type)
		{
			this.InitErrorActionAnalytics(reason, type, null, null);
		}

		public ErrorAnalytics(string reason, string type, string context)
		{
			this.InitErrorActionAnalytics(reason, type, context, null);
		}

		public ErrorAnalytics(string reason, string type, string context, IDictionary<string, object> custom)
		{
			this.InitErrorActionAnalytics(reason, type, context, custom);
		}

		private void InitErrorActionAnalytics(string reason, string type, string context, IDictionary<string, object> custom)
		{
			this._reason = reason;
			this._type = type;
			this._context = context;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["reason"] = this._reason;
			if (!string.IsNullOrEmpty(this._type))
			{
				dictionary["type"] = this._type;
			}
			if (!string.IsNullOrEmpty(this._context))
			{
				dictionary["context"] = this._context;
			}
			if (this._custom != null)
			{
				foreach (KeyValuePair<string, object> current in this._custom)
				{
					dictionary[current.Key] = current.Value;
				}
			}
			return dictionary;
		}

		public override string GetSwrveEvent()
		{
			return "error";
		}
	}
}
