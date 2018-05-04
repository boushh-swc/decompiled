using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class TrainUnitCondition : AbstractCondition, IEventObserver
	{
		private const string ANY_STRING = "any";

		private const int AMOUNT_ARG = 0;

		private const int UNIT_MATCH_ARG = 1;

		private const int MIN_LEVEL_ARG = 2;

		private string unitMatch;

		private ConditionMatchType matchType;

		private int level;

		protected int unitsToTrain;

		protected int unitsTrained;

		private bool any;

		public TrainUnitCondition(ConditionVO vo, IConditionParent parent, int startingValue, ConditionMatchType matchType) : base(vo, parent)
		{
			this.matchType = matchType;
			this.unitMatch = this.prepareArgs[1];
			this.unitsToTrain = Convert.ToInt32(this.prepareArgs[0]);
			this.unitsTrained = startingValue;
			if (this.unitMatch == "any")
			{
				this.any = true;
			}
			if (matchType == ConditionMatchType.Uid)
			{
				this.level = Service.StaticDataController.Get<TroopTypeVO>(this.unitMatch).Lvl;
			}
			else if (!this.any)
			{
				this.level = Convert.ToInt32(this.prepareArgs[2]);
			}
		}

		public override void Start()
		{
			this.events.RegisterObserver(this, EventId.ContractCompleted, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ContractCompleted)
			{
				StaticDataController staticDataController = Service.StaticDataController;
				ContractEventData contractEventData = (ContractEventData)cookie;
				Contract contract = contractEventData.Contract;
				DeliveryType deliveryType = contract.DeliveryType;
				if (deliveryType == DeliveryType.Infantry || deliveryType == DeliveryType.Vehicle || deliveryType == DeliveryType.Mercenary)
				{
					TroopTypeVO troop = staticDataController.Get<TroopTypeVO>(contract.ProductUid);
					if (this.IsTroopValid(troop))
					{
						this.unitsTrained++;
						this.parent.ChildUpdated(this, 1);
						this.EvaluateAmount();
					}
				}
			}
			return EatResponse.NotEaten;
		}

		private bool IsTroopValid(TroopTypeVO troop)
		{
			if (this.any)
			{
				return true;
			}
			if (troop.Lvl >= this.level)
			{
				ConditionMatchType conditionMatchType = this.matchType;
				if (conditionMatchType == ConditionMatchType.Uid)
				{
					return troop.Uid == this.unitMatch;
				}
				if (conditionMatchType == ConditionMatchType.Id)
				{
					return troop.TroopID == this.unitMatch;
				}
				if (conditionMatchType == ConditionMatchType.Type)
				{
					return troop.Type == StringUtils.ParseEnum<TroopType>(this.unitMatch);
				}
			}
			return false;
		}

		protected virtual void EvaluateAmount()
		{
			if (this.IsConditionSatisfied())
			{
				this.parent.ChildSatisfied(this);
			}
		}

		public override void Destroy()
		{
			this.events.UnregisterObserver(this, EventId.ContractCompleted);
		}

		public override bool IsConditionSatisfied()
		{
			return this.unitsTrained >= this.unitsToTrain;
		}

		public override void GetProgress(out int current, out int total)
		{
			current = this.unitsTrained;
			total = this.unitsToTrain;
		}
	}
}
