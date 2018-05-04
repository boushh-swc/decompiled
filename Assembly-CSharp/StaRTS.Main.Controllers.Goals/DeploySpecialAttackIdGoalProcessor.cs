using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class DeploySpecialAttackIdGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private string specialAttackID;

		public DeploySpecialAttackIdGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			this.specialAttackID = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(this.specialAttackID))
			{
				Service.Logger.ErrorFormat("Special Attack ID not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			Service.EventManager.RegisterObserver(this, EventId.SpecialAttackSpawned);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SpecialAttackSpawned)
			{
				if (this.IsEventValidForGoal())
				{
					SpecialAttack specialAttack = (SpecialAttack)cookie;
					if (specialAttack.VO.SpecialAttackID == this.specialAttackID && specialAttack.PlayerDeployed)
					{
						this.parent.Progress(this, 1);
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
