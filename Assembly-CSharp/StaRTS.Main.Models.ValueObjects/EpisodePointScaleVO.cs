using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodePointScaleVO : IValueObject
	{
		public static int COLUMN_uid
		{
			get;
			private set;
		}

		public static int COLUMN_objective
		{
			get;
			private set;
		}

		public static int COLUMN_raid
		{
			get;
			private set;
		}

		public static int COLUMN_conflict
		{
			get;
			private set;
		}

		public static int COLUMN_pvp
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int[] Objective
		{
			get;
			set;
		}

		public int[] Raid
		{
			get;
			set;
		}

		public int[] Conflict
		{
			get;
			set;
		}

		public int[] PvP
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			string[] array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_objective);
			this.Objective = Array.ConvertAll<string, int>(array, new Converter<string, int>(int.Parse));
			array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_raid);
			this.Raid = Array.ConvertAll<string, int>(array, new Converter<string, int>(int.Parse));
			array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_conflict);
			this.Conflict = Array.ConvertAll<string, int>(array, new Converter<string, int>(int.Parse));
			array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_pvp);
			this.PvP = Array.ConvertAll<string, int>(array, new Converter<string, int>(int.Parse));
		}
	}
}
