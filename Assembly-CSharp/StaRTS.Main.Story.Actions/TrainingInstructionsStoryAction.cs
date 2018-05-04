using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class TrainingInstructionsStoryAction : AbstractStoryAction, IEventObserver
	{
		private const int INSTRUCTION_STRING_KEY_ARG = 0;

		private const int COUNT_KEY_ARG = 1;

		private TroopTrainingScreen tts;

		private EventManager events;

		public TrainingInstructionsStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
		{
		}

		public override void Prepare()
		{
			base.VerifyArgumentCount(2);
			this.parent.ChildPrepared(this);
		}

		public override void Execute()
		{
			base.Execute();
			this.tts = Service.ScreenController.GetHighestLevelScreen<TroopTrainingScreen>();
			if (this.tts == null || !this.tts.IsLoaded())
			{
				this.events = Service.EventManager;
				this.events.RegisterObserver(this, EventId.ScreenLoaded, EventPriority.Default);
			}
			else if (!this.tts.TransitionedIn)
			{
				TroopTrainingScreen expr_69 = this.tts;
				expr_69.OnTransitionInComplete = (OnTransInComplete)Delegate.Combine(expr_69.OnTransitionInComplete, new OnTransInComplete(this.SetSpecialInstructions));
			}
			else
			{
				this.SetSpecialInstructions();
			}
		}

		private void SetSpecialInstructions()
		{
			string instructionsUid = this.prepareArgs[0];
			int maxCount = Convert.ToInt32(this.prepareArgs[1]);
			this.tts.DisableTroopItemScrolling();
			this.tts.DisableTabSelection();
			this.tts.SetSpecialInstructions(instructionsUid, maxCount);
			this.parent.ChildComplete(this);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ScreenLoaded)
			{
				if (cookie is TroopTrainingScreen)
				{
					this.RemoveListeners();
					this.Execute();
				}
			}
			return EatResponse.NotEaten;
		}

		private void RemoveListeners()
		{
			this.events.UnregisterObserver(this, EventId.ScreenLoaded);
		}
	}
}
