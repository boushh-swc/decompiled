using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CommonAssetVO : IAssetVO, IValueObject
	{
		public static int COLUMN_name
		{
			get;
			private set;
		}

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

		public string Uid
		{
			get;
			set;
		}

		public string Name
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
			this.Name = row.TryGetString(CommonAssetVO.COLUMN_name);
			this.AssetName = row.TryGetString(CommonAssetVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(CommonAssetVO.COLUMN_bundleName);
		}
	}
}
