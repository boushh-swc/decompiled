using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodeTaskActionVO : IValueObject
	{
		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_minHQ
		{
			get;
			private set;
		}

		public static int COLUMN_maxHQ
		{
			get;
			private set;
		}

		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_item
		{
			get;
			private set;
		}

		public static int COLUMN_actionName
		{
			get;
			private set;
		}

		public static int COLUMN_actionDesc
		{
			get;
			private set;
		}

		public static int COLUMN_actionIcon
		{
			get;
			private set;
		}

		public static int COLUMN_scaleId
		{
			get;
			private set;
		}

		public static int COLUMN_isSkippable
		{
			get;
			private set;
		}

		public static int COLUMN_skipCost
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
			private set;
		}

		public int MinHQ
		{
			get;
			set;
		}

		public int MaxHQ
		{
			get;
			set;
		}

		public string Type
		{
			get;
			set;
		}

		public string Item
		{
			get;
			set;
		}

		public string ActionName
		{
			get;
			set;
		}

		public string ActionDesc
		{
			get;
			set;
		}

		public string ActionIcon
		{
			get;
			set;
		}

		public string ScaleId
		{
			get;
			set;
		}

		public bool IsSkippable
		{
			get
			{
				return this.SkipCost > 0;
			}
		}

		public int SkipCost
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(EpisodeTaskActionVO.COLUMN_faction));
			this.MinHQ = row.TryGetInt(EpisodeTaskActionVO.COLUMN_minHQ, -1);
			this.MaxHQ = row.TryGetInt(EpisodeTaskActionVO.COLUMN_maxHQ, -1);
			this.Type = row.TryGetString(EpisodeTaskActionVO.COLUMN_type);
			this.Item = row.TryGetString(EpisodeTaskActionVO.COLUMN_item);
			this.ActionName = row.TryGetString(EpisodeTaskActionVO.COLUMN_actionName);
			this.ActionDesc = row.TryGetString(EpisodeTaskActionVO.COLUMN_actionDesc);
			this.ActionIcon = row.TryGetString(EpisodeTaskActionVO.COLUMN_actionIcon);
			this.SkipCost = row.TryGetInt(EpisodeTaskActionVO.COLUMN_skipCost);
			this.ScaleId = row.TryGetString(EpisodeTaskActionVO.COLUMN_scaleId);
		}
	}
}
