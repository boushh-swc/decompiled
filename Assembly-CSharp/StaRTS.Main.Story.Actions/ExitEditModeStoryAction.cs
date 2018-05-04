using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class ExitEditModeStoryAction : AbstractStoryAction
	{
		public ExitEditModeStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			if (Service.GameStateMachine.CurrentState is EditBaseState)
			{
				HomeState.GoToHomeState(null, false);
			}
			else
			{
				Service.Logger.WarnFormat("Story Action {0} is attempting to exit Edit mode, which we are not in", new object[]
				{
					this.vo.Uid
				});
			}
			this.parent.ChildComplete(this);
		}
	}
}
