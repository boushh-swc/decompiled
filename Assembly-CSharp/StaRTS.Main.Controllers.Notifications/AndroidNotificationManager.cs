using DCPI.Platforms.SwrveManager;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace StaRTS.Main.Controllers.Notifications
{
	public class AndroidNotificationManager : INotificationManager
	{
		private DateTime epochDate = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private AndroidJavaObject notificationHandler;

		private AndroidJavaObject pluginActivity;

		public AndroidNotificationManager()
		{
			AndroidJavaClass androidJavaClass = new AndroidJavaClass("com.disney.starts.PluginActivity");
			this.pluginActivity = androidJavaClass.CallStatic<AndroidJavaObject>("getInstance", new object[0]);
			this.notificationHandler = this.pluginActivity.Get<AndroidJavaObject>("notificationHandler");
		}

		public void Init()
		{
			this.LogReceivedLocalNotifications();
			this.ClearAllPendingLocalNotifications(true);
			this.ClearReceivedLocalNotifications();
			SwrveComponent.Instance.Config.GCMSenderId = "376640613548";
			SwrveComponent.Instance.Config.TalkEnabled = false;
			SwrveComponent.Instance.Config.ConversationsEnabled = false;
			SwrveComponent.Instance.Config.PushNotificationEnabled = true;
			SwrveComponent.Instance.Config.GCMPushNotificationTitle = Application.productName;
			SwrveComponent.Instance.Config.GCMPushNotificationIconId = "notif_icon";
			SwrveComponent.Instance.Config.GCMPushNotificationLargeIconId = "notif_icon";
			SwrveComponent.Instance.Config.GCMPushNotificationAccentColor = -1;
			SwrveComponent.Instance.Config.UserId = Service.CurrentPlayer.PlayerId;
			SwrveManager.instance.InitWithAnalyticsKeySecret(4283, "3ccBeOdUkm7sRTrvxMz");
		}

		public string GetDeviceToken()
		{
			string empty = string.Empty;
			return SwrveComponent.Instance.SDK.GetGCMDeviceToken();
		}

		private void LogReceivedLocalNotifications()
		{
			string text = this.notificationHandler.Call<string>("GetReceivedLocalNotifications", new object[0]);
			Service.Logger.Debug("Received Notification Data: " + text);
			Dictionary<string, object> dictionary = new JsonParser(text).Parse() as Dictionary<string, object>;
			if (!dictionary.ContainsKey("receivedLocalNotifs"))
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			DateTime now = DateTime.Now;
			now.AddMinutes(-5.0);
			List<object> list = dictionary["receivedLocalNotifs"] as List<object>;
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				IDictionary<string, object> dictionary2 = list[i] as Dictionary<string, object>;
				string text2 = dictionary2["notifId"] as string;
				long num = Convert.ToInt64(dictionary2["date"] as string);
				Service.Logger.Debug(string.Concat(new object[]
				{
					"notifId: ",
					text2,
					" date: ",
					num
				}));
				if (!string.IsNullOrEmpty(text2))
				{
					NotificationTypeVO optional = staticDataController.GetOptional<NotificationTypeVO>(text2);
					if (optional != null)
					{
						DateTime t = DateUtils.DateFromMillis(num);
						StringBuilder stringBuilder = new StringBuilder();
						stringBuilder.Append(text2);
						stringBuilder.Append("|");
						stringBuilder.Append("none");
						stringBuilder.Append("|");
						stringBuilder.Append(optional.SoundName);
						int num2 = DateTime.Compare(t, now);
						if (num2 >= 0)
						{
							Service.DMOAnalyticsController.LogNotificationReengage(text2, true, optional.Desc, stringBuilder.ToString());
						}
						else
						{
							Service.DMOAnalyticsController.LogNotificationImpression(text2, true, optional.Desc, stringBuilder.ToString());
						}
					}
				}
			}
		}

		public void ScheduleLocalNotification(string notificationUid, string inProgressMessage, string message, string soundName, DateTime time, string key, string objectId)
		{
			string text = this.GetEpochTime(time.ToUniversalTime()).ToString();
			this.notificationHandler.Call("ScheduleLocalNotification", new object[]
			{
				notificationUid,
				message,
				soundName,
				text,
				key,
				objectId
			});
		}

		public void BatchScheduleLocalNotifications(List<NotificationObject> list)
		{
			if (list == null)
			{
				return;
			}
			for (int i = 0; i < list.Count; i++)
			{
				NotificationObject notificationObject = list[i];
				this.ScheduleLocalNotification(notificationObject.NotificationUid, notificationObject.InProgressMessage, notificationObject.Message, notificationObject.SoundName, notificationObject.Time, notificationObject.Key, notificationObject.ObjectId);
			}
		}

		public void ClearReceivedLocalNotifications()
		{
			this.notificationHandler.Call("ClearReceivedLocalNotifications", new object[0]);
		}

		public void ClearAllPendingLocalNotifications(bool clearCountdownTimers)
		{
			this.notificationHandler.Call("ClearAllNotifications", new object[0]);
		}

		public void ClearPendingLocalNotification(string key, string objectId)
		{
			this.notificationHandler.Call("ClearPendingLocalNotification", new object[]
			{
				key,
				objectId
			});
		}

		public void RegisterForRemoteNotifications()
		{
		}

		public void UnregisterForRemoteNotifications()
		{
		}

		private long GetEpochTime(DateTime time)
		{
			return (long)(time - this.epochDate).TotalMilliseconds;
		}

		public bool HasAuthorizedPushNotifications()
		{
			return !string.IsNullOrEmpty(this.GetDeviceToken());
		}
	}
}
