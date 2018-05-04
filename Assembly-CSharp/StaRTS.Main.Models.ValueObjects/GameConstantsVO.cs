using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class GameConstantsVO : IValueObject
	{
		public static int COLUMN_value
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Value
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Value = row.TryGetString(GameConstantsVO.COLUMN_value);
		}
	}
}
