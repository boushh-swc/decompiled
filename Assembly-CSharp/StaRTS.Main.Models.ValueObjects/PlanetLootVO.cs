using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class PlanetLootVO : IValueObject
	{
		public static int COLUMN_empirePlanetLootEntryUids
		{
			get;
			private set;
		}

		public static int COLUMN_rebelPlanetLootEntryUids
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] EmpirePlanetLootEntryUids
		{
			get;
			private set;
		}

		public string[] RebelPlanetLootEntryUids
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.EmpirePlanetLootEntryUids = row.TryGetStringArray(PlanetLootVO.COLUMN_empirePlanetLootEntryUids);
			this.RebelPlanetLootEntryUids = row.TryGetStringArray(PlanetLootVO.COLUMN_rebelPlanetLootEntryUids);
		}
	}
}
