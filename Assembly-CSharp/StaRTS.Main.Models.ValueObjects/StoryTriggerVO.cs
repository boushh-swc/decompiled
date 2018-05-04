using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class StoryTriggerVO : IValueObject
	{
		public static int COLUMN_trigger_type
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

		public static int COLUMN_ui_action
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string TriggerType
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

		public string UpdateAction
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TriggerType = row.TryGetString(StoryTriggerVO.COLUMN_trigger_type);
			this.PrepareString = row.TryGetString(StoryTriggerVO.COLUMN_prepare_string);
			this.Reaction = row.TryGetString(StoryTriggerVO.COLUMN_reaction);
			this.UpdateAction = row.TryGetString(StoryTriggerVO.COLUMN_ui_action);
		}
	}
}
