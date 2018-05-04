using StaRTS.Audio;
using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class AudioTypeVO : IAssetVO, IValueObject
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

		public static int COLUMN_category
		{
			get;
			private set;
		}

		public static int COLUMN_loop
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

		public AudioCategory Category
		{
			get;
			set;
		}

		public bool Loop
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(AudioTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(AudioTypeVO.COLUMN_bundleName);
			this.Category = StringUtils.ParseEnum<AudioCategory>(row.TryGetString(AudioTypeVO.COLUMN_category));
			this.Loop = row.TryGetBool(AudioTypeVO.COLUMN_loop);
		}
	}
}
