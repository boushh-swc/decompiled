using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Assets
{
	public class AssetInfo
	{
		public AssetHandle BundleHandle
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			private set;
		}

		public AssetType AssetType
		{
			get;
			private set;
		}

		public object AssetObject
		{
			get;
			set;
		}

		public int LoadCount
		{
			get;
			set;
		}

		public List<AssetHandle> UnloadHandles
		{
			get;
			set;
		}

		public bool AllContentsExtracted
		{
			get;
			set;
		}

		public UnityEngine.Object[] Prefabs
		{
			get;
			set;
		}

		public List<AssetRequest> AssetRequests
		{
			get;
			set;
		}

		public AssetInfo(string assetName, AssetType assetType)
		{
			this.BundleHandle = AssetHandle.Invalid;
			this.AssetName = assetName;
			this.AssetType = assetType;
			this.AssetObject = null;
			this.AssetRequests = null;
			this.LoadCount = 0;
			this.UnloadHandles = null;
			this.AllContentsExtracted = false;
			this.Prefabs = null;
		}
	}
}
