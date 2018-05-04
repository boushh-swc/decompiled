using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class DeploySpecialAttackGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public DeploySpecialAttackGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			Service.EventManager.RegisterObserver(this, EventId.SpecialAttackSpawned);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SpecialAttackSpawned)
			{
				if (this.IsEventValidForGoal())
				{
					SpecialAttack specialAttack = (SpecialAttack)cookie;
					if (specialAttack.TeamType == TeamType.Attacker && specialAttack.PlayerDeployed)
					{
						this.parent.Progress(this, specialAttack.VO.Size);
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.SpecialAttackSpawned);
			base.Destroy();
		}
	}
}
