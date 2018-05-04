using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class PlacementStartupTask : StartupTask
	{
		public PlacementStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new DeployerController();
			new TroopController();
			new SpecialAttackController();
			new SquadTroopAttackController();
			base.Complete();
		}
	}
}
