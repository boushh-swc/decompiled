using StaRTS.Externals.Manimal;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Story;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Notifications
{
	public class SocialPushNotificationController : IEventObserver
	{
		private const string SQUAD_JOIN_REPROMPT = "SocialPushNotifRepromptActionForSquadJoin";

		private const string TROOP_REQUEST_REPROMPT = "SocialPushNotifRepromptActionForTroopRequest";

		private const string WAR_TROOP_REQUEST_REPROMPT = "SocialPushNotifRepromptActionForWarTroopRequest";

		private const string RAID_NOTIFY_REPROMPT = "SocialPushNotifRepromptActionForRaids";

		public SocialPushNotificationController()
		{
			Service.SocialPushNotificationController = this;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.SquadJoinedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadJoinInviteAcceptedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadTroopsRequestedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.SquadWarTroopsRequestedByCurrentPlayer);
			eventManager.RegisterObserver(this, EventId.RaidNotifyRequest);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (Service.NotificationController.HasAgreedToNotifications())
			{
				return EatResponse.NotEaten;
			}
			uint time = ServerTime.Time;
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			int num = Convert.ToInt32(serverPlayerPrefs.GetPref(ServerPref.PushAuthPromptedCount));
			bool flag = this.HasReachedPushNotificationLimit(num);
			switch (id)
			{
			case EventId.SquadJoinedByCurrentPlayer:
			case EventId.SquadJoinInviteAcceptedByCurrentPlayer:
				if (!flag && this.HasEnoughTimeElapsed(time, ServerPref.LastPushAuthPromptSquadJoinedTime, GameConstants.PUSH_NOTIFICATION_SQUAD_JOIN_COOLDOWN) && !this.IsHolonetOpen())
				{
					new ActionChain("SocialPushNotifRepromptActionForSquadJoin");
					serverPlayerPrefs.SetPref(ServerPref.LastPushAuthPromptSquadJoinedTime, time.ToString());
					serverPlayerPrefs.SetPref(ServerPref.PushAuthPromptedCount, (num + 1).ToString());
					Service.ServerAPI.Sync(new SetPrefsCommand(false));
				}
				return EatResponse.NotEaten;
			case EventId.SquadJoinApplicationAcceptedByCurrentPlayer:
			case EventId.SquadWarTroopsRequestStartedByCurrentPlayer:
				IL_5B:
				if (id != EventId.RaidNotifyRequest)
				{
					return EatResponse.NotEaten;
				}
				new ActionChain("SocialPushNotifRepromptActionForRaids");
				serverPlayerPrefs.SetPref(ServerPref.LastPushAuthPromptTroopRequestTime, time.ToString());
				serverPlayerPrefs.SetPref(ServerPref.PushAuthPromptedCount, (num + 1).ToString());
				Service.ServerAPI.Sync(new SetPrefsCommand(false));
				return EatResponse.NotEaten;
			case EventId.SquadTroopsRequestedByCurrentPlayer:
				if (!flag && this.HasEnoughTimeElapsed(time, ServerPref.LastPushAuthPromptTroopRequestTime, GameConstants.PUSH_NOTIFICATIONS_TROOP_REQUEST_COOLDOWN))
				{
					new ActionChain("SocialPushNotifRepromptActionForTroopRequest");
					serverPlayerPrefs.SetPref(ServerPref.LastPushAuthPromptTroopRequestTime, time.ToString());
					serverPlayerPrefs.SetPref(ServerPref.PushAuthPromptedCount, (num + 1).ToString());
					Service.ServerAPI.Sync(new SetPrefsCommand(false));
				}
				return EatResponse.NotEaten;
			case EventId.SquadWarTroopsRequestedByCurrentPlayer:
				if (!flag && this.HasEnoughTimeElapsed(time, ServerPref.LastPushAuthPromptTroopRequestTime, GameConstants.PUSH_NOTIFICATIONS_TROOP_REQUEST_COOLDOWN))
				{
					new ActionChain("SocialPushNotifRepromptActionForWarTroopRequest");
					serverPlayerPrefs.SetPref(ServerPref.LastPushAuthPromptTroopRequestTime, time.ToString());
					serverPlayerPrefs.SetPref(ServerPref.PushAuthPromptedCount, (num + 1).ToString());
					Service.ServerAPI.Sync(new SetPrefsCommand(false));
				}
				return EatResponse.NotEaten;
			}
			goto IL_5B;
		}

		private bool HasEnoughTimeElapsed(uint nowSeconds, ServerPref pref, float cooldown)
		{
			ServerPlayerPrefs serverPlayerPrefs = Service.ServerPlayerPrefs;
			uint num = Convert.ToUInt32(serverPlayerPrefs.GetPref(pref));
			uint num2 = nowSeconds - num;
			uint num3 = (uint)(cooldown * 3600f);
			return num2 > num3;
		}

		private bool HasReachedPushNotificationLimit(int timesAsked)
		{
			return timesAsked >= GameConstants.PUSH_NOTIFICATION_MAX_REACHED;
		}

		private bool IsHolonetOpen()
		{
			return Service.ScreenController.GetHighestLevelScreen<HolonetScreen>() != null;
		}
	}
}
