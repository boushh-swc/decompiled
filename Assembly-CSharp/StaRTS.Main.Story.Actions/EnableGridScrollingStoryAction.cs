using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class EnableGridScrollingStoryAction : AbstractStoryAction
	{
		public EnableGridScrollingStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			string elementName = this.prepareArgs[0];
			UXGrid uXGrid = Service.ScreenController.FindElement<UXGrid>(elementName);
			uXGrid.IsScrollable = true;
			this.parent.ChildComplete(this);
		}
	}
}
