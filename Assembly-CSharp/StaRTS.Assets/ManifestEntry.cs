using System;

namespace StaRTS.Assets
{
	public class ManifestEntry
	{
		public AssetType AssetType
		{
			get;
			set;
		}

		public string AssetPath
		{
			get;
			set;
		}

		public ManifestEntry(AssetType assetType, string assetPath)
		{
			this.AssetType = assetType;
			this.AssetPath = assetPath;
		}
	}
}
