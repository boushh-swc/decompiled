using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EncounterProfileVO : IValueObject
	{
		public const char GROUP_DELIMITER = '|';

		public const char ELEMENT_DELIMITER = ',';

		public const int BUILDING_UID_ARG = 0;

		public const int TRIGGER_TYPE_ARG = 1;

		public const int TROOP_UID_ARG = 2;

		public const int TROOP_QUANTITY_ARG = 3;

		public const int LEASHED_ARG = 4;

		public const int STAGGER_ARG = 5;

		public const int LAST_DITCH_DELAY_ARG = 6;

		public static int COLUMN_groups
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string GroupString
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.GroupString = row.TryGetString(EncounterProfileVO.COLUMN_groups);
		}
	}
}
