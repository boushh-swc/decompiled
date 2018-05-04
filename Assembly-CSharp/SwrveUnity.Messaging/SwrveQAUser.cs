using SwrveUnity.Helpers;
using SwrveUnity.REST;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using System.Text;

namespace SwrveUnity.Messaging
{
	public class SwrveQAUser
	{
		private const int ApiVersion = 1;

		private const long SessionInterval = 1000L;

		private const long TriggerInterval = 500L;

		private const long PushNotificationInterval = 1000L;

		private const string PushTrackingKey = "_p";

		private readonly SwrveSDK swrve;

		private readonly IRESTClient restClient;

		private readonly string loggingUrl;

		private long lastSessionRequestTime;

		private long lastTriggerRequestTime;

		private long lastPushNotificationRequestTime;

		public readonly bool ResetDevice;

		public readonly bool Logging;

		public Dictionary<int, string> campaignReasons = new Dictionary<int, string>();

		public Dictionary<int, SwrveBaseMessage> campaignMessages = new Dictionary<int, SwrveBaseMessage>();

		public SwrveQAUser(SwrveSDK swrve, Dictionary<string, object> jsonQa)
		{
			this.swrve = swrve;
			this.ResetDevice = MiniJsonHelper.GetBool(jsonQa, "reset_device_state", false);
			this.Logging = MiniJsonHelper.GetBool(jsonQa, "logging", false);
			if (this.Logging)
			{
				this.restClient = new RESTClient();
				this.loggingUrl = MiniJsonHelper.GetString(jsonQa, "logging_url", null);
				this.loggingUrl = this.loggingUrl.Replace("http://", "https://");
				if (!this.loggingUrl.EndsWith("/"))
				{
					this.loggingUrl += "/";
				}
			}
			this.campaignReasons = new Dictionary<int, string>();
			this.campaignMessages = new Dictionary<int, SwrveBaseMessage>();
		}

		protected string getEndpoint(string path)
		{
			while (path.StartsWith("/"))
			{
				path = path.Substring(1);
			}
			return this.loggingUrl + path;
		}

		public void TalkSession(Dictionary<int, string> campaignsDownloaded)
		{
			try
			{
				if (this.CanMakeSessionRequest())
				{
					this.lastSessionRequestTime = SwrveHelper.GetMilliseconds();
					string endpoint = this.getEndpoint(string.Concat(new string[]
					{
						"talk/game/",
						this.swrve.ApiKey,
						"/user/",
						this.swrve.UserId,
						"/session"
					}));
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					IList<object> list = new List<object>();
					Dictionary<int, string>.Enumerator enumerator = campaignsDownloaded.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, string> current = enumerator.Current;
						int key = current.Key;
						KeyValuePair<int, string> current2 = enumerator.Current;
						string value = current2.Value;
						list.Add(new Dictionary<string, object>
						{
							{
								"id",
								key
							},
							{
								"reason",
								(value != null) ? value : string.Empty
							},
							{
								"loaded",
								value == null
							}
						});
					}
					dictionary.Add("campaigns", list);
					Dictionary<string, string> deviceInfo = this.swrve.GetDeviceInfo();
					dictionary.Add("device", deviceInfo);
					this.MakeRequest(endpoint, dictionary);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk session failed: " + ex.ToString());
			}
		}

