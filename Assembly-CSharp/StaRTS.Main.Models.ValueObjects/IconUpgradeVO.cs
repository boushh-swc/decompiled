using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class IconUpgradeVO : IValueObject
	{
		public static int COLUMN_level
		{
			get;
			private set;
		}

		public static int COLUMN_rating
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int Level
		{
			get;
			set;
		}

		public int Rating
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Level = row.TryGetInt(IconUpgradeVO.COLUMN_level);
			this.Rating = row.TryGetInt(IconUpgradeVO.COLUMN_rating);
		}
	}
}
