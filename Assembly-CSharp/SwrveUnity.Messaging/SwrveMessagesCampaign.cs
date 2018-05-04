using SwrveUnity.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace SwrveUnity.Messaging
{
	public class SwrveMessagesCampaign : SwrveBaseCampaign
	{
		public List<SwrveMessage> Messages;

		private SwrveMessagesCampaign(DateTime initialisedTime) : base(initialisedTime)
		{
			this.Messages = new List<SwrveMessage>();
		}

		public SwrveMessage GetMessageForEvent(string triggerEvent, IDictionary<string, string> payload, SwrveQAUser qaUser)
		{
			int count = this.Messages.Count;
			if (count == 0)
			{
				base.LogAndAddReason("No messages in campaign " + this.Id, qaUser);
				return null;
			}
			if (base.checkCampaignLimits(triggerEvent, payload, qaUser))
			{
				SwrveLog.Log(string.Format("[{0}] {1} matches a trigger in {2}", this, triggerEvent, this.Id));
				return this.GetNextMessage(count, qaUser);
			}
			return null;
		}

		public SwrveMessage GetMessageForId(int id)
		{
			for (int i = 0; i < this.Messages.Count; i++)
			{
				SwrveMessage swrveMessage = this.Messages[i];
				if (swrveMessage.Id == id)
				{
					return swrveMessage;
				}
			}
			return null;
		}

		protected SwrveMessage GetNextMessage(int messagesCount, SwrveQAUser qaUser)
		{
			if (this.RandomOrder)
			{
				List<SwrveMessage> list = new List<SwrveMessage>(this.Messages);
				list.Shuffle<SwrveMessage>();
				for (int i = 0; i < list.Count; i++)
				{
					SwrveMessage swrveMessage = list[i];
					if (swrveMessage.IsDownloaded())
					{
						return swrveMessage;
					}
				}
			}
			else if (base.Next < messagesCount)
			{
				SwrveMessage swrveMessage2 = this.Messages[base.Next];
				if (swrveMessage2.IsDownloaded())
				{
					return swrveMessage2;
				}
			}
			base.LogAndAddReason("Campaign " + this.Id + " hasn't finished downloading.", qaUser);
			return null;
		}

		protected void AddMessage(SwrveMessage message)
		{
			this.Messages.Add(message);
		}

		public override bool AreAssetsReady()
		{
			return this.Messages.All((SwrveMessage m) => m.IsDownloaded());
		}

		public override bool SupportsOrientation(SwrveOrientation orientation)
		{
			return this.Messages.Any((SwrveMessage m) => m.SupportsOrientation(orientation));
		}

		public HashSet<SwrveAssetsQueueItem> GetImageAssets()
		{
			HashSet<SwrveAssetsQueueItem> hashSet = new HashSet<SwrveAssetsQueueItem>();
			for (int i = 0; i < this.Messages.Count; i++)
			{
				SwrveMessage swrveMessage = this.Messages[i];
				hashSet.UnionWith(swrveMessage.SetOfAssets());
			}
			return hashSet;
		}

		public void MessageWasShownToUser(SwrveMessageFormat messageFormat)
		{
			base.WasShownToUser();
			if (this.Messages.Count > 0)
			{
				if (!this.RandomOrder)
				{
					int num = (base.Next + 1) % this.Messages.Count;
					base.Next = num;
					SwrveLog.Log(string.Concat(new object[]
					{
						"Round Robin: Next message in campaign ",
						this.Id,
						" is ",
						num
					}));
				}
				else
				{
					SwrveLog.Log("Next message in campaign " + this.Id + " is random");
				}
			}
		}

		public static SwrveMessagesCampaign LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, Dictionary<string, object> campaignData, int id, DateTime initialisedTime, SwrveQAUser qaUser, Color? defaultBackgroundColor)
		{
			SwrveMessagesCampaign swrveMessagesCampaign = new SwrveMessagesCampaign(initialisedTime);
			object obj = null;
			campaignData.TryGetValue("messages", out obj);
			IList<object> list = null;
			try
			{
				list = (IList<object>)obj;
			}
			catch (Exception ex)
			{
				swrveMessagesCampaign.LogAndAddReason(string.Concat(new object[]
				{
					"Campaign [",
					id,
					"] invalid messages found, skipping.  Error: ",
					ex
				}), qaUser);
			}
			if (list == null)
			{
				swrveMessagesCampaign.LogAndAddReason("Campaign [" + id + "] JSON messages are null, skipping.", qaUser);
				return null;
			}
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				Dictionary<string, object> messageData = (Dictionary<string, object>)list[i];
				SwrveMessage swrveMessage = SwrveMessage.LoadFromJSON(swrveAssetsManager, swrveMessagesCampaign, messageData, defaultBackgroundColor);
				if (swrveMessage.Formats.Count > 0)
				{
					swrveMessagesCampaign.AddMessage(swrveMessage);
				}
				i++;
			}
			if (swrveMessagesCampaign.Messages.Count == 0)
			{
				swrveMessagesCampaign.LogAndAddReason("Campaign [" + id + "] no messages found, skipping.", qaUser);
			}
			return swrveMessagesCampaign;
		}
	}
}
