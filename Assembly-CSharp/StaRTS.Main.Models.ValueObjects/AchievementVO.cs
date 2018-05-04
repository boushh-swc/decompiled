using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class AchievementVO : IValueObject
	{
		public static int COLUMN_rebel_data
		{
			get;
			private set;
		}

		public static int COLUMN_empire_data
		{
			get;
			private set;
		}

		public static int COLUMN_google_achievement_id
		{
			get;
			private set;
		}

		public static int COLUMN_ios_achievement_id
		{
			get;
			private set;
		}

		public static int COLUMN_amazon_achievement_id
		{
			get;
			private set;
		}

		public static int COLUMN_achievement_type_id
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public AchievementType AchievementType
		{
			get;
			private set;
		}

		public string RebelData
		{
			get;
			private set;
		}

		public string EmpireData
		{
			get;
			private set;
		}

		public string GoogleAchievementId
		{
			get;
			private set;
		}

		public string IosAchievementId
		{
			get;
			private set;
		}

		public string AmazonAchievementId
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.RebelData = row.TryGetString(AchievementVO.COLUMN_rebel_data);
			this.EmpireData = row.TryGetString(AchievementVO.COLUMN_empire_data);
			this.GoogleAchievementId = row.TryGetString(AchievementVO.COLUMN_google_achievement_id);
			this.IosAchievementId = row.TryGetString(AchievementVO.COLUMN_ios_achievement_id);
			this.AmazonAchievementId = row.TryGetString(AchievementVO.COLUMN_amazon_achievement_id);
			string name = row.TryGetString(AchievementVO.COLUMN_achievement_type_id);
			this.AchievementType = StringUtils.ParseEnum<AchievementType>(name);
		}
	}
}
