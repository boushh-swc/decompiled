using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class PlanetAttachmentVO : IAssetVO, IValueObject
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

		public static int COLUMN_attachmentId
		{
			get;
			private set;
		}

		public static int COLUMN_planets
		{
			get;
			private set;
		}

		public static int COLUMN_locator
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

		public string AttachmentId
		{
			get;
			set;
		}

		public string[] Planets
		{
			get;
			set;
		}

		public string Locator
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(PlanetAttachmentVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(PlanetAttachmentVO.COLUMN_bundleName);
			this.AttachmentId = row.TryGetString(PlanetAttachmentVO.COLUMN_attachmentId);
			this.Planets = row.TryGetStringArray(PlanetAttachmentVO.COLUMN_planets);
			this.Locator = row.TryGetString(PlanetAttachmentVO.COLUMN_locator);
		}
	}
}
