using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class DataCardTierVO : IValueObject
	{
		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_crateEffect
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
		}

		public string CrateEffect
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(DataCardTierVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(DataCardTierVO.COLUMN_bundleName);
			this.CrateEffect = row.TryGetString(DataCardTierVO.COLUMN_crateEffect);
		}
	}
}
