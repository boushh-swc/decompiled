using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Actions
{
	public class HideHologramStoryAction : AbstractStoryAction, IEventObserver
	{
		public HideHologramStoryAction(StoryActionVO vo, IStoryReactor parent) : base(vo, parent)
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
			if (!Service.HoloController.HasAnyCharacter())
			{
				this.parent.ChildComplete(this);
				return;
			}
			Service.EventManager.RegisterObserver(this, EventId.HideHologramComplete, EventPriority.Default);
			Service.EventManager.SendEvent(EventId.HideHologram, this);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.HideHologramComplete)
			{
				Service.EventManager.UnregisterObserver(this, EventId.HideHologramComplete);
				this.parent.ChildComplete(this);
			}
			return EatResponse.NotEaten;
		}
	}
}
