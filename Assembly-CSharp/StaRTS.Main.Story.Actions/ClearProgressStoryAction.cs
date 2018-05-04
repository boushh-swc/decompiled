using StaRTS.Main.Models.Commands.Player.Fue;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ClearProgressStoryAction : AbstractStoryAction
	{
		public ClearProgressStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			Service.CurrentPlayer.CurrentQuest = string.Empty;
			Service.ServerAPI.Enqueue(new FueUpdateStateCommand());
			this.parent.ChildComplete(this);
		}
	}
}
