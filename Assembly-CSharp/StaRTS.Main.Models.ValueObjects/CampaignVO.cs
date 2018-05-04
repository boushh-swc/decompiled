using StaRTS.Main.Controllers;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CampaignVO : ITimedEventVO, IValueObject
	{
		private bool miniCampaign;

		private bool miniCampaignAlreadyCalculated;

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_title
		{
			get;
			private set;
		}

		public static int COLUMN_timed
		{
			get;
			private set;
		}

		public static int COLUMN_unlockOrder
		{
			get;
			private set;
		}

		public static int COLUMN_description
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_reward
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_introStory
		{
			get;
			private set;
		}

		public static int COLUMN_purchaseLimit
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public string Title
		{
			get;
			set;
		}

		public bool Timed
		{
			get;
			set;
		}

		public int UnlockOrder
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
		}

		public string IntroStory
		{
			get;
			set;
		}

		public string Reward
		{
			get;
			set;
		}

		public int PurchaseLimit
		{
			get;
			set;
		}

		public int TotalMasteryStars
		{
			get;
			set;
		}

		public int TotalMissions
		{
			get;
			set;
		}

		public int StartTimestamp
		{
			get;
			set;
		}

		public int EndTimestamp
		{
			get;
			set;
		}

		public bool UseTimeZoneOffset
		{
			get
			{
				return true;
			}
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(CampaignVO.COLUMN_faction));
			this.Title = row.TryGetString(CampaignVO.COLUMN_title);
			this.Timed = row.TryGetBool(CampaignVO.COLUMN_timed);
			this.UnlockOrder = row.TryGetInt(CampaignVO.COLUMN_unlockOrder);
			this.Description = row.TryGetString(CampaignVO.COLUMN_description);
			this.BundleName = row.TryGetString(CampaignVO.COLUMN_bundleName);
			this.AssetName = row.TryGetString(CampaignVO.COLUMN_assetName);
			this.Reward = row.TryGetString(CampaignVO.COLUMN_reward);
			string dateString = row.TryGetString(CampaignVO.COLUMN_startDate);
			string dateString2 = row.TryGetString(CampaignVO.COLUMN_endDate);
			this.IntroStory = row.TryGetString(CampaignVO.COLUMN_introStory);
			this.PurchaseLimit = row.TryGetInt(CampaignVO.COLUMN_purchaseLimit);
			this.TotalMasteryStars = 0;
			this.TotalMissions = 0;
			if (this.Timed)
			{
				this.StartTimestamp = TimedEventUtils.GetTimestamp(this.Uid, dateString);
				this.EndTimestamp = TimedEventUtils.GetTimestamp(this.Uid, dateString2);
			}
		}

		public int GetUpcomingDurationSeconds()
		{
			return GameConstants.CAMPAIGN_HOURS_UPCOMING * 3600;
		}

		public int GetClosingDurationSeconds()
		{
			return GameConstants.CAMPAIGN_HOURS_CLOSING * 3600;
		}

		public bool IsMiniCampaign()
		{
			if (this.miniCampaignAlreadyCalculated)
			{
				return this.miniCampaign;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			this.miniCampaign = true;
			foreach (CampaignMissionVO current in staticDataController.GetAll<CampaignMissionVO>())
			{
				if (current.CampaignUid == this.Uid && current.UnlockOrder > 1)
				{
					this.miniCampaign = false;
					break;
				}
			}
			this.miniCampaignAlreadyCalculated = true;
			return this.miniCampaign;
		}
	}
}
