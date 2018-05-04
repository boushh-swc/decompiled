using DCPI.Platforms.SwrveManager.Analytics;
using DCPI.Platforms.SwrveManager.Utils;
using SwrveUnity;
using SwrveUnity.ResourceManager;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace DCPI.Platforms.SwrveManager
{
	public class SwrveManager : MonoBehaviour, ISwrveManager
	{
		private HashSet<string> EventsSent = new HashSet<string>();

		private const int SWRVE_MAX_PAYLOAD = 500;

		private const int SWRVE_MAX_EVENT_NAMES = 1000;

		private const string SWRVE_MANAGER_LIBVERSION = "1.1.8";

		public static SwrveManager instance;

		private SwrveResourceManager resourceManager;

		private Dictionary<string, string> customUserData;

		private string userId;

		private static bool _isDebugEnvLog;

		public SwrveResourceManager SwrveResourceManager
		{
			get
			{
				return this.resourceManager;
			}
		}

		public string AnalyticsId
		{
			get;
			private set;
		}

		public string AnalyticsKey
		{
			get;
			private set;
		}

		public static bool DebugLogging
		{
			get
			{
				return SwrveManager._isDebugEnvLog;
			}
			set
			{
				SwrveManager._isDebugEnvLog = true;
			}
		}

		private void Awake()
		{
			if (SwrveManager.instance == null)
			{
				SwrveManager.instance = this;
				UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
				SwrveComponent swrveComponent = base.gameObject.AddComponent<SwrveComponent>();
				swrveComponent.InitialiseOnStart = false;
			}
			else if (SwrveManager.instance != this)
			{
				UnityEngine.Object.Destroy(base.gameObject);
			}
		}

		public SwrveSDK GetSDK()
		{
			return SwrveComponent.Instance.SDK;
		}

		public SwrveConfig GetSwrveConfig()
		{
			return SwrveComponent.Instance.Config;
		}

		public static void Log(string msg)
		{
			if (SwrveManager._isDebugEnvLog)
			{
				Debug.Log("SwrveManager: " + msg);
			}
		}

		public void InitWithAnalyticsKeySecretConfigAndCustomData(int appId, string apiKey, SwrveConfig customConfig = null, Dictionary<string, string> customData = null)
		{
			if (customData != null)
			{
				this.customUserData = customData;
			}
			this.InitWithAnalyticsKeySecretAndConfig(appId, apiKey, customConfig);
		}

		public void InitWithAnalyticsKeySecretAndConfig(int appId, string apiKey, SwrveConfig customConfig)
		{
			if (customConfig != null)
			{
				SwrveComponent.Instance.Config = customConfig;
			}
			this.InitWithAnalyticsKeySecret(appId, apiKey);
		}

		public void InitWithAnalyticsKeySecret(int appId, string apiKey)
		{
			this.AnalyticsId = appId.ToString();
			this.AnalyticsKey = apiKey;
			SwrveManager.Log("Init with appId " + this.AnalyticsId + " and apiKey: " + this.AnalyticsKey);
			SwrveComponent.Instance.Config.UseHttpsForEventsServer = true;
			SwrveComponent.Instance.Config.UseHttpsForContentServer = true;
			SwrveComponent.Instance.Config.SendEventsIfBufferTooLarge = true;
			SwrveComponent.Instance.Config.AutomaticSessionManagement = true;
			if (string.IsNullOrEmpty(SwrveComponent.Instance.Config.AppVersion))
			{
				SwrveComponent.Instance.Config.AppVersion = Application.version;
			}
			this.FinishSwrveSDKInit();
		}

		private void FinishSwrveSDKInit()
		{
			SwrveComponent.Instance.Init(int.Parse(this.AnalyticsId), this.AnalyticsKey);
			SwrveConfig config = SwrveComponent.Instance.Config;
			if (config.AndroidPushProvider == AndroidPushProvider.GOOGLE_GCM && config.PushNotificationEnabled && !string.IsNullOrEmpty(config.GCMSenderId))
			{
				SwrveManagerUtils.RegisterGCMDevice(SwrveComponent.Instance.name, config.GCMSenderId, config.GCMPushNotificationTitle, config.GCMPushNotificationIconId, config.GCMPushNotificationMaterialIconId, config.GCMPushNotificationLargeIconId, config.GCMPushNotificationAccentColor, config.GCMSenderId.ToLower() + "Group");
			}
			Dictionary<string, string> deviceInfo = SwrveComponent.Instance.SDK.GetDeviceInfo();
			string text = deviceInfo["swrve.device_name"];
			string value = deviceInfo["swrve.os"];
			string value2 = deviceInfo["swrve.device_dpi"];
			string value3 = deviceInfo["swrve.device_width"];
			string value4 = deviceInfo["swrve.device_height"];
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			dictionary.Add("device_name", text);
			dictionary.Add("os", value);
			dictionary.Add("device_dpi", value2);
			dictionary.Add("device_width", value3);
			dictionary.Add("device_height", value4);
			if (!string.IsNullOrEmpty(SwrveComponent.Instance.Config.UserId))
			{
				dictionary.Add("swrve_user_id", SwrveComponent.Instance.Config.UserId);
			}
			else
			{
				SwrveManager.Log("### !!! unable to add userId to the userProps");
			}
			dictionary.Add("jailbroken.is_jailbroken", SwrveManagerUtils.GetIsJailBroken());
			dictionary.Add("lat.is_lat", SwrveManagerUtils.GetIsLat().ToString());
			string text2 = string.Empty;
			if (Application.platform == RuntimePlatform.Android)
			{
				text2 = "gida";
			}
			else if (Application.platform == RuntimePlatform.IPhonePlayer)
			{
				text2 = "idfa";
			}
			if (SwrveManagerUtils.IsAndiAvailable() && SwrveManagerUtils.IsAndiInitialized())
			{
				string text3 = (string)SwrveManagerUtils.ANDIType.GetMethod("GetAndiu").Invoke(null, null);
				SwrveManager.Log("### Let's send Swrve the andiu: " + text3);
				dictionary.Add("andiu", text3);
			}
			string text4 = SwrveManagerUtils.AESEncrypt(text, SwrveManagerUtils.GetAdvertiserID());
			SwrveManager.Log("### encryptedAdvertiserId: " + text4);
			if (!string.IsNullOrEmpty(text2))
			{
				dictionary.Add(text2, text4);
			}
			string rSAEncryptedKey = SwrveManagerUtils.GetRSAEncryptedKey();
			SwrveManager.Log("### eskKey: " + rSAEncryptedKey);
			if (!string.IsNullOrEmpty(rSAEncryptedKey))
			{
				dictionary.Add("esk", rSAEncryptedKey);
			}
			if (this.customUserData != null)
			{
				foreach (KeyValuePair<string, string> current in this.customUserData)
				{
					if (!dictionary.ContainsKey(current.Key))
					{
						dictionary.Add(current.Key, current.Value);
					}
					else
					{
						SwrveManager.Log("###Duplicate KEY!! unable to add " + current.Key + " - " + current.Value);
					}
				}
			}
			SwrveComponent.Instance.SDK.UserUpdate(dictionary);
			this.resourceManager = SwrveComponent.Instance.SDK.ResourceManager;
		}

		public void InitWithAnalyticsKeySecretAndUserId(int appId, string apiKey, string userId)
		{
			this.RegisterPlayer(userId);
			this.InitWithAnalyticsKeySecret(appId, apiKey);
		}

		public void InitWithAnalyticsKeySecretAndUserIdAndAppVersion(int appId, string apiKey, string userId, string appVersion)
		{
			SwrveComponent.Instance.Config.AppVersion = appVersion;
			this.InitWithAnalyticsKeySecretAndUserId(appId, apiKey, userId);
		}

		private void RegisterPlayerCore(string playerId)
		{
			SwrveComponent.Instance.Config.UserId = WWW.EscapeURL(playerId);
		}

		public void RegisterPlayer(string playerId)
		{
			this.userId = playerId;
			this.RegisterPlayerCore(this.userId);
		}

		public string GetLibVersion()
		{
			return "SwrveManager: 1.1.8, Swrve SDK: 4.10.1";
		}

		public void RegisterPlayer(int appId, string apiKey, string playerId)
		{
			this.userId = playerId;
			SwrveComponent.Instance.Config.UserId = WWW.EscapeURL(this.userId);
			this.AnalyticsId = appId.ToString();
			this.AnalyticsKey = apiKey;
			SwrveManager.Log(string.Concat(new object[]
			{
				"Init with appId ",
				appId,
				" and apiKey: ",
				apiKey
			}));
			SwrveComponent.Instance.Init(appId, apiKey);
			this.resourceManager = SwrveComponent.Instance.SDK.ResourceManager;
		}

		public void LogAdAction(AdActionAnalytics analytics)
		{
			SwrveManager.Log("Logging AdAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogFunnelAction(FunnelStepsAnalytics analytics)
		{
			SwrveManager.Log("Logging FunnelAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogNavigationAction(NavigationActionAnalytics analytics)
		{
			SwrveManager.Log("Logging NavigationAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogTimingAction(TimingAnalytics analytics)
		{
			SwrveManager.Log("Logging TimingAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogSwrvePurchase(string itemId, int cost, int quantity, string currency)
		{
			SwrveComponent.Instance.SDK.Purchase(itemId, currency, cost, quantity);
		}

		public void LogSwrvePurchase(string itemId, int cost, string currency)
		{
			SwrveComponent.Instance.SDK.Purchase(itemId, currency, cost, 1);
		}

		public void LogSwrveCurrencyGiven(string givenCurrency, double givenAmount)
		{
			SwrveComponent.Instance.SDK.CurrencyGiven(givenCurrency, givenAmount);
		}

		public void LogIAPAction(IAPAnalytics analytics)
		{
			SwrveManager.Log("Logging IAP_custom: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogPurchaseAction(PurchaseAnalytics analytics)
		{
			SwrveManager.Log("Logging purchase_custom: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogCurrencyGivenAction(CurrencyGivenAnalytics analytics)
		{
			SwrveManager.Log("Logging currency_given_custom: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogAction(ActionAnalytics analytics)
		{
			SwrveManager.Log("Logging Action: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogTestImpressionAction(TestImpressionAnalytics analytics)
		{
			SwrveManager.Log("Logging TestImpressionAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogErrorAction(ErrorAnalytics analytics)
		{
			SwrveManager.Log("Logging ErrorAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogFailedReceiptAction(FailedReceiptAnalytics analytics)
		{
			SwrveManager.Log("Logging FailedReceiptAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogAnalyticsAction(IAnalytics analytics)
		{
			SwrveManager.Log("Logging GenericAction: " + analytics.ToString());
			this.GenericSendLogAction(analytics);
		}

		public void LogGenericAction(string action)
		{
			GenericStringAnalytics genericStringAnalytics = new GenericStringAnalytics(action);
			SwrveManager.Log("Logging GenericAction: " + genericStringAnalytics.ToString());
			this.GenericSendLogAction(genericStringAnalytics);
		}

		public void LogGenericAction(string action, Dictionary<string, object> messageDetails)
		{
			GenericWrapperAnalytics genericWrapperAnalytics = new GenericWrapperAnalytics(action, messageDetails);
			SwrveManager.Log("Logging GenericAction: " + genericWrapperAnalytics.ToString());
			this.GenericSendLogAction(genericWrapperAnalytics);
		}

		private void GenericSendLogAction(IAnalytics analytics)
		{
			if (SwrveManager.instance != null)
			{
				SwrveManager.instance.LogAction(analytics.GetSwrveEvent(), analytics.Serialize());
			}
		}

		public void LogSwrveGoogleIAPAction(string productId, double localCost, string localCurrency, string receipt, string receiptSignature, IapRewards rewards)
		{
			if (rewards == null)
			{
				SwrveComponent.Instance.SDK.IapGooglePlay(productId, localCost, localCurrency, receipt, receiptSignature);
			}
			else
			{
				SwrveComponent.Instance.SDK.IapGooglePlay(productId, localCost, localCurrency, rewards, receipt, receiptSignature);
			}
		}

		private Dictionary<string, string> CreateSwrvePayload(Dictionary<string, object> dataDetails)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			foreach (KeyValuePair<string, object> current in dataDetails)
			{
				dictionary.Add(current.Key, Json.Serialize(current.Value).Replace("\"", string.Empty));
			}
			return dictionary;
		}

		private void LogAction(string eventName, int value)
		{
			this.LogAction(eventName, new Dictionary<string, object>
			{
				{
					eventName,
					value
				}
			});
		}

		private void LogAction(string eventName, string value)
		{
			this.LogAction(eventName, new Dictionary<string, object>
			{
				{
					eventName,
					value
				}
			});
		}

		private void LogAction(string eventName)
		{
			Dictionary<string, object> payload = null;
			this.LogAction(eventName, payload);
		}

		private void LogAction(string eventName, Dictionary<string, object> payload)
		{
			if (eventName == null)
			{
				return;
			}
			if (eventName.ToLower().StartsWith("swrve."))
			{
				this.RaiseAlert(string.Format("Event name {0} cannot be used because 'swrve.' is a reserved prefix.", eventName));
				return;
			}
			if (!this.EventsSent.Contains(eventName) && this.EventsSent.Count >= 1000)
			{
				this.RaiseAlert(string.Format("There is a limit of {0} unique event namse that can be sent to Swrve. Not tracking {1}", 1000, eventName));
				return;
			}
			if (SwrveComponent.Instance.SDK != null && SwrveComponent.Instance.SDK.Initialised)
			{
				if (payload != null)
				{
					Dictionary<string, string> dictionary = new Dictionary<string, string>();
					this.FlattenPayload(dictionary, payload, null);
					SwrveComponent.Instance.SDK.NamedEvent(eventName, dictionary);
				}
				else
				{
					SwrveComponent.Instance.SDK.NamedEvent(eventName, null);
				}
				this.EventsSent.Add(eventName);
			}
		}

		private void RaiseAlert(string message)
		{
			SwrveManager.Log(message);
		}

		private void FlattenPayload(Dictionary<string, string> targetPayload, Dictionary<string, object> srcPayload, string prefix)
		{
			if (srcPayload == null || targetPayload == null)
			{
				return;
			}
			foreach (string current in srcPayload.Keys)
			{
				if (targetPayload.Count > 500)
				{
					this.RaiseAlert(string.Format("Cannot have more than {0} keys in an event payload. Ignoring rest.", 500));
					break;
				}
				object obj = srcPayload[current];
				if (obj is Dictionary<string, object>)
				{
					string prefix2 = (prefix != null) ? string.Format("{0}|{1}", prefix, current) : current;
					this.FlattenPayload(targetPayload, obj as Dictionary<string, object>, prefix2);
				}
				else
				{
					targetPayload.Add(current, obj.ToString());
				}
			}
		}
	}
}
