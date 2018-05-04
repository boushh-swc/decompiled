using StaRTS.Main.Models;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class UnlockPlanetTrigger : AbstractStoryTrigger, IEventObserver
	{
		public UnlockPlanetTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ContractCompletedForStoryAction)
			{
				ContractTO contractTO = (ContractTO)cookie;
				if (contractTO.ContractType == ContractType.Upgrade)
				{
					BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(contractTO.Uid);
					if (buildingTypeVO.Type == BuildingType.NavigationCenter)
					{
						this.CheckSatisfyTrigger(contractTO.Tag);
					}
				}
			}
			else if (id == EventId.PlanetUnlocked)
			{
				this.CheckSatisfyTrigger((string)cookie);
			}
			return EatResponse.NotEaten;
		}

		private void CheckSatisfyTrigger(string uid)
		{
			if (string.IsNullOrEmpty(this.vo.PrepareString) || this.vo.PrepareString.Equals(uid))
			{
				this.UnregisterObservers();
				this.parent.SatisfyTrigger(this);
			}
		}

		public override void Activate()
		{
			base.Activate();
			Service.EventManager.RegisterObserver(this, EventId.ContractCompletedForStoryAction, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.PlanetUnlocked, EventPriority.Default);
		}

		private void UnregisterObservers()
		{
			Service.EventManager.UnregisterObserver(this, EventId.ContractCompletedForStoryAction);
			Service.EventManager.UnregisterObserver(this, EventId.PlanetUnlocked);
		}
	}
}
