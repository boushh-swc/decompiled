using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class HomeStartupTask : StartupTask
	{
		public HomeStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new HomeModeController();
			base.Complete();
		}
	}
}
