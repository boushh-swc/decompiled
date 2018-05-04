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
	public class DestroyBuildingGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public DestroyBuildingGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			Service.EventManager.RegisterObserver(this, EventId.EntityKilled);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.EntityKilled)
			{
				if (this.IsEventValidForGoal())
				{
					Entity entity = (Entity)cookie;
					if (entity != null)
					{
						BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
						if (buildingComponent != null && buildingComponent.BuildingType != null && this.IsBuildingTypeValid(buildingComponent.BuildingType.Type))
						{
							this.parent.Progress(this, 1);
						}
					}
				}
			}
			return EatResponse.NotEaten;
		}

		private bool IsBuildingTypeValid(BuildingType type)
		{
			return type != BuildingType.Trap && type != BuildingType.Wall && type != BuildingType.Rubble && type != BuildingType.Clearable && type != BuildingType.Blocker;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.EntityKilled);
			base.Destroy();
		}
	}
}
