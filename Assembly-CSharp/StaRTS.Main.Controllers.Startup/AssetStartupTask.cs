using StaRTS.Assets;
using StaRTS.Main.Models;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Projectors;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers.Startup
{
	public class AssetStartupTask : StartupTask
	{
		public AssetStartupTask(float startPercentage) : base(startPercentage)
		{
		}

		public override void Start()
		{
			Service.EventManager.SendEvent(EventId.AssetLoadStart, null);
			if (!GameConstants.ASSET_BUNDLE_CACHE_CLEAN_DISABLED)
			{
				int @int = PlayerPrefs.GetInt("cacheCleanVersion", 0);
				if (@int < GameConstants.ASSET_BUNDLE_CACHE_CLEAN_VERSION)
				{
					bool isSuccess = Caching.ClearCache();
					Service.BILoggingController.TrackAssetBundleCacheClean(GameConstants.ASSET_BUNDLE_CACHE_CLEAN_VERSION, isSuccess);
					PlayerPrefs.SetInt("cacheCleanVersion", GameConstants.ASSET_BUNDLE_CACHE_CLEAN_VERSION);
				}
			}
			new ProjectorManager();
			AssetManager assetManager = Service.AssetManager;
			bool fueInProgress = Service.CurrentPlayer.CampaignProgress.FueInProgress;
			assetManager.SetupManifest(fueInProgress);
			assetManager.LoadGameShaders(new AssetsCompleteDelegate(this.OnShadersComplete), null);
		}

		private void OnShadersComplete(object cookie)
		{
			Service.EventManager.SendEvent(EventId.AssetLoadEnd, null);
			base.Complete();
		}
	}
}
