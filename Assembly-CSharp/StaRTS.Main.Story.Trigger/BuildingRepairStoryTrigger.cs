using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Story.Trigger
{
	public class BuildingRepairStoryTrigger : AbstractStoryTrigger, IEventObserver
	{
		private const int BUILDING_ID_ARG = 0;

		private BuildingType buildingType;

		private EventId eventToListenFor;

		public BuildingRepairStoryTrigger(StoryTriggerVO vo, ITriggerReactor parent) : base(vo, parent)
		{
		}

		public override void Activate()
		{
			base.Activate();
			if (this.prepareArgs.Length > 0)
			{
				this.buildingType = StringUtils.ParseEnum<BuildingType>(this.prepareArgs[0]);
				this.eventToListenFor = EventId.EntityPostBattleRepairFinished;
			}
			else
			{
				this.eventToListenFor = EventId.AllPostBattleRepairFinished;
			}
			Service.EventManager.RegisterObserver(this, this.eventToListenFor, EventPriority.Default);
		}

		private void RemoveListeners()
		{
			Service.EventManager.UnregisterObserver(this, this.eventToListenFor);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.EntityPostBattleRepairFinished)
			{
				if (id == EventId.AllPostBattleRepairFinished)
				{
					this.RemoveListeners();
					this.parent.SatisfyTrigger(this);
				}
			}
			else
			{
				Entity entity = cookie as Entity;
				BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
				if (buildingComponent.BuildingType.Type == this.buildingType)
				{
					this.RemoveListeners();
					this.parent.SatisfyTrigger(this);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			this.RemoveListeners();
			base.Destroy();
		}
	}
}
