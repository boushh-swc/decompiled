using StaRTS.Assets;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UserInput;
using StaRTS.Main.Views.UX;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class UserInputStartupTask : StartupTask, IEventObserver
	{
		public UserInputStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			Service.UserInputManager.Init();
			Service.EventManager.RegisterObserver(this, EventId.HudComplete, EventPriority.Default);
			new ScreenController();
			new PromoPopupManager();
			new UXController();
			new UserInputInhibitor();
			new EpisodeWidgetViewController();
			new BackButtonManager();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.HudComplete)
			{
				Service.EventManager.UnregisterObserver(this, EventId.HudComplete);
				if (!(cookie is HUD))
				{
					throw new Exception("Unable to load the HUD");
				}
				Service.ScreenController.PreloadAndCacheScreens(new AssetsCompleteDelegate(this.OnPreloadAndCacheScreensComplete), null);
			}
			return EatResponse.NotEaten;
		}

		private void OnPreloadAndCacheScreensComplete(object cookie)
		{
			Service.AssetManager.UnloadDependencyBundle("gui_shared");
			base.Complete();
		}
	}
}
