using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class DefenseEncounterVO : IValueObject
	{
		public static int COLUMN_waves
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string WaveGroup
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.WaveGroup = row.TryGetString(DefenseEncounterVO.COLUMN_waves);
		}
	}
}
