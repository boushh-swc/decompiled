using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class EditBaseStartupTask : StartupTask
	{
		public EditBaseStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			new EditBaseController();
			new BaseLayoutToolController();
			new WarBaseEditController();
			base.Complete();
		}
	}
}
