using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX.Elements;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowUIElementStoryAction : AbstractStoryAction
	{
		private const int ARG_ELEMENT_NAME = 0;

		private string elementName;

		private bool show;

		public ShowUIElementStoryAction(StoryActionVO vo, IStoryReactor parent, bool show) : base(vo, parent)
		{
			this.show = show;
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.elementName = this.prepareArgs[0];
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			UXElement uXElement = Service.ScreenController.FindElement<UXElement>(this.elementName);
			if (uXElement != null)
			{
				uXElement.Visible = this.show;
			}
			else
			{
				Service.Logger.WarnFormat("No element of name {0} exists in the UI currently!", new object[]
				{
					this.elementName
				});
			}
			this.parent.ChildComplete(this);
		}
	}
}
