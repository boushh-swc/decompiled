using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.World;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class TransitionToWorldStoryAction : AbstractStoryAction
	{
		private const int WORLD_UID_ARG = 0;

		public TransitionToWorldStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(1);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			BattleInitializationData data = BattleInitializationData.CreateFromBattleTypeVO(this.prepareArgs[0]);
			BattleStartState.GoToBattleStartState(data, new TransitionCompleteDelegate(this.OnTransitionComplete));
		}

		private void OnTransitionComplete()
		{
			this.parent.ChildComplete(this);
		}
	}
}
