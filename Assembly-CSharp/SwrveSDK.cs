using SwrveUnity;
using SwrveUnity.Device;
using SwrveUnity.Helpers;
using SwrveUnity.Input;
using SwrveUnity.Messaging;
using SwrveUnity.ResourceManager;
using SwrveUnity.REST;
using SwrveUnity.Storage;
using SwrveUnityMiniJSON;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using UnityEngine;

public class SwrveSDK
{
	private const string SwrveAndroidPushPluginPackageName = "com.swrve.unity.gcm.SwrveGcmDeviceRegistration";

	private const string SwrveAndroidADMPushPluginPackageName = "com.swrve.unity.adm.SwrveAdmPushSupport";

	private const string SwrveAndroidUnityCommonName = "com.swrve.sdk.SwrveUnityCommon";

	private const string SwrvePushSupport = "com.swrve.unity.SwrvePushSupport";

	private const string IsInitialisedName = "isInitialised";

	private const string GetConversationVersionName = "getConversationVersion";

	private const string ShowConversationName = "showConversation";

	private const string SwrveStartLocationName = "StartLocation";

	private const string SwrveLocationUserUpdateName = "LocationUserUpdate";

	private const string SwrveGetPlotNotificationsName = "GetPlotNotifications";

	private const string SwrveIsOSSupportedVersionName = "sdkAvailable";

	private const string GetInfluencedDataJsonName = "getInfluenceDataJson";

	private const string UnityPlayerName = "com.unity3d.player.UnityPlayer";

	private const string UnityCurrentActivityName = "currentActivity";

	private string gcmDeviceToken;

	private static AndroidJavaObject androidPlugin;

	private static bool androidPluginInitialized;

	private static bool androidPluginInitializedSuccessfully;

	private string admDeviceToken;

	private static AndroidJavaObject androidADMPlugin;

	private static bool androidADMPluginInitialized;

	private static bool androidADMPluginInitializedSuccessfully;

	private string googlePlayAdvertisingId;

	private const int GooglePlayPushPluginVersion = 4;

	private const int AdmPushPluginVersion = 1;

	private const string InitialiseAdmName = "initialiseAdm";

	private const string GetVersionName = "getVersion";

	private const string AckReceivedNotificationName = "sdkAcknowledgeReceivedNotification";

	private const string AckOpenedNotificationName = "sdkAcknowledgeOpenedNotification";

	private const string RegisterDeviceName = "registerDevice";

	private const string RequestAdvertisingIdName = "requestAdvertisingId";

	private const string GCMIdentKey = "google.message_id";

	private const string ADMIdentKey = "adm_message_md5";

	private static string LastOpenedNotification;

	private string _androidId;

	private AndroidJavaObject _androidBridge;

	public const string SdkVersion = "4.10.1";

	protected int appId;

	protected string apiKey;

	protected string userId;

	protected SwrveConfig config;

	public string Language;

	public SwrveResourceManager ResourceManager;

	protected ISwrveAssetsManager SwrveAssetsManager;

	public MonoBehaviour Container;

	public ISwrveInstallButtonListener GlobalInstallButtonListener;

	public ISwrveCustomButtonListener GlobalCustomButtonListener;

	public ISwrveMessageListener GlobalMessageListener;

	public ISwrveConversationListener GlobalConversationListener;

	public ISwrvePushNotificationListener PushNotificationListener;

	public ISwrveTriggeredMessageListener TriggeredMessageListener;

	public Action ResourcesUpdatedCallback;

	public bool Initialised;

	public bool Destroyed;

	private const string Platform = "Unity ";

	private const float DefaultDPI = 160f;

	protected const string EventsSave = "Swrve_Events";

	protected const string InstallTimeEpochSave = "Swrve_JoinedDate";

	protected const string iOSdeviceTokenSave = "Swrve_iOSDeviceToken";

	protected const string GcmDeviceTokenSave = "Swrve_gcmDeviceToken";

	protected const string AdmDeviceTokenSave = "Swrve_admDeviceToken";

	protected const string WindowsDeviceTokenSave = "Swrve_windowsDeviceToken";

	protected const string GoogleAdvertisingIdSave = "Swrve_googleAdvertisingId";

	protected const string AbTestUserResourcesSave = "srcngt2";

	protected const string AbTestUserResourcesDiffSave = "rsdfngt2";

	protected const string DeviceIdSave = "Swrve_DeviceId";

	protected const string SeqNumSave = "Swrve_SeqNum";

	protected const string ResourcesCampaignTagSave = "cmpg_etag";

	protected const string ResourcesCampaignFlushFrequencySave = "swrve_cr_flush_frequency";

	protected const string ResourcesCampaignFlushDelaySave = "swrve_cr_flush_delay";

	private const string DeviceIdKey = "Swrve.deviceUniqueIdentifier";

	private const string EmptyJSONObject = "{}";

	private const float DefaultCampaignResourcesFlushFrenquency = 60f;

	private const float DefaultCampaignResourcesFlushRefreshDelay = 5f;

	public const string DefaultAutoShowMessagesTrigger = "Swrve.Messages.showAtSessionStart";

	private const string PushTrackingKey = "_p";

	private const string SilentPushTrackingKey = "_sp";

	private const string PushDeeplinkKey = "_sd";

	private string escapedUserId;

	private long installTimeEpoch;

	private string installTimeFormatted;

	private string lastPushEngagedId;

	private int deviceWidth;

	private int deviceHeight;

	private long lastSessionTick;

	private ICarrierInfo deviceCarrierInfo;

	private System.Random rnd = new System.Random();

	protected StringBuilder eventBufferStringBuilder;

	protected string eventsPostString;

	protected string swrvePath;

	protected ISwrveStorage storage;

	protected IRESTClient restClient;

	private string eventsUrl;

	private string abTestResourcesDiffUrl;

	protected bool eventsConnecting;

	protected bool abTestUserResourcesDiffConnecting;

	protected string userResourcesRaw;

	protected Dictionary<string, Dictionary<string, string>> userResources;

	protected float campaignsAndResourcesFlushFrequency;

	protected float campaignsAndResourcesFlushRefreshDelay;

	protected string lastETag;

	protected long campaignsAndResourcesLastRefreshed;

	protected bool campaignsAndResourcesInitialized;

	private static readonly int CampaignEndpointVersion = 6;

	private static readonly int CampaignResponseVersion = 2;

	protected static readonly string CampaignsSave = "cmcc2";

	protected static readonly string CampaignsSettingsSave = "Swrve_CampaignsData";

	protected static readonly string LocationSave = "loccc2";

	private static readonly string WaitTimeFormat = "HH\\:mm\\:ss zzz";

	protected static readonly string InstallTimeFormat = "yyyyMMdd";

	private string resourcesAndCampaignsUrl;

	protected string swrveTemporaryPath;

	protected bool campaignsConnecting;

	protected bool autoShowMessagesEnabled;

	protected Dictionary<int, SwrveCampaignState> campaignsState = new Dictionary<int, SwrveCampaignState>();

	protected List<SwrveBaseCampaign> campaigns = new List<SwrveBaseCampaign>();

	protected Dictionary<string, object> campaignSettings = new Dictionary<string, object>();

	protected Dictionary<string, string> appStoreLinks = new Dictionary<string, string>();

	protected SwrveMessageFormat currentMessage;

	protected SwrveMessageFormat currentDisplayingMessage;

	protected SwrveOrientation currentOrientation;

	protected IInputManager inputManager = NativeInputManager.Instance;

	protected string prefabName;

	private const int DefaultDelayFirstMessage = 150;

	private const long DefaultMaxShows = 99999L;

	private const int DefaultMinDelay = 55;

	private DateTime initialisedTime;

	private DateTime showMessagesAfterLaunch;

	private DateTime showMessagesAfterDelay;

	private long messagesLeftToShow;

	private int minDelayBetweenMessage;

	protected SwrveQAUser qaUser;

	private bool campaignAndResourcesCoroutineEnabled = true;

	private IEnumerator campaignAndResourcesCoroutineInstance;

	private int locationSegmentVersion;

	private int conversationVersion;

	public int AppId
	{
		get
		{
			return this.appId;
		}
	}

	public string ApiKey
	{
		get
		{
			return this.apiKey;
		}
	}

	public string UserId
	{
		get
		{
			return this.userId;
		}
	}

	public void IapGooglePlay(string productId, double productPrice, string currency, string purchaseData, string dataSignature)
	{
		IapRewards rewards = new IapRewards();
		this.IapGooglePlay(productId, productPrice, currency, rewards, purchaseData, dataSignature);
	}

	public void IapGooglePlay(string productId, double productPrice, string currency, IapRewards rewards, string purchaseData, string dataSignature)
	{
		if (this.config.AppStore != "google")
		{
			throw new Exception("This function can only be called to validate IAP events from Google");
		}
		if (string.IsNullOrEmpty(purchaseData))
		{
			SwrveLog.LogError("IAP event not sent: purchase data cannot be empty for Google Play Store verification");
			return;
		}
		if (string.IsNullOrEmpty(dataSignature))
		{
			SwrveLog.LogError("IAP event not sent: data signature cannot be empty for Google Play Store verification");
			return;
		}
		this._Iap(1, productId, productPrice, currency, rewards, purchaseData, dataSignature, string.Empty, this.config.AppStore);
	}

	private void setNativeInfo(Dictionary<string, string> deviceInfo)
	{
		if (!string.IsNullOrEmpty(this.gcmDeviceToken))
		{
			deviceInfo["swrve.gcm_token"] = this.gcmDeviceToken;
		}
		if (!string.IsNullOrEmpty(this.admDeviceToken))
		{
			deviceInfo["swrve.adm_token"] = this.admDeviceToken;
		}
		string value = this.AndroidGetTimezone();
		if (!string.IsNullOrEmpty(value))
		{
			deviceInfo["swrve.timezone_name"] = value;
		}
		string value2 = this.AndroidGetRegion();
		if (!string.IsNullOrEmpty(value2))
		{
			deviceInfo["swrve.device_region"] = value2;
		}
		if (this.config.LogAndroidId)
		{
			try
			{
				deviceInfo["swrve.android_id"] = this.AndroidGetAndroidId();
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get device IDFA, make sure you have the plugin inside your project and you are running on a device: " + ex.ToString());
			}
		}
		if (this.config.LogGoogleAdvertisingId && !string.IsNullOrEmpty(this.googlePlayAdvertisingId))
		{
			deviceInfo["swrve.GAID"] = this.googlePlayAdvertisingId;
		}
	}

