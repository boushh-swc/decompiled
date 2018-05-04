using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class GameStateStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const int STATE_ARG = 0;

		private const string HOME_STATE = "home";

		private const string EDIT_STATE = "edit";

		private const string BATTLE_START_STATE = "battle_start";

		private const string BATTLE_PLAY_STATE = "battle_play";

		private const string BATTLE_END_STATE = "battle_end";

		public GameStateStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			Service.EventManager.RegisterObserver(this, EventId.GameStateChanged, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.GameStateChanged)
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				if (this.EvaluateState(currentState, this.prepareArgs[0]))
				{
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		private bool EvaluateState(IState state, string desiredState)
		{
			if (desiredState == "home")
			{
				return state is HomeState;
			}
			if (desiredState == "edit")
			{
				return state is EditBaseState;
			}
			if (desiredState == "battle_start")
			{
				return state is BattleStartState;
			}
			if (desiredState == "battle_end")
			{
				return state is BattleEndState;
			}
			return desiredState == "battle_play" && state is BattlePlayState;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
			base.Destroy();
		}
	}
}
