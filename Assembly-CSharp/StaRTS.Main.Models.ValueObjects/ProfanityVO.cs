using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ProfanityVO : IValueObject
	{
		public static int COLUMN_words
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] Words
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Words = row.TryGetStringArray(ProfanityVO.COLUMN_words);
		}
	}
}
