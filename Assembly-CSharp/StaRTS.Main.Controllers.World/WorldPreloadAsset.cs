using StaRTS.Assets;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.World
{
	public class WorldPreloadAsset
	{
		public AssetHandle Handle;

		public string AssetName
		{
			get;
			private set;
		}

		public GameObject GameObj
		{
			get;
			set;
		}

		public WorldPreloadAsset(string assetName)
		{
			this.AssetName = assetName;
			this.Handle = AssetHandle.Invalid;
			this.GameObj = null;
		}
	}
}
