using StaRTS.Assets;
using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class AssetTypeVO : IValueObject
	{
		public const string UID_PREFIX = "asset_";

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_assetCategory
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

		public AssetCategory Category
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.AssetName = row.TryGetString(AssetTypeVO.COLUMN_assetName);
			this.Category = StringUtils.ParseEnum<AssetCategory>(row.TryGetString(AssetTypeVO.COLUMN_assetCategory));
			this.Uid = "asset_" + this.AssetName;
		}
	}
}
