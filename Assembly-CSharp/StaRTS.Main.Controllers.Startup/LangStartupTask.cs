using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Startup
{
	public class LangStartupTask : StartupTask
	{
		public LangStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			Lang lang = Service.Lang;
			string pref = Service.ServerPlayerPrefs.GetPref(ServerPref.Locale);
			if (pref != lang.Locale)
			{
				lang.Locale = pref;
				LangUtils.AddLocalStringsData(pref);
			}
			LangUtils.LoadStringData(new AssetsCompleteDelegate(this.OnComplete));
			Service.EventManager.SendEvent(EventId.StringsLoadStart, null);
		}

		private void OnComplete(object cookie)
		{
			Lang lang = Service.Lang;
			lang.UnloadAssets();
			lang.SetupAvailableLocales(GameConstants.ALL_LOCALES, lang.Get("ALL_LANGUAGES", new object[0]));
			lang.Initialized = true;
			Service.EventManager.SendEvent(EventId.LanguageLoaded, null);
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (string.IsNullOrEmpty(currentPlayer.PlayerName))
			{
				currentPlayer.PlayerName = Service.Lang.Get("general_none", new object[0]);
				currentPlayer.PlayerNameInvalid = true;
			}
			new ProfanityController();
			base.Complete();
			Service.EventManager.SendEvent(EventId.StringsLoadEnd, null);
		}
	}
}
