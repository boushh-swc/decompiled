using DCPI.Platforms.SwrveManager;
using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Configs;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.Notifications
{
	public class NotificationController
	{
		public const string SOUND_DEFAULT = "sfx_ui_notification";

		private INotificationManager notificationManager;

		private NotificationEventManager notificationEventManager;

		private bool isEnabled;

		public bool Enabled
		{
			get
			{
				return this.isEnabled;
			}
			set
			{
				this.isEnabled = value;
				if (this.isEnabled)
				{
					this.RegisterForRemoteNotifications();
				}
				else
				{
					this.UnregisterForRemoteNotifications();
				}
			}
		}

		public NotificationController()
		{
			Service.NotificationController = this;
			if (SwrveManager.instance == null)
			{
				UnityEngine.Object.Instantiate(Resources.Load("SwrveManager"));
			}
			SwrveLog.Level = SwrveLog.LogLevel.Disabled;
			this.notificationManager = new AndroidNotificationManager();
			this.notificationEventManager = new NotificationEventManager();
		}

		public bool HasAgreedToNotifications()
		{
			return PlayerSettings.GetNotificationsLevel() > 0;
		}

		public void TryEnableNotifications()
		{
			this.Enabled = (PlayerSettings.GetNotificationsLevel() > 0);
		}

		public bool HasAuthorizedPushNotifications()
		{
			return this.notificationManager.HasAuthorizedPushNotifications();
		}

		public void Init()
		{
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				return;
			}
			this.notificationManager.Init();
			this.notificationEventManager.Init();
		}

		public string GetDeviceToken()
		{
			return this.notificationManager.GetDeviceToken();
		}

		public void ScheduleLocalNotification(string message, string soundName, DateTime time)
		{
			this.ScheduleLocalNotification(string.Empty, string.Empty, message, soundName, time, string.Empty, string.Empty);
		}

		public void ScheduleLocalNotification(string notificationUID, string inProgressMessage, string message, string soundName, DateTime time, string key, string objectId)
		{
			this.notificationManager.ScheduleLocalNotification(notificationUID, inProgressMessage, message, soundName, time, key, objectId);
		}

		public void BatchScheduleLocalNotifications(List<NotificationObject> notifs)
		{
			this.ClearAllPendingLocalNotifications();
			this.notificationManager.BatchScheduleLocalNotifications(notifs);
		}

		public void ClearReceivedLocalNotifications()
		{
			this.notificationManager.ClearReceivedLocalNotifications();
		}

		public void ClearAllPendingLocalNotifications()
		{
			this.notificationManager.ClearAllPendingLocalNotifications(true);
		}

		public void ClearPendingLocalNotification(string key, string objectId)
		{
			this.notificationManager.ClearPendingLocalNotification(key, objectId);
		}

		public void RegisterForRemoteNotifications()
		{
			if (this.notificationManager == null)
			{
				return;
			}
			this.notificationManager.RegisterForRemoteNotifications();
			string deviceToken = this.notificationManager.GetDeviceToken();
			RegisterDeviceRequest registerDeviceRequest = new RegisterDeviceRequest();
			registerDeviceRequest.DeviceToken = deviceToken;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (currentPlayer == null)
			{
				Service.Logger.Warn("Trying to register for remote notification before CurrentPlayer is available");
				return;
			}
			registerDeviceRequest.PlayerId = currentPlayer.PlayerId;
			RegisterDeviceCommand registerDeviceCommand = new RegisterDeviceCommand(registerDeviceRequest);
			registerDeviceCommand.AddSuccessCallback(new AbstractCommand<RegisterDeviceRequest, RegisterDeviceResponse>.OnSuccessCallback(this.OnRegisterSuccess));
			if (!Service.ServerAPI.Enabled && Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.ServerAPI.Enabled = true;
			}
			Service.ServerAPI.Sync(registerDeviceCommand);
		}

		private void OnRegisterSuccess(RegisterDeviceResponse response, object cookie)
		{
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.ServerAPI.Enabled = false;
			}
		}

		public void UnregisterForRemoteNotifications()
		{
			if (this.notificationManager == null)
			{
				return;
			}
			this.notificationManager.UnregisterForRemoteNotifications();
			string deviceToken = this.notificationManager.GetDeviceToken();
			if (!string.IsNullOrEmpty(deviceToken))
			{
				DeregisterDeviceRequest deregisterDeviceRequest = new DeregisterDeviceRequest();
				deregisterDeviceRequest.DeviceToken = deviceToken;
				CurrentPlayer currentPlayer = Service.CurrentPlayer;
				if (currentPlayer == null)
				{
					Service.Logger.Warn("Trying to unregister for remote notification before CurrentPlayer is available");
					return;
				}
				deregisterDeviceRequest.PlayerId = currentPlayer.PlayerId;
				DeregisterDeviceCommand command = new DeregisterDeviceCommand(deregisterDeviceRequest);
				Service.ServerAPI.Async(command);
			}
			if (Service.BuildingLookupController != null)
			{
				Service.BILoggingController.TrackGameAction("push_notification", "04_standard_deny", Service.BuildingLookupController.GetHighestLevelHQ().ToString(), string.Empty);
			}
		}
	}
}