		public void UpdateDeviceInfo()
		{
			try
			{
				if (this.CanMakeRequest())
				{
					string endpoint = this.getEndpoint(string.Concat(new string[]
					{
						"talk/game/",
						this.swrve.ApiKey,
						"/user/",
						this.swrve.UserId,
						"/device_info"
					}));
					Dictionary<string, object> dictionary = new Dictionary<string, object>();
					Dictionary<string, string> deviceInfo = this.swrve.GetDeviceInfo();
					Dictionary<string, string>.Enumerator enumerator = deviceInfo.GetEnumerator();
					while (enumerator.MoveNext())
					{
						Dictionary<string, object> arg_90_0 = dictionary;
						KeyValuePair<string, string> current = enumerator.Current;
						string arg_90_1 = current.Key;
						KeyValuePair<string, string> current2 = enumerator.Current;
						arg_90_0.Add(arg_90_1, current2.Value);
					}
					this.MakeRequest(endpoint, dictionary);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk device info update failed: " + ex.ToString());
			}
		}

		private void MakeRequest(string endpoint, Dictionary<string, object> json)
		{
			json.Add("version", 1);
			json.Add("client_time", DateTime.UtcNow.ToString("yyyy-MM-ddTHH\\:mm\\:ss.fffffffzzz"));
			string s = Json.Serialize(json);
			byte[] bytes = Encoding.UTF8.GetBytes(s);
			Dictionary<string, string> headers = new Dictionary<string, string>
			{
				{
					"Content-Type",
					"application/json; charset=utf-8"
				},
				{
					"Content-Length",
					bytes.Length.ToString()
				}
			};
			this.swrve.Container.StartCoroutine(this.restClient.Post(endpoint, bytes, headers, new Action<RESTResponse>(this.RestListener)));
		}

		public void TriggerFailure(string eventName, string globalReason)
		{
			try
			{
				if (this.CanMakeTriggerRequest())
				{
					string endpoint = this.getEndpoint(string.Concat(new string[]
					{
						"talk/game/",
						this.swrve.ApiKey,
						"/user/",
						this.swrve.UserId,
						"/trigger"
					}));
					this.MakeRequest(endpoint, new Dictionary<string, object>
					{
						{
							"trigger_name",
							eventName
						},
						{
							"displayed",
							false
						},
						{
							"reason",
							globalReason
						},
						{
							"campaigns",
							new List<object>()
						}
					});
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk session failed: " + ex.ToString());
			}
		}

		public void Trigger(string eventName, SwrveBaseMessage baseMessage)
		{
			try
			{
				if (this.CanMakeTriggerRequest())
				{
					this.lastTriggerRequestTime = SwrveHelper.GetMilliseconds();
					Dictionary<int, string> dictionary = this.campaignReasons;
					Dictionary<int, SwrveBaseMessage> dictionary2 = this.campaignMessages;
					this.campaignReasons = new Dictionary<int, string>();
					this.campaignMessages = new Dictionary<int, SwrveBaseMessage>();
					string endpoint = this.getEndpoint(string.Concat(new string[]
					{
						"talk/game/",
						this.swrve.ApiKey,
						"/user/",
						this.swrve.UserId,
						"/trigger"
					}));
					Dictionary<string, object> dictionary3 = new Dictionary<string, object>();
					dictionary3.Add("trigger_name", eventName);
					dictionary3.Add("displayed", baseMessage != null);
					dictionary3.Add("reason", (baseMessage != null) ? string.Empty : "The loaded campaigns returned no conversation or message");
					IList<object> list = new List<object>();
					Dictionary<int, string>.Enumerator enumerator = dictionary.GetEnumerator();
					while (enumerator.MoveNext())
					{
						KeyValuePair<int, string> current = enumerator.Current;
						int key = current.Key;
						KeyValuePair<int, string> current2 = enumerator.Current;
						string value = current2.Value;
						Dictionary<string, object> dictionary4 = new Dictionary<string, object>();
						dictionary4.Add("id", key);
						dictionary4.Add("displayed", false);
						dictionary4.Add("reason", (value != null) ? value : string.Empty);
						if (dictionary2.ContainsKey(key))
						{
							SwrveBaseMessage swrveBaseMessage = dictionary2[key];
							dictionary4.Add(swrveBaseMessage.GetBaseMessageType() + "_id", swrveBaseMessage.Id);
						}
						list.Add(dictionary4);
					}
					if (baseMessage != null)
					{
						list.Add(new Dictionary<string, object>
						{
							{
								"id",
								baseMessage.Campaign.Id
							},
							{
								"displayed",
								true
							},
							{
								baseMessage.GetBaseMessageType() + "_id",
								baseMessage.Id
							},
							{
								"reason",
								string.Empty
							}
						});
					}
					dictionary3.Add("campaigns", list);
					this.MakeRequest(endpoint, dictionary3);
				}
			}
			catch (Exception ex)
			{
				SwrveLog.LogError("QA request talk session failed: " + ex.ToString());
			}
		}

		private bool CanMakeRequest()
		{
			return this.swrve != null && this.Logging;
		}

		private bool CanMakeTimedRequest(long lastTime, long intervalTime)
		{
			return this.CanMakeRequest() && (lastTime == 0L || SwrveHelper.GetMilliseconds() - lastTime > 1000L);
		}

		private bool CanMakeSessionRequest()
		{
			return this.CanMakeTimedRequest(this.lastSessionRequestTime, 1000L);
		}

		private bool CanMakeTriggerRequest()
		{
			return this.CanMakeTimedRequest(this.lastTriggerRequestTime, 500L);
		}

		private bool CanMakePushNotificationRequest()
		{
			return this.CanMakeTimedRequest(this.lastPushNotificationRequestTime, 1000L);
		}

		private void RestListener(RESTResponse response)
		{
			if (response.Error != WwwDeducedError.NoError)
			{
				SwrveLog.LogError("QA request to failed with error code " + response.Error.ToString() + ": " + response.Body);
			}
		}
	}
}
