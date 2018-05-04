using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class TrainTroopIdGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private string troopId;

		public TrainTroopIdGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			this.troopId = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(this.troopId))
			{
				Service.Logger.ErrorFormat("Troop ID not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			Service.EventManager.RegisterObserver(this, EventId.TroopRecruited);
			Service.EventManager.RegisterObserver(this, EventId.HeroMobilized);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.TroopRecruited:
			case EventId.HeroMobilized:
			{
				ContractEventData contractEventData = cookie as ContractEventData;
				StaticDataController staticDataController = Service.StaticDataController;
				TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(contractEventData.Contract.ProductUid);
				if (troopTypeVO.TroopID == this.troopId)
				{
					this.parent.Progress(this, 1);
				}
				break;
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
