using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class GenericWrapperAnalytics : GameAnalytics
	{
		private string _action = string.Empty;

		private Dictionary<string, object> _payload;

		public GenericWrapperAnalytics(string action, Dictionary<string, object> payload)
		{
			this._action = action;
			this._payload = payload;
		}

		public override string GetSwrveEvent()
		{
			return this._action;
		}

		public override Dictionary<string, object> Serialize()
		{
			return this._payload;
		}
	}
}
