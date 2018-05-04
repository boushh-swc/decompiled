using StaRTS.Main.Controllers.Entities.StateMachines.Attack;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class StunController : IEventObserver
	{
		public StunController()
		{
			Service.StunController = this;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ProcBuff);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			BuffEventData buffEventData = (BuffEventData)cookie;
			if (buffEventData.BuffObj.BuffType.Modify != BuffModify.Stun)
			{
				return EatResponse.NotEaten;
			}
			AttackFSM attackFSM = null;
			if (buffEventData.Target.ShooterComp != null)
			{
				attackFSM = buffEventData.Target.ShooterComp.AttackFSM;
			}
			if (attackFSM != null)
			{
				attackFSM.Stun(buffEventData.BuffObj.BuffType.Duration);
			}
			return EatResponse.NotEaten;
		}
	}
}
