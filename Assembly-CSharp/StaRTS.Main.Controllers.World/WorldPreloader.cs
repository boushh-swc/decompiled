using StaRTS.Assets;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.World
{
	public class WorldPreloader
	{
		public delegate void PreloadSuccessDelegate();

		private WorldPreloader.PreloadSuccessDelegate onPreloadSuccess;

		private int numAssetsRemainingToLoad;

		private Dictionary<string, Queue<WorldPreloadAsset>> preloadedAssets;

		public WorldPreloader()
		{
			Service.WorldPreloader = this;
			this.preloadedAssets = new Dictionary<string, Queue<WorldPreloadAsset>>();
		}

		public void Load(List<IAssetVO> assetsToLoad, WorldPreloader.PreloadSuccessDelegate onPreloadSuccess)
		{
			if (this.numAssetsRemainingToLoad != 0)
			{
				Service.Logger.Error("WorldPreloader.Load() is called when it's still loading!");
				return;
			}
			int num = (assetsToLoad != null) ? assetsToLoad.Count : 0;
			if (num == 0)
			{
				onPreloadSuccess();
				return;
			}
			this.onPreloadSuccess = onPreloadSuccess;
			this.numAssetsRemainingToLoad = num;
			AssetManager assetManager = Service.AssetManager;
			for (int i = 0; i < num; i++)
			{
				string assetName = assetsToLoad[i].AssetName;
				WorldPreloadAsset worldPreloadAsset = new WorldPreloadAsset(assetName);
				assetManager.Load(ref worldPreloadAsset.Handle, assetName, new AssetSuccessDelegate(this.OnAssetLoadSuccess), new AssetFailureDelegate(this.OnAssetLoadFailure), worldPreloadAsset);
				Queue<WorldPreloadAsset> queue;
				if (this.preloadedAssets.ContainsKey(assetName))
				{
					queue = this.preloadedAssets[assetName];
				}
				else
				{
					queue = new Queue<WorldPreloadAsset>();
					this.preloadedAssets.Add(assetName, queue);
				}
				queue.Enqueue(worldPreloadAsset);
			}
		}

		public void Unload()
		{
			AssetManager assetManager = Service.AssetManager;
			foreach (Queue<WorldPreloadAsset> current in this.preloadedAssets.Values)
			{
				int i = 0;
				int count = current.Count;
				while (i < count)
				{
					WorldPreloadAsset worldPreloadAsset = current.Dequeue();
					if (worldPreloadAsset.GameObj != null && assetManager.IsAssetCloned(worldPreloadAsset.AssetName))
					{
						UnityEngine.Object.Destroy(worldPreloadAsset.GameObj);
						worldPreloadAsset.GameObj = null;
					}
					if (worldPreloadAsset.Handle != AssetHandle.Invalid)
					{
						assetManager.Unload(worldPreloadAsset.Handle);
						worldPreloadAsset.Handle = AssetHandle.Invalid;
					}
					i++;
				}
			}
			this.preloadedAssets.Clear();
		}

		public WorldPreloadAsset GetPreloadedAsset(string assetName)
		{
			WorldPreloadAsset worldPreloadAsset = null;
			if (this.preloadedAssets.ContainsKey(assetName))
			{
				Queue<WorldPreloadAsset> queue = this.preloadedAssets[assetName];
				worldPreloadAsset = queue.Dequeue();
				if (queue.Count == 0)
				{
					this.preloadedAssets.Remove(assetName);
				}
				if (worldPreloadAsset.GameObj != null)
				{
					worldPreloadAsset.GameObj.SetActive(true);
				}
			}
			return worldPreloadAsset;
		}

		private void OnAssetLoadFailure(object cookie)
		{
			this.OnAssetLoadSuccess(null, cookie);
		}

		private void OnAssetLoadSuccess(object asset, object cookie)
		{
			if (asset != null)
			{
				GameObject gameObject = asset as GameObject;
				gameObject.SetActive(false);
				WorldPreloadAsset worldPreloadAsset = (WorldPreloadAsset)cookie;
				worldPreloadAsset.GameObj = gameObject;
			}
			if (--this.numAssetsRemainingToLoad == 0)
			{
				this.onPreloadSuccess();
			}
		}
	}
}
