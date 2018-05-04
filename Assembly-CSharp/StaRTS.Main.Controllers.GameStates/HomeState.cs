using StaRTS.Main.Controllers.World;
using StaRTS.Main.Controllers.World.Transitions;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.World;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class HomeState : IGameState, IState
	{
		private TransitionCompleteDelegate pendingCompletion;

		private bool transitionToHomeState;

		public bool ForceReloadMap
		{
			get;
			private set;
		}

		private HomeState()
		{
			this.transitionToHomeState = false;
			this.ForceReloadMap = false;
		}

		public static bool GoToHomeState(TransitionCompleteDelegate onComplete, bool zoom)
		{
			Service.DeployerController.ExitAllDeployModes();
			Service.PvpManager.KillTimer();
			if (zoom)
			{
				PlanetView view = Service.WorldInitializer.View;
				view.ZoomOutImmediate();
				view.ZoomIn();
			}
			HomeState homeState = new HomeState();
			bool result = homeState.Setup(onComplete);
			Service.AssetManager.UnloadPreloadables();
			return result;
		}

		public static bool GoToHomeStateAndReloadMap()
		{
			return new HomeState
			{
				ForceReloadMap = true
			}.Setup(null);
		}

		private bool Setup(TransitionCompleteDelegate onComplete)
		{
			this.pendingCompletion = onComplete;
			HomeMapDataLoader homeMapDataLoader = Service.HomeMapDataLoader;
			GameStateMachine gameStateMachine = Service.GameStateMachine;
			bool flag = gameStateMachine.CurrentState is WarBoardState;
			this.transitionToHomeState = (!Service.WorldTransitioner.IsCurrentWorldHome() || flag);
			if (this.transitionToHomeState)
			{
				AbstractTransition transition;
				if (flag)
				{
					transition = new WarboardToBaseTransition(this, homeMapDataLoader, this.pendingCompletion);
				}
				else
				{
					transition = new WorldToWorldTransition(this, homeMapDataLoader, this.pendingCompletion, false, true);
				}
				Service.WorldTransitioner.StartTransition(transition);
				this.pendingCompletion = null;
			}
			else
			{
				gameStateMachine.SetState(this);
			}
			return this.transitionToHomeState;
		}

		public void OnEnter()
		{
			HudConfig config = new HudConfig(new string[]
			{
				"Currency",
				"Droids",
				"Crystals",
				"PlayerInfo",
				"Shield",
				"ButtonBattle",
				"ButtonWar",
				"ButtonLog",
				"ButtonLeaderboard",
				"ButtonSettings",
				"ButtonClans",
				"ButtonStore",
				"Newspaper",
				"SquadScreen",
				"SpecialPromo"
			});
			Service.UXController.HUD.ConfigureControls(config);
			if (!this.transitionToHomeState)
			{
				if (Service.WorldTransitioner.IsCurrentWorldHome() && this.ForceReloadMap)
				{
					Service.ProjectileViewManager.UnloadProjectileAssetsAndPools();
					Service.SpecialAttackController.UnloadPreloads();
					Service.WorldInitializer.ProcessMapData(Service.CurrentPlayer.Map);
					Service.EventManager.SendEvent(EventId.HomeStateTransitionComplete, null);
				}
				else
				{
					Service.EventManager.SendEvent(EventId.WorldInTransitionComplete, null);
				}
				if (this.pendingCompletion != null)
				{
					this.pendingCompletion();
					this.pendingCompletion = null;
				}
			}
			Service.BuildingController.EnterSelectMode();
			Service.DeployerController.ExitAllDeployModes();
			Service.BILoggingController.SchedulePerformanceLogging(true);
			Service.InventoryCrateRewardController.ScheduleGivingNextDailyCrate();
		}

		public void OnExit(IState nextState)
		{
			Service.BuildingController.ExitAllModes();
			Service.BILoggingController.UnschedulePerformanceLogging();
			Service.InventoryCrateRewardController.CancelDailyCrateScheduledTimer();
		}

		public bool CanUpdateHomeContracts()
		{
			return true;
		}
	}
}
