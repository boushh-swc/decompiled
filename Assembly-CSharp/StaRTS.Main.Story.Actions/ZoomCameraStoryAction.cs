using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ZoomCameraStoryAction : AbstractStoryAction
	{
		private const int ZOOM_AMOUNT_ARG = 0;

		private float zoomAmount;

		public ZoomCameraStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.zoomAmount = (float)Convert.ToInt32(this.prepareArgs[0]) / 100f;
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			Service.WorldInitializer.View.ZoomTo(this.zoomAmount);
			this.parent.ChildComplete(this);
		}
	}
}
