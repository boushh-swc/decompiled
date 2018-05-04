using StaRTS.Main.Controllers;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections;
using System.Diagnostics;
using UnityEngine;

namespace StaRTS.Main.Utils
{
	public class NetworkConnectionTester : IEventObserver
	{
		private const string NO_INTERNET_ERROR_PREFIX = "Could not resolve host";

		private const string CDN_URL = "https://d50ea5a0.content.disney.io/";

		private const string TEST_ASSET = "1490399447/connection_test.txt";

		private Engine engine;

		public NetworkConnectionTester()
		{
			Service.EventManager.RegisterObserver(this, EventId.ApplicationPauseToggled, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.BattleEndFullyProcessed, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.ApplicationPauseToggled)
			{
				this.CheckNetworkConnectionAvailable((bool)cookie);
			}
			return EatResponse.NotEaten;
		}

		public void CheckNetworkConnectionAvailable(bool paused)
		{
			if (!paused)
			{
				this.engine = Service.Engine;
				string url = "https://d50ea5a0.content.disney.io/1490399447/connection_test.txt?r=" + StringUtils.GenerateRandom(32u);
				this.engine.StartCoroutine(this.Download(url));
			}
		}

		[DebuggerHidden]
		private IEnumerator Download(string url)
		{
			WWW wWW = new WWW(url);
			WWWManager.Add(wWW);
			yield return wWW;
			if (WWWManager.Remove(wWW))
			{
				if (!string.IsNullOrEmpty(wWW.error) && wWW.error.StartsWith("Could not resolve host"))
				{
					Lang lang = Service.Lang;
					AlertScreen.ShowModal(true, lang.Get("NO_INTERNET_TITLE", new object[0]), lang.Get("NO_INTERNET", new object[0]), null, null);
				}
				wWW.Dispose();
			}
			yield break;
		}
	}
}
