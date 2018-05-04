using StaRTS.Externals.IAP;
using StaRTS.Main.Controllers.Notifications;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class ExternalsStartupTask : StartupTask
	{
		public ExternalsStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			Service.DMOAnalyticsController.Init();
			new NetworkConnectionTester().CheckNetworkConnectionAvailable(false);
			new InAppPurchaseController();
			new NotificationController();
			base.Complete();
		}
	}
}
