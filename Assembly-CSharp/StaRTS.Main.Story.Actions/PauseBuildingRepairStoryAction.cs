using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class PauseBuildingRepairStoryAction : AbstractStoryAction
	{
		public PauseBuildingRepairStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(0);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			PostBattleRepairController postBattleRepairController = Service.PostBattleRepairController;
			postBattleRepairController.PauseHealthRepairs(true);
			this.parent.ChildComplete(this);
		}
	}
}
