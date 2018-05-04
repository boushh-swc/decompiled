using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SquadLevelVO : IValueObject
	{
		public static int COLUMN_repThreshold
		{
			get;
			private set;
		}

		public static int COLUMN_level
		{
			get;
			private set;
		}

		public static int COLUMN_slots
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int RepThreshold
		{
			get;
			private set;
		}

		public int Level
		{
			get;
			private set;
		}

		public int Slots
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.RepThreshold = row.TryGetInt(SquadLevelVO.COLUMN_repThreshold);
			this.Level = row.TryGetInt(SquadLevelVO.COLUMN_level);
			this.Slots = row.TryGetInt(SquadLevelVO.COLUMN_slots);
		}
	}
}
