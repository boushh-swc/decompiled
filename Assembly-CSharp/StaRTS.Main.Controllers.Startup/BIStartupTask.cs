using StaRTS.Externals.BI;
using StaRTS.Externals.DMOAnalytics;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class BIStartupTask : StartupTask
	{
		public BIStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new BILoggingController();
			new DMOAnalyticsController();
			base.Complete();
		}
	}
}
