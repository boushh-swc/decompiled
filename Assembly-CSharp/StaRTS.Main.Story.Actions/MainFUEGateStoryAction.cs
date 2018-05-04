using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class MainFUEGateStoryAction : AbstractStoryAction
	{
		private string reactionUID;

		public override string Reaction
		{
			get
			{
				return this.reactionUID;
			}
		}

		public MainFUEGateStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
			this.reactionUID = string.Empty;
			if (string.IsNullOrEmpty(vo.Reaction))
			{
			}
			if (!Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				this.reactionUID = vo.Reaction;
			}
		}

		public override void Prepare()
		{
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			this.parent.ChildComplete(this);
		}
	}
}
