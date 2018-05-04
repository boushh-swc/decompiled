using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Externals.BI
{
	public class StepTimingController
	{
		private Dictionary<string, float> timeStamps;

		public StepTimingController()
		{
			Service.StepTimingController = this;
			this.timeStamps = new Dictionary<string, float>();
		}

		public void StartStep(string stepName)
		{
			float realTimeSinceStartUpInMilliseconds = DateUtils.GetRealTimeSinceStartUpInMilliseconds();
			if (this.timeStamps.ContainsKey(stepName))
			{
				this.timeStamps[stepName] = realTimeSinceStartUpInMilliseconds;
			}
			else
			{
				this.timeStamps.Add(stepName, realTimeSinceStartUpInMilliseconds);
			}
		}

		public void IntermediaryStep(string stepName, BILog log)
		{
			this.AddElapsedTime(stepName, log);
		}

		public void EndStep(string stepName, BILog log)
		{
			this.AddElapsedTime(stepName, log);
			this.timeStamps.Remove(stepName);
		}

		public bool IsStepStarted(string stepName)
		{
			return this.timeStamps.ContainsKey(stepName);
		}

		private void AddElapsedTime(string stepName, BILog log)
		{
			float realTimeSinceStartUpInMilliseconds = DateUtils.GetRealTimeSinceStartUpInMilliseconds();
			if (!this.timeStamps.ContainsKey(stepName))
			{
				this.timeStamps.Add(stepName, realTimeSinceStartUpInMilliseconds);
			}
			float num = this.timeStamps[stepName];
			log.AddParam("elapsed_time_ms", ((int)(realTimeSinceStartUpInMilliseconds - num)).ToString());
		}
	}
}
