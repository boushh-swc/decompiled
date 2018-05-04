using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodeWidgetStateVO : IValueObject
	{
		public static int COLUMN_uid
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

		public static int COLUMN_empireIconTextureId
		{
			get;
			private set;
		}

		public static int COLUMN_rebelIconTextureId
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

		public static int COLUMN_showTimer
		{
			get;
			private set;
		}

		public string Uid
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

		public string EmpireIconTextureId
		{
			get;
			set;
		}

		public string RebelIconTextureId
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

		public bool ShowTimer
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.EmpireBackgroundTextureId = row.TryGetString(EpisodeWidgetStateVO.COLUMN_empireBackgroundTextureId);
			this.RebelBackgroundTextureId = row.TryGetString(EpisodeWidgetStateVO.COLUMN_rebelBackgroundTextureId);
			this.EmpireIconTextureId = row.TryGetString(EpisodeWidgetStateVO.COLUMN_empireIconTextureId);
			this.RebelIconTextureId = row.TryGetString(EpisodeWidgetStateVO.COLUMN_rebelIconTextureId);
			this.EmpireTitleString = row.TryGetString(EpisodeWidgetStateVO.COLUMN_empireTitleString);
			this.RebelTitleString = row.TryGetString(EpisodeWidgetStateVO.COLUMN_rebelTitleString);
			this.ShowTimer = row.TryGetBool(EpisodeWidgetStateVO.COLUMN_showTimer);
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

		public string GetIconTextureId(FactionType faction)
		{
			if (faction == FactionType.Empire)
			{
				return this.EmpireIconTextureId;
			}
			if (faction == FactionType.Rebel)
			{
				return this.RebelIconTextureId;
			}
			return string.Empty;
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
	}
}
