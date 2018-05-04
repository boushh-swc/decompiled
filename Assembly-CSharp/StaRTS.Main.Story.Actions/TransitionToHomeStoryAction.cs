using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class TransitionToHomeStoryAction : AbstractStoryAction
	{
		public TransitionToHomeStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			if (Service.GameStateMachine.CurrentState is HomeState)
			{
				Service.Logger.Warn("TransitionToHomeStoryAction executed but already home.");
				this.OnTransitionComplete();
			}
			else
			{
				HomeState.GoToHomeState(new TransitionCompleteDelegate(this.OnTransitionComplete), false);
			}
		}

		private void OnTransitionComplete()
		{
			this.parent.ChildComplete(this);
		}
	}
}
