using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class AppServerEnvironmentController
	{
		public const string EVENT_2_CLIENT_BI_LOGGING_URL = "https://starts-bi-prod.disney.io/bi_event2";

		public const string EVENT_2_NO_PROXY_CLIENT_BI_LOGGING_URL = "https://di-dtaectolog-us-prod-1.appspot.com/events2/mob";

		public const string EVENT_2_AUTHORIZATION_URL = "FD B276DCA8-9CD5-4493-85D6-75D2E500BCC9:19042B0A0628EF36039165C24A240F78F6EDBDFB8BD86BA3";

		public const string REMOTE_PRIMARY = "https://starts-app-prod.disney.io/starts";

		public const string REMOTE_SECONDARY = "https://starts-integration-prod.disney.io/starts";

		public const string LOCAL = "http://localhost:8080/starts";

		private const string PREF_ENVIRONMENT = "Environment";

		public static readonly string[] ENVIRONMENT_NAMES_NONPROD = new string[]
		{
			"DEV01",
			"DEV02",
			"DEV03",
			"DEV04",
			"DEV05",
			"DEV06",
			"DEV07",
			"DEV08",
			"QA01",
			"QA02",
			"STAGING",
			"STAGE02"
		};

		public static readonly string[] ENVIRONMENT_NAMES_PROD = new string[]
		{
			"PRODUCTION",
			"PRODUCTION_INACTIVE",
			"INTEGRATION"
		};

		public string Server
		{
			get;
			private set;
		}

		public AppServerEnvironmentController()
		{
			Service.AppServerEnvironmentController = this;
			this.Server = AppServerEnvironmentController.GetCompileTimeServer();
			this.OverrideEnvironment();
		}

		private static string GetCompileTimeServer()
		{
			return "https://starts-app-prod.disney.io/starts";
		}

		public static bool IsLocalServer()
		{
			return AppServerEnvironmentController.GetCompileTimeServer() == "http://localhost:8080/starts";
		}

		public static string GetCpipeUser()
		{
			Service.Logger.Error("Trying to get Cpipe service user for a prod environment.");
			return null;
		}

		public static string GetPhotonAppId()
		{
			return "810b521d-7d01-4ecf-bc0c-2831d3e27174";
		}

		public static string GetCpipeKeyFileName()
		{
			Service.Logger.Error("Trying to get Cpipe key file for a prod environment.");
			return string.Empty;
		}

		private void OverrideEnvironment()
		{
		}

		private string GetEnvironmentOverridePreference()
		{
			return PlayerPrefs.GetString("Environment", AppServerEnvironmentOverride.DEFAULT.ToString());
		}

		public AppServerEnvironmentOverride GetEnvironmentOverride()
		{
			string environmentOverridePreference = this.GetEnvironmentOverridePreference();
			return AppServerEnvironmentController.GetAppServerEnvironmentOverrideFromString(environmentOverridePreference);
		}

		public void SetEnvironmentOverride(string environment)
		{
			environment = environment.ToUpper();
			if (!this.IsValidEnvironmentOverride(environment))
			{
				environment = AppServerEnvironmentOverride.DEFAULT.ToString();
			}
			PlayerPrefs.SetString("Environment", environment);
		}

		private static AppServerEnvironmentOverride GetAppServerEnvironmentOverrideFromString(string environmentOverrideString)
		{
			return (AppServerEnvironmentOverride)((int)Enum.Parse(typeof(AppServerEnvironmentOverride), environmentOverrideString.ToUpper()));
		}

		public bool IsValidEnvironmentOverride(string environment)
		{
			environment = environment.ToUpper();
			return Enum.IsDefined(typeof(AppServerEnvironmentOverride), environment) && AppServerEnvironmentController.GetAppServerEnvironmentOverrideFromString(environment) == AppServerEnvironmentOverride.DEFAULT;
		}

		public bool IsEnvironmentOverrideSupported()
		{
			return false;
		}

		public static string GetEvent2ClientBILoggingUrl()
		{
			return "https://starts-bi-prod.disney.io/bi_event2";
		}
	}
}
