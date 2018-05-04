using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Controllers.ServerMessages;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class PopupsManager : IEventObserver
	{
		private const float LOGIN_NOTIFICATION_TIME_DELAY = 2f;

		private EventManager eventManager;

		private bool haveShownLoginNotification;

		private List<string> seenMessages;

		private Queue<AdminMessageData> queuedMessages;

		public bool ShowHQCelebrationPopup
		{
			get;
			private set;
		}

		public PopupsManager()
		{
			this.seenMessages = new List<string>();
			this.queuedMessages = new Queue<AdminMessageData>();
			Service.PopupsManager = this;
			this.haveShownLoginNotification = false;
			this.ShowHQCelebrationPopup = false;
			this.eventManager = Service.EventManager;
			this.eventManager.RegisterObserver(this, EventId.WorldInTransitionComplete, EventPriority.Default);
			this.eventManager.RegisterObserver(this, EventId.ServerAdminMessage, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.WorldInTransitionComplete)
			{
				if (id == EventId.ServerAdminMessage)
				{
					AdminMessage adminMessage = cookie as AdminMessage;
					for (int i = 0; i < adminMessage.Messages.Count; i++)
					{
						AdminMessageData adminMessageData = adminMessage.Messages[i];
						if (!this.seenMessages.Contains(adminMessageData.Uid))
						{
							this.seenMessages.Add(adminMessageData.Uid);
							this.queuedMessages.Enqueue(adminMessageData);
							if (Service.GameStateMachine.CurrentState is HomeState || Service.GameStateMachine.CurrentState is EditBaseState)
							{
								this.DisplayAdminMessagesOnQueue(false);
							}
						}
					}
				}
			}
			else if (Service.GameStateMachine.CurrentState is HomeState)
			{
				if (!this.haveShownLoginNotification)
				{
					SetPrefsCommand command = new SetPrefsCommand(false);
					Service.ServerAPI.Enqueue(command);
				}
				this.DisplayAdminMessagesOnQueue(Service.CurrentPlayer.CampaignProgress.FueInProgress);
			}
			return EatResponse.NotEaten;
		}

		public bool DisplayAdminMessagesOnQueue(bool criticalOnly)
		{
			bool result = false;
			List<AdminMessageData> list = new List<AdminMessageData>();
			Lang lang = Service.Lang;
			while (this.queuedMessages.Count > 0)
			{
				AdminMessageData adminMessageData = this.queuedMessages.Dequeue();
				if (!criticalOnly || (criticalOnly && adminMessageData.IsCritical))
				{
					result = true;
					string empty = string.Empty;
					string text;
					lang.GetOptional(adminMessageData.Message, out text);
					AlertScreen.ShowModalWithBI(adminMessageData.IsCritical, empty, text, (!adminMessageData.IsCritical) ? null : (text + " :" + adminMessageData.Uid));
				}
				else
				{
					list.Add(adminMessageData);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				this.queuedMessages.Enqueue(list[i]);
			}
			return result;
		}
	}
}
