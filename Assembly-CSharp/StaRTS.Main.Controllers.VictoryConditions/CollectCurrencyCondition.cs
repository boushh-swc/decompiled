using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class CollectCurrencyCondition : AbstractCondition, IEventObserver
	{
		private const int AMOUNT_ARG = 0;

		private const int CURRENCY_TYPE_ARG = 1;

		private int currencyCollected;

		private int threshold;

		private CurrencyType currencyType;

		public CollectCurrencyCondition(ConditionVO vo, IConditionParent parent, int startingValue) : base(vo, parent)
		{
			this.currencyCollected = startingValue;
			this.threshold = Convert.ToInt32(this.prepareArgs[0]);
			this.currencyType = StringUtils.ParseEnum<CurrencyType>(this.prepareArgs[1]);
		}

		public override void Start()
		{
			this.events.RegisterObserver(this, EventId.CurrencyCollected, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.CurrencyCollected)
			{
				CurrencyCollectionTag currencyCollectionTag = cookie as CurrencyCollectionTag;
				if (currencyCollectionTag.Type == this.currencyType)
				{
					this.parent.ChildUpdated(this, currencyCollectionTag.Delta);
					this.IncrementCollection(currencyCollectionTag.Delta);
				}
			}
			return EatResponse.NotEaten;
		}

		private void IncrementCollection(int delta)
		{
			this.currencyCollected += delta;
			if (this.IsConditionSatisfied())
			{
				this.parent.ChildSatisfied(this);
			}
		}

		public override void Destroy()
		{
			this.events.UnregisterObserver(this, EventId.CurrencyCollected);
		}

		public override bool IsConditionSatisfied()
		{
			return this.currencyCollected >= this.threshold;
		}

		public override void GetProgress(out int current, out int total)
		{
			current = this.currencyCollected;
			total = this.threshold;
		}
	}
}
