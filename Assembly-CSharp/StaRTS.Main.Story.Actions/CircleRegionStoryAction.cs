using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class CircleRegionStoryAction : AbstractStoryAction
	{
		private const int BOARD_X_ARG = 0;

		private const int BOARD_Z_ARG = 1;

		private const int WIDTH_ARG = 2;

		private const int DEPTH_ARG = 3;

		private int boardX;

		private int boardZ;

		private int width;

		private int depth;

		public CircleRegionStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(4);
			this.boardX = Convert.ToInt32(this.prepareArgs[0]);
			this.boardZ = Convert.ToInt32(this.prepareArgs[1]);
			this.width = Convert.ToInt32(this.prepareArgs[2]);
			this.depth = Convert.ToInt32(this.prepareArgs[3]);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.UXController.MiscElementsManager.HighlightRegion((float)this.boardX, (float)this.boardZ, this.width, this.depth);
			this.parent.ChildComplete(this);
		}
	}
}