	private string getNativeLanguage()
	{
		string text = null;
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
			{
				AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]);
				text = androidJavaObject.Call<string>("getLanguage", new object[0]);
				string text2 = androidJavaObject.Call<string>("getCountry", new object[0]);
				if (!string.IsNullOrEmpty(text2))
				{
					text = text + "-" + text2;
				}
				string text3 = androidJavaObject.Call<string>("getVariant", new object[0]);
				if (!string.IsNullOrEmpty(text3))
				{
					text = text + "-" + text3;
				}
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get the device language, make sure you are running on an Android device: " + ex.ToString());
		}
		return text;
	}

	private void GooglePlayRegisterForPushNotification(MonoBehaviour container, string senderId)
	{
		try
		{
			bool flag = false;
			this.gcmDeviceToken = this.storage.Load("Swrve_gcmDeviceToken", null);
			if (!SwrveSDK.androidPluginInitialized)
			{
				SwrveSDK.androidPluginInitialized = true;
				using (new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					string name = "com.swrve.unity.gcm.SwrveGcmDeviceRegistration".Replace(".", "/");
					if (AndroidJNI.FindClass(name).ToInt32() != 0)
					{
						SwrveSDK.androidPlugin = new AndroidJavaClass("com.swrve.unity.gcm.SwrveGcmDeviceRegistration");
						if (SwrveSDK.androidPlugin != null)
						{
							int num = SwrveSDK.androidPlugin.CallStatic<int>("getVersion", new object[0]);
							if (num != 4)
							{
								SwrveSDK.androidPlugin = null;
								throw new Exception("The version of the Swrve Android Push plugin is different. This Swrve SDK needs version " + 4);
							}
							SwrveSDK.androidPluginInitializedSuccessfully = true;
						}
					}
				}
			}
			if (SwrveSDK.androidPluginInitializedSuccessfully)
			{
				flag = SwrveSDK.androidPlugin.CallStatic<bool>("registerDevice", new object[]
				{
					container.name,
					senderId,
					this.config.GCMPushNotificationTitle,
					this.config.GCMPushNotificationIconId,
					this.config.GCMPushNotificationMaterialIconId,
					this.config.GCMPushNotificationLargeIconId,
					this.config.GCMPushNotificationAccentColor
				});
			}
			if (!flag)
			{
				SwrveLog.LogError("Could not communicate with the Swrve Android Push plugin. Have you copied all the jars to the directory?");
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogError("Could not retrieve the device Registration Id: " + ex.ToString());
		}
	}

	public void SetGooglePlayAdvertisingId(string advertisingId)
	{
		this.googlePlayAdvertisingId = advertisingId;
		this.storage.Save("Swrve_googleAdvertisingId", advertisingId, null);
	}

	private void RequestGooglePlayAdvertisingId(MonoBehaviour container)
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				this.googlePlayAdvertisingId = this.storage.Load("Swrve_googleAdvertisingId", null);
				using (new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					string name = "com.swrve.unity.gcm.SwrveGcmDeviceRegistration".Replace(".", "/");
					if (AndroidJNI.FindClass(name).ToInt32() != 0)
					{
						SwrveSDK.androidPlugin = new AndroidJavaClass("com.swrve.unity.gcm.SwrveGcmDeviceRegistration");
						if (SwrveSDK.androidPlugin != null)
						{
							SwrveSDK.androidPlugin.CallStatic<bool>("requestAdvertisingId", new object[]
							{
								container.name
							});
						}
					}
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Could not retrieve the device Registration Id: " + ex.ToString());
			}
		}
	}

	private void InitialisePushADM(MonoBehaviour container)
	{
		try
		{
			bool flag = false;
			this.admDeviceToken = this.storage.Load("Swrve_admDeviceToken", null);
			if (!SwrveSDK.androidADMPluginInitialized)
			{
				SwrveSDK.androidADMPluginInitialized = true;
				string text = "com.swrve.unity.adm.SwrveAdmPushSupport";
				string text2 = text.Replace(".", "/");
				if (AndroidJNI.FindClass(text2).ToInt32() == 0)
				{
					SwrveLog.LogError("Could not find class: " + text2 + " Are you using the correct SwrveSDKPushSupport plugin given the swrve config.AndroidPushProvider setting?");
					AndroidJNI.FindClass(text2);
					return;
				}
				SwrveSDK.androidADMPlugin = new AndroidJavaClass(text);
				if (SwrveSDK.androidADMPlugin == null)
				{
					SwrveLog.LogError("Found class, but unable to construct AndroidJavaClass: " + text2);
					return;
				}
				int num = SwrveSDK.androidADMPlugin.CallStatic<int>("getVersion", new object[0]);
				if (num != 1)
				{
					SwrveSDK.androidADMPlugin = null;
					throw new Exception(string.Concat(new object[]
					{
						"The version of the Swrve Android Push plugin",
						text,
						"is different. This Swrve SDK needs version ",
						num
					}));
				}
				SwrveSDK.androidADMPluginInitializedSuccessfully = true;
				SwrveLog.LogInfo("Android Push Plugin initialised successfully: " + text2);
			}
			if (SwrveSDK.androidADMPluginInitializedSuccessfully)
			{
				flag = SwrveSDK.androidADMPlugin.CallStatic<bool>("initialiseAdm", new object[]
				{
					container.name,
					this.config.ADMPushNotificationTitle,
					this.config.ADMPushNotificationIconId,
					this.config.ADMPushNotificationMaterialIconId,
					this.config.ADMPushNotificationLargeIconId,
					this.config.ADMPushNotificationAccentColor
				});
			}
			if (!flag)
			{
				SwrveLog.LogError("Could not communicate with the Swrve Android ADM Push plugin.");
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogError("Could not initalise push: " + ex.ToString());
		}
	}

	private string AndroidGetTimezone()
	{
		try
		{
			AndroidJavaObject androidJavaObject = new AndroidJavaObject("java.util.GregorianCalendar", new object[0]);
			return androidJavaObject.Call<AndroidJavaObject>("getTimeZone", new object[0]).Call<string>("getID", new object[0]);
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get the device timezone, make sure you are running on an Android device: " + ex.ToString());
		}
		return null;
	}

	private string AndroidGetRegion()
	{
		try
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("java.util.Locale"))
			{
				AndroidJavaObject androidJavaObject = androidJavaClass.CallStatic<AndroidJavaObject>("getDefault", new object[0]);
				return androidJavaObject.Call<string>("getCountry", new object[0]);
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get the device region, make sure you are running on an Android device: " + ex.ToString());
		}
		return null;
	}

	private string AndroidGetAppVersion()
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					string text = @static.Call<string>("getPackageName", new object[0]);
					return @static.Call<AndroidJavaObject>("getPackageManager", new object[0]).Call<AndroidJavaObject>("getPackageInfo", new object[]
					{
						text,
						0
					}).Get<string>("versionName");
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get the device app version, make sure you are running on an Android device: " + ex.ToString());
			}
		}
		return null;
	}

	private string AndroidGetAndroidId()
	{
		if (SwrveHelper.IsOnDevice() && this._androidId == null)
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getContentResolver", new object[0]);
					AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("android.provider.Settings$Secure");
					this._androidId = androidJavaClass2.CallStatic<string>("getString", new object[]
					{
						androidJavaObject,
						"android_id"
					});
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get the \"android_id\" resource, make sure you are running on an Android device: " + ex.ToString());
			}
		}
		return this._androidId;
	}

	public void RegistrationIdReceived(string registrationId)
	{
		if (!string.IsNullOrEmpty(registrationId))
		{
			bool flag = this.gcmDeviceToken != registrationId;
			if (flag)
			{
				this.gcmDeviceToken = registrationId;
				this.storage.Save("Swrve_gcmDeviceToken", this.gcmDeviceToken, null);
				if (this.qaUser != null)
				{
					this.qaUser.UpdateDeviceInfo();
				}
				this.SendDeviceInfo();
			}
		}
	}

	public string GetGCMDeviceToken()
	{
		return this.gcmDeviceToken;
	}

	public void RegistrationIdReceivedADM(string registrationId)
	{
		if (!string.IsNullOrEmpty(registrationId))
		{
			bool flag = this.admDeviceToken != registrationId;
			if (flag)
			{
				this.admDeviceToken = registrationId;
				this.storage.Save("Swrve_admDeviceToken", this.admDeviceToken, null);
				if (this.qaUser != null)
				{
					this.qaUser.UpdateDeviceInfo();
				}
				this.SendDeviceInfo();
			}
		}
	}

	public void NotificationReceived(string notificationJson)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(notificationJson);
		if (SwrveSDK.androidPlugin != null && dictionary != null)
		{
			string pushId = this.GetPushId(dictionary);
			if (pushId != null)
			{
				SwrveSDK.androidPlugin.CallStatic("sdkAcknowledgeReceivedNotification", new object[]
				{
					pushId
				});
			}
		}
		if (this.PushNotificationListener != null)
		{
			try
			{
				this.PushNotificationListener.OnNotificationReceived(dictionary);
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Error processing the push notification: " + ex.Message);
			}
		}
	}

	public void NotificationReceivedADM(string notificationJson)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(notificationJson);
		if (SwrveSDK.androidADMPlugin != null && dictionary != null)
		{
			string pushId = this.GetPushId(dictionary);
			if (pushId != null)
			{
				SwrveSDK.androidADMPlugin.CallStatic("sdkAcknowledgeReceivedNotification", new object[]
				{
					pushId
				});
			}
		}
		if (this.PushNotificationListener != null)
		{
			try
			{
				this.PushNotificationListener.OnNotificationReceived(dictionary);
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Error processing the push notification: " + ex.Message);
			}
		}
	}

	private string GetPushId(Dictionary<string, object> notification)
	{
		if (notification != null && notification.ContainsKey("_p"))
		{
			return notification["_p"].ToString();
		}
		SwrveLog.Log("Got unidentified notification");
		return null;
	}

	public void OpenedFromPushNotification(string notificationJson)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(notificationJson);
		string text = (string)dictionary["google.message_id"];
		if (!string.IsNullOrEmpty(text) && text == SwrveSDK.LastOpenedNotification)
		{
			return;
		}
		SwrveSDK.LastOpenedNotification = text;
		string pushId = this.GetPushId(dictionary);
		this.SendPushEngagedEvent(pushId);
		if (pushId != null && SwrveSDK.androidPlugin != null)
		{
			SwrveSDK.androidPlugin.CallStatic("sdkAcknowledgeOpenedNotification", new object[]
			{
				pushId
			});
		}
		if (dictionary != null && dictionary.ContainsKey("_sd"))
		{
			object obj = dictionary["_sd"];
			if (obj != null)
			{
				this.OpenURL(obj.ToString());
			}
		}
		if (this.PushNotificationListener != null)
		{
			try
			{
				this.PushNotificationListener.OnOpenedFromPushNotification(dictionary);
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Error processing the push notification: " + ex.Message);
			}
		}
	}

	public void OpenedFromPushNotificationADM(string notificationJson)
	{
		Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(notificationJson);
		string text = (string)dictionary["adm_message_md5"];
		if (!string.IsNullOrEmpty(text) && text == SwrveSDK.LastOpenedNotification)
		{
			return;
		}
		SwrveSDK.LastOpenedNotification = text;
		string pushId = this.GetPushId(dictionary);
		this.SendPushEngagedEvent(pushId);
		if (pushId != null && SwrveSDK.androidADMPlugin != null)
		{
			SwrveSDK.androidADMPlugin.CallStatic("sdkAcknowledgeOpenedNotification", new object[]
			{
				pushId
			});
		}
		if (dictionary != null && dictionary.ContainsKey("_sd"))
		{
			object obj = dictionary["_sd"];
			if (obj != null)
			{
				this.OpenURL(obj.ToString());
			}
		}
		if (this.PushNotificationListener != null)
		{
			try
			{
				this.PushNotificationListener.OnOpenedFromPushNotification(dictionary);
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Error processing the push notification: " + ex.Message);
			}
		}
	}

	private void initNative()
	{
		this.AndroidInitNative();
	}

	private void AndroidInitNative()
	{
		try
		{
			this.AndroidGetBridge();
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't init common from Android: " + ex.ToString());
		}
	}

	private AndroidJavaObject AndroidGetBridge()
	{
		if (SwrveHelper.IsOnDevice())
		{
			using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.swrve.sdk.SwrveUnityCommon"))
			{
				if (this._androidBridge == null || !androidJavaClass.CallStatic<bool>("isInitialised", new object[0]))
				{
					this._androidBridge = new AndroidJavaObject("com.swrve.sdk.SwrveUnityCommon", new object[]
					{
						this.GetNativeDetails()
					});
				}
			}
		}
		return this._androidBridge;
	}

	private void setNativeAppVersion()
	{
		this.config.AppVersion = this.AndroidGetAppVersion();
	}

	private void showNativeConversation(string conversation)
	{
		try
		{
			this.AndroidGetBridge().Call("showConversation", new object[]
			{
				conversation,
				this.config.Orientation.ToString()
			});
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't show conversation from Android: " + ex.ToString());
		}
	}

	private void startNativeLocation()
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				this.AndroidGetBridge().CallStatic("StartLocation", new object[0]);
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't start Swrve location from Android: " + ex.ToString());
			}
		}
	}

	public void LocationUserUpdate(Dictionary<string, string> map)
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				this.AndroidGetBridge().CallStatic("LocationUserUpdate", new object[]
				{
					Json.Serialize(map)
				});
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't update location details from Android: " + ex.ToString());
			}
		}
	}

	public string GetPlotNotifications()
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				return this.AndroidGetBridge().CallStatic<string>("GetPlotNotifications", new object[0]);
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get plot notifications from Android: " + ex.ToString());
			}
		}
		return "[]";
	}

	private void setNativeConversationVersion()
	{
		try
		{
			this.SetConversationVersion(this.AndroidGetBridge().Call<int>("getConversationVersion", new object[0]));
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get conversations version from Android: " + ex.ToString());
		}
	}

	private bool NativeIsBackPressed()
	{
		return Input.GetKeyDown(KeyCode.Escape);
	}

	public static bool IsSupportedAndroidVersion()
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.swrve.sdk.SwrveUnityCommon"))
				{
					return androidJavaClass.CallStatic<bool>("sdkAvailable", new object[0]);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get supported OS version from Android: " + ex.ToString());
			}
			return false;
		}
		return false;
	}

	public string GetInfluencedDataJson()
	{
		if (SwrveHelper.IsOnDevice())
		{
			try
			{
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.swrve.unity.SwrvePushSupport"))
				{
					return androidJavaClass.CallStatic<string>("getInfluenceDataJson", new object[0]);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Couldn't get influence data from Android: " + ex.ToString());
			}
		}
		return null;
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey)
	{
		this.Init(container, appId, apiKey, new SwrveConfig());
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey, string userId)
	{
		this.Init(container, appId, apiKey, new SwrveConfig
		{
			UserId = userId
		});
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey, string userId, SwrveConfig config)
	{
		config.UserId = userId;
		this.Init(container, appId, apiKey, config);
	}

	public virtual void Init(MonoBehaviour container, int appId, string apiKey, SwrveConfig config)
	{
		this.Container = container;
		this.ResourceManager = new SwrveResourceManager();
		this.config = config;
		this.prefabName = container.name;
		this.appId = appId;
		this.apiKey = apiKey;
		this.userId = config.UserId;
		this.Language = config.Language;
		this.lastSessionTick = this.GetSessionTime();
		this.initialisedTime = SwrveHelper.GetNow();
		this.campaignsAndResourcesInitialized = false;
		this.autoShowMessagesEnabled = true;
		this.swrveTemporaryPath = SwrveSDK.GetSwrveTemporaryCachePath();
		this.InitAssetsManager(container, this.swrveTemporaryPath);
		if (string.IsNullOrEmpty(apiKey))
		{
			throw new Exception("The api key has not been specified.");
		}
		if (string.IsNullOrEmpty(this.userId))
		{
			this.userId = this.GetDeviceUniqueId();
		}
		if (!string.IsNullOrEmpty(this.userId))
		{
			PlayerPrefs.SetString("Swrve.deviceUniqueIdentifier", this.userId);
			PlayerPrefs.Save();
		}
		SwrveLog.Log("Your user id is: " + this.userId);
		this.escapedUserId = WWW.EscapeURL(this.userId);
		if (string.IsNullOrEmpty(this.Language))
		{
			this.Language = this.GetDeviceLanguage();
			if (string.IsNullOrEmpty(this.Language))
			{
				this.Language = config.DefaultLanguage;
			}
		}
		config.CalculateEndpoints(appId);
		string contentServer = config.ContentServer;
		this.eventsUrl = config.EventsServer + "/1/batch";
		this.abTestResourcesDiffUrl = contentServer + "/api/1/user_resources_diff";
		this.resourcesAndCampaignsUrl = contentServer + "/api/1/user_resources_and_campaigns";
		this.swrvePath = SwrveSDK.GetSwrvePath();
		if (this.storage == null)
		{
			this.storage = this.CreateStorage();
		}
		this.storage.SetSecureFailedListener(delegate
		{
			this.NamedEventInternal("Swrve.signature_invalid", null, false);
		});
		this.restClient = this.CreateRestClient();
		this.eventBufferStringBuilder = new StringBuilder(config.MaxBufferChars);
		string savedInstallTimeEpoch = this.GetSavedInstallTimeEpoch();
		this.LoadData();
		this.InitUserResources();
		this.deviceCarrierInfo = new DeviceCarrierInfo();
		this.GetDeviceScreenInfo();
		this.Initialised = true;
		if (config.AutomaticSessionManagement)
		{
			this.QueueSessionStart();
			this.GenerateNewSessionInterval();
		}
		if (string.IsNullOrEmpty(savedInstallTimeEpoch))
		{
			this.NamedEventInternal("Swrve.first_session", null, true);
		}
		if (config.AndroidPushProvider == AndroidPushProvider.GOOGLE_GCM)
		{
			if (config.PushNotificationEnabled && !string.IsNullOrEmpty(config.GCMSenderId))
			{
				this.GooglePlayRegisterForPushNotification(this.Container, config.GCMSenderId);
			}
			if (config.LogGoogleAdvertisingId)
			{
				this.RequestGooglePlayAdvertisingId(this.Container);
			}
		}
		else if (config.AndroidPushProvider == AndroidPushProvider.AMAZON_ADM && config.PushNotificationEnabled)
		{
			this.InitialisePushADM(this.Container);
		}
		this.QueueDeviceInfo();
		this.SendQueuedEvents();
		if (config.TalkEnabled)
		{
			if (string.IsNullOrEmpty(this.Language))
			{
				throw new Exception("Language needed to use Talk");
			}
			if (string.IsNullOrEmpty(config.AppStore))
			{
				config.AppStore = "google";
			}
			try
			{
				this.LoadTalkData();
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while initializing " + arg);
			}
		}
		this.DisableAutoShowAfterDelay();
		if (SwrveHelper.IsOnDevice())
		{
			this.InitNative();
		}
		this.ProcessInfluenceData();
		this.StartCampaignsAndResourcesTimer();
	}

	protected virtual void InitAssetsManager(MonoBehaviour container, string swrveTemporaryPath)
	{
		this.SwrveAssetsManager = new SwrveAssetsManager(container, swrveTemporaryPath);
	}

	public virtual void SessionStart()
	{
		this.QueueSessionStart();
		this.SendQueuedEvents();
	}

	public virtual void SessionEnd()
	{
		Dictionary<string, object> eventParameters = new Dictionary<string, object>();
		this.AppendEventToBuffer("session_end", eventParameters, true);
	}

	public virtual void NamedEvent(string name, Dictionary<string, string> payload = null)
	{
		if (name != null && !name.ToLower().StartsWith("swrve."))
		{
			this.NamedEventInternal(name, payload, true);
		}
		else
		{
			SwrveLog.LogError("Event cannot begin with \"Swrve.\". The event " + name + " will not be sent");
		}
	}

	public virtual void UserUpdate(Dictionary<string, string> attributes)
	{
		if (attributes != null && attributes.Count > 0)
		{
			this.AppendEventToBuffer("user", new Dictionary<string, object>
			{
				{
					"attributes",
					attributes
				}
			}, true);
		}
		else
		{
			SwrveLog.LogError("Invoked user update with no update attributes");
		}
	}

	public virtual void UserUpdate(string name, DateTime date)
	{
		if (name != null)
		{
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			string value = date.Date.ToUniversalTime().ToString("yyyy-MM-ddTHH:mm:ss.fffZ");
			dictionary.Add(name, value);
			this.AppendEventToBuffer("user", new Dictionary<string, object>
			{
				{
					"attributes",
					dictionary
				}
			}, true);
		}
		else
		{
			SwrveLog.LogError("Invoked user update with date with no name specified");
		}
	}

	public virtual void Purchase(string item, string currency, int cost, int quantity)
	{
		this.AppendEventToBuffer("purchase", new Dictionary<string, object>
		{
			{
				"item",
				item
			},
			{
				"currency",
				currency
			},
			{
				"cost",
				cost
			},
			{
				"quantity",
				quantity
			}
		}, true);
	}

	public virtual void Iap(int quantity, string productId, double productPrice, string currency)
	{
		IapRewards rewards = new IapRewards();
		this.Iap(quantity, productId, productPrice, currency, rewards);
	}

	public virtual void Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards)
	{
		this._Iap(quantity, productId, productPrice, currency, rewards, string.Empty, string.Empty, string.Empty, "unknown_store");
	}

	public virtual void CurrencyGiven(string givenCurrency, double amount)
	{
		this.AppendEventToBuffer("currency_given", new Dictionary<string, object>
		{
			{
				"given_currency",
				givenCurrency
			},
			{
				"given_amount",
				amount
			}
		}, true);
	}

	public virtual bool SendQueuedEvents()
	{
		bool result = false;
		if (this.Initialised)
		{
			if (!this.eventsConnecting)
			{
				byte[] array = null;
				if (this.eventsPostString == null || this.eventsPostString.Length == 0)
				{
					this.eventsPostString = this.eventBufferStringBuilder.ToString();
					this.eventBufferStringBuilder.Length = 0;
				}
				if (this.eventsPostString.Length > 0)
				{
					long seconds = SwrveHelper.GetSeconds();
					array = PostBodyBuilder.Build(this.apiKey, this.appId, this.userId, this.GetDeviceId(), this.GetAppVersion(), seconds, this.eventsPostString);
				}
				if (array != null)
				{
					this.eventsConnecting = true;
					SwrveLog.Log("Sending events to Swrve");
					Dictionary<string, string> requestHeaders = new Dictionary<string, string>
					{
						{
							"Content-Type",
							"application/json; charset=utf-8"
						}
					};
					result = true;
					this.StartTask("PostEvents_Coroutine", this.PostEvents_Coroutine(requestHeaders, array));
				}
				else
				{
					this.eventsPostString = null;
				}
			}
			else
			{
				SwrveLog.LogWarning("Sending events already in progress");
			}
		}
		return result;
	}

	public virtual void GetUserResources(Action<Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
		if (this.Initialised)
		{
			if (this.userResources != null)
			{
				onResult(this.userResources, this.userResourcesRaw);
			}
			else
			{
				onResult(new Dictionary<string, Dictionary<string, string>>(), "[]");
			}
		}
	}

	public virtual void GetUserResourcesDiff(Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError)
	{
		if (this.Initialised && !this.abTestUserResourcesDiffConnecting)
		{
			this.abTestUserResourcesDiffConnecting = true;
			StringBuilder stringBuilder = new StringBuilder(this.abTestResourcesDiffUrl);
			stringBuilder.AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", new object[]
			{
				this.escapedUserId,
				this.apiKey,
				WWW.EscapeURL(this.GetAppVersion()),
				this.installTimeEpoch
			});
			SwrveLog.Log("AB Test User Resources Diff request: " + stringBuilder.ToString());
			this.StartTask("GetUserResourcesDiff_Coroutine", this.GetUserResourcesDiff_Coroutine(stringBuilder.ToString(), onResult, onError, "rsdfngt2"));
		}
		else
		{
			string message = "Failed to initiate A/B test Diff GET request";
			SwrveLog.LogError(message);
			if (onError != null)
			{
				onError(new Exception(message));
			}
		}
	}

	public virtual void LoadFromDisk()
	{
		this.LoadEventsFromDisk();
	}

	public virtual void FlushToDisk(bool saveEventsBeingSent = false)
	{
		if (this.Initialised && this.eventBufferStringBuilder != null)
		{
			StringBuilder stringBuilder = new StringBuilder();
			string text = this.eventBufferStringBuilder.ToString();
			this.eventBufferStringBuilder.Length = 0;
			if (saveEventsBeingSent)
			{
				stringBuilder.Append(this.eventsPostString);
				this.eventsPostString = null;
				if (text.Length > 0)
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(text);
				}
			}
			else
			{
				stringBuilder.Append(text);
			}
			try
			{
				string value = this.storage.Load("Swrve_Events", this.userId);
				if (!string.IsNullOrEmpty(value))
				{
					if (stringBuilder.Length != 0)
					{
						stringBuilder.Append(",");
					}
					stringBuilder.Append(value);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogWarning("Could not read events from cache (" + ex.ToString() + ")");
			}
			string data = stringBuilder.ToString();
			this.storage.Save("Swrve_Events", data, this.userId);
		}
	}

	public string BasePath()
	{
		return this.swrvePath;
	}

	public virtual Dictionary<string, string> GetDeviceInfo()
	{
		string deviceModel = this.GetDeviceModel();
		string operatingSystem = SystemInfo.operatingSystem;
		string value = "Android";
		float num = (Screen.dpi != 0f) ? Screen.dpi : 160f;
		Dictionary<string, string> dictionary = new Dictionary<string, string>
		{
			{
				"swrve.device_name",
				deviceModel
			},
			{
				"swrve.os",
				value
			},
			{
				"swrve.device_width",
				this.deviceWidth.ToString()
			},
			{
				"swrve.device_height",
				this.deviceHeight.ToString()
			},
			{
				"swrve.device_dpi",
				num.ToString()
			},
			{
				"swrve.language",
				this.Language
			},
			{
				"swrve.os_version",
				operatingSystem
			},
			{
				"swrve.app_store",
				this.config.AppStore
			},
			{
				"swrve.sdk_version",
				"Unity 4.10.1"
			},
			{
				"swrve.unity_version",
				Application.unityVersion
			},
			{
				"swrve.install_date",
				this.installTimeFormatted
			}
		};
		string value2 = DateTimeOffset.Now.Offset.TotalSeconds.ToString();
		dictionary["swrve.utc_offset_seconds"] = value2;
		this.setNativeInfo(dictionary);
		ICarrierInfo carrierInfoProvider = this.GetCarrierInfoProvider();
		if (carrierInfoProvider != null)
		{
			string name = carrierInfoProvider.GetName();
			if (!string.IsNullOrEmpty(name))
			{
				dictionary["swrve.sim_operator.name"] = name;
			}
			string isoCountryCode = carrierInfoProvider.GetIsoCountryCode();
			if (!string.IsNullOrEmpty(isoCountryCode))
			{
				dictionary["swrve.sim_operator.iso_country_code"] = isoCountryCode;
			}
			string carrierCode = carrierInfoProvider.GetCarrierCode();
			if (!string.IsNullOrEmpty(carrierCode))
			{
				dictionary["swrve.sim_operator.code"] = carrierCode;
			}
		}
		return dictionary;
	}

	public virtual void OnSwrvePause()
	{
		if (this.Initialised)
		{
			this.FlushToDisk(false);
			this.GenerateNewSessionInterval();
			if (this.config != null && this.config.AutoDownloadCampaignsAndResources)
			{
				this.StopCheckForCampaignAndResources();
			}
		}
	}

	public virtual void OnSwrveResume()
	{
		if (this.Initialised)
		{
			this.LoadFromDisk();
			this.QueueDeviceInfo();
			long sessionTime = this.GetSessionTime();
			if (sessionTime >= this.lastSessionTick)
			{
				this.SessionStart();
			}
			else
			{
				this.SendQueuedEvents();
			}
			this.GenerateNewSessionInterval();
			this.StartCampaignsAndResourcesTimer();
			this.DisableAutoShowAfterDelay();
			this.ProcessInfluenceData();
		}
	}

	public virtual void OnSwrveDestroy()
	{
		if (!this.Destroyed)
		{
			this.Destroyed = true;
			if (this.Initialised)
			{
				this.FlushToDisk(true);
			}
			if (this.config != null && this.config.AutoDownloadCampaignsAndResources)
			{
				this.StopCheckForCampaignAndResources();
			}
		}
	}

	public virtual List<SwrveBaseCampaign> GetCampaigns()
	{
		return this.campaigns;
	}

	public virtual void ButtonWasPressedByUser(SwrveButton button)
	{
		if (button != null)
		{
			try
			{
				SwrveLog.Log(string.Concat(new object[]
				{
					"Button ",
					button.ActionType,
					": ",
					button.Action,
					" app id: ",
					button.AppId
				}));
				if (button.ActionType != SwrveActionType.Dismiss)
				{
					string text = "Swrve.Messages.Message-" + button.Message.Id + ".click";
					SwrveLog.Log("Sending click event: " + text);
					this.NamedEventInternal(text, new Dictionary<string, string>
					{
						{
							"name",
							button.Name
						}
					}, false);
				}
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while processing button click " + arg);
			}
		}
	}

	public virtual void MessageWasShownToUser(SwrveMessageFormat messageFormat)
	{
		try
		{
			this.SetMessageMinDelayThrottle();
			this.messagesLeftToShow -= 1L;
			SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)messageFormat.Message.Campaign;
			if (swrveMessagesCampaign != null)
			{
				swrveMessagesCampaign.MessageWasShownToUser(messageFormat);
				this.SaveCampaignData(swrveMessagesCampaign);
			}
			string text = "Swrve.Messages.Message-" + messageFormat.Message.Id + ".impression";
			SwrveLog.Log("Sending view event: " + text);
			this.NamedEventInternal(text, new Dictionary<string, string>
			{
				{
					"format",
					messageFormat.Name
				},
				{
					"orientation",
					messageFormat.Orientation.ToString()
				},
				{
					"size",
					messageFormat.Size.X + "x" + messageFormat.Size.Y
				}
			}, false);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while processing message impression " + arg);
		}
	}

	public virtual bool IsMessageDisplaying()
	{
		return this.currentMessage != null;
	}

	[Obsolete("IsMessageDispaying is deprecated, please use IsMessageDisplaying instead.")]
	public virtual bool IsMessageDispaying()
	{
		return this.IsMessageDisplaying();
	}

	public void SetLocationSegmentVersion(int locationSegmentVersion)
	{
		this.locationSegmentVersion = locationSegmentVersion;
	}

	public void SetConversationVersion(int conversationVersion)
	{
		this.conversationVersion = conversationVersion;
	}

	public string GetAppStoreLink(int appId)
	{
		string result = null;
		if (this.appStoreLinks != null)
		{
			this.appStoreLinks.TryGetValue(appId.ToString(), out result);
		}
		return result;
	}

	public virtual SwrveMessage GetMessageForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		if (!this.checkCampaignRules(eventName, SwrveHelper.GetNow()))
		{
			return null;
		}
		try
		{
			return this._getMessageForEvent(eventName, payload);
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "message");
		}
		return null;
	}

	private SwrveMessage _getMessageForEvent(string eventName, IDictionary<string, string> payload)
	{
		SwrveMessage swrveMessage = null;
		SwrveBaseCampaign swrveBaseCampaign = null;
		SwrveLog.Log("Trying to get message for: " + eventName);
		IEnumerator<SwrveBaseCampaign> enumerator = this.campaigns.GetEnumerator();
		List<SwrveMessage> list = new List<SwrveMessage>();
		int num = 2147483647;
		List<SwrveMessage> list2 = new List<SwrveMessage>();
		SwrveOrientation deviceOrientation = this.GetDeviceOrientation();
		while (enumerator.MoveNext() && swrveMessage == null)
		{
			if (enumerator.Current.IsA<SwrveMessagesCampaign>())
			{
				SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)enumerator.Current;
				SwrveMessage messageForEvent = swrveMessagesCampaign.GetMessageForEvent(eventName, payload, this.qaUser);
				if (messageForEvent != null)
				{
					if (messageForEvent.SupportsOrientation(deviceOrientation))
					{
						list.Add(messageForEvent);
						if (messageForEvent.Priority <= num)
						{
							if (messageForEvent.Priority < num)
							{
								list2.Clear();
							}
							num = messageForEvent.Priority;
							list2.Add(messageForEvent);
						}
					}
					else if (this.qaUser != null)
					{
						this.qaUser.campaignMessages[swrveMessagesCampaign.Id] = messageForEvent;
						this.qaUser.campaignReasons[swrveMessagesCampaign.Id] = "Message didn't support the current device orientation: " + deviceOrientation;
					}
				}
			}
		}
		if (list2.Count > 0)
		{
			list2.Shuffle<SwrveMessage>();
			swrveMessage = list2[0];
			swrveBaseCampaign = swrveMessage.Campaign;
		}
		if (this.qaUser != null && swrveBaseCampaign != null && swrveMessage != null)
		{
			IEnumerator<SwrveMessage> enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				SwrveMessage current = enumerator2.Current;
				if (current != swrveMessage)
				{
					int id = current.Campaign.Id;
					if (this.qaUser != null && !this.qaUser.campaignMessages.ContainsKey(id))
					{
						this.qaUser.campaignMessages.Add(id, current);
						this.qaUser.campaignReasons.Add(id, "Campaign " + swrveBaseCampaign.Id + " was selected for display ahead of this campaign");
					}
				}
			}
		}
		return swrveMessage;
	}

	public virtual SwrveConversation GetConversationForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		if (!this.checkCampaignRules(eventName, SwrveHelper.GetNow()))
		{
			return null;
		}
		try
		{
			return this._getConversationForEvent(eventName, payload);
		}
		catch (Exception ex)
		{
			SwrveLog.LogError(ex.ToString(), "conversation");
		}
		return null;
	}

	private SwrveConversation _getConversationForEvent(string eventName, IDictionary<string, string> payload = null)
	{
		SwrveConversation swrveConversation = null;
		SwrveBaseCampaign swrveBaseCampaign = null;
		SwrveLog.Log("Trying to get conversation for: " + eventName);
		IEnumerator<SwrveBaseCampaign> enumerator = this.campaigns.GetEnumerator();
		List<SwrveConversation> list = new List<SwrveConversation>();
		int num = 2147483647;
		List<SwrveConversation> list2 = new List<SwrveConversation>();
		while (enumerator.MoveNext() && swrveConversation == null)
		{
			if (enumerator.Current.IsA<SwrveConversationCampaign>())
			{
				SwrveConversationCampaign swrveConversationCampaign = (SwrveConversationCampaign)enumerator.Current;
				SwrveConversation conversationForEvent = swrveConversationCampaign.GetConversationForEvent(eventName, payload, this.qaUser);
				if (conversationForEvent != null)
				{
					list.Add(conversationForEvent);
					if (conversationForEvent.Priority <= num)
					{
						if (conversationForEvent.Priority < num)
						{
							list2.Clear();
						}
						num = conversationForEvent.Priority;
						list2.Add(conversationForEvent);
					}
				}
			}
		}
		if (list2.Count > 0)
		{
			list2.Shuffle<SwrveConversation>();
			swrveConversation = list2[0];
		}
		if (this.qaUser != null && swrveBaseCampaign != null && swrveConversation != null)
		{
			IEnumerator<SwrveConversation> enumerator2 = list.GetEnumerator();
			while (enumerator2.MoveNext())
			{
				SwrveConversation current = enumerator2.Current;
				if (current != swrveConversation)
				{
					int id = current.Campaign.Id;
					if (this.qaUser != null && !this.qaUser.campaignMessages.ContainsKey(id))
					{
						this.qaUser.campaignMessages[id] = current;
						this.qaUser.campaignReasons[id] = "Campaign " + swrveBaseCampaign.Id + " was selected for display ahead of this campaign";
					}
				}
			}
		}
		return swrveConversation;
	}

	private bool checkCampaignRules(string eventName, DateTime now)
	{
		if (this.campaigns == null || this.campaigns.Count == 0)
		{
			this.NoMessagesWereShown(eventName, "No campaigns available");
			return false;
		}
		if (!string.Equals(eventName, "Swrve.Messages.showAtSessionStart", StringComparison.OrdinalIgnoreCase) && this.IsTooSoonToShowMessageAfterLaunch(now))
		{
			this.NoMessagesWereShown(eventName, "{App throttle limit} Too soon after launch. Wait until " + this.showMessagesAfterLaunch.ToString(SwrveSDK.WaitTimeFormat));
			return false;
		}
		if (this.IsTooSoonToShowMessageAfterDelay(now))
		{
			this.NoMessagesWereShown(eventName, "{App throttle limit} Too soon after last base message. Wait until " + this.showMessagesAfterDelay.ToString(SwrveSDK.WaitTimeFormat));
			return false;
		}
		if (this.HasShowTooManyMessagesAlready())
		{
			this.NoMessagesWereShown(eventName, "{App throttle limit} Too many base messages shown");
			return false;
		}
		return true;
	}

	public virtual void ShowMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
		this.ShowMessageCenterCampaign(campaign, this.GetDeviceOrientation());
	}

	public virtual void ShowMessageCenterCampaign(SwrveBaseCampaign campaign, SwrveOrientation orientation)
	{
		if (campaign.IsA<SwrveMessagesCampaign>())
		{
			this.Container.StartCoroutine(this.LaunchMessage((from a in ((SwrveMessagesCampaign)campaign).Messages
			where a.SupportsOrientation(orientation)
			select a).First<SwrveMessage>(), this.GlobalInstallButtonListener, this.GlobalCustomButtonListener, this.GlobalMessageListener));
		}
		else if (campaign.IsA<SwrveConversationCampaign>())
		{
			this.Container.StartCoroutine(this.LaunchConversation(((SwrveConversationCampaign)campaign).Conversation));
		}
		campaign.Status = SwrveCampaignState.Status.Seen;
		this.SaveCampaignData(campaign);
	}

	public virtual List<SwrveBaseCampaign> GetMessageCenterCampaigns()
	{
		return this.GetMessageCenterCampaigns(this.GetDeviceOrientation());
	}

	public virtual List<SwrveBaseCampaign> GetMessageCenterCampaigns(SwrveOrientation orientation)
	{
		List<SwrveBaseCampaign> list = new List<SwrveBaseCampaign>();
		IEnumerator<SwrveBaseCampaign> enumerator = this.campaigns.GetEnumerator();
		while (enumerator.MoveNext())
		{
			SwrveBaseCampaign current = enumerator.Current;
			if (this.isValidMessageCenter(current, orientation))
			{
				list.Add(current);
			}
		}
		return list;
	}

	public virtual void RemoveMessageCenterCampaign(SwrveBaseCampaign campaign)
	{
		campaign.Status = SwrveCampaignState.Status.Deleted;
		this.SaveCampaignData(campaign);
	}

	[Obsolete("This method is for internal use only and will be removed in later version.")]
	public bool IsAssetInCache(string asset)
	{
		return asset != null && this.GetAssetsOnDisk().Contains(asset);
	}

	[Obsolete("This method is for internal use only and will be removed in later version.")]
	public HashSet<string> GetAssetsOnDisk()
	{
		return this.SwrveAssetsManager.AssetsOnDisk;
	}

	public virtual SwrveMessage GetMessageForId(int messageId)
	{
		SwrveMessage swrveMessage = null;
		IEnumerator<SwrveBaseCampaign> enumerator = this.campaigns.GetEnumerator();
		while (enumerator.MoveNext() && swrveMessage == null)
		{
			if (enumerator.Current.IsA<SwrveMessagesCampaign>())
			{
				SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)enumerator.Current;
				swrveMessage = swrveMessagesCampaign.GetMessageForId(messageId);
				if (swrveMessage != null)
				{
					return swrveMessage;
				}
			}
		}
		SwrveLog.LogWarning("Message with id " + messageId + " not found");
		return null;
	}

	[DebuggerHidden]
	public virtual IEnumerator ShowMessageForEvent(string eventName, SwrveMessage message, ISwrveInstallButtonListener installButtonListener = null, ISwrveCustomButtonListener customButtonListener = null, ISwrveMessageListener messageListener = null)
	{
		if (this.TriggeredMessageListener != null)
		{
			if (message != null)
			{
				this.TriggeredMessageListener.OnMessageTriggered(message);
			}
		}
		else if (this.currentMessage == null)
		{
			yield return this.Container.StartCoroutine(this.LaunchMessage(message, installButtonListener, customButtonListener, messageListener));
		}
		this.TaskFinished("ShowMessageForEvent");
		yield break;
	}

	[DebuggerHidden]
	public virtual IEnumerator ShowConversationForEvent(string eventName, SwrveConversation conversation)
	{
		yield return this.Container.StartCoroutine(this.LaunchConversation(conversation));
		this.TaskFinished("ShowConversationForEvent");
		yield break;
	}

	public virtual void DismissMessage()
	{
		if (this.TriggeredMessageListener != null)
		{
			this.TriggeredMessageListener.DismissCurrentMessage();
		}
		else
		{
			try
			{
				if (this.currentMessage != null)
				{
					this.SetMessageMinDelayThrottle();
					this.currentMessage.Dismiss();
				}
			}
			catch (Exception arg)
			{
				SwrveLog.LogError("Error while dismissing a message " + arg);
			}
		}
	}

	public virtual void RefreshUserResourcesAndCampaigns()
	{
		this.LoadResourcesAndCampaigns();
	}

	private void QueueSessionStart()
	{
		Dictionary<string, object> eventParameters = new Dictionary<string, object>();
		this.AppendEventToBuffer("session_start", eventParameters, true);
	}

	protected void NamedEventInternal(string name, Dictionary<string, string> payload = null, bool allowShowMessage = true)
	{
		if (payload == null)
		{
			payload = new Dictionary<string, string>();
		}
		this.AppendEventToBuffer("event", new Dictionary<string, object>
		{
			{
				"name",
				name
			},
			{
				"payload",
				payload
			}
		}, allowShowMessage);
	}

	protected static string GetSwrvePath()
	{
		string text = Application.persistentDataPath;
		if (string.IsNullOrEmpty(text))
		{
			text = Application.temporaryCachePath;
			SwrveLog.Log("Swrve path (tried again): " + text);
		}
		return text;
	}

	protected static string GetSwrveTemporaryCachePath()
	{
		string text = Application.temporaryCachePath;
		if (text == null || text.Length == 0)
		{
			text = Application.persistentDataPath;
		}
		if (!File.Exists(text))
		{
			Directory.CreateDirectory(text);
		}
		return text;
	}

	private void _Iap(int quantity, string productId, double productPrice, string currency, IapRewards rewards, string receipt, string receiptSignature, string transactionId, string appStore)
	{
		if (!this._Iap_check_arguments(quantity, productId, productPrice, currency, appStore))
		{
			SwrveLog.LogError("ERROR: IAP event not sent because it received an illegal argument");
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		dictionary.Add("app_store", appStore);
		dictionary.Add("local_currency", currency);
		dictionary.Add("cost", productPrice);
		dictionary.Add("product_id", productId);
		dictionary.Add("quantity", quantity);
		dictionary.Add("rewards", rewards.getRewards());
		if (!string.IsNullOrEmpty(this.GetAppVersion()))
		{
			dictionary.Add("app_version", this.GetAppVersion());
		}
		if (appStore == "apple")
		{
			dictionary.Add("receipt", receipt);
			if (!string.IsNullOrEmpty(transactionId))
			{
				dictionary.Add("transaction_id", transactionId);
			}
		}
		else if (appStore == "google")
		{
			dictionary.Add("receipt", receipt);
			dictionary.Add("receipt_signature", receiptSignature);
		}
		else
		{
			dictionary.Add("receipt", receipt);
		}
		this.AppendEventToBuffer("iap", dictionary, true);
		if (this.config.AutoDownloadCampaignsAndResources)
		{
			this.CheckForCampaignsAndResourcesUpdates(false);
		}
	}

	protected virtual SwrveOrientation GetDeviceOrientation()
	{
		switch (Screen.orientation)
		{
		case ScreenOrientation.Portrait:
		case ScreenOrientation.PortraitUpsideDown:
			return SwrveOrientation.Portrait;
		case ScreenOrientation.LandscapeLeft:
		case ScreenOrientation.LandscapeRight:
			return SwrveOrientation.Landscape;
		default:
			if (Screen.height >= Screen.width)
			{
				return SwrveOrientation.Portrait;
			}
			return SwrveOrientation.Landscape;
		}
	}

	private bool _Iap_check_arguments(int quantity, string productId, double productPrice, string currency, string appStore)
	{
		if (string.IsNullOrEmpty(productId))
		{
			SwrveLog.LogError("IAP event illegal argument: productId cannot be empty");
			return false;
		}
		if (string.IsNullOrEmpty(currency))
		{
			SwrveLog.LogError("IAP event illegal argument: currency cannot be empty");
			return false;
		}
		if (string.IsNullOrEmpty(appStore))
		{
			SwrveLog.LogError("IAP event illegal argument: appStore cannot be empty");
			return false;
		}
		if (quantity <= 0)
		{
			SwrveLog.LogError("IAP event illegal argument: quantity must be greater than zero");
			return false;
		}
		if (productPrice < 0.0)
		{
			SwrveLog.LogError("IAP event illegal argument: productPrice must be greater than or equal to zero");
			return false;
		}
		return true;
	}

	private Dictionary<string, Dictionary<string, string>> ProcessUserResources(IList<object> userResources)
	{
		Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
		if (userResources != null)
		{
			IEnumerator<object> enumerator = userResources.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)enumerator.Current;
				string key = (string)dictionary2["uid"];
				dictionary.Add(key, this.NormalizeJson(dictionary2));
			}
		}
		return dictionary;
	}

	private Dictionary<string, string> NormalizeJson(Dictionary<string, object> json)
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		Dictionary<string, object>.Enumerator enumerator = json.GetEnumerator();
		while (enumerator.MoveNext())
		{
			KeyValuePair<string, object> current = enumerator.Current;
			if (current.Value != null)
			{
				dictionary.Add(current.Key, current.Value.ToString());
			}
		}
		return dictionary;
	}

	[DebuggerHidden]
	private IEnumerator GetUserResourcesDiff_Coroutine(string getRequest, Action<Dictionary<string, Dictionary<string, string>>, Dictionary<string, Dictionary<string, string>>, string> onResult, Action<Exception> onError, string saveCategory)
	{
		Exception wwwException = null;
		string abTestCandidate = null;
		yield return this.Container.StartCoroutine(this.restClient.Get(getRequest, delegate(RESTResponse response)
		{
			if (response.Error == WwwDeducedError.NoError)
			{
				abTestCandidate = response.Body;
				SwrveLog.Log("AB Test result: " + abTestCandidate);
				this.$this.storage.SaveSecure(saveCategory, abTestCandidate, this.$this.userId);
				this.$this.TaskFinished("GetUserResourcesDiff_Coroutine");
			}
			else
			{
				wwwException = new Exception(response.Error.ToString());
				SwrveLog.LogError("AB Test request failed: " + response.Error.ToString());
				this.$this.TaskFinished("GetUserResourcesDiff_Coroutine");
			}
		}));
		this.abTestUserResourcesDiffConnecting = false;
		if (wwwException == null)
		{
			if (!string.IsNullOrEmpty(abTestCandidate))
			{
				if (!string.IsNullOrEmpty(abTestCandidate))
				{
					Dictionary<string, Dictionary<string, string>> dictionary = new Dictionary<string, Dictionary<string, string>>();
					Dictionary<string, Dictionary<string, string>> dictionary2 = new Dictionary<string, Dictionary<string, string>>();
					this.ProcessUserResourcesDiff(abTestCandidate, dictionary, dictionary2);
					onResult(dictionary, dictionary2, abTestCandidate);
					goto IL_236;
				}
				goto IL_236;
			}
		}
		try
		{
			string text = this.storage.LoadSecure(saveCategory, this.userId);
			if (string.IsNullOrEmpty(text))
			{
				onError(wwwException);
			}
			else if (ResponseBodyTester.TestUTF8(text, out abTestCandidate))
			{
				Dictionary<string, Dictionary<string, string>> dictionary3 = new Dictionary<string, Dictionary<string, string>>();
				Dictionary<string, Dictionary<string, string>> dictionary4 = new Dictionary<string, Dictionary<string, string>>();
				this.ProcessUserResourcesDiff(abTestCandidate, dictionary3, dictionary4);
				onResult(dictionary3, dictionary4, abTestCandidate);
			}
			else
			{
				onError(wwwException);
			}
		}
		catch (Exception var_4_1A2)
		{
		}
		IL_236:
		yield break;
	}

	private void ProcessUserResourcesDiff(string abTestJson, Dictionary<string, Dictionary<string, string>> newResources, Dictionary<string, Dictionary<string, string>> oldResources)
	{
		IList<object> list = (List<object>)Json.Deserialize(abTestJson);
		if (list != null)
		{
			IEnumerator<object> enumerator = list.GetEnumerator();
			while (enumerator.MoveNext())
			{
				Dictionary<string, object> dictionary = (Dictionary<string, object>)enumerator.Current;
				string key = (string)dictionary["uid"];
				Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["diff"];
				IEnumerator<string> enumerator2 = dictionary2.Keys.GetEnumerator();
				Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
				Dictionary<string, string> dictionary4 = new Dictionary<string, string>();
				while (enumerator2.MoveNext())
				{
					Dictionary<string, string> dictionary5 = this.NormalizeJson((Dictionary<string, object>)dictionary2[enumerator2.Current]);
					dictionary3.Add(enumerator2.Current, dictionary5["new"]);
					dictionary4.Add(enumerator2.Current, dictionary5["old"]);
				}
				newResources.Add(key, dictionary3);
				oldResources.Add(key, dictionary4);
			}
		}
	}

	private long GetInstallTimeEpoch()
	{
		string savedInstallTimeEpoch = this.GetSavedInstallTimeEpoch();
		if (!string.IsNullOrEmpty(savedInstallTimeEpoch))
		{
			long result = 0L;
			if (long.TryParse(savedInstallTimeEpoch, out result))
			{
				return result;
			}
		}
		long sessionTime = this.GetSessionTime();
		this.storage.Save("Swrve_JoinedDate", sessionTime.ToString(), this.userId);
		return sessionTime;
	}

	private string GetDeviceId()
	{
		string text = this.storage.Load("Swrve_DeviceId", this.userId);
		if (!string.IsNullOrEmpty(text))
		{
			return text;
		}
		short num = (short)new System.Random().Next(32767);
		this.storage.Save("Swrve_DeviceId", num.ToString(), this.userId);
		return num.ToString();
	}

	private string getNextSeqNum()
	{
		string text = this.storage.Load("Swrve_SeqNum", this.userId);
		int num;
		string arg_41_0;
		if (int.TryParse(text, out num))
		{
			int num2;
			num = (num2 = num + 1);
			arg_41_0 = num2.ToString();
		}
		else
		{
			arg_41_0 = "1";
		}
		text = arg_41_0;
		this.storage.Save("Swrve_SeqNum", text, this.userId);
		return text;
	}

	protected string GetDeviceLanguage()
	{
		string text = this.getNativeLanguage();
		if (string.IsNullOrEmpty(text))
		{
			CultureInfo currentUICulture = CultureInfo.CurrentUICulture;
			string text2 = currentUICulture.TwoLetterISOLanguageName.ToLower();
			if (text2 != "iv")
			{
				text = text2;
			}
		}
		return text;
	}

	protected string GetSavedInstallTimeEpoch()
	{
		try
		{
			string text = this.storage.Load("Swrve_JoinedDate", this.userId);
			if (!string.IsNullOrEmpty(text))
			{
				return text;
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogError("Couldn't obtain saved install time: " + ex.Message);
		}
		return null;
	}

	protected void InvalidateETag()
	{
		this.lastETag = string.Empty;
		this.storage.Remove("cmpg_etag", this.userId);
	}

	private void InitUserResources()
	{
		this.userResourcesRaw = this.storage.LoadSecure("srcngt2", this.userId);
		if (!string.IsNullOrEmpty(this.userResourcesRaw))
		{
			IList<object> list = (IList<object>)Json.Deserialize(this.userResourcesRaw);
			this.userResources = this.ProcessUserResources(list);
			this.NotifyUpdateUserResources();
		}
		else
		{
			this.InvalidateETag();
		}
	}

	private void NotifyUpdateUserResources()
	{
		if (this.userResources != null)
		{
			this.ResourceManager.SetResourcesFromJSON(this.userResources);
			if (this.ResourcesUpdatedCallback != null)
			{
				this.ResourcesUpdatedCallback();
			}
		}
	}

	private void LoadEventsFromDisk()
	{
		try
		{
			string value = this.storage.Load("Swrve_Events", this.userId);
			this.storage.Remove("Swrve_Events", this.userId);
			if (!string.IsNullOrEmpty(value))
			{
				if (this.eventBufferStringBuilder.Length != 0)
				{
					this.eventBufferStringBuilder.Insert(0, ",");
				}
				this.eventBufferStringBuilder.Insert(0, value);
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Could not read events from cache (" + ex.ToString() + ")");
		}
	}

	private void LoadData()
	{
		this.LoadEventsFromDisk();
		this.installTimeEpoch = this.GetInstallTimeEpoch();
		this.installTimeFormatted = SwrveHelper.EpochToFormat(this.installTimeEpoch, SwrveSDK.InstallTimeFormat);
		this.lastETag = this.storage.Load("cmpg_etag", this.userId);
		string text = this.storage.Load("swrve_cr_flush_frequency", this.userId);
		if (!string.IsNullOrEmpty(text) && float.TryParse(text, out this.campaignsAndResourcesFlushFrequency))
		{
			this.campaignsAndResourcesFlushFrequency /= 1000f;
		}
		if (this.campaignsAndResourcesFlushFrequency == 0f)
		{
			this.campaignsAndResourcesFlushFrequency = 60f;
		}
		string text2 = this.storage.Load("swrve_cr_flush_delay", this.userId);
		if (!string.IsNullOrEmpty(text2) && float.TryParse(text2, out this.campaignsAndResourcesFlushRefreshDelay))
		{
			this.campaignsAndResourcesFlushRefreshDelay /= 1000f;
		}
		if (this.campaignsAndResourcesFlushRefreshDelay == 0f)
		{
			this.campaignsAndResourcesFlushRefreshDelay = 5f;
		}
	}

	protected string GetUniqueKey()
	{
		return this.apiKey + this.userId;
	}

	private string GetDeviceUniqueId()
	{
		string text = PlayerPrefs.GetString("Swrve.deviceUniqueIdentifier", null);
		if (string.IsNullOrEmpty(text))
		{
			text = this.GetRandomUUID();
		}
		return text;
	}

	private string GetRandomUUID()
	{
		try
		{
			Type type = Type.GetType("System.Guid");
			if (type != null)
			{
				MethodInfo method = type.GetMethod("NewGuid");
				if (method != null)
				{
					object obj = method.Invoke(null, null);
					if (obj != null)
					{
						string text = obj.ToString();
						if (!string.IsNullOrEmpty(text))
						{
							return text;
						}
					}
				}
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get random UUID: " + ex.ToString());
		}
		string text2 = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
		string text3 = string.Empty;
		for (int i = 0; i < 128; i++)
		{
			int index = this.rnd.Next(text2.Length);
			text3 += text2[index];
		}
		return text3;
	}

	protected virtual IRESTClient CreateRestClient()
	{
		return new RESTClient();
	}

	protected virtual ISwrveStorage CreateStorage()
	{
		if (this.config.StoreDataInPlayerPrefs)
		{
			return new SwrvePlayerPrefsStorage();
		}
		return new SwrveFileStorage(this.swrvePath, this.GetUniqueKey());
	}

	[DebuggerHidden]
	private IEnumerator PostEvents_Coroutine(Dictionary<string, string> requestHeaders, byte[] eventsPostEncodedData)
	{
		yield return this.Container.StartCoroutine(this.restClient.Post(this.eventsUrl, eventsPostEncodedData, requestHeaders, delegate(RESTResponse response)
		{
			if (response.Error != WwwDeducedError.NetworkError)
			{
				this.$this.ClearEventBuffer();
				eventsPostEncodedData = null;
			}
			this.$this.eventsConnecting = false;
			this.$this.TaskFinished("PostEvents_Coroutine");
		}));
		yield break;
	}

	protected virtual void ClearEventBuffer()
	{
		this.eventsPostString = null;
	}

	private void AppendEventToBuffer(string eventType, Dictionary<string, object> eventParameters, bool allowShowMessage = true)
	{
		eventParameters.Add("type", eventType);
		eventParameters.Add("seqnum", this.getNextSeqNum());
		eventParameters.Add("time", this.GetSessionTime());
		string text = Json.Serialize(eventParameters);
		string eventName = SwrveHelper.GetEventName(eventParameters);
		bool flag = this.eventBufferStringBuilder.Length + text.Length <= this.config.MaxBufferChars;
		if (flag || this.config.SendEventsIfBufferTooLarge)
		{
			if (!flag && this.config.SendEventsIfBufferTooLarge)
			{
				this.SendQueuedEvents();
			}
			if (this.eventBufferStringBuilder.Length > 0)
			{
				this.eventBufferStringBuilder.Append(',');
			}
			this.AppendEventToBuffer(text);
		}
		else
		{
			SwrveLog.LogError("Could not append the event to the buffer. Please consider enabling SendEventsIfBufferTooLarge");
		}
		if (allowShowMessage)
		{
			object obj;
			eventParameters.TryGetValue("payload", out obj);
			this.ShowBaseMessage(eventName, (IDictionary<string, string>)obj);
		}
	}

	protected virtual void AppendEventToBuffer(string eventJson)
	{
		this.eventBufferStringBuilder.Append(eventJson);
	}

	protected virtual Coroutine StartTask(string tag, IEnumerator task)
	{
		return this.Container.StartCoroutine(task);
	}

	protected virtual void TaskFinished(string tag)
	{
	}

	protected void ShowBaseMessage(string eventName, IDictionary<string, string> payload)
	{
		SwrveBaseMessage baseMessage = this.GetBaseMessage(eventName, payload);
		if (baseMessage != null)
		{
			if (baseMessage.Campaign.IsA<SwrveConversationCampaign>())
			{
				this.StartTask("ShowConversationForEvent", this.ShowConversationForEvent(eventName, (SwrveConversation)baseMessage));
			}
			else
			{
				this.StartTask("ShowMessageForEvent", this.ShowMessageForEvent(eventName, (SwrveMessage)baseMessage, this.GlobalInstallButtonListener, this.GlobalCustomButtonListener, this.GlobalMessageListener));
			}
		}
		if (this.qaUser != null)
		{
			this.qaUser.Trigger(eventName, baseMessage);
		}
		if (baseMessage != null)
		{
			this.NamedEventInternal(baseMessage.GetEventPrefix() + "returned", new Dictionary<string, string>
			{
				{
					"id",
					baseMessage.Id.ToString()
				}
			}, false);
		}
	}

	public SwrveBaseMessage GetBaseMessage(string eventName, IDictionary<string, string> payload = null)
	{
		if (!this.checkCampaignRules(eventName, SwrveHelper.GetNow()))
		{
			return null;
		}
		SwrveBaseMessage swrveBaseMessage = null;
		if (this.config.ConversationsEnabled)
		{
			swrveBaseMessage = this.GetConversationForEvent(eventName, payload);
		}
		if (swrveBaseMessage == null && this.config.TalkEnabled)
		{
			swrveBaseMessage = this.GetMessageForEvent(eventName, payload);
		}
		if (swrveBaseMessage == null)
		{
			SwrveLog.Log("Not showing message: no candidate for " + eventName);
		}
		else
		{
			SwrveLog.Log(string.Format("[{0}] {1} has been chosen for {2}\nstate: {3}", new object[]
			{
				swrveBaseMessage,
				swrveBaseMessage.Campaign.Id,
				eventName,
				swrveBaseMessage.Campaign.State
			}));
		}
		return swrveBaseMessage;
	}

	private bool IsAlive()
	{
		return this.Container != null && !this.Destroyed;
	}

	protected virtual void GetDeviceScreenInfo()
	{
		this.deviceWidth = Screen.width;
		this.deviceHeight = Screen.height;
		if (this.deviceWidth > this.deviceHeight)
		{
			int num = this.deviceWidth;
			this.deviceWidth = this.deviceHeight;
			this.deviceHeight = num;
		}
	}

	private void QueueDeviceInfo()
	{
		Dictionary<string, string> deviceInfo = this.GetDeviceInfo();
		this.UserUpdate(deviceInfo);
	}

	private void SendDeviceInfo()
	{
		this.QueueDeviceInfo();
		this.SendQueuedEvents();
	}

	[DebuggerHidden]
	private IEnumerator WaitAndRefreshResourcesAndCampaigns_Coroutine(float delay)
	{
		yield return new WaitForSeconds(delay);
		this.RefreshUserResourcesAndCampaigns();
		yield break;
	}

	private void CheckForCampaignsAndResourcesUpdates(bool invokedByTimer)
	{
		if (!this.IsAlive())
		{
			return;
		}
		bool flag = this.SendQueuedEvents();
		if (flag)
		{
			this.Container.StartCoroutine(this.WaitAndRefreshResourcesAndCampaigns_Coroutine(this.campaignsAndResourcesFlushRefreshDelay));
		}
		if (!invokedByTimer)
		{
			this.StopCheckForCampaignAndResources();
			this.StartCheckForCampaignsAndResources();
		}
	}

	private void StartCheckForCampaignsAndResources()
	{
		if (this.campaignAndResourcesCoroutineInstance == null)
		{
			this.campaignAndResourcesCoroutineInstance = this.CheckForCampaignsAndResourcesUpdates_Coroutine();
			this.Container.StartCoroutine(this.campaignAndResourcesCoroutineInstance);
		}
		this.campaignAndResourcesCoroutineEnabled = true;
	}

	private void StopCheckForCampaignAndResources()
	{
		if (this.campaignAndResourcesCoroutineInstance != null)
		{
			this.Container.StopCoroutine("campaignAndResourcesCoroutineInstance");
			this.campaignAndResourcesCoroutineInstance = null;
		}
		this.campaignAndResourcesCoroutineEnabled = false;
	}

	[DebuggerHidden]
	private IEnumerator CheckForCampaignsAndResourcesUpdates_Coroutine()
	{
		yield return new WaitForSeconds(this.campaignsAndResourcesFlushFrequency);
		this.CheckForCampaignsAndResourcesUpdates(true);
		if (this.campaignAndResourcesCoroutineEnabled)
		{
			this.campaignAndResourcesCoroutineInstance = null;
			this.StartCheckForCampaignsAndResources();
		}
		yield break;
	}

	protected virtual long GetSessionTime()
	{
		return SwrveHelper.GetMilliseconds();
	}

	private void GenerateNewSessionInterval()
	{
		this.lastSessionTick = this.GetSessionTime() + (long)(this.config.NewSessionInterval * 1000);
	}

	public void Update()
	{
		if (this.currentDisplayingMessage != null)
		{
			if (!this.currentMessage.Closing)
			{
				if (this.inputManager.GetMouseButtonDown(0))
				{
					this.ProcessButtonDown();
				}
				else if (this.inputManager.GetMouseButtonUp(0))
				{
					this.ProcessButtonUp();
				}
			}
			if (!this.currentMessage.Closing && this.NativeIsBackPressed())
			{
				this.currentMessage.Dismiss();
			}
		}
	}

	public void OnGUI()
	{
		if (this.currentDisplayingMessage != null)
		{
			SwrveOrientation deviceOrientation = this.GetDeviceOrientation();
			if (deviceOrientation != this.currentOrientation)
			{
				if (this.currentDisplayingMessage.Orientation != deviceOrientation)
				{
					bool flag = this.currentDisplayingMessage.Message.SupportsOrientation(deviceOrientation);
					if (flag)
					{
						this.StartTask("SwitchMessageOrienation", this.SwitchMessageOrienation(deviceOrientation));
					}
					else
					{
						this.currentDisplayingMessage.Rotate = true;
					}
				}
				else
				{
					this.currentDisplayingMessage.Rotate = false;
				}
			}
			int depth = GUI.depth;
			Matrix4x4 matrix = GUI.matrix;
			GUI.depth = 0;
			SwrveMessageRenderer.DrawMessage(this.currentMessage, Screen.width / 2 + this.currentMessage.Message.Position.X, Screen.height / 2 + this.currentMessage.Message.Position.Y);
			GUI.matrix = matrix;
			GUI.depth = depth;
			if (this.currentDisplayingMessage.MessageListener != null)
			{
				this.currentDisplayingMessage.MessageListener.OnShowing(this.currentDisplayingMessage);
			}
			if (this.currentMessage.Dismissed)
			{
				this.currentMessage = null;
				this.currentDisplayingMessage = null;
			}
			this.currentOrientation = deviceOrientation;
		}
	}

	[DebuggerHidden]
	private IEnumerator SwitchMessageOrienation(SwrveOrientation newOrientation)
	{
		SwrveMessageFormat format = this.currentMessage.Message.GetFormat(newOrientation);
		if (format != null && format != this.currentMessage)
		{
			SwrveMessageFormat swrveMessageFormat = this.currentMessage;
			CoroutineReference<bool> coroutineReference = new CoroutineReference<bool>(false);
			yield return this.StartTask("PreloadFormatAssets", this.PreloadFormatAssets(format, coroutineReference));
			if (coroutineReference.Value())
			{
				this.currentOrientation = this.GetDeviceOrientation();
				format.Init(this.currentOrientation);
				format.MessageListener = swrveMessageFormat.MessageListener;
				format.CustomButtonListener = swrveMessageFormat.CustomButtonListener;
				format.InstallButtonListener = swrveMessageFormat.InstallButtonListener;
				this.currentMessage = (this.currentDisplayingMessage = format);
				swrveMessageFormat.UnloadAssets();
			}
			else
			{
				SwrveLog.LogError("Could not switch orientation. Not all assets could be preloaded");
			}
			this.TaskFinished("SwitchMessageOrienation");
		}
		yield break;
	}

	private void ProcessButtonDown()
	{
		Vector3 mousePosition = this.inputManager.GetMousePosition();
		for (int i = 0; i < this.currentMessage.Buttons.Count; i++)
		{
			SwrveButton swrveButton = this.currentMessage.Buttons[i];
			if (swrveButton.PointerRect.Contains(mousePosition))
			{
				swrveButton.Pressed = true;
			}
		}
	}

	private void ProcessButtonUp()
	{
		SwrveButton swrveButton = null;
		int num = this.currentMessage.Buttons.Count - 1;
		while (num >= 0 && swrveButton == null)
		{
			SwrveButton swrveButton2 = this.currentMessage.Buttons[num];
			Vector3 mousePosition = this.inputManager.GetMousePosition();
			if (swrveButton2.PointerRect.Contains(mousePosition) && swrveButton2.Pressed)
			{
				swrveButton = swrveButton2;
			}
			else
			{
				swrveButton2.Pressed = false;
			}
			num--;
		}
		if (swrveButton != null)
		{
			SwrveLog.Log("Clicked button " + swrveButton.ActionType);
			this.ButtonWasPressedByUser(swrveButton);
			try
			{
				if (swrveButton.ActionType == SwrveActionType.Install)
				{
					string text = swrveButton.AppId.ToString();
					if (this.appStoreLinks.ContainsKey(text))
					{
						string text2 = this.appStoreLinks[text];
						if (!string.IsNullOrEmpty(text2))
						{
							bool flag = true;
							if (this.currentMessage.InstallButtonListener != null)
							{
								flag = this.currentMessage.InstallButtonListener.OnAction(text2);
							}
							if (flag)
							{
								this.OpenURL(text2);
							}
						}
						else
						{
							SwrveLog.LogError("No app store url for app " + text);
						}
					}
					else
					{
						SwrveLog.LogError("Install button app store url empty!");
					}
				}
				else if (swrveButton.ActionType == SwrveActionType.Custom)
				{
					string action = swrveButton.Action;
					if (this.currentMessage.CustomButtonListener != null)
					{
						this.currentMessage.CustomButtonListener.OnAction(action);
					}
					else
					{
						SwrveLog.Log("No custom button listener, treating action as URL");
						if (!string.IsNullOrEmpty(action))
						{
							this.OpenURL(action);
						}
					}
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("Error processing the clicked button: " + ex.Message);
			}
			swrveButton.Pressed = false;
			this.DismissMessage();
		}
	}

	protected virtual void OpenURL(string url)
	{
		Application.OpenURL(url);
	}

	protected void SetMessageMinDelayThrottle()
	{
		this.showMessagesAfterDelay = SwrveHelper.GetNow() + TimeSpan.FromSeconds((double)this.minDelayBetweenMessage);
	}

	private void AutoShowMessages()
	{
		if (!this.autoShowMessagesEnabled)
		{
			return;
		}
		if (!this.campaignsAndResourcesInitialized || this.campaigns == null || this.campaigns.Count == 0)
		{
			return;
		}
		SwrveBaseMessage swrveBaseMessage = null;
		for (int i = 0; i < this.campaigns.Count; i++)
		{
			if (this.campaigns[i].IsA<SwrveConversationCampaign>())
			{
				SwrveConversationCampaign swrveConversationCampaign = (SwrveConversationCampaign)this.campaigns[i];
				if (swrveConversationCampaign.CanTrigger("Swrve.Messages.showAtSessionStart", null, null))
				{
					if (swrveConversationCampaign.AreAssetsReady())
					{
						this.Container.StartCoroutine(this.LaunchConversation(swrveConversationCampaign.Conversation));
						swrveBaseMessage = swrveConversationCampaign.Conversation;
						break;
					}
					if (this.qaUser != null)
					{
						int id = swrveConversationCampaign.Id;
						this.qaUser.campaignMessages[id] = swrveConversationCampaign.Conversation;
						this.qaUser.campaignReasons[id] = "Campaign " + id + " was selected to autoshow, but assets aren't fully downloaded";
					}
				}
			}
		}
		if (swrveBaseMessage == null)
		{
			for (int j = 0; j < this.campaigns.Count; j++)
			{
				if (this.campaigns[j].IsA<SwrveMessagesCampaign>())
				{
					SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)this.campaigns[j];
					if (swrveMessagesCampaign.CanTrigger("Swrve.Messages.showAtSessionStart", null, null))
					{
						if (this.TriggeredMessageListener != null)
						{
							SwrveMessage messageForEvent = this.GetMessageForEvent("Swrve.Messages.showAtSessionStart", null);
							if (messageForEvent != null)
							{
								this.autoShowMessagesEnabled = false;
								this.TriggeredMessageListener.OnMessageTriggered(messageForEvent);
								swrveBaseMessage = messageForEvent;
							}
						}
						else if (this.currentMessage == null)
						{
							SwrveMessage messageForEvent2 = this.GetMessageForEvent("Swrve.Messages.showAtSessionStart", null);
							if (messageForEvent2 != null)
							{
								this.autoShowMessagesEnabled = false;
								this.Container.StartCoroutine(this.LaunchMessage(messageForEvent2, this.GlobalInstallButtonListener, this.GlobalCustomButtonListener, this.GlobalMessageListener));
								swrveBaseMessage = messageForEvent2;
							}
						}
						break;
					}
				}
			}
		}
		if (this.qaUser != null)
		{
			this.qaUser.Trigger("Swrve.Messages.showAtSessionStart", swrveBaseMessage);
		}
	}

	[DebuggerHidden]
	private IEnumerator LaunchMessage(SwrveMessage message, ISwrveInstallButtonListener installButtonListener, ISwrveCustomButtonListener customButtonListener, ISwrveMessageListener messageListener)
	{
		if (message != null)
		{
			SwrveOrientation deviceOrientation = this.GetDeviceOrientation();
			SwrveMessageFormat format = message.GetFormat(deviceOrientation);
			if (format != null)
			{
				this.currentMessage = format;
				CoroutineReference<bool> coroutineReference = new CoroutineReference<bool>(false);
				yield return this.StartTask("PreloadFormatAssets", this.PreloadFormatAssets(format, coroutineReference));
				if (coroutineReference.Value())
				{
					this.ShowMessageFormat(format, installButtonListener, customButtonListener, messageListener);
				}
				else
				{
					SwrveLog.LogError("Could not preload all the assets for message " + message.Id);
					this.currentMessage = null;
				}
			}
			else
			{
				SwrveLog.LogError("Could not get a format for the current orientation: " + deviceOrientation.ToString());
			}
		}
		yield break;
	}

	private bool isValidMessageCenter(SwrveBaseCampaign campaign, SwrveOrientation orientation)
	{
		return campaign.MessageCenter && campaign.Status != SwrveCampaignState.Status.Deleted && campaign.IsActive(this.qaUser) && campaign.SupportsOrientation(orientation) && campaign.AreAssetsReady();
	}

	[DebuggerHidden]
	private IEnumerator LaunchConversation(SwrveConversation conversation)
	{
		if (conversation != null)
		{
			yield return null;
			this.ShowConversation(conversation.Conversation);
			this.ConversationWasShownToUser(conversation);
		}
		yield break;
	}

	public void ConversationWasShownToUser(SwrveConversation conversation)
	{
		this.SetMessageMinDelayThrottle();
		if (conversation.Campaign != null)
		{
			conversation.Campaign.WasShownToUser();
			this.SaveCampaignData(conversation.Campaign);
		}
	}

	private void NoMessagesWereShown(string eventName, string reason)
	{
		SwrveLog.Log("Not showing message for " + eventName + ": " + reason);
		if (this.qaUser != null)
		{
			this.qaUser.TriggerFailure(eventName, reason);
		}
	}

	[DebuggerHidden]
	private IEnumerator PreloadFormatAssets(SwrveMessageFormat format, CoroutineReference<bool> wereAllLoaded)
	{
		SwrveLog.Log("Preloading format");
		bool val = true;
		for (int i = 0; i < format.Images.Count; i++)
		{
			SwrveImage swrveImage = format.Images[i];
			if (swrveImage.Texture == null && !string.IsNullOrEmpty(swrveImage.File))
			{
				SwrveLog.Log("Preloading image file " + swrveImage.File);
				CoroutineReference<Texture2D> coroutineReference = new CoroutineReference<Texture2D>();
				yield return this.StartTask("LoadAsset", this.LoadAsset(swrveImage.File, coroutineReference));
				if (coroutineReference.Value() != null)
				{
					swrveImage.Texture = coroutineReference.Value();
				}
				else
				{
					val = false;
				}
			}
		}
		for (int j = 0; j < format.Buttons.Count; j++)
		{
			SwrveButton swrveButton = format.Buttons[j];
			if (swrveButton.Texture == null && !string.IsNullOrEmpty(swrveButton.Image))
			{
				SwrveLog.Log("Preloading button image " + swrveButton.Image);
				CoroutineReference<Texture2D> coroutineReference2 = new CoroutineReference<Texture2D>();
				yield return this.StartTask("LoadAsset", this.LoadAsset(swrveButton.Image, coroutineReference2));
				if (coroutineReference2.Value() != null)
				{
					swrveButton.Texture = coroutineReference2.Value();
				}
				else
				{
					val = false;
				}
			}
		}
		wereAllLoaded.Value(val);
		this.TaskFinished("PreloadFormatAssets");
		yield break;
	}

	private bool HasShowTooManyMessagesAlready()
	{
		return this.messagesLeftToShow <= 0L;
	}

	private bool IsTooSoonToShowMessageAfterLaunch(DateTime now)
	{
		return now < this.showMessagesAfterLaunch;
	}

	private bool IsTooSoonToShowMessageAfterDelay(DateTime now)
	{
		return now < this.showMessagesAfterDelay;
	}

	private SwrveMessageFormat ShowMessageFormat(SwrveMessageFormat format, ISwrveInstallButtonListener installButtonListener, ISwrveCustomButtonListener customButtonListener, ISwrveMessageListener messageListener)
	{
		this.currentMessage = format;
		format.MessageListener = messageListener;
		format.CustomButtonListener = customButtonListener;
		format.InstallButtonListener = installButtonListener;
		this.currentDisplayingMessage = this.currentMessage;
		this.currentOrientation = this.GetDeviceOrientation();
		SwrveMessageRenderer.InitMessage(this.currentDisplayingMessage, this.currentOrientation);
		if (messageListener != null)
		{
			messageListener.OnShow(format);
		}
		this.MessageWasShownToUser(this.currentDisplayingMessage);
		return format;
	}

	private string GetTemporaryPathFileName(string fileName)
	{
		return Path.Combine(this.swrveTemporaryPath, fileName);
	}

	[DebuggerHidden]
	private IEnumerator LoadAsset(string fileName, CoroutineReference<Texture2D> texture)
	{
		string temporaryPathFileName = this.GetTemporaryPathFileName(fileName);
		WWW wWW = new WWW("file://" + temporaryPathFileName);
		yield return wWW;
		if (wWW != null && wWW.error == null)
		{
			Texture2D texture2 = wWW.texture;
			texture.Value(texture2);
		}
		else
		{
			SwrveLog.LogError("Could not load asset with WWW " + temporaryPathFileName + ": " + wWW.error);
			if (CrossPlatformFile.Exists(temporaryPathFileName))
			{
				byte[] data = CrossPlatformFile.ReadAllBytes(temporaryPathFileName);
				Texture2D texture2D = new Texture2D(4, 4);
				if (texture2D.LoadImage(data))
				{
					texture.Value(texture2D);
				}
				else
				{
					SwrveLog.LogWarning("Could not load asset from I/O" + temporaryPathFileName);
				}
			}
			else
			{
				SwrveLog.LogError("The file " + temporaryPathFileName + " does not exist.");
			}
		}
		this.TaskFinished("LoadAsset");
		yield break;
	}

	protected virtual void ProcessCampaigns(Dictionary<string, object> root)
	{
		List<SwrveBaseCampaign> list = new List<SwrveBaseCampaign>();
		HashSet<SwrveAssetsQueueItem> hashSet = new HashSet<SwrveAssetsQueueItem>();
		try
		{
			if (root != null && root.ContainsKey("version"))
			{
				int @int = MiniJsonHelper.GetInt(root, "version");
				if (@int == SwrveSDK.CampaignResponseVersion)
				{
					this.UpdateCdnPaths(root);
					Dictionary<string, object> dictionary = (Dictionary<string, object>)root["game_data"];
					Dictionary<string, object>.Enumerator enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<string, object> current = enumerator.Current;
						string key = current.Key;
						if (this.appStoreLinks.ContainsKey(key))
						{
							this.appStoreLinks.Remove(key);
						}
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary[key];
						if (dictionary2 != null && dictionary2.ContainsKey("app_store_url"))
						{
							object obj = dictionary2["app_store_url"];
							if (obj != null && obj is string)
							{
								this.appStoreLinks.Add(key, (string)obj);
							}
						}
					}
					Dictionary<string, object> dictionary3 = (Dictionary<string, object>)root["rules"];
					int num = (!dictionary3.ContainsKey("delay_first_message")) ? 150 : MiniJsonHelper.GetInt(dictionary3, "delay_first_message");
					long num2 = (!dictionary3.ContainsKey("max_messages_per_session")) ? 99999L : MiniJsonHelper.GetLong(dictionary3, "max_messages_per_session");
					int num3 = (!dictionary3.ContainsKey("min_delay_between_messages")) ? 55 : MiniJsonHelper.GetInt(dictionary3, "min_delay_between_messages");
					DateTime now = SwrveHelper.GetNow();
					this.minDelayBetweenMessage = num3;
					this.messagesLeftToShow = num2;
					this.showMessagesAfterLaunch = this.initialisedTime + TimeSpan.FromSeconds((double)num);
					SwrveLog.Log(string.Concat(new object[]
					{
						"App rules OK: Delay Seconds: ",
						num,
						" Max shows: ",
						num2
					}));
					SwrveLog.Log("Time is " + now.ToString() + " show messages after " + this.showMessagesAfterLaunch.ToString());
					Dictionary<int, string> dictionary4 = null;
					bool flag = this.qaUser != null;
					if (root.ContainsKey("qa"))
					{
						Dictionary<string, object> dictionary5 = (Dictionary<string, object>)root["qa"];
						SwrveLog.Log("You are a QA user!");
						dictionary4 = new Dictionary<int, string>();
						this.qaUser = new SwrveQAUser(this, dictionary5);
						if (dictionary5.ContainsKey("campaigns"))
						{
							IList<object> list2 = (List<object>)dictionary5["campaigns"];
							for (int i = 0; i < list2.Count; i++)
							{
								Dictionary<string, object> dictionary6 = (Dictionary<string, object>)list2[i];
								int int2 = MiniJsonHelper.GetInt(dictionary6, "id");
								string text = (string)dictionary6["reason"];
								SwrveLog.Log(string.Concat(new object[]
								{
									"Campaign ",
									int2,
									" not downloaded because: ",
									text
								}));
								dictionary4.Add(int2, text);
							}
						}
					}
					else
					{
						this.qaUser = null;
					}
					IList<object> list3 = (List<object>)root["campaigns"];
					int j = 0;
					int count = list3.Count;
					while (j < count)
					{
						Dictionary<string, object> campaignData = (Dictionary<string, object>)list3[j];
						SwrveBaseCampaign swrveBaseCampaign = SwrveBaseCampaign.LoadFromJSON(this.SwrveAssetsManager, campaignData, this.initialisedTime, this.qaUser, this.config.DefaultBackgroundColor);
						if (swrveBaseCampaign != null)
						{
							if (swrveBaseCampaign.GetType() == typeof(SwrveConversationCampaign))
							{
								SwrveConversationCampaign swrveConversationCampaign = (SwrveConversationCampaign)swrveBaseCampaign;
								hashSet.UnionWith(swrveConversationCampaign.Conversation.ConversationAssets);
							}
							else if (swrveBaseCampaign.GetType() == typeof(SwrveMessagesCampaign))
							{
								SwrveMessagesCampaign swrveMessagesCampaign = (SwrveMessagesCampaign)swrveBaseCampaign;
								hashSet.UnionWith(swrveMessagesCampaign.GetImageAssets());
							}
							if (this.campaignSettings != null && (flag || this.qaUser == null || !this.qaUser.ResetDevice))
							{
								SwrveCampaignState swrveCampaignState = null;
								this.campaignsState.TryGetValue(swrveBaseCampaign.Id, out swrveCampaignState);
								if (swrveCampaignState != null)
								{
									swrveBaseCampaign.State = swrveCampaignState;
								}
								else
								{
									swrveBaseCampaign.State = new SwrveCampaignState(swrveBaseCampaign.Id, this.campaignSettings);
								}
							}
							this.campaignsState[swrveBaseCampaign.Id] = swrveBaseCampaign.State;
							list.Add(swrveBaseCampaign);
							if (this.qaUser != null)
							{
								dictionary4.Add(swrveBaseCampaign.Id, null);
							}
						}
						j++;
					}
					if (this.qaUser != null)
					{
						this.qaUser.TalkSession(dictionary4);
					}
				}
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogError("Could not process campaigns: " + ex.ToString());
		}
		this.StartTask("SwrveAssetsManager.DownloadAssets", this.SwrveAssetsManager.DownloadAssets(hashSet, new Action(this.AutoShowMessages)));
		this.campaigns = new List<SwrveBaseCampaign>(list);
	}

	private void UpdateCdnPaths(Dictionary<string, object> root)
	{
		if (root.ContainsKey("cdn_root"))
		{
			string text = (string)root["cdn_root"];
			this.SwrveAssetsManager.CdnImages = text;
			SwrveLog.Log("CDN URL " + text);
		}
		else if (root.ContainsKey("cdn_paths"))
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)root["cdn_paths"];
			string text2 = (string)dictionary["message_images"];
			string text3 = (string)dictionary["message_fonts"];
			this.SwrveAssetsManager.CdnImages = text2;
			this.SwrveAssetsManager.CdnFonts = text3;
			SwrveLog.Log("CDN URL images:" + text2 + " fonts:" + text3);
		}
	}

	private void LoadResourcesAndCampaigns()
	{
		if (!this.IsAlive())
		{
			return;
		}
		try
		{
			if (!this.campaignsConnecting)
			{
				if (!this.config.AutoDownloadCampaignsAndResources)
				{
					if (this.campaignsAndResourcesLastRefreshed != 0L)
					{
						long sessionTime = this.GetSessionTime();
						if (sessionTime < this.campaignsAndResourcesLastRefreshed)
						{
							SwrveLog.Log("Request to retrieve campaign and user resource data was rate-limited.");
							return;
						}
					}
					this.campaignsAndResourcesLastRefreshed = this.GetSessionTime() + (long)(this.campaignsAndResourcesFlushFrequency * 1000f);
				}
				this.campaignsConnecting = true;
				float num = (Screen.dpi != 0f) ? Screen.dpi : 160f;
				string deviceModel = this.GetDeviceModel();
				string operatingSystem = SystemInfo.operatingSystem;
				StringBuilder stringBuilder = new StringBuilder(this.resourcesAndCampaignsUrl).AppendFormat("?user={0}&api_key={1}&app_version={2}&joined={3}", new object[]
				{
					this.escapedUserId,
					this.ApiKey,
					WWW.EscapeURL(this.GetAppVersion()),
					this.installTimeEpoch
				});
				if (this.config.TalkEnabled)
				{
					stringBuilder.AppendFormat("&version={0}&orientation={1}&language={2}&app_store={3}&device_width={4}&device_height={5}&device_dpi={6}&os_version={7}&device_name={8}", new object[]
					{
						SwrveSDK.CampaignEndpointVersion,
						this.config.Orientation.ToString().ToLower(),
						this.Language,
						this.config.AppStore,
						this.deviceWidth,
						this.deviceHeight,
						num,
						WWW.EscapeURL(operatingSystem),
						WWW.EscapeURL(deviceModel)
					});
				}
				if (this.config.ConversationsEnabled)
				{
					stringBuilder.AppendFormat("&conversation_version={0}", this.conversationVersion);
				}
				if (this.config.LocationEnabled)
				{
					stringBuilder.AppendFormat("&location_version={0}", this.locationSegmentVersion);
				}
				if (!string.IsNullOrEmpty(this.lastETag))
				{
					stringBuilder.AppendFormat("&etag={0}", this.lastETag);
				}
				this.StartTask("GetCampaignsAndResources_Coroutine", this.GetCampaignsAndResources_Coroutine(stringBuilder.ToString()));
			}
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while trying to get user resources and campaign data: " + arg);
		}
	}

	private string GetDeviceModel()
	{
		string text = SystemInfo.deviceModel;
		if (string.IsNullOrEmpty(text))
		{
			text = "ModelUnknown";
		}
		return text;
	}

	[DebuggerHidden]
	private IEnumerator GetCampaignsAndResources_Coroutine(string getRequest)
	{
		SwrveLog.Log("Campaigns and resources request: " + getRequest);
		yield return this.Container.StartCoroutine(this.restClient.Get(getRequest, delegate(RESTResponse response)
		{
			if (response.Error == WwwDeducedError.NoError)
			{
				string text = null;
				if (response.Headers != null)
				{
					response.Headers.TryGetValue("ETAG", out text);
					if (!string.IsNullOrEmpty(text))
					{
						this.$this.lastETag = text;
						this.$this.storage.Save("cmpg_etag", text, this.$this.userId);
					}
				}
				if (!string.IsNullOrEmpty(response.Body))
				{
					Dictionary<string, object> dictionary = (Dictionary<string, object>)Json.Deserialize(response.Body);
					if (dictionary != null)
					{
						if (dictionary.ContainsKey("flush_frequency"))
						{
							string @string = MiniJsonHelper.GetString(dictionary, "flush_frequency");
							if (!string.IsNullOrEmpty(@string) && float.TryParse(@string, out this.$this.campaignsAndResourcesFlushFrequency))
							{
								this.$this.campaignsAndResourcesFlushFrequency /= 1000f;
								this.$this.storage.Save("swrve_cr_flush_frequency", @string, this.$this.userId);
							}
						}
						if (dictionary.ContainsKey("flush_refresh_delay"))
						{
							string string2 = MiniJsonHelper.GetString(dictionary, "flush_refresh_delay");
							if (!string.IsNullOrEmpty(string2) && float.TryParse(string2, out this.$this.campaignsAndResourcesFlushRefreshDelay))
							{
								this.$this.campaignsAndResourcesFlushRefreshDelay /= 1000f;
								this.$this.storage.Save("swrve_cr_flush_delay", string2, this.$this.userId);
							}
						}
						if (dictionary.ContainsKey("user_resources"))
						{
							IList<object> obj = (IList<object>)dictionary["user_resources"];
							string data = Json.Serialize(obj);
							this.$this.storage.SaveSecure("srcngt2", data, this.$this.userId);
							this.$this.userResources = this.$this.ProcessUserResources(obj);
							this.$this.userResourcesRaw = data;
							if (this.$this.campaignsAndResourcesInitialized)
							{
								this.$this.NotifyUpdateUserResources();
							}
						}
						if (this.$this.config.TalkEnabled && dictionary.ContainsKey("campaigns"))
						{
							Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["campaigns"];
							string cacheContent = Json.Serialize(dictionary2);
							this.$this.SaveCampaignsCache(cacheContent);
							this.$this.AutoShowMessages();
							this.$this.ProcessCampaigns(dictionary2);
							StringBuilder stringBuilder = new StringBuilder();
							int i = 0;
							int count = this.$this.campaigns.Count;
							while (i < count)
							{
								SwrveBaseCampaign swrveBaseCampaign = this.$this.campaigns[i];
								if (i != 0)
								{
									stringBuilder.Append(',');
								}
								stringBuilder.Append(swrveBaseCampaign.Id);
								i++;
							}
							Dictionary<string, string> dictionary3 = new Dictionary<string, string>();
							dictionary3.Add("ids", stringBuilder.ToString());
							dictionary3.Add("count", (this.$this.campaigns != null) ? this.$this.campaigns.Count.ToString() : "0");
							this.$this.NamedEventInternal("Swrve.Messages.campaigns_downloaded", dictionary3, false);
						}
						if (this.$this.config.LocationEnabled && dictionary.ContainsKey("location_campaigns"))
						{
							Dictionary<string, object> obj2 = (Dictionary<string, object>)dictionary["location_campaigns"];
							string cacheContent2 = Json.Serialize(obj2);
							this.$this.SaveLocationCache(cacheContent2);
						}
					}
				}
			}
			else
			{
				SwrveLog.LogError("Resources and campaigns request error: " + response.Error.ToString() + ":" + response.Body);
			}
			if (!this.$this.campaignsAndResourcesInitialized)
			{
				this.$this.campaignsAndResourcesInitialized = true;
				this.$this.AutoShowMessages();
				this.$this.NotifyUpdateUserResources();
			}
			this.$this.campaignsConnecting = false;
			this.$this.TaskFinished("GetCampaignsAndResources_Coroutine");
		}));
		yield break;
	}

	private void SaveCampaignsCache(string cacheContent)
	{
		try
		{
			if (cacheContent == null)
			{
				cacheContent = string.Empty;
			}
			this.storage.SaveSecure(SwrveSDK.CampaignsSave, cacheContent, this.userId);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while saving campaigns to the cache " + arg);
		}
	}

	private void SaveLocationCache(string cacheContent)
	{
		try
		{
			if (cacheContent == null)
			{
				cacheContent = string.Empty;
			}
			this.storage.SaveSecure(SwrveSDK.LocationSave, cacheContent, this.userId);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while saving campaigns to the cache " + arg);
		}
	}

	private void SaveCampaignData(SwrveBaseCampaign campaign)
	{
		try
		{
			this.campaignSettings["Next" + campaign.Id] = campaign.Next;
			this.campaignSettings["Impressions" + campaign.Id] = campaign.Impressions;
			this.campaignSettings["Status" + campaign.Id] = campaign.Status.ToString();
			string data = Json.Serialize(this.campaignSettings);
			this.storage.Save(SwrveSDK.CampaignsSettingsSave, data, this.userId);
		}
		catch (Exception arg)
		{
			SwrveLog.LogError("Error while trying to save campaign settings " + arg);
		}
	}

	private void LoadTalkData()
	{
		try
		{
			string text = this.storage.Load(SwrveSDK.CampaignsSettingsSave, this.userId);
			string json;
			if (text != null && text.Length != 0 && ResponseBodyTester.TestUTF8(text, out json))
			{
				this.campaignSettings = (Dictionary<string, object>)Json.Deserialize(json);
			}
		}
		catch (Exception)
		{
		}
		try
		{
			string text2 = this.storage.LoadSecure(SwrveSDK.CampaignsSave, this.userId);
			if (!string.IsNullOrEmpty(text2))
			{
				string json2 = null;
				if (ResponseBodyTester.TestUTF8(text2, out json2))
				{
					Dictionary<string, object> root = (Dictionary<string, object>)Json.Deserialize(json2);
					this.ProcessCampaigns(root);
				}
				else
				{
					SwrveLog.Log("Failed to parse campaigns cache");
					this.InvalidateETag();
				}
			}
			else
			{
				this.InvalidateETag();
			}
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Could not read campaigns from cache, using default (" + ex.ToString() + ")");
			this.InvalidateETag();
		}
	}

	public void SendPushEngagedEvent(string pushId)
	{
		if ("0" == pushId || pushId != this.lastPushEngagedId)
		{
			this.lastPushEngagedId = pushId;
			string name = "Swrve.Messages.Push-" + pushId + ".engaged";
			this.NamedEventInternal(name, null, true);
			SwrveLog.Log("Got Swrve notification with ID " + pushId);
		}
	}

	protected int ConvertInt64ToInt32Hack(long val)
	{
		return (int)(val & (long)((ulong)-1));
	}

	protected virtual ICarrierInfo GetCarrierInfoProvider()
	{
		return this.deviceCarrierInfo;
	}

	public string GetAppVersion()
	{
		if (string.IsNullOrEmpty(this.config.AppVersion))
		{
			this.setNativeAppVersion();
		}
		return this.config.AppVersion;
	}

	private void ShowConversation(string conversation)
	{
		this.showNativeConversation(conversation);
	}

	private void SetInputManager(IInputManager inputManager)
	{
		this.inputManager = inputManager;
	}

	protected void StartCampaignsAndResourcesTimer()
	{
		if (!this.config.AutoDownloadCampaignsAndResources)
		{
			return;
		}
		this.RefreshUserResourcesAndCampaigns();
		this.StartCheckForCampaignsAndResources();
		this.Container.StartCoroutine(this.WaitAndRefreshResourcesAndCampaigns_Coroutine(this.campaignsAndResourcesFlushRefreshDelay));
	}

	protected void DisableAutoShowAfterDelay()
	{
		this.Container.StartCoroutine(this.DisableAutoShowAfterDelay_Coroutine());
	}

	[DebuggerHidden]
	private IEnumerator DisableAutoShowAfterDelay_Coroutine()
	{
		yield return new WaitForSeconds(this.config.AutoShowMessagesMaxDelay);
		this.autoShowMessagesEnabled = false;
		yield break;
	}

	private string GetNativeDetails()
	{
		Dictionary<string, object> obj = new Dictionary<string, object>
		{
			{
				"sdkVersion",
				"4.10.1"
			},
			{
				"apiKey",
				this.apiKey
			},
			{
				"appId",
				this.appId
			},
			{
				"userId",
				this.userId
			},
			{
				"deviceId",
				this.GetDeviceId()
			},
			{
				"appVersion",
				this.GetAppVersion()
			},
			{
				"uniqueKey",
				this.GetUniqueKey()
			},
			{
				"deviceInfo",
				this.GetDeviceInfo()
			},
			{
				"batchUrl",
				"/1/batch"
			},
			{
				"eventsServer",
				this.config.EventsServer
			},
			{
				"contentServer",
				this.config.ContentServer
			},
			{
				"locationCampaignCategory",
				"LocationCampaign"
			},
			{
				"httpTimeout",
				60000
			},
			{
				"maxEventsPerFlush",
				50
			},
			{
				"locTag",
				SwrveSDK.LocationSave
			},
			{
				"swrvePath",
				this.swrvePath
			},
			{
				"prefabName",
				this.prefabName
			},
			{
				"swrveTemporaryPath",
				this.swrveTemporaryPath
			},
			{
				"sigSuffix",
				"_SGT"
			}
		};
		return Json.Serialize(obj);
	}

	private void InitNative()
	{
		this.initNative();
		this.setNativeConversationVersion();
		if (this.config.LocationAutostart)
		{
			this.startLocation();
		}
	}

	protected void startLocation()
	{
		if (this.config.LocationEnabled)
		{
			this.startNativeLocation();
		}
	}

	private void ProcessInfluenceData()
	{
		string influencedDataJsonPerPlatform = this.GetInfluencedDataJsonPerPlatform();
		if (influencedDataJsonPerPlatform != null)
		{
			List<object> list = (List<object>)Json.Deserialize(influencedDataJsonPerPlatform);
			if (list != null)
			{
				for (int i = 0; i < list.Count; i++)
				{
					this.CheckInfluenceData((Dictionary<string, object>)list[i]);
				}
			}
			else
			{
				SwrveLog.LogError("Could not parse influence data");
			}
		}
	}

	protected virtual string GetInfluencedDataJsonPerPlatform()
	{
		string result = null;
		try
		{
			result = this.GetInfluencedDataJson();
		}
		catch (Exception ex)
		{
			SwrveLog.LogWarning("Couldn't get influence data from the native side correctly, make sure you have the Android plugin inside your project and you are running on an Android device: " + ex.ToString());
		}
		return result;
	}

	public void CheckInfluenceData(Dictionary<string, object> influenceData)
	{
		if (influenceData != null)
		{
			object obj = influenceData["trackingId"];
			object obj2 = influenceData["maxInfluencedMillis"];
			long num = 0L;
			if (obj2 != null && (obj2 is long || obj2 is int || obj2 is long))
			{
				num = (long)obj2;
			}
			if (obj != null && obj is string && num > 0L)
			{
				string text = (string)obj;
				long milliseconds = SwrveHelper.GetMilliseconds();
				if (milliseconds <= num)
				{
					this.AppendEventToBuffer("generic_campaign_event", new Dictionary<string, object>
					{
						{
							"id",
							text
						},
						{
							"campaignType",
							"push"
						},
						{
							"actionType",
							"influenced"
						},
						{
							"payload",
							new Dictionary<string, string>
							{
								{
									"delta",
									((num - milliseconds) / 6000L).ToString()
								}
							}
						}
					}, false);
					SwrveLog.Log("User was influenced by push " + text);
				}
			}
		}
	}
}
