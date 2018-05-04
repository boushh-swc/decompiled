using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class TrainTroopGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public TrainTroopGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			Service.EventManager.RegisterObserver(this, EventId.TroopRecruited);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.TroopRecruited)
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				StaticDataController staticDataController = Service.StaticDataController;
				TroopTypeVO optional = staticDataController.GetOptional<TroopTypeVO>(contractEventData.Contract.ProductUid);
				if (optional != null)
				{
					this.parent.Progress(this, optional.Size);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.TroopRecruited);
			base.Destroy();
		}
	}
}
