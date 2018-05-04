using StaRTS.Assets;
using StaRTS.Externals.GameServices;
using StaRTS.Main.Controllers.Startup;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Cameras;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class ApplicationLoadState : IGameState, IState
	{
		private LoadingScreen loadingScreen;

		private StartupTaskController startup;

		public void OnEnter()
		{
			new WWWManager();
			Crittercism.SetLogUnhandledExceptionAsCrash(true);
			new EventManager();
			new CrashEventDataController();
			LangUtils.CreateLangService();
			new CameraManager();
			Service.CameraManager.MainCamera.Camera.enabled = false;
			new AssetManager();
			this.loadingScreen = new LoadingScreen();
			this.loadingScreen.Visible = true;
			this.InitStartupTasks();
		}

		public void OnExit(IState nextState)
		{
			Service.CameraManager.MainCamera.Camera.enabled = true;
			this.loadingScreen.Fade();
			this.loadingScreen = null;
			this.KillStartup();
		}

		public void KillStartup()
		{
			if (this.startup != null)
			{
				this.startup.KillStartup();
				this.startup = null;
			}
		}

		private void InitStartupTasks()
		{
			Service.EnvironmentController.SetupAutoRotation();
			this.startup = new StartupTaskController(new StartupTaskProgress(this.OnStartupTaskProgress), new StartupTaskComplete(this.OnStartupTaskComplete));
			this.startup.AddTask(new StaticInitStartupTask(0f));
			this.startup.AddTask(new SchedulingStartupTask(10f));
			this.startup.AddTask(new BIStartupTask(12f));
			this.startup.AddTask(new ExternalsStartupTask(13f));
			this.startup.AddTask(new ServerStartupTask(15f));
			this.startup.AddTask(new EndpointStartupTask(16f));
			this.startup.AddTask(new PlayerIdentityStartupTask(17f));
			this.startup.AddTask(new PlayerLoginStartupTask(18f));
			this.startup.AddTask(new PlayerContentStartupTask(22f));
			this.startup.AddTask(new PlayerSquadStartupTask(23f));
			this.startup.AddTask(new AssetStartupTask(25f));
			this.startup.AddTask(new MobileConnectorAdsStartupTask(27f));
			this.startup.AddTask(new PreloadStartupTask(28f));
			this.startup.AddTask(new GameDataStartupTask(29f));
			this.startup.AddTask(new AudioStartupTask(35f));
			this.startup.AddTask(new DamageStartupTask(40f));
			this.startup.AddTask(new LangStartupTask(50f));
			this.startup.AddTask(new UserInputStartupTask(53f));
			this.startup.AddTask(new ShowLoadingScreenPopupsStartupTask(60f));
			this.startup.AddTask(new BoardStartupTask(65f));
			this.startup.AddTask(new GeneralStartupTask(70f));
			this.startup.AddTask(new WorldStartupTask(75f));
			this.startup.AddTask(new DonePreloadingStartupTask(95f));
			this.startup.AddTask(new PlacementStartupTask(96f));
			this.startup.AddTask(new HomeStartupTask(97f));
			this.startup.AddTask(new EditBaseStartupTask(98f));
		}

		public void OnStartupTaskProgress(float percentage, string description)
		{
			this.loadingScreen.Progress(percentage, description);
			if (Service.CurrentPlayer != null)
			{
				string playerId = Service.CurrentPlayer.PlayerId;
				if (!string.IsNullOrEmpty(playerId))
				{
					this.loadingScreen.SetPlayerId(playerId);
				}
			}
		}

		private void OnStartupTaskComplete()
		{
			Service.SquadController.ServerManager.EnablePolling();
			GameServicesManager.Startup();
			if (!Service.CurrentPlayer.HasNotCompletedFirstFueStep())
			{
				GameServicesManager.OnReady();
			}
			if (!Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.ISocialDataController.CheckFacebookLoginOnStartup();
			}
			Service.WorldInitializer.View.StartMapManipulation();
			Service.UserInputManager.Enable(true);
			Service.UXController.HUD.ReadyToToggleVisiblity = true;
			Service.RUFManager.PrepareReturningUserFlow();
			Service.EventManager.SendEvent(EventId.StartupTasksCompleted, null);
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
