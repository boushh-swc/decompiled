using JsonFx.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

[ExecuteInEditMode]
public class Kochava : MonoBehaviour
{
	public enum KochSessionTracking
	{
		full = 0,
		basic = 1,
		minimal = 2,
		none = 3
	}

	public enum KochLogLevel
	{
		error = 0,
		warning = 1,
		debug = 2
	}

	public class LogEvent
	{
		public string text;

		public float time;

		public Kochava.KochLogLevel level;

		public LogEvent(string text, Kochava.KochLogLevel level)
		{
			this.text = text;
			this.time = Time.time;
			this.level = level;
		}
	}

	public class QueuedEvent
	{
		public float eventTime;

		public Dictionary<string, object> eventData;
	}

	public delegate void AttributionCallback(string callbackString);

	private const string CURRENCY_DEFAULT = "USD";

	public const string KOCHAVA_VERSION = "20160914";

	public const string KOCHAVA_PROTOCOL_VERSION = "4";

	private const int MAX_LOG_SIZE = 50;

	private const int MAX_QUEUE_SIZE = 75;

	private const int MAX_POST_TIME = 15;

	private const int POST_FAIL_RETRY_DELAY = 30;

	private const int QUEUE_KVINIT_WAIT_DELAY = 15;

	private const string API_URL = "https://control.kochava.com";

	private const string TRACKING_URL = "https://control.kochava.com/track/kvTracker?v4";

	private const string INIT_URL = "https://control.kochava.com/track/kvinit";

	private const string QUERY_URL = "https://control.kochava.com/track/kvquery";

	private const string KOCHAVA_QUEUE_STORAGE_KEY = "kochava_queue_storage";

	private const int KOCHAVA_ATTRIBUTION_INITIAL_TIMER = 7;

	private const int KOCHAVA_ATTRIBUTION_DEFAULT_TIMER = 60;

	public string kochavaAppId = string.Empty;

	public string kochavaAppIdIOS = string.Empty;

	public string kochavaAppIdAndroid = string.Empty;

	public string kochavaAppIdKindle = string.Empty;

	public string kochavaAppIdBlackberry = string.Empty;

	public string kochavaAppIdWindowsPhone = string.Empty;

	public bool debugMode;

	public bool incognitoMode;

	public bool requestAttribution;

	private bool retrieveAttribution;

	private bool debugServer;

	[HideInInspector]
	public string appVersion = string.Empty;

	[HideInInspector]
	public string partnerName = string.Empty;

	public bool appLimitAdTracking;

	[HideInInspector]
	public string userAgent = string.Empty;

	public bool adidSupressed;

	private static int device_id_delay = 60;

	private string whitelist;

	private static bool adidBlacklisted = false;

	private static Kochava.AttributionCallback attributionCallback;

	private string appIdentifier = string.Empty;

	private string appPlatform = "desktop";

	private string kochavaDeviceId = string.Empty;

	private string attributionDataStr = string.Empty;

	private List<string> devIdBlacklist = new List<string>();

	private List<string> eventNameBlacklist = new List<string>();

	public string appCurrency = "USD";

	public Kochava.KochSessionTracking sessionTracking;

	private int KVTRACKER_WAIT = 60;

	private List<Kochava.LogEvent> _EventLog = new List<Kochava.LogEvent>();

	private Dictionary<string, object> hardwareIdentifierData = new Dictionary<string, object>();

	private Dictionary<string, object> hardwareIntegrationData = new Dictionary<string, object>();

	private Dictionary<string, object> appData;

	private Queue<Kochava.QueuedEvent> eventQueue = new Queue<Kochava.QueuedEvent>();

	private float processQueueKickstartTime;

	private bool queueIsProcessing;

	private float _eventPostingTime;

	private bool doReportLocation;

	private int locationAccuracy = 50;

	private int locationTimeout = 15;

	private int locationStaleness = 15;

	private int iAdAttributionAttempts = 3;

	private int iAdAttributionWait = 20;

	private int iAdRetryWait = 10;

	private bool send_id_updates;

	private static Kochava _S;

	private static readonly DateTime Jan1st1970 = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

	private static float uptimeDelta;

	private static float uptimeDeltaUpdate;

	public static bool DebugMode
	{
		get
		{
			return Kochava._S.debugMode;
		}
		set
		{
			Kochava._S.debugMode = value;
		}
	}

	public static bool IncognitoMode
	{
		get
		{
			return Kochava._S.incognitoMode;
		}
		set
		{
			Kochava._S.incognitoMode = value;
		}
	}

	public static bool RequestAttribution
	{
		get
		{
			return Kochava._S.requestAttribution;
		}
		set
		{
			Kochava._S.requestAttribution = value;
		}
	}

	public static bool AppLimitAdTracking
	{
		get
		{
			return Kochava._S.appLimitAdTracking;
		}
		set
		{
			Kochava._S.appLimitAdTracking = value;
		}
	}

	public static bool AdidSupressed
	{
		get
		{
			return Kochava._S.adidSupressed;
		}
		set
		{
			Kochava._S.adidSupressed = value;
		}
	}

	public static string AttributionDataStr
	{
		get
		{
			return Kochava._S.attributionDataStr;
		}
		set
		{
			Kochava._S.attributionDataStr = value;
		}
	}

	public static List<string> DevIdBlacklist
	{
		get
		{
			return Kochava._S.devIdBlacklist;
		}
		set
		{
			Kochava._S.devIdBlacklist = value;
		}
	}

	public static List<string> EventNameBlacklist
	{
		get
		{
			return Kochava._S.eventNameBlacklist;
		}
		set
		{
			Kochava._S.eventNameBlacklist = value;
		}
	}

	public static Kochava.KochSessionTracking SessionTracking
	{
		get
		{
			return Kochava._S.sessionTracking;
		}
		set
		{
			Kochava._S.sessionTracking = value;
		}
	}

	public static List<Kochava.LogEvent> EventLog
	{
		get
		{
			return Kochava._S._EventLog;
		}
	}

	public static int eventQueueLength
	{
		get
		{
			return Kochava._S.eventQueue.Count;
		}
	}

	public static float eventPostingTime
	{
		get
		{
			return Kochava._S._eventPostingTime;
		}
	}

	public static void SetAttributionCallback(Kochava.AttributionCallback callback)
	{
		Kochava.attributionCallback = callback;
	}

