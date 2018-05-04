using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Story.Actions
{
	public class HighlightAreaStoryAction : AbstractStoryAction
	{
		private const int GRID_ARG = 0;

		private const int BUTTON_ARG = 1;

		private const int BUFFER_WIDTH_ARG = 2;

		private const int BUFFER_HEIGHT_ARG = 3;

		private UXGrid grid;

		private UXButton dismissButton;

		public HighlightAreaStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(4);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			string elementName = this.prepareArgs[0];
			string elementName2 = this.prepareArgs[1];
			int num;
			int.TryParse(this.prepareArgs[2], out num);
			int num2;
			int.TryParse(this.prepareArgs[3], out num2);
			this.dismissButton = Service.ScreenController.FindElement<UXButton>(elementName2);
			if (this.dismissButton == null)
			{
				this.parent.ChildComplete(this);
				return;
			}
			this.grid = Service.ScreenController.FindElement<UXGrid>(elementName);
			if (this.grid == null)
			{
				Service.Logger.ErrorFormat("The StoryAction {0} has an incorrect number of arguments: {1}, wrong name used.story action requires grid.", new object[]
				{
					this.vo.Uid,
					this.vo.PrepareString
				});
				this.parent.ChildComplete(this);
				return;
			}
			UserInputInhibitor userInputInhibitor = Service.UserInputInhibitor;
			userInputInhibitor.DenyAll();
			List<UXElement> list = new List<UXElement>();
			list.AddRange(this.grid.GetElementList());
			list.Add(this.dismissButton);
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				userInputInhibitor.AddToAllow(list[i]);
				i++;
			}
			NavigationCenterUpgradeScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<NavigationCenterUpgradeScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.ActivateHighlight();
			}
		}

		public override void Destroy()
		{
			Service.UXController.MiscElementsManager.HideHighlight();
			NavigationCenterUpgradeScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<NavigationCenterUpgradeScreen>();
			if (highestLevelScreen != null)
			{
				highestLevelScreen.DeactivateHighlight();
			}
			base.Destroy();
		}
	}
}
