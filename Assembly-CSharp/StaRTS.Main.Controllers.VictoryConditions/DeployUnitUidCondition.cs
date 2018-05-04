using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class DeployUnitUidCondition : AbstractCondition, IEventObserver
	{
		private const int AMOUNT_ARG = 0;

		private const int UNIT_UID_ARG = 1;

		private string unitMatchUid;

		protected int unitsToDeploy;

		protected int unitsDeployed;

		public DeployUnitUidCondition(ConditionVO vo, IConditionParent parent) : base(vo, parent)
		{
			this.unitMatchUid = this.prepareArgs[1];
			this.unitsToDeploy = Convert.ToInt32(this.prepareArgs[0]);
			this.unitsDeployed = 0;
		}

		public override void Start()
		{
			this.events.RegisterObserver(this, EventId.TroopDeployed, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.TroopDeployed)
			{
				Entity entity = (Entity)cookie;
				TroopComponent troopComponent = entity.Get<TroopComponent>();
				if (troopComponent != null && this.IsTroopValid(troopComponent))
				{
					this.unitsDeployed++;
					this.EvaluateAmount();
				}
			}
			return EatResponse.NotEaten;
		}

		private bool IsTroopValid(TroopComponent component)
		{
			return component.TroopType.Uid == this.unitMatchUid;
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
			this.events.UnregisterObserver(this, EventId.TroopDeployed);
		}

		public override bool IsConditionSatisfied()
		{
			return this.unitsDeployed >= this.unitsToDeploy;
		}

		public override void GetProgress(out int current, out int total)
		{
			current = this.unitsDeployed;
			total = this.unitsToDeploy;
		}
	}
}
