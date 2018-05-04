using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.RUF.RUFTasks
{
	public class AbstractRUFTask
	{
		protected const float LOGIN_NOTIFICATION_TIME_DELAY = 2f;

		public int Priority
		{
			get;
			set;
		}

		public bool ShouldPurgeQueue
		{
			get;
			set;
		}

		public bool ShouldProcess
		{
			get;
			set;
		}

		public bool ShouldPlayFromLoadState
		{
			get;
			set;
		}

		public int PriorityPurgeThreshold
		{
			get;
			set;
		}

		public virtual void Process(bool continueProcessing)
		{
			Service.RUFManager.ProcessQueue(continueProcessing);
		}
	}
}
