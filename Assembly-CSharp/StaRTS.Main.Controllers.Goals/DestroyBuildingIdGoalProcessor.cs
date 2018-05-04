using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class DestroyBuildingIdGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private string buildingID;

		public DestroyBuildingIdGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			this.buildingID = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(this.buildingID))
			{
				Service.Logger.ErrorFormat("Building ID not found for goal {0}", new object[]
				{
					vo.Uid
				});
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
					if (buildingComponent != null && buildingComponent.BuildingType.BuildingID == this.buildingID)
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
