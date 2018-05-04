using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Notifications
{
	public interface INotificationManager
	{
		string GetDeviceToken();

		void Init();

		void ScheduleLocalNotification(string notificationUid, string inProgressMessage, string message, string soundName, DateTime time, string key, string objectId);

		void BatchScheduleLocalNotifications(List<NotificationObject> list);

		void ClearReceivedLocalNotifications();

		void ClearAllPendingLocalNotifications(bool clearCountdownTimers);

		void ClearPendingLocalNotification(string key, string objectId);

		void RegisterForRemoteNotifications();

		void UnregisterForRemoteNotifications();

		bool HasAuthorizedPushNotifications();
	}
}
