using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Bot
{
	public class BPEndRandomPvp : BotPerformer, IEventObserver
	{
		public override void Perform()
		{
			Service.BotRunner.Performing = true;
			Service.BotRunner.Log("Ending pvp", new object[0]);
			Service.EventManager.RegisterObserver(this, EventId.ScreenLoaded);
			Service.BattleController.CancelBattleRightAway();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ScreenLoaded)
			{
				if (id == EventId.WorldInTransitionComplete)
				{
					Service.EventManager.UnregisterObserver(this, EventId.WorldInTransitionComplete);
					Service.BotRunner.Performing = false;
					base.Perform();
				}
			}
			else if (cookie is BattleEndScreen)
			{
				Service.EventManager.UnregisterObserver(this, EventId.ScreenLoaded);
				Service.EventManager.RegisterObserver(this, EventId.WorldInTransitionComplete);
				HomeState.GoToHomeStateAndReloadMap();
			}
			return EatResponse.NotEaten;
		}
	}
}
