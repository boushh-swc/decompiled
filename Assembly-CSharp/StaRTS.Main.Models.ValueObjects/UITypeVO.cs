using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class UITypeVO : IAssetVO, IValueObject
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

		public static int COLUMN_ShowHUDWhileDisplayed
		{
			get;
			private set;
		}

		public static int COLUMN_HideHUDAfterClosing
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

		public bool ShowHUDWhileDisplayed
		{
			get;
			set;
		}

		public bool HideHUDAfterClosing
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(UITypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(UITypeVO.COLUMN_bundleName);
			this.ShowHUDWhileDisplayed = row.TryGetBool(UITypeVO.COLUMN_ShowHUDWhileDisplayed);
			this.HideHUDAfterClosing = row.TryGetBool(UITypeVO.COLUMN_HideHUDAfterClosing);
		}
	}
}
