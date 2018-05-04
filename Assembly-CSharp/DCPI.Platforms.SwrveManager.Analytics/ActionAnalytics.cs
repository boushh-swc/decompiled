using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class ActionAnalytics : GameAnalytics
	{
		private string _tier1;

		private string _subEvent;

		private int _level = -1;

		private string _tier2;

		private string _context;

		private string _message;

		private string _tier3;

		private string _tier4;

		private IDictionary<string, object> _custom;

		public string Tier1
		{
			get
			{
				return this._tier1;
			}
		}

		public string SubEvent
		{
			get
			{
				return this._subEvent;
			}
		}

		public int Level
		{
			get
			{
				return this._level;
			}
		}

		public string Tier2
		{
			get
			{
				return this._tier2;
			}
		}

		public string Tier3
		{
			get
			{
				return this._tier3;
			}
		}

		public string Tier4
		{
			get
			{
				return this._tier4;
			}
		}

		public string Context
		{
			get
			{
				return this._context;
			}
		}

		public string Message
		{
			get
			{
				return this._message;
			}
		}

		public IDictionary<string, object> Custom
		{
			get
			{
				return this._custom;
			}
		}

		public ActionAnalytics(string tier1)
		{
			this.InitActionAnalytics(tier1, null, -1, null, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent)
		{
			this.InitActionAnalytics(tier1, subEvent, -1, null, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level)
		{
			this.InitActionAnalytics(tier1, subEvent, level, null, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2)
		{
			this.InitActionAnalytics(tier1, subEvent, level, tier2, null, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3)
		{
			this.InitActionAnalytics(tier1, subEvent, level, tier2, tier3, null, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4)
		{
			this.InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, null, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context)
		{
			this.InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, context, null, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context, string message)
		{
			this.InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, context, message, null);
		}

		public ActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context, string message, IDictionary<string, object> custom)
		{
			this.InitActionAnalytics(tier1, subEvent, level, tier2, tier3, tier4, context, message, custom);
		}

		private void InitActionAnalytics(string tier1, string subEvent, int level, string tier2, string tier3, string tier4, string context, string message, IDictionary<string, object> custom)
		{
			this._tier1 = tier1;
			this._subEvent = subEvent;
			this._level = level;
			this._context = context;
			this._message = message;
			this._tier2 = tier2;
			this._tier3 = tier3;
			this._tier4 = tier4;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "action" + (string.IsNullOrEmpty(this._subEvent) ? string.Empty : ("." + this._subEvent));
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("tier1", this._tier1);
			if (this._level > -1)
			{
				dictionary.Add("level", this._level);
			}
			if (!string.IsNullOrEmpty(this._tier2))
			{
				dictionary.Add("tier2", this._tier2);
			}
			if (!string.IsNullOrEmpty(this._tier3))
			{
				dictionary.Add("tier3", this._tier3);
			}
			if (!string.IsNullOrEmpty(this._tier4))
			{
				dictionary.Add("tier4", this._tier4);
			}
			if (!string.IsNullOrEmpty(this._context))
			{
				dictionary.Add("context", this._context);
			}
			if (!string.IsNullOrEmpty(this._message))
			{
				dictionary.Add("message", this._message);
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
