using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class EnableCancelBuildingPlacementStoryAction : AbstractStoryAction
	{
		private bool enable;

		public EnableCancelBuildingPlacementStoryAction(StoryActionVO vo, IStoryReactor parent, bool enable) : base(vo, parent)
		{
			this.enable = enable;
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.UXController.MiscElementsManager.EnableCancelBuildingPlacement = this.enable;
			this.parent.ChildComplete(this);
		}
	}
}
