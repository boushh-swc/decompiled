using System;
using System.Collections.Generic;

namespace DCPI.Platforms.SwrveManager.Analytics
{
	public class TimingAnalytics : GameAnalytics
	{
		private int _elapsedTime;

		private string _context;

		private int _stepNumber;

		private string _stepName;

		private IDictionary<string, object> _custom;

		public int ElapsedTime
		{
			get
			{
				return this._elapsedTime;
			}
		}

		public string Context
		{
			get
			{
				return this._context;
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

		public IDictionary<string, object> Custom
		{
			get
			{
				return this._custom;
			}
		}

		public TimingAnalytics(int elapsedTime, string context)
		{
			this.InitTimingAnalytics(elapsedTime, context, -1, null, null);
		}

		public TimingAnalytics(int elapsedTime, string context, int stepNumber)
		{
			this.InitTimingAnalytics(elapsedTime, context, stepNumber, null, null);
		}

		public TimingAnalytics(int elapsedTime, string context, int stepNumber, string stepName)
		{
			this.InitTimingAnalytics(elapsedTime, context, stepNumber, stepName, null);
		}

		public TimingAnalytics(int elapsedTime, string context, int stepNumber, string stepName, IDictionary<string, object> custom)
		{
			this.InitTimingAnalytics(elapsedTime, context, stepNumber, stepName, custom);
		}

		private void InitTimingAnalytics(int elapsedTime, string context, int stepNumber, string stepName, IDictionary<string, object> custom)
		{
			this._elapsedTime = elapsedTime;
			this._context = context;
			this._stepNumber = stepNumber;
			this._stepName = stepName;
			if (custom != null)
			{
				this._custom = new Dictionary<string, object>(custom);
			}
		}

		public override string GetSwrveEvent()
		{
			return "timing";
		}

		public override Dictionary<string, object> Serialize()
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary["elapsed_time"] = this._elapsedTime;
			dictionary["context"] = this._context;
			if (this._stepNumber > 0)
			{
				dictionary["step_number"] = this._stepNumber;
			}
			if (!string.IsNullOrEmpty(this._stepName))
			{
				dictionary["step_name"] = this._stepName;
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
	}
}
