using StaRTS.Utils.MetaData;
using System;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EpisodePointScaleVO : IValueObject
	{
		[CompilerGenerated]
		private static Converter<string, int> <>f__mg$cache0;

		[CompilerGenerated]
		private static Converter<string, int> <>f__mg$cache1;

		[CompilerGenerated]
		private static Converter<string, int> <>f__mg$cache2;

		[CompilerGenerated]
		private static Converter<string, int> <>f__mg$cache3;

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
			string[] arg_37_0 = array;
			if (EpisodePointScaleVO.<>f__mg$cache0 == null)
			{
				EpisodePointScaleVO.<>f__mg$cache0 = new Converter<string, int>(int.Parse);
			}
			this.Objective = Array.ConvertAll<string, int>(arg_37_0, EpisodePointScaleVO.<>f__mg$cache0);
			array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_raid);
			string[] arg_6C_0 = array;
			if (EpisodePointScaleVO.<>f__mg$cache1 == null)
			{
				EpisodePointScaleVO.<>f__mg$cache1 = new Converter<string, int>(int.Parse);
			}
			this.Raid = Array.ConvertAll<string, int>(arg_6C_0, EpisodePointScaleVO.<>f__mg$cache1);
			array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_conflict);
			string[] arg_A1_0 = array;
			if (EpisodePointScaleVO.<>f__mg$cache2 == null)
			{
				EpisodePointScaleVO.<>f__mg$cache2 = new Converter<string, int>(int.Parse);
			}
			this.Conflict = Array.ConvertAll<string, int>(arg_A1_0, EpisodePointScaleVO.<>f__mg$cache2);
			array = row.TryGetStringArray(EpisodePointScaleVO.COLUMN_pvp);
			string[] arg_D6_0 = array;
			if (EpisodePointScaleVO.<>f__mg$cache3 == null)
			{
				EpisodePointScaleVO.<>f__mg$cache3 = new Converter<string, int>(int.Parse);
			}
			this.PvP = Array.ConvertAll<string, int>(arg_D6_0, EpisodePointScaleVO.<>f__mg$cache3);
		}
	}
}
