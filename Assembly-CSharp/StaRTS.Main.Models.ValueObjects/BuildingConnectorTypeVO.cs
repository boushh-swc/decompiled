using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class BuildingConnectorTypeVO : IValueObject
	{
		public static int COLUMN_assetConnectorsNE
		{
			get;
			private set;
		}

		public static int COLUMN_bundleConnectorsNE
		{
			get;
			private set;
		}

		public static int COLUMN_assetConnectorsNW
		{
			get;
			private set;
		}

		public static int COLUMN_bundleConnectorsNW
		{
			get;
			private set;
		}

		public static int COLUMN_assetConnectorsBoth
		{
			get;
			private set;
		}

		public static int COLUMN_bundleConnectorsBoth
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string AssetNameNE
		{
			get;
			set;
		}

		public string BundleNameNE
		{
			get;
			set;
		}

		public string AssetNameNW
		{
			get;
			set;
		}

		public string BundleNameNW
		{
			get;
			set;
		}

		public string AssetNameBoth
		{
			get;
			set;
		}

		public string BundleNameBoth
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetNameNE = row.TryGetString(BuildingConnectorTypeVO.COLUMN_assetConnectorsNE);
			this.BundleNameNE = row.TryGetString(BuildingConnectorTypeVO.COLUMN_bundleConnectorsNE);
			this.AssetNameNW = row.TryGetString(BuildingConnectorTypeVO.COLUMN_assetConnectorsNW);
			this.BundleNameNW = row.TryGetString(BuildingConnectorTypeVO.COLUMN_bundleConnectorsNW);
			this.AssetNameBoth = row.TryGetString(BuildingConnectorTypeVO.COLUMN_assetConnectorsBoth);
			this.BundleNameBoth = row.TryGetString(BuildingConnectorTypeVO.COLUMN_bundleConnectorsBoth);
		}
	}
}
