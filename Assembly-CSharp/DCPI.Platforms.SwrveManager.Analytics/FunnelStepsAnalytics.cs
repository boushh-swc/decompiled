using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class FunnelStepsAnalytics : GameAnalytics
	{
		private string _type;

		private int _stepNumber;

		private string _stepName;

		private string _message = string.Empty;

		private IDictionary<string, object> _custom;

		public string Type
		{
			get
			{
				return this._type;
			}
		}

		public int StepNumber
		{
			get
			{
				return this._stepNumber;
			}
		}

		public string StepName
		{
			get
			{
				return this._stepName;
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

		public FunnelStepsAnalytics(string type, int stepNumber, string stepName)
		{
			this.InitFunnelStepsAnalytics(type, stepNumber, stepName, null, null);
		}

		public FunnelStepsAnalytics(string type, int stepNumber, string stepName, string message)
		{
			this.InitFunnelStepsAnalytics(type, stepNumber, stepName, message, null);
		}

		public FunnelStepsAnalytics(string type, int stepNumber, string stepName, string message, IDictionary<string, object> custom)
		{
			this.InitFunnelStepsAnalytics(type, stepNumber, stepName, message, custom);
		}

		private void InitFunnelStepsAnalytics(string type, int stepNumber, string stepName, string message, IDictionary<string, object> custom)
		{
			this._type = type;
			this._stepNumber = stepNumber;
			this._stepName = stepName;
			this._message = message;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return string.Concat(new string[]
			{
				"funnel.",
				this._type,
				".",
				(this._stepNumber >= 10) ? this._stepNumber.ToString() : ("0" + this._stepNumber.ToString()),
				".",
				this._stepName
			});
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("type", this._type);
			dictionary.Add("step_number", this._stepNumber);
			dictionary.Add("step_name", this._stepName);
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
