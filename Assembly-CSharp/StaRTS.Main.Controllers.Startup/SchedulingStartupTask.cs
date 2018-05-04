using StaRTS.Main.Controllers.Performance;
using StaRTS.Utils;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class SchedulingStartupTask : StartupTask
	{
		public SchedulingStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new Rand();
			new SimTimerManager();
			new ViewTimerManager();
			new PerformanceMonitor();
			base.Complete();
		}
	}
}
