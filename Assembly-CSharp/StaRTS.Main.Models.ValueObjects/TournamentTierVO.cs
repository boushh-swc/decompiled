using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TournamentTierVO : IValueObject
	{
		public static int COLUMN_order
		{
			get;
			private set;
		}

		public static int COLUMN_percentage
		{
			get;
			private set;
		}

		public static int COLUMN_points
		{
			get;
			private set;
		}

		public static int COLUMN_rankName
		{
			get;
			private set;
		}

		public static int COLUMN_spriteNameEmpire
		{
			get;
			private set;
		}

		public static int COLUMN_spriteNameRebel
		{
			get;
			private set;
		}

		public static int COLUMN_division
		{
			get;
			private set;
		}

		public static int COLUMN_divisionSmall
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int Order
		{
			get;
			private set;
		}

		public float Percentage
		{
			get;
			private set;
		}

		public int Points
		{
			get;
			private set;
		}

		public string RankName
		{
			get;
			private set;
		}

		public string SpriteNameEmpire
		{
			get;
			private set;
		}

		public string SpriteNameRebel
		{
			get;
			private set;
		}

		public string Division
		{
			get;
			set;
		}

		public string DivisionSmall
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Order = row.TryGetInt(TournamentTierVO.COLUMN_order);
			this.Percentage = row.TryGetFloat(TournamentTierVO.COLUMN_percentage);
			this.Points = row.TryGetInt(TournamentTierVO.COLUMN_points);
			this.RankName = row.TryGetString(TournamentTierVO.COLUMN_rankName);
			this.SpriteNameEmpire = row.TryGetString(TournamentTierVO.COLUMN_spriteNameEmpire);
			this.SpriteNameRebel = row.TryGetString(TournamentTierVO.COLUMN_spriteNameRebel);
			this.Division = row.TryGetString(TournamentTierVO.COLUMN_division);
			this.DivisionSmall = row.TryGetString(TournamentTierVO.COLUMN_divisionSmall);
		}
	}
}
