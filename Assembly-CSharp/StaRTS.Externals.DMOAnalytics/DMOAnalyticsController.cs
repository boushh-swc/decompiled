using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Text;

namespace StaRTS.Externals.DMOAnalytics
{
	public class DMOAnalyticsController : IEventObserver
	{
		private const string IN_APP_CURRENCY_JSON_FORMAT = "{{\"item_id\":\"{0}|{1}\",\"item_count\":\"{2}\"}}";

		private const string IN_APP_CURRENCY_JSON_FORMAT_WITHOUT_TYPE = "{{\"item_id\":\"{0}\",\"item_count\":\"{1}\"}}";

		private const string PAYMENT_ACTION_JSON_FORMAT = "{{\"item_id\":\"{0}\",\"item_count\":\"{1}\"}}";

		private IDMOAnalyticsManager analytics;

		public DMOAnalyticsController()
		{
			Service.DMOAnalyticsController = this;
			this.analytics = AndroidDMOAnalyticsController.CreateAndInitializeAndroidDMOAnalyticsController("CBA6B997-F072-4C23-978E-39A23D947BD4", "5DF2AAE8-5FD8-4373-BDB0-C2D7BBABD8B3");
			this.LogAppStart();
		}

		public void Init()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ApplicationPauseToggled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ApplicationQuit, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.PlayerLoginSuccess, EventPriority.Default);
		}

		public void Destroy()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.UnregisterObserver(this, EventId.ApplicationPauseToggled);
			eventManager.UnregisterObserver(this, EventId.ApplicationQuit);
			eventManager.UnregisterObserver(this, EventId.PlayerLoginSuccess);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ApplicationPauseToggled)
			{
				if (id != EventId.ApplicationQuit)
				{
					if (id == EventId.PlayerLoginSuccess)
					{
						CurrentPlayer currentPlayer = Service.CurrentPlayer;
						this.LogUserInfo("guid", currentPlayer.PlayerId);
					}
				}
				else
				{
					this.LogAppEnd();
				}
			}
			else
			{
				this.HandleApplicationPause((bool)cookie);
			}
			return EatResponse.NotEaten;
		}

		public void LogAge(int age)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Serializer serializer = Serializer.Start();
			serializer.AddString("player_id", currentPlayer.PlayerId);
			serializer.AddString("action", "age_gate");
			serializer.AddString("context", "age_gate");
			serializer.AddString("type", age.ToString());
			serializer.Add<int>("level", this.GetHQLevel());
			string parameters = serializer.End().ToString();
			this.LogGameAction(parameters);
		}

		public void LogNotificationImpression(string notifID, bool isLocalNotif, string desc, string message)
		{
			string placement = "Push_Notification";
			if (isLocalNotif)
			{
				placement = "Local_Notification";
			}
			this.LogAdAction(placement, notifID, "Impression", desc, message);
		}

		public void LogNotificationReengage(string notifID, bool isLocalNotif, string desc, string message)
		{
			string placement = "Push_Notification";
			if (isLocalNotif)
			{
				placement = "Local_Notification";
			}
			this.LogAdAction(placement, notifID, "Reengage", desc, message);
		}

		public void LogAdAction(string placement, string creative, string type, string desc, string message)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Serializer serializer = Serializer.Start();
			serializer.AddString("player_id", currentPlayer.PlayerId);
			serializer.AddString("creative", creative);
			serializer.AddString("placement", placement);
			serializer.AddString("offer", desc);
			serializer.AddString("type", type);
			serializer.AddString("locale", Service.EnvironmentController.GetLocale());
			serializer.AddString("message", message);
			serializer.AddString("currency", "USD");
			serializer.Add<double>("gross_revenue", 0.0);
			serializer.End();
			Service.Logger.Debug("LogAdAction: " + serializer.ToString());
			this.analytics.LogEventWithContext("ad_action", serializer.ToString());
		}

		public void LogUserInfo(string userIdDomain, string userId)
		{
			if (string.IsNullOrEmpty(userId))
			{
				Service.Logger.Error("LogUserInfo: " + userIdDomain + " : userId is null");
				return;
			}
			if (Service.CurrentPlayer == null)
			{
				Service.Logger.Error("LogUserInfo: CurrentPlayer is not set.");
				return;
			}
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			if (string.IsNullOrEmpty(currentPlayer.PlayerId))
			{
				Service.Logger.Error("LogUserInfo: Player.PlayerId is NullOrEmpty");
				return;
			}
			Serializer serializer = Serializer.Start();
			serializer.AddString("player_id", currentPlayer.PlayerId);
			serializer.AddString("user_id", userId);
			serializer.AddString("user_id_domain", userIdDomain);
			string parameters = serializer.End().ToString();
			this.analytics.LogEventWithContext("user_info", parameters);
		}

		public void LogInAppCurrencyAction(int currencyAmount, string itemType, string itemId, int itemCount, string type, string subType)
		{
			this.LogInAppCurrencyAction(currencyAmount, itemType, itemId, itemCount, type, subType, string.Empty);
		}

		public void LogInAppCurrencyAction(int currencyAmount, string itemType, string itemId, int itemCount, string type, string subType, string context)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			StringBuilder stringBuilder = new StringBuilder();
			if (itemType.Length == 0)
			{
				stringBuilder.AppendFormat("{{\"item_id\":\"{0}\",\"item_count\":\"{1}\"}}", itemId, itemCount);
			}
			else
			{
				stringBuilder.AppendFormat("{{\"item_id\":\"{0}|{1}\",\"item_count\":\"{2}\"}}", itemType, itemId, itemCount);
			}
			string val = stringBuilder.ToString();
			int itemAmount = currentPlayer.Inventory.GetItemAmount("crystals");
			Serializer serializer = Serializer.Start();
			serializer.AddString("player_id", currentPlayer.PlayerId);
			serializer.Add<int>("amount", currencyAmount);
			serializer.AddString("currency", "crystals");
			serializer.Add<int>("balance", itemAmount);
			serializer.Add<string>("item", val);
			serializer.AddString("type", type);
			serializer.AddString("subtype", subType);
			if (context.Length > 0)
			{
				serializer.AddString("context", context);
			}
			serializer.Add<int>("level", this.GetHQLevel());
			string parameters = serializer.End().ToString();
			this.analytics.LogEventWithContext("in_app_currency_action", parameters);
		}

		public void LogPaymentAction(string currency, double amountPaid, string productId, int amount, string type)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Serializer serializer = Serializer.Start();
			serializer.AddString("player_id", currentPlayer.PlayerId);
			serializer.AddString("currency", currency);
			serializer.AddString("locale", Service.EnvironmentController.GetLocale());
			serializer.Add<double>("amount_paid", -amountPaid);
			serializer.AddString("type", type);
			serializer.Add<int>("level", this.GetHQLevel());
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.AppendFormat("{{\"item_id\":\"{0}\",\"item_count\":\"{1}\"}}", productId, amount);
			serializer.Add<string>("item", stringBuilder.ToString());
			serializer.End();
			this.analytics.LogEventWithContext("payment_action", serializer.ToString());
		}

		private int GetHQLevel()
		{
			if (Service.BuildingLookupController != null)
			{
				return Service.BuildingLookupController.GetHighestLevelHQ();
			}
			return 0;
		}

		public void LogEvent(string appEvent)
		{
			this.analytics.LogEvent(appEvent);
		}

		public void LogAppStart()
		{
			this.analytics.LogAppStart();
		}

		public void LogAppEnd()
		{
			this.analytics.LogAppEnd();
		}

		public void LogAppForeground()
		{
			this.analytics.LogAppForeground();
		}

		public void LogAppBackground()
		{
			this.analytics.LogAppBackground();
		}

		public void LogEventWithContext(string eventName, string parameters)
		{
			this.analytics.LogEventWithContext(eventName, parameters);
		}

		public void LogGameAction(string parameters)
		{
			this.analytics.LogEventWithContext("game_action", parameters);
		}

		public void FlushAnalyticsQueue()
		{
			this.analytics.FlushAnalyticsQueue();
		}

		public void SetDebugLogging(bool isEnable)
		{
			this.analytics.SetDebugLogging(isEnable);
		}

		public void SetCanUseNetwork(bool isEnable)
		{
			this.analytics.SetCanUseNetwork(isEnable);
		}

		private void HandleApplicationPause(bool paused)
		{
			if (paused)
			{
				this.LogAppBackground();
			}
			else
			{
				this.LogAppForeground();
			}
		}
	}
}
