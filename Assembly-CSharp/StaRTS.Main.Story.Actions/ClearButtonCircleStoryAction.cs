using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ClearButtonCircleStoryAction : AbstractStoryAction
	{
		public ClearButtonCircleStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			Service.UXController.MiscElementsManager.HideHighlight();
			NavigationCenterUpgradeScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<NavigationCenterUpgradeScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.DeactivateHighlight();
			}
			this.parent.ChildComplete(this);
		}
	}
}
