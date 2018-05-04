using System;
using System.Collections.Generic;

namespace SwrveUnity
{
	public interface ISwrvePushNotificationListener
	{
		void OnNotificationReceived(Dictionary<string, object> notificationJson);

		void OnOpenedFromPushNotification(Dictionary<string, object> notificationJson);
	}
}
