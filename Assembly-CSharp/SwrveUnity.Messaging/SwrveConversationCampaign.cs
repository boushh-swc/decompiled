using System;
using System.Collections.Generic;

namespace SwrveUnity.Messaging
{
	public class SwrveConversationCampaign : SwrveBaseCampaign
	{
		public SwrveConversation Conversation;

		private SwrveConversationCampaign(DateTime initialisedTime) : base(initialisedTime)
		{
		}

		public SwrveConversation GetConversationForEvent(string triggerEvent, IDictionary<string, string> payload, SwrveQAUser qaUser)
		{
			if (this.Conversation == null)
			{
				base.LogAndAddReason("No conversation in campaign " + this.Id, qaUser);
				return null;
			}
			if (base.checkCampaignLimits(triggerEvent, payload, qaUser))
			{
				SwrveLog.Log(string.Format("[{0}] {1} matches a trigger in {2}", this, triggerEvent, this.Id));
				if (this.AreAssetsReady())
				{
					return this.Conversation;
				}
				base.LogAndAddReason("Assets not downloaded to show conversation in campaign " + this.Id, qaUser);
			}
			return null;
		}

		public override bool AreAssetsReady()
		{
			return this.Conversation.AreAssetsReady();
		}

		public override bool SupportsOrientation(SwrveOrientation orientation)
		{
			return true;
		}

		public static SwrveConversationCampaign LoadFromJSON(ISwrveAssetsManager swrveAssetsManager, Dictionary<string, object> campaignData, int campaignId, DateTime initialisedTime)
		{
			SwrveConversationCampaign swrveConversationCampaign = new SwrveConversationCampaign(initialisedTime);
			swrveConversationCampaign.Conversation = SwrveConversation.LoadFromJSON(swrveAssetsManager, swrveConversationCampaign, (Dictionary<string, object>)campaignData["conversation"]);
			return swrveConversationCampaign;
		}
	}
}
