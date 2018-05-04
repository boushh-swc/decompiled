using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class TrainSpecialAttackIdGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private string specialAttackID;

		public TrainSpecialAttackIdGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			this.specialAttackID = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(this.specialAttackID))
			{
				Service.Logger.ErrorFormat("Special Attack ID not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
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
					SpecialAttackTypeVO specialAttackTypeVO = staticDataController.Get<SpecialAttackTypeVO>(contractEventData.Contract.ProductUid);
					if (specialAttackTypeVO.SpecialAttackID == this.specialAttackID)
					{
						this.parent.Progress(this, 1);
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
