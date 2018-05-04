using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Bot
{
	public class BPStartRandomPvp : BotPerformer, IEventObserver
	{
		public override void Perform()
		{
			Service.BotRunner.Performing = true;
			Service.BotRunner.Log("Preparing to fight random pvp", new object[0]);
			Service.EventManager.RegisterObserver(this, EventId.GameStateChanged);
			Service.PvpManager.PurchaseNextBattle();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.GameStateChanged)
			{
				IState currentState = Service.GameStateMachine.CurrentState;
				if (currentState is BattleStartState)
				{
					Service.EventManager.UnregisterObserver(this, EventId.GameStateChanged);
					Service.ViewTimerManager.CreateViewTimer(2f, false, new TimerDelegate(this.DelayComplete), null);
				}
			}
			return EatResponse.NotEaten;
		}

		private void DelayComplete(uint id, object cookie)
		{
			Service.BotRunner.Performing = false;
			base.Perform();
		}
	}
}
