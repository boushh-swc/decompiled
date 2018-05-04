using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class RaidVO : IValueObject
	{
		public static int COLUMN_buildingHoloAsset
		{
			get;
			private set;
		}

		public static int COLUMN_buildingHoloBundle
		{
			get;
			private set;
		}

		public static int COLUMN_squadEnabled
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string BuildingHoloAssetName
		{
			get;
			private set;
		}

		public string BuildingHoloAssetBundle
		{
			get;
			private set;
		}

		public bool SquadEnabled
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.BuildingHoloAssetName = row.TryGetString(RaidVO.COLUMN_buildingHoloAsset, string.Empty);
			this.BuildingHoloAssetBundle = row.TryGetString(RaidVO.COLUMN_buildingHoloBundle, string.Empty);
			this.SquadEnabled = row.TryGetBool(RaidVO.COLUMN_squadEnabled);
		}
	}
}
