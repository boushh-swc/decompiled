using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class DisplayButtonStoryAction : AbstractStoryAction, IEventObserver
	{
		private const int BUTTON_NAME_ARG = 0;

		private const string NEXT_BUTTON = "next";

		private const string STORE_BUTTON = "store";

		public DisplayButtonStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			EventManager eventManager = Service.EventManager;
			string a = this.prepareArgs[0];
			if (a == "next")
			{
				eventManager.RegisterObserver(this, EventId.StoryNextButtonClicked, EventPriority.Default);
				eventManager.SendEvent(EventId.ShowNextButton, this);
			}
			else if (a == "store")
			{
				eventManager.RegisterObserver(this, EventId.StoryNextButtonClicked, EventPriority.Default);
				eventManager.SendEvent(EventId.ShowStoreNextButton, this);
			}
			else
			{
				Service.Logger.ErrorFormat("No button with id {0} for DisplayButtonStoryAction!", new object[]
				{
					this.prepareArgs[0]
				});
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.StoryNextButtonClicked)
			{
				Service.EventManager.UnregisterObserver(this, EventId.StoryNextButtonClicked);
			}
			Service.ViewTimerManager.CreateViewTimer(0.15f, false, new TimerDelegate(this.CompleteActionAfterFrameDelay), null);
			return EatResponse.NotEaten;
		}

		private void CompleteActionAfterFrameDelay(uint id, object cookie)
		{
			this.parent.ChildComplete(this);
		}
	}
}
