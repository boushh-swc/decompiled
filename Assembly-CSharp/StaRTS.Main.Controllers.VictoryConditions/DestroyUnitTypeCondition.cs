using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class DestroyUnitTypeCondition : AbstractCondition, IEventObserver
	{
		private const int AMOUNT_ARG = 0;

		private const int UNIT_TYPE_ARG = 1;

		private TroopType unitMatchType;

		private TeamType teamMatchType;

		protected int unitsToKill;

		protected int unitsKilled;

		public DestroyUnitTypeCondition(ConditionVO vo, IConditionParent parent) : base(vo, parent)
		{
			this.unitMatchType = StringUtils.ParseEnum<TroopType>(this.prepareArgs[1]);
			this.unitsToKill = Convert.ToInt32(this.prepareArgs[0]);
			this.unitsKilled = 0;
		}

		public override void Start()
		{
			this.teamMatchType = Service.BattleController.CurrentPlayerTeamType;
			this.events.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.EntityKilled)
			{
				Entity entity = (Entity)cookie;
				TroopComponent troopComponent = entity.Get<TroopComponent>();
				if (troopComponent != null && this.IsTroopValid(troopComponent))
				{
					this.unitsKilled++;
					this.EvaluateAmount();
				}
			}
			return EatResponse.NotEaten;
		}

		private bool IsTroopValid(TroopComponent component)
		{
			return component.TroopType.Type == this.unitMatchType && component.Entity.Get<TeamComponent>().TeamType != this.teamMatchType;
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
			this.events.UnregisterObserver(this, EventId.EntityKilled);
		}

		public override bool IsConditionSatisfied()
		{
			return this.unitsKilled >= this.unitsToKill;
		}

		public override void GetProgress(out int current, out int total)
		{
			current = this.unitsKilled;
			total = this.unitsToKill;
		}
	}
}
