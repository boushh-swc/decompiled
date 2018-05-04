using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class GenericStringAnalytics : GameAnalytics
	{
		private string _message = string.Empty;

		private string _action = string.Empty;

		public string message
		{
			get
			{
				return this._message;
			}
		}

		public GenericStringAnalytics(string action)
		{
			this._action = action;
		}

		public GenericStringAnalytics(string action, string message)
		{
			this._action = action;
			this._message = message;
		}

		public override string GetSwrveEvent()
		{
			return this._action;
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			if (!string.IsNullOrEmpty(this._message))
			{
				dictionary.Add("message", this._message);
			}
			return dictionary;
		}
	}
}
