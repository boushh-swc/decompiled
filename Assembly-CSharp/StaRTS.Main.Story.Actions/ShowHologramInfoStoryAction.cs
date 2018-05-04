using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ShowHologramInfoStoryAction : AbstractStoryAction
	{
		private const int ARG_IMG_NAME = 0;

		private const int ARG_DISPLAY_TEXT = 1;

		private const int ARG_TITLE_TEXT = 2;

		public string ImageName
		{
			get;
			private set;
		}

		public string DisplayText
		{
			get;
			private set;
		}

		public string TitleText
		{
			get;
			private set;
		}

		public bool PlanetPanel
		{
			get;
			private set;
		}

		public ShowHologramInfoStoryAction(StoryActionVO vo, IStoryReactor parent, bool planetPanel) : base(vo, parent)
		{
			this.PlanetPanel = planetPanel;
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(new int[]
			{
				1,
				2,
				3
			});
			this.ImageName = this.prepareArgs[0];
			if (this.prepareArgs.Length > 1)
			{
				this.DisplayText = this.prepareArgs[1];
			}
			if (this.prepareArgs.Length > 2)
			{
				this.TitleText = this.prepareArgs[2];
			}
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.EventManager.SendEvent(EventId.ShowInfoPanel, this);
			this.parent.ChildComplete(this);
		}
	}
}
