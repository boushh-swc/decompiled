using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodePanelVO : IValueObject
	{
		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_empireTitleString
		{
			get;
			private set;
		}

		public static int COLUMN_rebelTitleString
		{
			get;
			private set;
		}

		public static int COLUMN_empireBackgroundTextureId
		{
			get;
			private set;
		}

		public static int COLUMN_rebelBackgroundTextureId
		{
			get;
			private set;
		}

		public static int COLUMN_crateId
		{
			get;
			private set;
		}

		public static int COLUMN_storyId
		{
			get;
			private set;
		}

		public static int COLUMN_researchTextureId
		{
			get;
			private set;
		}

		public static int COLUMN_rewardTextureId
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string EmpireTitleString
		{
			get;
			set;
		}

		public string RebelTitleString
		{
			get;
			set;
		}

		public string EmpireBackgroundTextureId
		{
			get;
			set;
		}

		public string RebelBackgroundTextureId
		{
			get;
			set;
		}

		public string ResearchTextureId
		{
			get;
			set;
		}

		public string RewardTextureId
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.EmpireTitleString = row.TryGetString(EpisodePanelVO.COLUMN_empireTitleString);
			this.RebelTitleString = row.TryGetString(EpisodePanelVO.COLUMN_rebelTitleString);
			this.EmpireBackgroundTextureId = row.TryGetString(EpisodePanelVO.COLUMN_empireBackgroundTextureId);
			this.RebelBackgroundTextureId = row.TryGetString(EpisodePanelVO.COLUMN_rebelBackgroundTextureId);
			this.ResearchTextureId = row.TryGetString(EpisodePanelVO.COLUMN_researchTextureId);
			this.RewardTextureId = row.TryGetString(EpisodePanelVO.COLUMN_rewardTextureId);
		}

		public string GetTitleString(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireTitleString;
			}
			if (faction == FactionType.Rebel)
			{
				return this.RebelTitleString;
			}
			return string.Empty;
		}

		public string GetBackgroundTextureId(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireBackgroundTextureId;
			}
			if (faction == FactionType.Rebel)
			{
				return this.RebelBackgroundTextureId;
			}
			return string.Empty;
		}
	}
}
