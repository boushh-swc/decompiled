using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class MobilizationHologramVO : IAssetVO, IValueObject
	{
		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_assetBundle
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

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(MobilizationHologramVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(MobilizationHologramVO.COLUMN_assetBundle);
		}
	}
}
