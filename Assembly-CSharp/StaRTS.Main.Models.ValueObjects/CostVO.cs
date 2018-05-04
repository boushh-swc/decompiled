using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CostVO : IValueObject
	{
		public static int COLUMN_credits
		{
			get;
			private set;
		}

		public static int COLUMN_materials
		{
			get;
			private set;
		}

		public static int COLUMN_contraband
		{
			get;
			private set;
		}

		public static int COLUMN_crystals
		{
			get;
			private set;
		}

		public static int COLUMN_reputation
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int Credits
		{
			get;
			set;
		}

		public int Materials
		{
			get;
			set;
		}

		public int Contraband
		{
			get;
			set;
		}

		public int Crystals
		{
			get;
			set;
		}

		public int Reputation
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Credits = row.TryGetInt(CostVO.COLUMN_credits);
			this.Materials = row.TryGetInt(CostVO.COLUMN_materials);
			this.Contraband = row.TryGetInt(CostVO.COLUMN_contraband);
			this.Crystals = row.TryGetInt(CostVO.COLUMN_crystals);
			this.Reputation = row.TryGetInt(CostVO.COLUMN_reputation);
		}
	}
}
