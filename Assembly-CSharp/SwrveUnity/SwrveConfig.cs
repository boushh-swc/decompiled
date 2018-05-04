using SwrveUnity.Messaging;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

namespace SwrveUnity
{
	[Serializable]
	public class SwrveConfig
	{
		public const string DefaultEventsServer = "https://api.swrve.com";

		public const string DefaultContentServer = "https://content.swrve.com";

		public string UserId;

		public string AppVersion;

		public string AppStore = "google";

		public string Language;

		public string DefaultLanguage = "en";

		public bool TalkEnabled = true;

		public bool ConversationsEnabled = true;

		public bool LocationEnabled;

		public bool LocationAutostart;

		public bool AutoDownloadCampaignsAndResources = true;

		public SwrveOrientation Orientation = SwrveOrientation.Both;

		public string EventsServer = "https://api.swrve.com";

		public bool UseHttpsForEventsServer = true;

		public string ContentServer = "https://content.swrve.com";

		public bool UseHttpsForContentServer = true;

		public bool AutomaticSessionManagement = true;

		public int NewSessionInterval = 30;

		public int MaxBufferChars = 262144;

		public bool SendEventsIfBufferTooLarge = true;

		public bool StoreDataInPlayerPrefs;

		public Stack SelectedStack;

		public bool PushNotificationEnabled;

		public HashSet<string> PushNotificationEvents = new HashSet<string>
		{
			"Swrve.session.start"
		};

		public string GCMSenderId;

		public string GCMPushNotificationTitle = "#Your App Title";

		public string GCMPushNotificationIconId;

		public string GCMPushNotificationMaterialIconId;

		public string GCMPushNotificationLargeIconId;

		public int GCMPushNotificationAccentColor = -1;

		public string ADMPushNotificationTitle = "#Your App Title";

		public string ADMPushNotificationIconId;

		public string ADMPushNotificationMaterialIconId;

		public string ADMPushNotificationLargeIconId;

		public int ADMPushNotificationAccentColor = -1;

		public AndroidPushProvider AndroidPushProvider;

		public float AutoShowMessagesMaxDelay = 5f;

		public Color? DefaultBackgroundColor;

		public bool LogGoogleAdvertisingId;

		public bool LogAndroidId;

		public bool LogAppleIDFV;

		public bool LogAppleIDFA;

		public List<UIUserNotificationCategory> pushCategories = new List<UIUserNotificationCategory>();

		public CultureInfo Culture
		{
			set
			{
				this.Language = value.Name;
			}
		}

		public void CalculateEndpoints(int appId)
		{
			if (string.IsNullOrEmpty(this.EventsServer) || this.EventsServer == "https://api.swrve.com")
			{
				this.EventsServer = SwrveConfig.CalculateEndpoint(this.UseHttpsForEventsServer, appId, this.SelectedStack, "api.swrve.com");
			}
			if (string.IsNullOrEmpty(this.ContentServer) || this.ContentServer == "https://content.swrve.com")
			{
				this.ContentServer = SwrveConfig.CalculateEndpoint(this.UseHttpsForContentServer, appId, this.SelectedStack, "content.swrve.com");
			}
		}

		private static string GetStackPrefix(Stack stack)
		{
			if (stack == Stack.EU)
			{
				return "eu-";
			}
			return string.Empty;
		}

		private static string HttpSchema(bool useHttps)
		{
			return (!useHttps) ? "http" : "https";
		}

		private static string CalculateEndpoint(bool useHttps, int appId, Stack stack, string suffix)
		{
			return string.Concat(new object[]
			{
				SwrveConfig.HttpSchema(useHttps),
				"://",
				appId,
				".",
				SwrveConfig.GetStackPrefix(stack),
				suffix
			});
		}
	}
}
