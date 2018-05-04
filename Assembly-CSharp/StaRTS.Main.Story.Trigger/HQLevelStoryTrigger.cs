using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class HQLevelStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private int hqLevelReq;

		private EventManager eventManager;

		private bool isSatisfied;

		public HQLevelStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
			this.isSatisfied = false;
		}

		public override void Activate()
		{
			this.eventManager = Service.EventManager;
			if (string.IsNullOrEmpty(this.vo.PrepareString))
			{
				Service.Logger.Error("HQLevelStoryTrigger: Missing HQ Level REQ for : " + this.vo.Uid);
			}
			this.hqLevelReq = int.Parse(this.prepareArgs[0]);
			Service.RUFManager.OmitRateAppLevels.Add(this.hqLevelReq);
			base.Activate();
			this.isSatisfied = this.CheckHQLevelAndSatisfy();
			this.AddHQLevelEventObserver();
		}

		private bool CheckHQLevelAndSatisfy()
		{
			bool result = false;
			int highestLevelHQ = Service.BuildingLookupController.GetHighestLevelHQ();
			if (highestLevelHQ >= this.hqLevelReq)
			{
				result = true;
				this.eventManager.UnregisterObserver(this, EventId.HQCelebrationScreenClosed);
				this.eventManager.UnregisterObserver(this, EventId.StartupTasksCompleted);
				this.parent.SatisfyTrigger(this);
			}
			return result;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.HQCelebrationScreenClosed || id == EventId.StartupTasksCompleted)
			{
				this.CheckHQLevelAndSatisfy();
			}
			return EatResponse.NotEaten;
		}

		private void AddHQLevelEventObserver()
		{
			if (!this.isSatisfied)
			{
				int highestLevelHQ = Service.BuildingLookupController.GetHighestLevelHQ();
				if (highestLevelHQ < this.hqLevelReq)
				{
					this.AddAppropriateHQUpgradeObserver();
				}
				else if (this.IsCelebrationScreenPending())
				{
					this.AddAppropriateHQUpgradeObserver();
				}
				else
				{
					this.eventManager.RegisterObserver(this, EventId.StartupTasksCompleted, EventPriority.Default);
				}
			}
		}

		private void AddAppropriateHQUpgradeObserver()
		{
			this.eventManager.RegisterObserver(this, EventId.HQCelebrationScreenClosed, EventPriority.Default);
		}

		private bool IsCelebrationScreenPending()
		{
			return Service.PopupsManager.ShowHQCelebrationPopup;
		}

		public override void Destroy()
		{
			base.Destroy();
		}
	}
}
