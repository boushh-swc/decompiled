using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CharacterVO : IAssetVO, IValueObject
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

		public static int COLUMN_greets
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

		public string[] Greets
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(CharacterVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(CharacterVO.COLUMN_bundleName);
			this.Greets = row.TryGetStringArray(CharacterVO.COLUMN_greets);
		}
	}
}
