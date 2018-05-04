using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class TrainSpecialAttackGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public TrainSpecialAttackGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			Service.EventManager.RegisterObserver(this, EventId.StarshipMobilized);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.StarshipMobilized)
			{
				if (this.IsEventValidForGoal())
				{
					ContractEventData contractEventData = (ContractEventData)cookie;
					StaticDataController staticDataController = Service.StaticDataController;
					SpecialAttackTypeVO optional = staticDataController.GetOptional<SpecialAttackTypeVO>(contractEventData.Contract.ProductUid);
					if (optional != null)
					{
						this.parent.Progress(this, optional.Size);
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.StarshipMobilized);
			base.Destroy();
		}
	}
}
