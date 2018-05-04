using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class StoryActionVO : IValueObject
	{
		public static int COLUMN_action_type
		{
			get;
			private set;
		}

		public static int COLUMN_prepare_string
		{
			get;
			private set;
		}

		public static int COLUMN_reaction
		{
			get;
			private set;
		}

		public static int COLUMN_log_type
		{
			get;
			private set;
		}

		public static int COLUMN_log_tag
		{
			get;
			private set;
		}

		public static int COLUMN_log_path
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string ActionType
		{
			get;
			set;
		}

		public string PrepareString
		{
			get;
			set;
		}

		public string Reaction
		{
			get;
			set;
		}

		public string LogType
		{
			get;
			set;
		}

		public string LogTag
		{
			get;
			set;
		}

		public string LogPath
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.ActionType = row.TryGetString(StoryActionVO.COLUMN_action_type);
			this.PrepareString = row.TryGetString(StoryActionVO.COLUMN_prepare_string);
			this.Reaction = row.TryGetString(StoryActionVO.COLUMN_reaction);
			this.LogType = row.TryGetString(StoryActionVO.COLUMN_log_type);
			this.LogTag = row.TryGetString(StoryActionVO.COLUMN_log_tag);
			this.LogPath = row.TryGetString(StoryActionVO.COLUMN_log_path);
		}
	}
}
