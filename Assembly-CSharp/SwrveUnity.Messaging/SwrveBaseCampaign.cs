using SwrveUnity.Helpers;
using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public abstract class SwrveBaseCampaign
	{
		private const string ID_KEY = "id";

		private const string CONVERSATION_KEY = "conversation";

		private const string MESSAGES_KEY = "messages";

		private const string SUBJECT_KEY = "subject";

		private const string MESSAGE_CENTER_KEY = "message_center";

		private const string TRIGGERS_KEY = "triggers";

		private const string EVENT_NAME_KEY = "event_name";

		private const string CONDITIONS_KEY = "conditions";

		private const string DISPLAY_ORDER_KEY = "display_order";

		private const string RULES_KEY = "rules";

		private const string RANDOM_KEY = "random";

		private const string DISMISS_AFTER_VIEWS_KEY = "dismiss_after_views";

		private const string DELAY_FIRST_MESSAGE_KEY = "delay_first_message";

		private const string MIN_DELAY_BETWEEN_MESSAGES_KEY = "min_delay_between_messages";

		private const string START_DATE_KEY = "start_date";

		private const string END_DATE_KEY = "end_date";

		protected const string WaitTimeFormat = "HH\\:mm\\:ss zzz";

		protected const int DefaultDelayFirstMessage = 180;

		protected const long DefaultMaxShows = 99999L;

		protected const int DefaultMinDelay = 60;

		protected readonly System.Random rnd = new System.Random();

		public int Id;

		protected string subject;

		protected List<SwrveTrigger> triggers;

		public DateTime StartDate;

		public DateTime EndDate;

		public bool RandomOrder;

		public SwrveCampaignState State;

		protected readonly DateTime swrveInitialisedTime;

		protected DateTime showMessagesAfterLaunch;

		protected int minDelayBetweenMessage;

		protected int delayFirstMessage = 180;

		protected int maxImpressions;

		public bool MessageCenter
		{
			get;
			protected set;
		}

		public int Impressions
		{
			get
			{
				return this.State.Impressions;
			}
			set
			{
				this.State.Impressions = value;
			}
		}

		public int Next
		{
			get
			{
				return this.State.Next;
			}
			set
			{
				this.State.Next = value;
			}
		}

		public SwrveCampaignState.Status Status
		{
			get
			{
				return this.State.CurStatus;
			}
			set
			{
				this.State.CurStatus = value;
			}
		}

		public string Subject
		{
			get
			{
				return this.subject;
			}
			protected set
			{
				this.subject = value;
			}
		}

		protected DateTime showMessagesAfterDelay
		{
			get
			{
				return this.State.ShowMessagesAfterDelay;
			}
			set
			{
				this.State.ShowMessagesAfterDelay = value;
			}
		}

		protected SwrveBaseCampaign(DateTime initialisedTime)
		{
			this.State = new SwrveCampaignState();
			this.swrveInitialisedTime = initialisedTime;
			this.triggers = new List<SwrveTrigger>();
			this.minDelayBetweenMessage = 60;
			this.showMessagesAfterLaunch = this.swrveInitialisedTime + TimeSpan.FromSeconds(180.0);
		}

		public bool checkCampaignLimits(string triggerEvent, IDictionary<string, string> payload, SwrveQAUser qaUser)
		{
			DateTime now = SwrveHelper.GetNow();
			if (!this.CanTrigger(triggerEvent, payload, qaUser))
			{
				this.LogAndAddReason(string.Concat(new object[]
				{
					"There is no trigger in ",
					this.Id,
					" that matches ",
					triggerEvent
				}), qaUser);
				return false;
			}
			if (!this.IsActive(qaUser))
			{
				return false;
			}
			if (this.Impressions >= this.maxImpressions)
			{
				this.LogAndAddReason(string.Concat(new object[]
				{
					"{Campaign throttle limit} Campaign ",
					this.Id,
					" has been shown ",
					this.maxImpressions,
					" times already"
				}), qaUser);
				return false;
			}
			if (!string.Equals(triggerEvent, "Swrve.Messages.showAtSessionStart", StringComparison.OrdinalIgnoreCase) && this.IsTooSoonToShowMessageAfterLaunch(now))
			{
				this.LogAndAddReason("{Campaign throttle limit} Too soon after launch. Wait until " + this.showMessagesAfterLaunch.ToString("HH\\:mm\\:ss zzz"), qaUser);
				return false;
			}
			if (this.IsTooSoonToShowMessageAfterDelay(now))
			{
				this.LogAndAddReason("{Campaign throttle limit} Too soon after last message. Wait until " + this.showMessagesAfterDelay.ToString("HH\\:mm\\:ss zzz"), qaUser);
				return false;
			}
			return true;
		}

		public bool IsActive(SwrveQAUser qaUser)
		{
			DateTime utcNow = SwrveHelper.GetUtcNow();
			if (this.StartDate > utcNow)
			{
				this.LogAndAddReason(string.Format("Campaign {0} not started yet (now: {1}, end: {2})", this.Id, utcNow, this.StartDate), qaUser);
				return false;
			}
			if (this.EndDate < utcNow)
			{
				this.LogAndAddReason(string.Format("Campaign {0} has finished (now: {1}, end: {2})", this.Id, utcNow, this.EndDate), qaUser);
				return false;
			}
			return true;
		}

		protected void LogAndAddReason(string reason, SwrveQAUser qaUser)
		{
			if (qaUser != null && !qaUser.campaignReasons.ContainsKey(this.Id))
			{
				qaUser.campaignReasons.Add(this.Id, reason);
			}
			SwrveLog.Log(string.Format("{0} {1}", this, reason));
		}

		protected void LogAndAddReason(int ident, string reason, SwrveQAUser qaUser)
		{
			this.LogAndAddReason(reason, qaUser);
		}

		public List<SwrveTrigger> GetTriggers()
		{
			return this.triggers;
		}

		public static SwrveBaseCampaign LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, Dictionary<string, object> campaignData, DateTime initialisedTime, SwrveQAUser qaUser, Color? defaultBackgroundColor)
		{
			int @int = MiniJsonHelper.GetInt(campaignData, "id");
			SwrveBaseCampaign swrveBaseCampaign = null;
			if (campaignData.ContainsKey("conversation"))
			{
				swrveBaseCampaign = SwrveConversationCampaign.LoadFromJSON(swrveAssetsManager, campaignData, @int, initialisedTime);
			}
			else if (campaignData.ContainsKey("messages"))
			{
				swrveBaseCampaign = SwrveMessagesCampaign.LoadFromJSON(swrveAssetsManager, campaignData, @int, initialisedTime, qaUser, defaultBackgroundColor);
			}
			if (swrveBaseCampaign == null)
			{
				return null;
			}
			swrveBaseCampaign.Id = @int;
			SwrveBaseCampaign.AssignCampaignTriggers(swrveBaseCampaign, campaignData);
			swrveBaseCampaign.MessageCenter = (campaignData.ContainsKey("message_center") && (bool)campaignData["message_center"]);
			if (!swrveBaseCampaign.MessageCenter && swrveBaseCampaign.GetTriggers().Count == 0)
			{
				swrveBaseCampaign.LogAndAddReason("Campaign [" + swrveBaseCampaign.Id + "], has no triggers. Skipping this campaign.", qaUser);
				return null;
			}
			SwrveBaseCampaign.AssignCampaignRules(swrveBaseCampaign, campaignData);
			SwrveBaseCampaign.AssignCampaignDates(swrveBaseCampaign, campaignData);
			swrveBaseCampaign.Subject = ((!campaignData.ContainsKey("subject")) ? string.Empty : ((string)campaignData["subject"]));
			if (swrveBaseCampaign.MessageCenter)
			{
				SwrveLog.Log(string.Format("message center campaign: {0}, {1}", swrveBaseCampaign.GetType(), swrveBaseCampaign.subject));
			}
			return swrveBaseCampaign;
		}

		public abstract bool AreAssetsReady();

		public abstract bool SupportsOrientation(SwrveOrientation orientation);

		protected static void AssignCampaignTriggers(SwrveBaseCampaign campaign, Dictionary<string, object> campaignData)
		{
			IList<object> list = (IList<object>)campaignData["triggers"];
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				object obj = list[i];
				if (obj.GetType() == typeof(string))
				{
					obj = new Dictionary<string, object>
					{
						{
							"event_name",
							obj
						},
						{
							"conditions",
							new Dictionary<string, object>()
						}
					};
				}
				try
				{
					SwrveTrigger item = SwrveTrigger.LoadFromJson((IDictionary<string, object>)obj);
					campaign.GetTriggers().Add(item);
				}
				catch (Exception ex)
				{
					SwrveLog.LogError(string.Concat(new object[]
					{
						"Unable to parse SwrveTrigger from json ",
						Json.Serialize(obj),
						", ",
						ex
					}));
				}
				i++;
			}
		}

		protected static void AssignCampaignRules(SwrveBaseCampaign campaign, Dictionary<string, object> campaignData)
		{
			Dictionary<string, object> dictionary = (Dictionary<string, object>)campaignData["rules"];
			campaign.RandomOrder = ((string)dictionary["display_order"]).Equals("random");
			if (dictionary.ContainsKey("dismiss_after_views"))
			{
				int @int = MiniJsonHelper.GetInt(dictionary, "dismiss_after_views");
				campaign.maxImpressions = @int;
			}
			if (dictionary.ContainsKey("delay_first_message"))
			{
				campaign.delayFirstMessage = MiniJsonHelper.GetInt(dictionary, "delay_first_message");
				campaign.showMessagesAfterLaunch = campaign.swrveInitialisedTime + TimeSpan.FromSeconds((double)campaign.delayFirstMessage);
			}
			if (dictionary.ContainsKey("min_delay_between_messages"))
			{
				int int2 = MiniJsonHelper.GetInt(dictionary, "min_delay_between_messages");
				campaign.minDelayBetweenMessage = int2;
			}
		}

		protected static void AssignCampaignDates(SwrveBaseCampaign campaign, Dictionary<string, object> campaignData)
		{
			DateTime unixEpoch = SwrveHelper.UnixEpoch;
			campaign.StartDate = unixEpoch.AddMilliseconds((double)MiniJsonHelper.GetLong(campaignData, "start_date"));
			campaign.EndDate = unixEpoch.AddMilliseconds((double)MiniJsonHelper.GetLong(campaignData, "end_date"));
		}

		public void IncrementImpressions()
		{
			this.Impressions++;
		}

		protected bool IsTooSoonToShowMessageAfterLaunch(DateTime now)
		{
			return now < this.showMessagesAfterLaunch;
		}

		protected bool IsTooSoonToShowMessageAfterDelay(DateTime now)
		{
			return now < this.showMessagesAfterDelay;
		}

		protected void SetMessageMinDelayThrottle()
		{
			this.showMessagesAfterDelay = SwrveHelper.GetNow() + TimeSpan.FromSeconds((double)this.minDelayBetweenMessage);
		}

		public void WasShownToUser()
		{
			this.Status = SwrveCampaignState.Status.Seen;
			this.IncrementImpressions();
			this.SetMessageMinDelayThrottle();
		}

		public void MessageDismissed()
		{
			this.SetMessageMinDelayThrottle();
		}

		public bool IsA<T>() where T : SwrveBaseCampaign
		{
			return base.GetType() == typeof(T);
		}

		public bool CanTrigger(string eventName, IDictionary<string, string> payload = null, SwrveQAUser qaUser = null)
		{
			return this.GetTriggers().Any((SwrveTrigger trig) => trig.CanTrigger(eventName, payload));
		}
	}
}
