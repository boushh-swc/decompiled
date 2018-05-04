using StaRTS.Externals.GameServices;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Animations;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class IntroCameraState : IGameState, IEventObserver, IState
	{
		private IntroCameraAnimation animation;

		public void OnEnter()
		{
			Service.EventManager.RegisterObserver(this, EventId.IntroComplete, EventPriority.Default);
			Service.EventManager.SendEvent(EventId.PurgeHomeStateRUFTask, null);
			this.animation = Service.UXController.Intro;
			this.animation.Start();
		}

		public void OnExit(IState nextState)
		{
			this.Done();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.IntroComplete)
			{
				this.Done();
			}
			return EatResponse.NotEaten;
		}

		private void Done()
		{
			Service.EventManager.UnregisterObserver(this, EventId.IntroComplete);
			if (this.animation != null)
			{
				this.animation = null;
				Service.UXController.Intro = null;
				Service.UXController.HUD.ConfigureControls(new HudConfig(new string[0]));
				Service.UXController.HUD.Visible = true;
				if (Service.CurrentPlayer.HasNotCompletedFirstFueStep() && GameConstants.START_FUE_IN_BATTLE_MODE)
				{
					Service.GameStateMachine.SetState(new FueBattleStartState(GameConstants.FUE_BATTLE));
					Service.BattleController.PrepareWorldForBattle();
				}
				else
				{
					GameServicesManager.OnReady();
					HomeState.GoToHomeState(null, true);
				}
			}
		}

		public bool CanUpdateHomeContracts()
		{
			return true;
		}
	}
}
