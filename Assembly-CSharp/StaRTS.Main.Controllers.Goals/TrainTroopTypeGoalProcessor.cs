using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class TrainTroopTypeGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private DeliveryType deliveryType;

		public TrainTroopTypeGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			TroopType troopType = TroopType.Invalid;
			string goalItem = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(goalItem))
			{
				Service.Logger.ErrorFormat("Troop Type not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			else
			{
				troopType = StringUtils.ParseEnum<TroopType>(goalItem);
			}
			if (troopType == TroopType.Champion)
			{
				this.deliveryType = DeliveryType.Champion;
			}
			else if (troopType == TroopType.Hero)
			{
				this.deliveryType = DeliveryType.Hero;
			}
			else if (troopType == TroopType.Infantry)
			{
				this.deliveryType = DeliveryType.Infantry;
			}
			else if (troopType == TroopType.Mercenary)
			{
				this.deliveryType = DeliveryType.Mercenary;
			}
			else if (troopType == TroopType.Vehicle)
			{
				this.deliveryType = DeliveryType.Vehicle;
			}
			Service.EventManager.RegisterObserver(this, EventId.TroopRecruited);
			Service.EventManager.RegisterObserver(this, EventId.HeroMobilized);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.TroopRecruited || id == EventId.HeroMobilized)
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				if (this.deliveryType == contractEventData.Contract.DeliveryType)
				{
					this.parent.Progress(this, 1);
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.TroopRecruited);
			Service.EventManager.UnregisterObserver(this, EventId.HeroMobilized);
			base.Destroy();
		}
	}
}
