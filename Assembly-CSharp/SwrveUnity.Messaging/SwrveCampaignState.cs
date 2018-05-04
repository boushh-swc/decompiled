using SwrveUnity.Helpers;
using System;
using System.Collections.Generic;

namespace SwrveUnity.Messaging
{
	public class SwrveCampaignState
	{
		public enum Status
		{
			Unseen = 0,
			Seen = 1,
			Deleted = 2
		}

		private const string SEEN_KEY = "seen";

		private const string DELETED_KEY = "deleted";

		public int Impressions;

		public int Next;

		public DateTime ShowMessagesAfterDelay;

		public SwrveCampaignState.Status CurStatus;

		public SwrveCampaignState()
		{
			this.ShowMessagesAfterDelay = SwrveHelper.GetNow();
		}

		public SwrveCampaignState(int campaignId, Dictionary<string, object> savedStatesJson)
		{
			string key = "Next" + campaignId;
			if (savedStatesJson.ContainsKey(key))
			{
				this.Next = MiniJsonHelper.GetInt(savedStatesJson, key);
			}
			key = "Impressions" + campaignId;
			if (savedStatesJson.ContainsKey(key))
			{
				this.Impressions = MiniJsonHelper.GetInt(savedStatesJson, key);
			}
			key = "Status" + campaignId;
			if (savedStatesJson.ContainsKey(key))
			{
				this.CurStatus = SwrveCampaignState.ParseStatus(MiniJsonHelper.GetString(savedStatesJson, key));
			}
			else
			{
				this.CurStatus = SwrveCampaignState.Status.Unseen;
			}
		}

		public static SwrveCampaignState.Status ParseStatus(string status)
		{
			if (status.ToLower().Equals("seen"))
			{
				return SwrveCampaignState.Status.Seen;
			}
			if (status.ToLower().Equals("deleted"))
			{
				return SwrveCampaignState.Status.Deleted;
			}
			return SwrveCampaignState.Status.Unseen;
		}

		public override string ToString()
		{
			return string.Format("[SwrveCampaignState] Impressions: {0}, Next: {1}, ShowMessagesAfterDelay: {2}, CurStatus: {3}", new object[]
			{
				this.Impressions,
				this.Next,
				this.ShowMessagesAfterDelay,
				this.CurStatus
			});
		}
	}
}
