using StaRTS.Main.Views;
using StaRTS.Main.Views.World;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class DamageStartupTask : StartupTask
	{
		public DamageStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new DerivedTransformationManager();
			new ProjectileController();
			new ProjectileViewManager();
			new HealthController();
			base.Complete();
		}
	}
}
