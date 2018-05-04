using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class DestroyBuildingTypeGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private BuildingType buildingType;

		public DestroyBuildingTypeGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			string goalItem = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(goalItem))
			{
				Service.Logger.ErrorFormat("Building Type not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			else
			{
				this.buildingType = StringUtils.ParseEnum<BuildingType>(goalItem);
			}
			Service.EventManager.RegisterObserver(this, EventId.EntityKilled);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.EntityKilled)
			{
				if (this.IsEventValidForGoal())
				{
					Entity entity = (Entity)cookie;
					BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
					if (buildingComponent != null && buildingComponent.BuildingType.Type == this.buildingType)
					{
						this.parent.Progress(this, 1);
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.EntityKilled);
			base.Destroy();
		}
	}
}
