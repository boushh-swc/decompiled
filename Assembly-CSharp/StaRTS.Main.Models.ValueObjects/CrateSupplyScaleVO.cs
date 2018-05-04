using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CrateSupplyScaleVO : IValueObject
	{
		public static int COLUMN_HQ2
		{
			get;
			private set;
		}

		public static int COLUMN_HQ3
		{
			get;
			private set;
		}

		public static int COLUMN_HQ4
		{
			get;
			private set;
		}

		public static int COLUMN_HQ5
		{
			get;
			private set;
		}

		public static int COLUMN_HQ6
		{
			get;
			private set;
		}

		public static int COLUMN_HQ7
		{
			get;
			private set;
		}

		public static int COLUMN_HQ8
		{
			get;
			private set;
		}

		public static int COLUMN_HQ9
		{
			get;
			private set;
		}

		public static int COLUMN_HQ10
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int HQ2
		{
			get;
			set;
		}

		public int HQ3
		{
			get;
			set;
		}

		public int HQ4
		{
			get;
			set;
		}

		public int HQ5
		{
			get;
			set;
		}

		public int HQ6
		{
			get;
			set;
		}

		public int HQ7
		{
			get;
			set;
		}

		public int HQ8
		{
			get;
			set;
		}

		public int HQ9
		{
			get;
			set;
		}

		public int HQ10
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.HQ2 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ2);
			this.HQ3 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ3);
			this.HQ4 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ4);
			this.HQ5 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ5);
			this.HQ6 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ6);
			this.HQ7 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ7);
			this.HQ8 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ8);
			this.HQ9 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ9);
			this.HQ10 = row.TryGetInt(CrateSupplyScaleVO.COLUMN_HQ10);
		}

		public int GetHQScaling(int hq)
		{
			switch (hq)
			{
			case 3:
				return this.HQ3;
			case 4:
				return this.HQ4;
			case 5:
				return this.HQ5;
			case 6:
				return this.HQ6;
			case 7:
				return this.HQ7;
			case 8:
				return this.HQ8;
			case 9:
				return this.HQ9;
			case 10:
				return this.HQ10;
			}
			return this.HQ2;
		}
	}
}
