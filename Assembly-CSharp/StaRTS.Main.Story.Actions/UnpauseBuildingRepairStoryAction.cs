using StaRTS.Main.Controllers;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class UnpauseBuildingRepairStoryAction : AbstractStoryAction
	{
		public UnpauseBuildingRepairStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			postBattleRepairController.PauseHealthRepairs(false);
			this.parent.ChildComplete(this);
		}
	}
}
