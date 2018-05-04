using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class PressHereStoryAction : AbstractStoryAction
	{
		private const int BOARDX_ARG = 0;

		private const int BOARDZ_ARG = 1;

		private float boardX;

		private float boardZ;

		private bool screen;

		public PressHereStoryAction(StoryActionVO vo, IStoryReactor parent, bool screen) : base(vo, parent)
		{
			this.screen = screen;
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.boardX = Convert.ToSingle(this.prepareArgs[0]);
			this.boardZ = Convert.ToSingle(this.prepareArgs[1]);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			if (this.screen)
			{
				Service.UXController.MiscElementsManager.ShowArrowOnScreen(this.boardX, this.boardZ);
			}
			else
			{
				Service.UXController.MiscElementsManager.ShowFingerAnimation(this.boardX, this.boardZ);
			}
			this.parent.ChildComplete(this);
		}
	}
}