	public void Awake()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (Kochava._S)
		{
			this.Log("detected two concurrent integration objects - please place your integration object in a scene which will not be reloaded.");
			UnityEngine.Object.Destroy(base.gameObject);
			return;
		}
		UnityEngine.Object.DontDestroyOnLoad(base.gameObject);
		if (Kochava._S == null)
		{
			Kochava._S = this;
		}
		base.gameObject.name = "_Kochava Analytics";
		this.Log("Kochava SDK Initialized.\nVersion: 20160914\nProtocol Version: 4", Kochava.KochLogLevel.debug);
		if (this.kochavaAppId.Length == 0 && this.kochavaAppIdIOS.Length == 0 && this.kochavaAppIdAndroid.Length == 0 && this.kochavaAppIdKindle.Length == 0 && this.kochavaAppIdBlackberry.Length == 0 && this.kochavaAppIdWindowsPhone.Length == 0 && this.partnerName.Length == 0)
		{
			this.Log("No Kochava App Id or Partner Name - SDK will terminate");
			UnityEngine.Object.Destroy(base.gameObject);
		}
		this.loadQueue();
	}

	public void Start()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (Kochava._S == null)
		{
			Kochava._S = this;
		}
		this.Init();
	}

	public void OnEnable()
	{
		if (!Application.isPlaying)
		{
			return;
		}
		if (Kochava._S == null)
		{
			Kochava._S = this;
		}
	}

	private void Init()
	{
		try
		{
			try
			{
				AndroidJNIHelper.debug = true;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
					AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
					string text = androidJavaClass2.CallStatic<string>("GetExternalKochavaDeviceIdentifiers_Android", new object[]
					{
						androidJavaObject,
						Kochava.AdidSupressed
					});
					this.Log("Hardware Integration Diagnostics: " + text);
					this.hardwareIdentifierData = JsonReader.Deserialize<Dictionary<string, object>>(text);
					this.Log(string.Concat(new object[]
					{
						"Received (",
						this.hardwareIdentifierData.Count,
						") parameters from Hardware Integration Library (identifiers): ",
						text
					}));
				}
			}
			catch (Exception arg)
			{
				this.Log("Failed GetExternalKochavaDeviceIdentifiers_Android: " + arg, Kochava.KochLogLevel.warning);
			}
			if (this.hardwareIdentifierData.ContainsKey("user_agent"))
			{
				this.userAgent = this.hardwareIdentifierData["user_agent"].ToString();
				this.Log("userAgent set to: " + this.userAgent, Kochava.KochLogLevel.debug);
			}
			if (this.userAgent.Contains("kindle") || this.userAgent.Contains("silk"))
			{
				this.appPlatform = "kindle";
				if (this.kochavaAppIdKindle != string.Empty)
				{
					this.kochavaAppId = this.kochavaAppIdKindle;
				}
			}
			else
			{
				this.appPlatform = "android";
				if (this.kochavaAppIdAndroid != string.Empty)
				{
					this.kochavaAppId = this.kochavaAppIdAndroid;
				}
			}
			if (this.hardwareIdentifierData.ContainsKey("package"))
			{
				this.appIdentifier = this.hardwareIdentifierData["package"].ToString();
				this.Log("appIdentifier set to: " + this.appIdentifier, Kochava.KochLogLevel.debug);
			}
			if (PlayerPrefs.HasKey("kochava_app_id"))
			{
				this.kochavaAppId = PlayerPrefs.GetString("kochava_app_id");
				this.Log("Loaded kochava_app_id from persistent storage: " + this.kochavaAppId, Kochava.KochLogLevel.debug);
			}
			if (PlayerPrefs.HasKey("kochava_device_id"))
			{
				this.kochavaDeviceId = PlayerPrefs.GetString("kochava_device_id");
				this.Log("Loaded kochava_device_id from persistent storage: " + this.kochavaDeviceId, Kochava.KochLogLevel.debug);
			}
			else if (this.incognitoMode)
			{
				this.kochavaDeviceId = "KA" + Guid.NewGuid().ToString().Replace("-", string.Empty);
				this.Log("Using autogenerated \"incognito\" kochava_device_id: " + this.kochavaDeviceId, Kochava.KochLogLevel.debug);
			}
			else
			{
				string a = string.Empty;
				if (PlayerPrefs.HasKey("data_orig_kochava_device_id"))
				{
					a = PlayerPrefs.GetString("data_orig_kochava_device_id");
				}
				if (a != string.Empty)
				{
					this.kochavaDeviceId = a;
					this.Log("Using \"orig\" kochava_device_id: " + this.kochavaDeviceId, Kochava.KochLogLevel.debug);
				}
				else
				{
					this.kochavaDeviceId = "KU" + Guid.NewGuid().ToString().Replace("-", string.Empty);
					this.Log("Using autogenerated kochava_device_id: " + this.kochavaDeviceId, Kochava.KochLogLevel.debug);
				}
			}
			if (!PlayerPrefs.HasKey("data_orig_kochava_app_id") && this.kochavaAppId != string.Empty)
			{
				PlayerPrefs.SetString("data_orig_kochava_app_id", this.kochavaAppId);
			}
			if (!PlayerPrefs.HasKey("data_orig_kochava_device_id") && this.kochavaDeviceId != string.Empty)
			{
				PlayerPrefs.SetString("data_orig_kochava_device_id", this.kochavaDeviceId);
			}
			if (!PlayerPrefs.HasKey("data_orig_session_tracking"))
			{
				PlayerPrefs.SetString("data_orig_session_tracking", this.sessionTracking.ToString());
			}
			if (!PlayerPrefs.HasKey("data_orig_currency") && this.appCurrency != string.Empty)
			{
				PlayerPrefs.SetString("data_orig_currency", this.appCurrency);
			}
			if (PlayerPrefs.HasKey("currency"))
			{
				this.appCurrency = PlayerPrefs.GetString("currency");
				this.Log("Loaded currency from persistent storage: " + this.appCurrency, Kochava.KochLogLevel.debug);
			}
			if (PlayerPrefs.HasKey("blacklist"))
			{
				try
				{
					string @string = PlayerPrefs.GetString("blacklist");
					this.devIdBlacklist = new List<string>();
					string[] array = JsonReader.Deserialize<string[]>(@string);
					for (int i = array.Length - 1; i >= 0; i--)
					{
						this.devIdBlacklist.Add(array[i]);
					}
					this.Log("Loaded device_id blacklist from persistent storage: " + @string, Kochava.KochLogLevel.debug);
				}
				catch (Exception arg2)
				{
					this.Log("Failed loading device_id blacklist from persistent storage: " + arg2, Kochava.KochLogLevel.warning);
				}
			}
			if (PlayerPrefs.HasKey("attribution"))
			{
				try
				{
					this.attributionDataStr = PlayerPrefs.GetString("attribution");
					this.Log("Loaded attribution data from persistent storage: " + this.attributionDataStr, Kochava.KochLogLevel.debug);
				}
				catch (Exception arg3)
				{
					this.Log("Failed loading attribution data from persistent storage: " + arg3, Kochava.KochLogLevel.warning);
				}
			}
			if (PlayerPrefs.HasKey("session_tracking"))
			{
				try
				{
					string string2 = PlayerPrefs.GetString("session_tracking");
					this.sessionTracking = (Kochava.KochSessionTracking)((int)Enum.Parse(typeof(Kochava.KochSessionTracking), string2, true));
					this.Log("Loaded session tracking mode from persistent storage: " + string2, Kochava.KochLogLevel.debug);
				}
				catch (Exception arg4)
				{
					this.Log("Failed loading session tracking mode from persistent storage: " + arg4, Kochava.KochLogLevel.warning);
				}
			}
			if (!PlayerPrefs.HasKey("kvinit_wait"))
			{
				PlayerPrefs.SetString("kvinit_wait", "60");
			}
			if (!PlayerPrefs.HasKey("kvinit_last_sent"))
			{
				PlayerPrefs.SetString("kvinit_last_sent", "0");
			}
			if (!PlayerPrefs.HasKey("kvtracker_wait"))
			{
				PlayerPrefs.SetString("kvtracker_wait", "60");
			}
			if (!PlayerPrefs.HasKey("last_location_time"))
			{
				PlayerPrefs.SetString("last_location_time", "0");
			}
			double num = double.Parse(PlayerPrefs.GetString("kvinit_last_sent"));
			double num2 = Kochava.CurrentTime();
			double num3 = double.Parse(PlayerPrefs.GetString("kvinit_wait"));
			if (num2 - num > num3)
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>
				{
					{
						"partner_name",
						this.partnerName
					},
					{
						"package",
						this.appIdentifier
					},
					{
						"platform",
						this.appPlatform
					},
					{
						"session_tracking",
						this.sessionTracking.ToString()
					},
					{
						"currency",
						(this.appCurrency != null && !(this.appCurrency == string.Empty)) ? this.appCurrency : "USD"
					},
					{
						"os_version",
						SystemInfo.operatingSystem
					}
				};
				if (this.requestAttribution && !PlayerPrefs.HasKey("attribution"))
				{
					this.retrieveAttribution = true;
				}
				this.Log("retrieve attrib: " + this.retrieveAttribution);
				if (this.hardwareIdentifierData.ContainsKey("IDFA"))
				{
					dictionary.Add("idfa", this.hardwareIdentifierData["IDFA"]);
				}
				if (this.hardwareIdentifierData.ContainsKey("IDFV"))
				{
					dictionary.Add("idfv", this.hardwareIdentifierData["IDFV"]);
				}
				Dictionary<string, object> value = new Dictionary<string, object>
				{
					{
						"kochava_app_id",
						PlayerPrefs.GetString("data_orig_kochava_app_id")
					},
					{
						"kochava_device_id",
						PlayerPrefs.GetString("data_orig_kochava_device_id")
					},
					{
						"session_tracking",
						PlayerPrefs.GetString("data_orig_session_tracking")
					},
					{
						"currency",
						PlayerPrefs.GetString("data_orig_currency")
					}
				};
				Dictionary<string, object> value2 = new Dictionary<string, object>
				{
					{
						"action",
						"init"
					},
					{
						"data",
						dictionary
					},
					{
						"data_orig",
						value
					},
					{
						"kochava_app_id",
						this.kochavaAppId
					},
					{
						"kochava_device_id",
						this.kochavaDeviceId
					},
					{
						"sdk_version",
						"Unity3D-20160914"
					},
					{
						"sdk_protocol",
						"4"
					}
				};
				base.StartCoroutine(this.Init_KV(JsonWriter.Serialize(value2)));
			}
			else
			{
				this.appData = new Dictionary<string, object>
				{
					{
						"kochava_app_id",
						this.kochavaAppId
					},
					{
						"kochava_device_id",
						this.kochavaDeviceId
					},
					{
						"sdk_version",
						"Unity3D-20160914"
					},
					{
						"sdk_protocol",
						"4"
					}
				};
				if (PlayerPrefs.HasKey("eventname_blacklist"))
				{
					string[] array2 = JsonReader.Deserialize<string[]>(PlayerPrefs.GetString("eventname_blacklist"));
					List<string> list = new List<string>();
					for (int j = 0; j < array2.Length; j++)
					{
						list.Add(array2[j]);
					}
					this.eventNameBlacklist = list;
				}
			}
		}
		catch (Exception arg5)
		{
			this.Log("Overall failure in init: " + arg5, Kochava.KochLogLevel.warning);
		}
	}

	[DebuggerHidden]
	private IEnumerator Init_KV(string postData)
	{
		if (Application.internetReachability == NetworkReachability.NotReachable)
		{
			this.Log("internet not reachable", Kochava.KochLogLevel.warning);
			yield return new WaitForSeconds(30f);
			base.StartCoroutine(this.Init_KV(postData));
		}
		else
		{
			this.Log("Initiating kvinit handshake...", Kochava.KochLogLevel.debug);
			Dictionary<string, string> headers = new Dictionary<string, string>
			{
				{
					"Content-Type",
					"application/xml"
				},
				{
					"User-Agent",
					this.userAgent
				}
			};
			this.Log(postData, Kochava.KochLogLevel.debug);
			float time = Time.time;
			WWW wWW = new WWW("https://control.kochava.com/track/kvinit", Encoding.UTF8.GetBytes(postData), headers);
			yield return wWW;
			if (!string.IsNullOrEmpty(wWW.error))
			{
				this.Log(string.Concat(new object[]
				{
					"Kvinit handshake failed: ",
					wWW.error,
					", seconds: ",
					Time.time - time,
					")"
				}), Kochava.KochLogLevel.warning);
				yield return new WaitForSeconds(30f);
				base.StartCoroutine(this.Init_KV(postData));
			}
			else
			{
				Dictionary<string, object> dictionary = new Dictionary<string, object>();
				if (wWW.text != string.Empty)
				{
					try
					{
						dictionary = JsonReader.Deserialize<Dictionary<string, object>>(wWW.text);
					}
					catch (Exception var_2_209)
					{
					}
				}
				this.Log(wWW.text, Kochava.KochLogLevel.debug);
				if (!dictionary.ContainsKey("success"))
				{
					this.Log("Kvinit handshake parsing failed: " + wWW.text, Kochava.KochLogLevel.warning);
					yield return new WaitForSeconds(30f);
					base.StartCoroutine(this.Init_KV(postData));
				}
				else
				{
					PlayerPrefs.SetString("kvinit_last_sent", Kochava.CurrentTime().ToString());
					this.Log("...kvinit handshake complete, processing response flags...", Kochava.KochLogLevel.debug);
					if (dictionary.ContainsKey("flags"))
					{
						Dictionary<string, object> dictionary2 = (Dictionary<string, object>)dictionary["flags"];
						if (dictionary2.ContainsKey("kochava_app_id"))
						{
							this.kochavaAppId = dictionary2["kochava_app_id"].ToString();
							PlayerPrefs.SetString("kochava_app_id", this.kochavaAppId);
							this.Log("Saved kochava_app_id to persistent storage: " + this.kochavaAppId, Kochava.KochLogLevel.debug);
						}
						if (dictionary2.ContainsKey("kochava_device_id"))
						{
							this.kochavaDeviceId = dictionary2["kochava_device_id"].ToString();
						}
						if (dictionary2.ContainsKey("resend_initial") && (bool)dictionary2["resend_initial"])
						{
							PlayerPrefs.DeleteKey("watchlistProperties");
							this.Log("Refiring initial event, as requested by kvinit response flag", Kochava.KochLogLevel.debug);
						}
						if (dictionary2.ContainsKey("session_tracking"))
						{
							try
							{
								this.sessionTracking = (Kochava.KochSessionTracking)((int)Enum.Parse(typeof(Kochava.KochSessionTracking), dictionary2["session_tracking"].ToString()));
								PlayerPrefs.SetString("session_tracking", this.sessionTracking.ToString());
								this.Log("Saved session_tracking mode to persistent storage: " + this.sessionTracking.ToString(), Kochava.KochLogLevel.debug);
							}
							catch (Exception var_4_497)
							{
							}
						}
						if (dictionary2.ContainsKey("currency"))
						{
							this.appCurrency = dictionary2["currency"].ToString();
							if (this.appCurrency.Equals(string.Empty))
							{
								this.appCurrency = "USD";
							}
							PlayerPrefs.SetString("currency", this.appCurrency);
							this.Log("Saved currency to persistent storage: " + this.appCurrency, Kochava.KochLogLevel.debug);
						}
						if (dictionary2.ContainsKey("getattribution_wait"))
						{
							string s = dictionary2["getattribution_wait"].ToString();
							int num = int.Parse(s);
							if (num < 1)
							{
								num = 1;
							}
							if (num > 30)
							{
								num = 30;
							}
							PlayerPrefs.SetString("getattribution_wait", num.ToString());
							this.Log("Saved getattribution_wait to persistent storage: " + num, Kochava.KochLogLevel.debug);
						}
						if (dictionary2.ContainsKey("delay_for_referrer_data"))
						{
							Kochava.device_id_delay = (int)dictionary2["delay_for_referrer_data"];
							this.Log("delay_for_referrer_data received: " + Kochava.device_id_delay, Kochava.KochLogLevel.debug);
							if (Kochava.device_id_delay < 0)
							{
								this.Log("device_id_delay returned was less than 0 (" + Kochava.device_id_delay + "), setting device_id_delay to 0.");
								Kochava.device_id_delay = 0;
							}
							else if (Kochava.device_id_delay > 120)
							{
								this.Log("device_id_delay returned was greater than 120 (" + Kochava.device_id_delay + "), setting device_id_delay to 120.");
								Kochava.device_id_delay = 120;
							}
							else
							{
								this.Log("setting device_id_delay to: " + Kochava.device_id_delay);
							}
						}
						if (dictionary2.ContainsKey("kvinit_wait"))
						{
							string s2 = dictionary2["kvinit_wait"].ToString();
							int num2 = int.Parse(s2);
							if (num2 < 60)
							{
								num2 = 60;
							}
							if (num2 > 604800)
							{
								num2 = 604800;
							}
							PlayerPrefs.SetString("kvinit_wait", num2.ToString());
							this.Log("Saved kvinit_wait to persistent storage: " + num2, Kochava.KochLogLevel.debug);
						}
						else
						{
							PlayerPrefs.SetString("kvinit_wait", "60");
							this.Log("Saved kvinit_wait to persistent storage: 60", Kochava.KochLogLevel.debug);
						}
						if (dictionary2.ContainsKey("kvtracker_wait"))
						{
							string s3 = dictionary2["kvtracker_wait"].ToString();
							int num3 = int.Parse(s3);
							if (num3 < 60)
							{
								num3 = 60;
							}
							if (num3 > 604800)
							{
								num3 = 604800;
							}
							PlayerPrefs.SetString("kvtracker_wait", num3.ToString());
							this.Log("Saved kvtracker_wait to persistent storage: " + num3, Kochava.KochLogLevel.debug);
							this.KVTRACKER_WAIT = num3;
						}
						else
						{
							PlayerPrefs.SetString("kvtracker_wait", "60");
							this.Log("Saved kvtracker_wait to persistent storage: 60", Kochava.KochLogLevel.debug);
							this.KVTRACKER_WAIT = 60;
						}
						if (dictionary2.ContainsKey("location_accuracy"))
						{
							string s4 = dictionary2["location_accuracy"].ToString();
							int num4 = int.Parse(s4);
							if (num4 < 10)
							{
								num4 = 10;
							}
							if (num4 > 5000)
							{
								num4 = 5000;
							}
							this.locationAccuracy = num4;
						}
						if (dictionary2.ContainsKey("location_timeout"))
						{
							string s5 = dictionary2["location_timeout"].ToString();
							int num5 = int.Parse(s5);
							if (num5 < 3)
							{
								num5 = 3;
							}
							if (num5 > 60)
							{
								num5 = 60;
							}
							this.locationTimeout = num5;
						}
						if (dictionary2.ContainsKey("location_staleness"))
						{
							string s6 = dictionary2["location_staleness"].ToString();
							int num6 = int.Parse(s6);
							if (num6 < 1)
							{
								num6 = 1;
							}
							if (num6 > 10080)
							{
								num6 = 10080;
							}
							this.locationStaleness = num6;
						}
						if (dictionary2.ContainsKey("iad_attribution_attempts"))
						{
							string s7 = dictionary2["iad_attribution_attempts"].ToString();
							int num7 = int.Parse(s7);
							if (num7 < 1)
							{
								num7 = 1;
							}
							if (num7 > 10)
							{
								num7 = 10;
							}
							this.iAdAttributionAttempts = num7;
						}
						if (dictionary2.ContainsKey("iad_attribution_wait"))
						{
							string s8 = dictionary2["iad_attribution_wait"].ToString();
							int num8 = int.Parse(s8);
							if (num8 < 1)
							{
								num8 = 1;
							}
							if (num8 > 120)
							{
								num8 = 120;
							}
							this.iAdAttributionWait = num8;
						}
						if (dictionary2.ContainsKey("iad_retry_wait"))
						{
							string s9 = dictionary2["iad_retry_wait"].ToString();
							int num9 = int.Parse(s9);
							if (num9 < 1)
							{
								num9 = 1;
							}
							if (num9 > 60)
							{
								num9 = 60;
							}
							this.iAdRetryWait = num9;
						}
						if (dictionary2.ContainsKey("send_id_updates") && (bool)dictionary2["send_id_updates"])
						{
							this.send_id_updates = true;
						}
					}
					if (dictionary.ContainsKey("blacklist"))
					{
						this.devIdBlacklist = new List<string>();
						if (dictionary["blacklist"].GetType().GetElementType() == typeof(string))
						{
							try
							{
								string[] array = (string[])dictionary["blacklist"];
								for (int i = array.Length - 1; i >= 0; i--)
								{
									this.devIdBlacklist.Add(array[i]);
									if (array[i].ToLower().Equals("adid"))
									{
										Kochava.adidBlacklisted = true;
									}
								}
							}
							catch (Exception var_5_C96)
							{
							}
						}
						try
						{
							string text = JsonWriter.Serialize(this.devIdBlacklist);
							PlayerPrefs.SetString("blacklist", text);
							this.Log(string.Concat(new object[]
							{
								"Saved device_identifier blacklist (",
								this.devIdBlacklist.Count,
								" elements) to persistent storage: ",
								text
							}), Kochava.KochLogLevel.debug);
						}
						catch (Exception var_6_D34)
						{
						}
					}
					if (dictionary.ContainsKey("whitelist") && dictionary["whitelist"].GetType().GetElementType() == typeof(string))
					{
						string text2 = "{";
						try
						{
							string[] array2 = (string[])dictionary["whitelist"];
							for (int j = array2.Length - 1; j >= 0; j--)
							{
								if (array2[j] == "location")
								{
									this.doReportLocation = true;
								}
								if (j != 0)
								{
									text2 = text2 + array2[j] + ",";
								}
								else
								{
									text2 += array2[j];
								}
							}
						}
						catch (Exception var_7_E70)
						{
						}
						text2 += "}";
						this.Log("whitelist string: " + text2);
						this.whitelist = text2;
					}
					if (dictionary.ContainsKey("eventname_blacklist"))
					{
						if (dictionary["eventname_blacklist"].GetType().GetElementType() == typeof(string))
						{
							try
							{
								string[] array3 = (string[])dictionary["eventname_blacklist"];
								for (int k = array3.Length - 1; k >= 0; k--)
								{
									this.eventNameBlacklist.Add(array3[k]);
								}
							}
							catch (Exception var_8_F87)
							{
							}
						}
						PlayerPrefs.SetString("eventname_blacklist", JsonWriter.Serialize(this.eventNameBlacklist));
					}
					this.appData = new Dictionary<string, object>
					{
						{
							"kochava_app_id",
							this.kochavaAppId
						},
						{
							"kochava_device_id",
							this.kochavaDeviceId
						},
						{
							"sdk_version",
							"Unity3D-20160914"
						},
						{
							"sdk_protocol",
							"4"
						}
					};
					PlayerPrefs.SetString("kochava_device_id", this.kochavaDeviceId);
					this.Log("Saved kochava_device_id to persistent storage: " + this.kochavaDeviceId, Kochava.KochLogLevel.debug);
					if (this.sessionTracking == Kochava.KochSessionTracking.full || this.sessionTracking == Kochava.KochSessionTracking.basic)
					{
						Kochava._S._fireEvent("session", new Dictionary<string, object>
						{
							{
								"state",
								"launch"
							}
						});
					}
					AndroidJNIHelper.debug = true;
					AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
					AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
					androidJavaClass2.CallStatic<string>("GetExternalKochavaInfo_Android", new object[]
					{
						androidJavaObject,
						this.whitelist,
						Kochava.device_id_delay,
						PlayerPrefs.GetString("blacklist"),
						Kochava.AdidSupressed
					});
					if (this.doReportLocation)
					{
						double num10 = Kochava.CurrentTime();
						double num11 = double.Parse(PlayerPrefs.GetString("last_location_time"));
						if (num10 - num11 > (double)(this.locationStaleness * 60))
						{
							AndroidJavaClass androidJavaClass3 = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
							AndroidJavaObject static2 = androidJavaClass3.GetStatic<AndroidJavaObject>("currentActivity");
							AndroidJavaObject androidJavaObject2 = static2.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
							AndroidJavaClass androidJavaClass4 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
							androidJavaClass4.CallStatic<string>("GetExternalLocationReport_Android", new object[]
							{
								this.locationAccuracy,
								this.locationTimeout,
								this.locationStaleness
							});
							this.Log("Calling android location gather");
						}
					}
				}
			}
		}
		yield break;
	}

	private void DeviceInformationCallback(string deviceInfo)
	{
		try
		{
			this.hardwareIntegrationData = JsonReader.Deserialize<Dictionary<string, object>>(deviceInfo);
			this.Log(string.Concat(new object[]
			{
				"Received (",
				this.hardwareIntegrationData.Count,
				") parameters from Hardware Integration Library (device info): ",
				deviceInfo
			}));
		}
		catch (Exception arg)
		{
			this.Log("Failed Deserialize hardwareIntegrationData: " + arg, Kochava.KochLogLevel.warning);
		}
		if (!PlayerPrefs.HasKey("watchlistProperties"))
		{
			this.initInitial();
		}
		else
		{
			this.ScanWatchlistChanges();
		}
	}

	public static void InitInitial()
	{
		Kochava._S.initInitial();
	}

	private void initInitial()
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		try
		{
			dictionary.Add("device", SystemInfo.deviceModel);
			if (this.hardwareIntegrationData.ContainsKey("package"))
			{
				dictionary.Add("package", this.hardwareIntegrationData["package"]);
			}
			else
			{
				dictionary.Add("package", this.appIdentifier);
			}
			if (this.hardwareIntegrationData.ContainsKey("app_version"))
			{
				dictionary.Add("app_version", this.hardwareIntegrationData["app_version"]);
			}
			else
			{
				dictionary.Add("app_version", this.appVersion);
			}
			if (this.hardwareIntegrationData.ContainsKey("app_short_string"))
			{
				dictionary.Add("app_short_string", this.hardwareIntegrationData["app_short_string"]);
			}
			else
			{
				dictionary.Add("app_short_string", this.appVersion);
			}
			dictionary.Add("currency", (!(this.appCurrency == string.Empty)) ? this.appCurrency : "USD");
			if (!this.devIdBlacklist.Contains("screen_size"))
			{
				dictionary.Add("disp_h", Screen.height);
				dictionary.Add("disp_w", Screen.width);
			}
			if (!this.devIdBlacklist.Contains("device_orientation") && this.hardwareIntegrationData.ContainsKey("device_orientation"))
			{
				dictionary.Add("device_orientation", this.hardwareIntegrationData["device_orientation"]);
			}
			if (!this.devIdBlacklist.Contains("screen_brightness") && this.hardwareIntegrationData.ContainsKey("screen_brightness"))
			{
				dictionary.Add("screen_brightness", this.hardwareIntegrationData["screen_brightness"]);
			}
			if (!this.devIdBlacklist.Contains("network_conn_type"))
			{
				bool flag = false;
				bool flag2 = false;
				NetworkReachability internetReachability = Application.internetReachability;
				if (internetReachability != NetworkReachability.ReachableViaCarrierDataNetwork)
				{
					if (internetReachability == NetworkReachability.ReachableViaLocalAreaNetwork)
					{
						flag2 = true;
					}
				}
				else
				{
					flag = true;
				}
				if (flag)
				{
					dictionary.Add("network_conn_type", "cellular");
				}
				else if (flag2)
				{
					dictionary.Add("network_conn_type", "wifi");
				}
			}
			dictionary.Add("os_version", SystemInfo.operatingSystem);
			dictionary.Add("app_limit_tracking", this.appLimitAdTracking);
			if (!this.devIdBlacklist.Contains("hardware"))
			{
				dictionary.Add("device_processor", SystemInfo.processorType);
				dictionary.Add("device_cores", SystemInfo.processorCount);
				dictionary.Add("device_memory", SystemInfo.systemMemorySize);
				dictionary.Add("graphics_memory_size", SystemInfo.graphicsMemorySize);
				dictionary.Add("graphics_device_name", SystemInfo.graphicsDeviceName);
				dictionary.Add("graphics_device_vendor", SystemInfo.graphicsDeviceVendor);
				dictionary.Add("graphics_device_id", SystemInfo.graphicsDeviceID);
				dictionary.Add("graphics_device_vendor_id", SystemInfo.graphicsDeviceVendorID);
				dictionary.Add("graphics_device_version", SystemInfo.graphicsDeviceVersion);
				dictionary.Add("graphics_shader_level", SystemInfo.graphicsShaderLevel);
			}
			if (!this.devIdBlacklist.Contains("is_genuine") && Application.genuineCheckAvailable)
			{
				dictionary.Add("is_genuine", (!Application.genuine) ? "0" : "1");
			}
			if (!this.devIdBlacklist.Contains("idfa") && this.hardwareIntegrationData.ContainsKey("IDFA"))
			{
				dictionary.Add("idfa", this.hardwareIntegrationData["IDFA"]);
			}
			if (!this.devIdBlacklist.Contains("idfv") && this.hardwareIntegrationData.ContainsKey("IDFV"))
			{
				dictionary.Add("idfv", this.hardwareIntegrationData["IDFV"]);
			}
			if (!this.devIdBlacklist.Contains("udid") && this.hardwareIntegrationData.ContainsKey("UDID"))
			{
				dictionary.Add("udid", this.hardwareIntegrationData["UDID"]);
			}
			if (!this.devIdBlacklist.Contains("iad_attribution") && this.hardwareIntegrationData.ContainsKey("iad_attribution"))
			{
				dictionary.Add("iad_attribution", this.hardwareIntegrationData["iad_attribution"]);
			}
			if (!this.devIdBlacklist.Contains("app_purchase_date") && this.hardwareIntegrationData.ContainsKey("app_purchase_date"))
			{
				dictionary.Add("app_purchase_date", this.hardwareIntegrationData["app_purchase_date"]);
			}
			if (!this.devIdBlacklist.Contains("iad_impression_date") && this.hardwareIntegrationData.ContainsKey("iad_impression_date"))
			{
				dictionary.Add("iad_impression_date", this.hardwareIntegrationData["iad_impression_date"]);
			}
			if (!this.devIdBlacklist.Contains("iad_attribution_details") && this.hardwareIntegrationData.ContainsKey("iad_attribution_details"))
			{
				dictionary.Add("iad_attribution_details", this.hardwareIntegrationData["iad_attribution_details"]);
			}
			if (!this.devIdBlacklist.Contains("android_id") && this.hardwareIntegrationData.ContainsKey("android_id"))
			{
				dictionary.Add("android_id", this.hardwareIntegrationData["android_id"]);
			}
			if (!this.devIdBlacklist.Contains("adid") && this.hardwareIntegrationData.ContainsKey("adid"))
			{
				dictionary.Add("adid", this.hardwareIntegrationData["adid"]);
			}
			if (!this.devIdBlacklist.Contains("fb_attribution_id") && this.hardwareIntegrationData.ContainsKey("fb_attribution_id"))
			{
				dictionary.Add("fb_attribution_id", this.hardwareIntegrationData["fb_attribution_id"]);
			}
			if (this.hardwareIntegrationData.ContainsKey("device_limit_tracking"))
			{
				dictionary.Add("device_limit_tracking", this.hardwareIntegrationData["device_limit_tracking"]);
			}
			if (!this.devIdBlacklist.Contains("bssid") && this.hardwareIntegrationData.ContainsKey("bssid"))
			{
				dictionary.Add("bssid", this.hardwareIntegrationData["bssid"]);
			}
			if (!this.devIdBlacklist.Contains("carrier_name") && this.hardwareIntegrationData.ContainsKey("carrier_name"))
			{
				dictionary.Add("carrier_name", this.hardwareIntegrationData["carrier_name"]);
			}
			if (!this.devIdBlacklist.Contains("volume") && this.hardwareIntegrationData.ContainsKey("volume"))
			{
				dictionary.Add("volume", this.hardwareIntegrationData["volume"]);
			}
			if (this.hardwareIntegrationData.ContainsKey("language"))
			{
				dictionary.Add("language", this.hardwareIntegrationData["language"]);
			}
			if (this.hardwareIntegrationData.ContainsKey("ids"))
			{
				dictionary.Add("ids", this.hardwareIntegrationData["ids"]);
			}
			if (this.hardwareIntegrationData.ContainsKey("conversion_type"))
			{
				dictionary.Add("conversion_type", this.hardwareIntegrationData["conversion_type"]);
			}
			if (this.hardwareIntegrationData.ContainsKey("conversion_data"))
			{
				dictionary.Add("conversion_data", this.hardwareIntegrationData["conversion_data"]);
			}
			dictionary.Add("usertime", (uint)Kochava.CurrentTime());
			if ((uint)Time.time > 0u)
			{
				dictionary.Add("uptime", (uint)Time.time);
			}
			float num = Kochava.UptimeDelta();
			if (num >= 1f)
			{
				dictionary.Add("updelta", (uint)num);
			}
		}
		catch (Exception arg)
		{
			this.Log("Error preparing initial event: " + arg, Kochava.KochLogLevel.error);
		}
		finally
		{
			this._fireEvent("initial", dictionary);
			if (this.retrieveAttribution)
			{
				int num2 = 7;
				if (PlayerPrefs.HasKey("getattribution_wait"))
				{
					string @string = PlayerPrefs.GetString("getattribution_wait");
					num2 = int.Parse(@string);
				}
				this.Log("Will check for attribution in: " + num2);
				base.StartCoroutine("KochavaAttributionTimerFired", num2);
			}
		}
		try
		{
			Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
			if (this.hardwareIntegrationData.ContainsKey("device_limit_tracking"))
			{
				dictionary2.Add("device_limit_tracking", this.hardwareIntegrationData["device_limit_tracking"].ToString());
			}
			dictionary2.Add("os_version", SystemInfo.operatingSystem);
			dictionary2.Add("app_limit_tracking", this.appLimitAdTracking);
			if (this.hardwareIntegrationData.ContainsKey("language"))
			{
				dictionary2.Add("language", this.hardwareIntegrationData["language"].ToString());
			}
			if (this.hardwareIntegrationData.ContainsKey("app_version"))
			{
				dictionary2.Add("app_version", this.hardwareIntegrationData["app_version"].ToString());
			}
			else
			{
				dictionary2.Add("app_version", this.appVersion);
			}
			if (this.hardwareIntegrationData.ContainsKey("app_short_string"))
			{
				dictionary2.Add("app_short_string", this.hardwareIntegrationData["app_short_string"].ToString());
			}
			else
			{
				dictionary2.Add("app_short_string", this.appVersion);
			}
			if (!this.devIdBlacklist.Contains("idfa") && this.hardwareIntegrationData.ContainsKey("IDFA"))
			{
				dictionary2.Add("idfa", this.hardwareIntegrationData["IDFA"].ToString());
			}
			if (!this.devIdBlacklist.Contains("adid") && this.hardwareIntegrationData.ContainsKey("adid"))
			{
				dictionary2.Add("adid", this.hardwareIntegrationData["adid"]);
			}
			string text = JsonWriter.Serialize(dictionary2);
			PlayerPrefs.SetString("watchlistProperties", text);
			this.Log("watchlistString: " + text);
		}
		catch (Exception arg2)
		{
			this.Log("Error setting watchlist: " + arg2, Kochava.KochLogLevel.error);
		}
	}

	public void ScanWatchlistChanges()
	{
		try
		{
			if (PlayerPrefs.HasKey("watchlistProperties"))
			{
				string @string = PlayerPrefs.GetString("watchlistProperties");
				this.Log("retrieve watchlist: " + @string);
				Dictionary<string, object> dictionary = JsonReader.Deserialize<Dictionary<string, object>>(@string);
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				if (dictionary.ContainsKey("app_version"))
				{
					if (this.hardwareIntegrationData.ContainsKey("app_version"))
					{
						if (dictionary["app_version"].ToString() != this.hardwareIntegrationData["app_version"].ToString())
						{
							dictionary2.Add("app_version", this.hardwareIntegrationData["app_version"].ToString());
							dictionary["app_version"] = this.hardwareIntegrationData["app_version"].ToString();
						}
					}
					else if (dictionary["app_version"].ToString() != this.appVersion)
					{
						dictionary2.Add("app_version", this.appVersion);
						dictionary["app_version"] = this.appVersion;
					}
				}
				if (dictionary.ContainsKey("app_short_string"))
				{
					if (this.hardwareIntegrationData.ContainsKey("app_short_string"))
					{
						if (dictionary["app_short_string"].ToString() != this.hardwareIntegrationData["app_short_string"].ToString())
						{
							dictionary2.Add("app_short_string", this.hardwareIntegrationData["app_short_string"].ToString());
							dictionary["app_short_string"] = this.hardwareIntegrationData["app_short_string"].ToString();
						}
					}
					else if (dictionary["app_short_string"].ToString() != this.appVersion)
					{
						dictionary2.Add("app_short_string", this.appVersion);
						dictionary["app_short_string"] = this.appVersion;
					}
				}
				if (dictionary.ContainsKey("os_version") && dictionary["os_version"].ToString() != SystemInfo.operatingSystem)
				{
					dictionary2.Add("os_version", SystemInfo.operatingSystem);
					dictionary["os_version"] = SystemInfo.operatingSystem;
				}
				if (dictionary.ContainsKey("language") && this.hardwareIntegrationData.ContainsKey("language") && dictionary["language"].ToString() != this.hardwareIntegrationData["language"].ToString())
				{
					dictionary2.Add("language", this.hardwareIntegrationData["language"].ToString());
					dictionary["language"] = this.hardwareIntegrationData["language"].ToString();
				}
				if (dictionary.ContainsKey("device_limit_tracking") && this.hardwareIntegrationData.ContainsKey("device_limit_tracking") && dictionary["device_limit_tracking"].ToString() != this.hardwareIntegrationData["device_limit_tracking"].ToString())
				{
					dictionary2.Add("device_limit_tracking", this.hardwareIntegrationData["device_limit_tracking"].ToString());
					dictionary["device_limit_tracking"] = this.hardwareIntegrationData["device_limit_tracking"].ToString();
				}
				if (dictionary.ContainsKey("app_limit_tracking") && bool.Parse(dictionary["app_limit_tracking"].ToString()) != this.appLimitAdTracking)
				{
					dictionary2.Add("app_limit_tracking", this.appLimitAdTracking);
					dictionary["app_limit_tracking"] = this.appLimitAdTracking;
				}
				if (this.send_id_updates)
				{
					if (!this.devIdBlacklist.Contains("idfa") && dictionary.ContainsKey("idfa") && this.hardwareIntegrationData.ContainsKey("IDFA") && dictionary["idfa"].ToString() != this.hardwareIntegrationData["IDFA"].ToString())
					{
						dictionary2.Add("idfa", this.hardwareIntegrationData["IDFA"].ToString());
						dictionary["idfa"] = this.hardwareIntegrationData["IDFA"].ToString();
					}
					if (!this.devIdBlacklist.Contains("adid") && dictionary.ContainsKey("adid") && this.hardwareIntegrationData.ContainsKey("adid") && dictionary["adid"].ToString() != this.hardwareIntegrationData["adid"].ToString())
					{
						dictionary2.Add("adid", this.hardwareIntegrationData["adid"].ToString());
						dictionary["adid"] = this.hardwareIntegrationData["adid"].ToString();
					}
				}
				if (dictionary2.Count > 0)
				{
					string text = JsonWriter.Serialize(dictionary);
					string str = JsonWriter.Serialize(dictionary2);
					this.Log("final watchlist: " + text);
					this.Log("changeData: " + str);
					PlayerPrefs.SetString("watchlistProperties", text);
					Kochava._S._fireEvent("update", dictionary2);
				}
				else
				{
					this.Log("No watchdata changed");
				}
			}
		}
		catch (Exception arg)
		{
			this.Log("Error scanning watchlist: " + arg, Kochava.KochLogLevel.error);
		}
	}

	public void Update()
	{
		if (Application.isPlaying)
		{
			if (this.processQueueKickstartTime != 0f && Time.time > this.processQueueKickstartTime)
			{
				this.processQueueKickstartTime = 0f;
				base.StartCoroutine("processQueue");
			}
		}
	}

	public static string GetKochavaDeviceId()
	{
		if (PlayerPrefs.HasKey("kochava_device_id"))
		{
			return PlayerPrefs.GetString("kochava_device_id");
		}
		return string.Empty;
	}

	public static void SetLimitAdTracking(bool appLimitTracking)
	{
		Kochava.AppLimitAdTracking = appLimitTracking;
		Kochava._S.ScanWatchlistChanges();
	}

	public static void FireEvent(Dictionary<string, object> properties)
	{
		Kochava._S._fireEvent("event", properties);
	}

	public static void FireEvent(Hashtable propHash)
	{
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		foreach (DictionaryEntry dictionaryEntry in propHash)
		{
			dictionary.Add((string)dictionaryEntry.Key, dictionaryEntry.Value);
		}
		Kochava._S._fireEvent("event", dictionary);
	}

	public static void FireEvent(string eventName, string eventData)
	{
		if (!Kochava.EventNameBlacklist.Contains(eventName))
		{
			Kochava._S._fireEvent("event", new Dictionary<string, object>
			{
				{
					"event_name",
					eventName
				},
				{
					"event_data",
					(eventData != null) ? eventData : string.Empty
				}
			});
		}
	}

	public static void FireEventStandard(FireEventParameters fireEventParameters)
	{
		if (fireEventParameters == null || fireEventParameters.eventName == null || fireEventParameters.eventName.Length < 1)
		{
			return;
		}
		string value = JsonWriter.Serialize(fireEventParameters.valuePayload) ?? string.Empty;
		if (!Kochava.EventNameBlacklist.Contains(fireEventParameters.eventName))
		{
			Kochava._S._fireEvent("event", new Dictionary<string, object>
			{
				{
					"event_name",
					fireEventParameters.eventName
				},
				{
					"event_data",
					value
				},
				{
					"event_standard",
					true.ToString()
				}
			});
		}
	}

	public static void FireSpatialEvent(string eventName, float x, float y)
	{
		Kochava.FireSpatialEvent(eventName, x, y, 0f, string.Empty);
	}

	public static void FireSpatialEvent(string eventName, float x, float y, string eventData)
	{
		Kochava.FireSpatialEvent(eventName, x, y, 0f, (eventData != null) ? eventData : string.Empty);
	}

	public static void FireSpatialEvent(string eventName, float x, float y, float z)
	{
		Kochava.FireSpatialEvent(eventName, x, y, z, string.Empty);
	}

	public static void FireSpatialEvent(string eventName, float x, float y, float z, string eventData)
	{
		if (!Kochava.EventNameBlacklist.Contains(eventName))
		{
			Kochava._S._fireEvent("spatial", new Dictionary<string, object>
			{
				{
					"event_name",
					eventName
				},
				{
					"event_data",
					eventData
				},
				{
					"x",
					x
				},
				{
					"y",
					y
				},
				{
					"z",
					z
				}
			});
		}
	}

	public static void IdentityLink(string key, string val)
	{
		Kochava._S._fireEvent("identityLink", new Dictionary<string, object>
		{
			{
				key,
				val
			}
		});
	}

	public static void IdentityLink(Dictionary<string, object> identities)
	{
		Kochava._S._fireEvent("identityLink", identities);
	}

	public static void DeeplinkEvent(string uri, string sourceApp)
	{
		Kochava._S._fireEvent("deeplink", new Dictionary<string, object>
		{
			{
				"uri",
				uri
			},
			{
				"source_app",
				sourceApp
			}
		});
	}

	private void _fireEvent(string eventAction, Dictionary<string, object> eventData)
	{
		if (eventData.ContainsKey("event_name") && (eventData["event_name"] == null || eventData["event_name"].Equals(string.Empty)))
		{
			this.Log("Cannot create event with null/empty event name.");
			return;
		}
		Dictionary<string, object> dictionary = new Dictionary<string, object>();
		if (!eventData.ContainsKey("usertime"))
		{
			eventData.Add("usertime", (uint)Kochava.CurrentTime());
		}
		if (!eventData.ContainsKey("uptime") && (uint)Time.time > 0u)
		{
			eventData.Add("uptime", (uint)Time.time);
		}
		float num = Kochava.UptimeDelta();
		if (!eventData.ContainsKey("updelta") && num >= 1f)
		{
			eventData.Add("updelta", (uint)num);
		}
		dictionary.Add("action", eventAction);
		dictionary.Add("data", eventData);
		if (Kochava.eventPostingTime != 0f)
		{
			dictionary.Add("last_post_time", Kochava.eventPostingTime);
		}
		if (this.debugMode)
		{
			dictionary.Add("debug", true);
		}
		if (this.debugServer)
		{
			dictionary.Add("debugServer", true);
		}
		bool isInitial = false;
		if (eventAction == "initial")
		{
			isInitial = true;
		}
		this.postEvent(dictionary, isInitial);
	}

	private void postEvent(Dictionary<string, object> data, bool isInitial)
	{
		Kochava.QueuedEvent queuedEvent = new Kochava.QueuedEvent();
		queuedEvent.eventTime = Time.time;
		queuedEvent.eventData = data;
		this.eventQueue.Enqueue(queuedEvent);
		if (isInitial)
		{
			base.StartCoroutine("processQueue");
		}
		else if (this.eventQueue.Count >= 75)
		{
			base.StartCoroutine("processQueue");
		}
		else
		{
			this.processQueueKickstartTime = Time.time + (float)this.KVTRACKER_WAIT;
		}
	}

	private void LocationReportCallback(string locationInfo)
	{
		this.Log("location info: " + locationInfo);
		PlayerPrefs.SetString("last_location_time", Kochava.CurrentTime().ToString());
		try
		{
			Dictionary<string, object> value = new Dictionary<string, object>();
			value = JsonReader.Deserialize<Dictionary<string, object>>(locationInfo);
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			dictionary.Add("location", value);
			Kochava._S._fireEvent("update", dictionary);
		}
		catch (Exception arg)
		{
			this.Log("Failed Deserialize hardwareIntegrationData: " + arg, Kochava.KochLogLevel.warning);
		}
	}

	[DebuggerHidden]
	private IEnumerator processQueue()
	{
		if (!this.queueIsProcessing)
		{
			this.queueIsProcessing = true;
			while (this.appData == null)
			{
				yield return new WaitForSeconds(15f);
				if (this.appData == null)
				{
					this.Log("Event posting delayed (AppData null, kvinit handshake incomplete or Unity reloaded assemblies)", Kochava.KochLogLevel.debug);
				}
			}
			List<object> list = new List<object>();
			List<object> list2 = new List<object>();
			float time = Time.time;
			while (this.eventQueue.Count > 0)
			{
				Kochava.QueuedEvent queuedEvent = this.eventQueue.Peek();
				list2.Add(queuedEvent);
				try
				{
					Dictionary<string, object> eventData = queuedEvent.eventData;
					foreach (KeyValuePair<string, object> current in this.appData)
					{
						if (!eventData.ContainsKey(current.Key))
						{
							eventData.Add(current.Key, current.Value);
						}
					}
					list.Add(eventData);
					this.eventQueue.Dequeue();
				}
				catch (Exception var_1_1AB)
				{
				}
			}
			if (list.Count > 0)
			{
				string text = JsonWriter.Serialize(list);
				this.Log("Posting event: " + text.Replace("{", "{\n").Replace(",", ",\n"), Kochava.KochLogLevel.debug);
				Dictionary<string, string> headers = new Dictionary<string, string>
				{
					{
						"Content-Type",
						"application/json"
					},
					{
						"User-Agent",
						this.userAgent
					}
				};
				this.Log(text, Kochava.KochLogLevel.debug);
				WWW wWW = new WWW("https://control.kochava.com/track/kvTracker?v4", Encoding.UTF8.GetBytes(text), headers);
				yield return wWW;
				try
				{
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					if (wWW.error == null && wWW.text != string.Empty)
					{
						this.Log("Server Response Received: " + WWW.UnEscapeURL(wWW.text), Kochava.KochLogLevel.debug);
						dictionary = JsonReader.Deserialize<Dictionary<string, object>>(wWW.text);
					}
					bool flag = true;
					bool flag2 = dictionary.ContainsKey("success");
					if (!string.IsNullOrEmpty(wWW.error) || !flag2)
					{
						this._eventPostingTime = -1f;
						if (!string.IsNullOrEmpty(wWW.error))
						{
							this.Log("Event Posting Failed: " + wWW.error, Kochava.KochLogLevel.error);
						}
						else
						{
							this.Log("Event Posting Did Not Succeed: " + ((!(wWW.text == string.Empty)) ? wWW.text : "(Blank response from server)"), Kochava.KochLogLevel.error);
							if (dictionary.ContainsKey("error") || wWW.text == string.Empty)
							{
								flag = false;
							}
						}
						this.RequeuePostEvents(list2);
						if (flag)
						{
							this.processQueueKickstartTime = Time.time + 30f;
							this.queueIsProcessing = false;
							goto IL_585;
						}
						this.Log("Event posting failure, event dequeued: " + dictionary["error"], Kochava.KochLogLevel.warning);
					}
					else
					{
						this._eventPostingTime = Time.time - time;
						this.Log("Event Posted (" + this._eventPostingTime + " seconds to upload)");
						if (dictionary.ContainsKey("cta") && dictionary["CTA"].ToString() == "1")
						{
							Application.OpenURL(dictionary["URL"].ToString());
						}
					}
				}
				catch (Exception var_3_549)
				{
				}
			}
			this.queueIsProcessing = false;
		}
		IL_585:
		yield break;
	}

	public void RequeuePostEvents(List<object> saveArray)
	{
		for (int i = 0; i < saveArray.Count; i++)
		{
			Kochava.QueuedEvent item = (Kochava.QueuedEvent)saveArray[i];
			this.eventQueue.Enqueue(item);
		}
	}

	public void OnApplicationPause(bool didPause)
	{
		if (this.sessionTracking == Kochava.KochSessionTracking.full && this.appData != null)
		{
			Kochava._S._fireEvent("session", new Dictionary<string, object>
			{
				{
					"state",
					(!didPause) ? "resume" : "pause"
				}
			});
		}
		if (didPause)
		{
			this.saveQueue();
		}
		else
		{
			this.Log("received - app resume");
			if (PlayerPrefs.HasKey("watchlistProperties"))
			{
				AndroidJNIHelper.debug = true;
				using (AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
				{
					AndroidJavaObject @static = androidJavaClass.GetStatic<AndroidJavaObject>("currentActivity");
					AndroidJavaObject androidJavaObject = @static.Call<AndroidJavaObject>("getApplicationContext", new object[0]);
					AndroidJavaClass androidJavaClass2 = new AndroidJavaClass("com.kochava.android.tracker.lite.KochavaSDKLite");
					androidJavaClass2.CallStatic<string>("GetExternalKochavaInfo_Android", new object[]
					{
						androidJavaObject,
						this.whitelist,
						Kochava.device_id_delay,
						PlayerPrefs.GetString("blacklist"),
						Kochava.AdidSupressed
					});
				}
				if (this.doReportLocation)
				{
					double num = Kochava.CurrentTime();
					double num2 = double.Parse(PlayerPrefs.GetString("last_location_time"));
					if (num - num2 > (double)(this.locationStaleness * 60))
					{
					}
				}
			}
		}
	}

	public void OnApplicationQuit()
	{
		if (this.sessionTracking == Kochava.KochSessionTracking.full || this.sessionTracking == Kochava.KochSessionTracking.basic || this.sessionTracking == Kochava.KochSessionTracking.minimal)
		{
			Kochava._S._fireEvent("session", new Dictionary<string, object>
			{
				{
					"state",
					"quit"
				}
			});
		}
		this.saveQueue();
	}

	private void saveQueue()
	{
		if (this.eventQueue.Count > 0)
		{
			try
			{
				string text = JsonWriter.Serialize(this.eventQueue);
				PlayerPrefs.SetString("kochava_queue_storage", text);
				this.Log("Event Queue saved: " + text, Kochava.KochLogLevel.debug);
			}
			catch (Exception arg)
			{
				this.Log("Failure saving event queue: " + arg, Kochava.KochLogLevel.error);
			}
		}
	}

	private void loadQueue()
	{
		try
		{
			if (PlayerPrefs.HasKey("kochava_queue_storage"))
			{
				string @string = PlayerPrefs.GetString("kochava_queue_storage");
				int num = 0;
				Kochava.QueuedEvent[] array = JsonReader.Deserialize<Kochava.QueuedEvent[]>(@string);
				Kochava.QueuedEvent[] array2 = array;
				for (int i = 0; i < array2.Length; i++)
				{
					Kochava.QueuedEvent item = array2[i];
					if (!this.eventQueue.Contains(item))
					{
						this.eventQueue.Enqueue(item);
						num++;
					}
				}
				this.Log("Loaded (" + num + ") events from persistent storage", Kochava.KochLogLevel.debug);
				PlayerPrefs.DeleteKey("kochava_queue_storage");
				base.StartCoroutine("processQueue");
			}
		}
		catch (Exception arg)
		{
			this.Log("Failure loading event queue: " + arg, Kochava.KochLogLevel.debug);
		}
	}

	public static void ClearQueue()
	{
		Kochava._S.StartCoroutine("clearQueue");
	}

	[DebuggerHidden]
	private IEnumerator clearQueue()
	{
		try
		{
			this.Log("Clearing (" + Kochava.eventQueueLength + ") events from upload queue...");
			Kochava._S.StopCoroutine("processQueue");
		}
		catch (Exception var_1_59)
		{
		}
		yield return null;
		try
		{
			Kochava._S.queueIsProcessing = false;
			Kochava._S.eventQueue = new Queue<Kochava.QueuedEvent>();
		}
		catch (Exception var_2_B4)
		{
		}
		yield break;
	}

	public void GetAd(int webView, int height, int width)
	{
		this.Log("Adserver Implementation Pending");
	}

	private static string[] Chop(string value, int length)
	{
		int num = value.Length;
		int num2 = (num + length - 1) / length;
		string[] array = new string[num2];
		for (int i = 0; i < num2; i++)
		{
			array[i] = value.Substring(i * length, Mathf.Min(length, num));
			num -= length;
		}
		return array;
	}

	private void Log(string msg)
	{
		this.Log(msg, Kochava.KochLogLevel.warning);
	}

	private void Log(string msg, Kochava.KochLogLevel level)
	{
		if (msg.Length > 1000)
		{
			string[] array = Kochava.Chop(msg, 1000);
			if (level == Kochava.KochLogLevel.error)
			{
				UnityEngine.Debug.Log("*** Kochava Error: ");
			}
			else if (this.debugMode)
			{
				UnityEngine.Debug.Log("Kochava: ");
			}
			string[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				string message = array2[i];
				UnityEngine.Debug.Log(message);
			}
		}
		else if (level == Kochava.KochLogLevel.error)
		{
			UnityEngine.Debug.Log("*** Kochava Error: " + msg + " ***");
		}
		else if (this.debugMode)
		{
			UnityEngine.Debug.Log("Kochava: " + msg);
		}
		if (this.debugMode || level == Kochava.KochLogLevel.error || level == Kochava.KochLogLevel.warning)
		{
			this._EventLog.Add(new Kochava.LogEvent(msg, level));
		}
		if (this._EventLog.Count > 50)
		{
			this._EventLog.RemoveAt(0);
		}
	}

	public static void ClearLog()
	{
		Kochava._S._EventLog.Clear();
	}

	protected internal static double CurrentTime()
	{
		return (DateTime.UtcNow - Kochava.Jan1st1970).TotalSeconds;
	}

	protected internal static float UptimeDelta()
	{
		Kochava.uptimeDelta = Time.time - Kochava.uptimeDeltaUpdate;
		Kochava.uptimeDeltaUpdate = Time.time;
		return Kochava.uptimeDelta;
	}

	private string CalculateMD5Hash(string input)
	{
		string result;
		try
		{
			MD5 mD = MD5.Create();
			byte[] bytes = Encoding.ASCII.GetBytes(input);
			byte[] array = mD.ComputeHash(bytes);
			StringBuilder stringBuilder = new StringBuilder();
			for (int i = 0; i < array.Length; i++)
			{
				stringBuilder.Append(array[i].ToString("x2"));
			}
			result = stringBuilder.ToString();
		}
		catch (Exception arg)
		{
			this.Log("Failure calculating MD5 hash: " + arg, Kochava.KochLogLevel.error);
			result = string.Empty;
		}
		return result;
	}

	private string CalculateSHA1Hash(string input)
	{
		string result;
		try
		{
			byte[] array = new SHA1Managed().ComputeHash(Encoding.ASCII.GetBytes(input));
			string text = string.Empty;
			byte[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				byte b = array2[i];
				text += b.ToString("x2");
			}
			result = text;
		}
		catch (Exception arg)
		{
			this.Log("Failure calculating SHA1 hash: " + arg, Kochava.KochLogLevel.error);
			result = string.Empty;
		}
		return result;
	}

	[DebuggerHidden]
	public IEnumerator KochavaAttributionTimerFired(int delayTime)
	{
		yield return new WaitForSeconds((float)delayTime);
		this.Log("attribution timer wait completed");
		Dictionary<string, object> value = new Dictionary<string, object>
		{
			{
				"action",
				"get_attribution"
			},
			{
				"kochava_app_id",
				this.kochavaAppId
			},
			{
				"kochava_device_id",
				this.kochavaDeviceId
			},
			{
				"sdk_version",
				"Unity3D-20160914"
			},
			{
				"sdk_protocol",
				"4"
			}
		};
		string text = JsonWriter.Serialize(value);
		Dictionary<string, string> headers = new Dictionary<string, string>
		{
			{
				"Content-Type",
				"application/xml"
			},
			{
				"User-Agent",
				this.userAgent
			}
		};
		this.Log(text, Kochava.KochLogLevel.debug);
		float time = Time.time;
		WWW wWW = new WWW("https://control.kochava.com/track/kvquery", Encoding.UTF8.GetBytes(text), headers);
		yield return wWW;
		if (!string.IsNullOrEmpty(wWW.error))
		{
			this.Log(string.Concat(new object[]
			{
				"kvquery (attribution) handshake failed: ",
				wWW.error,
				", seconds: ",
				Time.time - time,
				")"
			}), Kochava.KochLogLevel.warning);
			base.StartCoroutine("KochavaAttributionTimerFired", 60);
		}
		else
		{
			Dictionary<string, object> dictionary = new Dictionary<string, object>();
			this.Log("server response: " + wWW.text);
			if (wWW.text != string.Empty)
			{
				try
				{
					dictionary = JsonReader.Deserialize<Dictionary<string, object>>(wWW.text);
				}
				catch (Exception var_3_23E)
				{
				}
			}
			this.Log(wWW.text, Kochava.KochLogLevel.debug);
			if (!dictionary.ContainsKey("success"))
			{
				this.Log("kvquery (attribution) handshake parsing failed: " + wWW.text, Kochava.KochLogLevel.warning);
				base.StartCoroutine("KochavaAttributionTimerFired", 60);
			}
			else if (int.Parse(dictionary["success"].ToString()) == 0)
			{
				this.Log("kvquery (attribution) did not return success = true " + wWW.text, Kochava.KochLogLevel.warning);
				base.StartCoroutine("KochavaAttributionTimerFired", 60);
			}
			else if (dictionary.ContainsKey("data"))
			{
				Dictionary<string, object> dictionary2 = new Dictionary<string, object>();
				try
				{
					dictionary2 = (Dictionary<string, object>)dictionary["data"];
				}
				catch (Exception var_4_36E)
				{
				}
				int num = 0;
				if (dictionary2.ContainsKey("retry"))
				{
					num = int.Parse(dictionary2["retry"].ToString());
					this.Log("attribution retry: " + num, Kochava.KochLogLevel.warning);
				}
				if (num == -1 && dictionary2.ContainsKey("attribution"))
				{
					string text2 = JsonWriter.Serialize(dictionary2["attribution"]);
					PlayerPrefs.SetString("attribution", text2);
					this.attributionDataStr = text2;
					this.Log("Saved attribution chunk to persistent storage: " + text2, Kochava.KochLogLevel.warning);
					if (Kochava.attributionCallback != null)
					{
						Kochava.attributionCallback(text2);
					}
				}
				if (num == 0)
				{
					base.StartCoroutine("KochavaAttributionTimerFired", 60);
				}
				if (num > 0)
				{
					base.StartCoroutine("KochavaAttributionTimerFired", num);
				}
			}
		}
		yield break;
	}

	public static string GetAttributionData()
	{
		return Kochava.AttributionDataStr;
	}
}
