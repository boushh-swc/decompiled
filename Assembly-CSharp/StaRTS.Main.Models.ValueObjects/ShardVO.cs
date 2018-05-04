using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ShardVO : IValueObject
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

		public static int COLUMN_stringName
		{
			get;
			private set;
		}

		public static int COLUMN_stringDesc
		{
			get;
			private set;
		}

		public static int COLUMN_quality
		{
			get;
			private set;
		}

		public static int COLUMN_targetType
		{
			get;
			private set;
		}

		public static int COLUMN_targetGroupId
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
			private set;
		}

		public string BundleName
		{
			get;
			private set;
		}

		public string StringNameId
		{
			get;
			private set;
		}

		public string StringDescId
		{
			get;
			private set;
		}

		public ShardQuality Quality
		{
			get;
			private set;
		}

		public string TargetType
		{
			get;
			private set;
		}

		public string TargetGroupId
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(ShardVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(ShardVO.COLUMN_bundleName);
			this.StringNameId = row.TryGetString(ShardVO.COLUMN_stringName);
			this.StringDescId = row.TryGetString(ShardVO.COLUMN_stringDesc);
			this.Quality = StringUtils.ParseEnum<ShardQuality>(row.TryGetString(ShardVO.COLUMN_quality));
			this.TargetType = row.TryGetString(ShardVO.COLUMN_targetType);
			this.TargetGroupId = row.TryGetString(ShardVO.COLUMN_targetGroupId);
		}
	}
}
