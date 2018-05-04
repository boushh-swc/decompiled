using System;

namespace StaRTS.Main.Controllers.Notifications
{
	public class NotificationObject
	{
		public string NotificationUid
		{
			get;
			private set;
		}

		public string InProgressMessage
		{
			get;
			private set;
		}

		public string Message
		{
			get;
			private set;
		}

		public string SoundName
		{
			get;
			private set;
		}

		public DateTime Time
		{
			get;
			set;
		}

		public string Key
		{
			get;
			private set;
		}

		public string ObjectId
		{
			get;
			private set;
		}

		public NotificationObject(string notificationUid, string inProgressMessage, string message, string soundName, DateTime time, string key, string objectId)
		{
			this.NotificationUid = notificationUid;
			this.InProgressMessage = inProgressMessage;
			this.Message = message;
			this.SoundName = soundName;
			this.Time = time;
			this.Key = key;
			this.ObjectId = objectId;
		}
	}
}
