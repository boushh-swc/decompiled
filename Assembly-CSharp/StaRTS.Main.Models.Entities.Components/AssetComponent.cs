using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Entities.Components
{
	public class AssetComponent : ComponentBase
	{
		public string AssetName
		{
			get;
			set;
		}

		public string RequestedAssetName
		{
			get;
			set;
		}

		public AssetHandle RequestedAssetHandle
		{
			get;
			set;
		}

		public List<AssetHandle> AddOnsAssetHandles
		{
			get;
			private set;
		}

		public AssetComponent(string assetName)
		{
			this.AssetName = assetName;
			this.RequestedAssetName = null;
			this.RequestedAssetHandle = AssetHandle.Invalid;
			this.AddOnsAssetHandles = new List<AssetHandle>();
		}
	}
}
