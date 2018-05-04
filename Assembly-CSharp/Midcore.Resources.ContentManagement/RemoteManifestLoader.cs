using Midcore.Web;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace Midcore.Resources.ContentManagement
{
	public class RemoteManifestLoader : IManifestLoader
	{
		private const int MAX_LOAD_ATTEMPTS = 3;

		private const double LOAD_ATTEMPT_INTERVAL = 0.2;

		private const double CAP_LOAD_ATTEMPT_INTERVAL = 10.0;

		private const string STATIC_MANIFEST_PATH = "{0}{1}{2}";

		private const string TOKEN_PARAM = "?access_token={0}";

		private ContentManagerOptions options;

		private ManifestLoadDelegate onSuccess;

		private ManifestLoadDelegate onFailure;

		private WebManager webManager;

		private int loadRetryCounter;

		private string manifestUrl;

		public RemoteManifestLoader(WebManager webManager)
		{
			this.webManager = webManager;
			this.loadRetryCounter = 0;
		}

		public void Load(ContentManagerOptions options, ManifestLoadDelegate onSuccess, ManifestLoadDelegate onFailure)
		{
			this.options = options;
			this.onSuccess = onSuccess;
			this.onFailure = onFailure;
			string arg = string.IsNullOrEmpty(options.AccessToken) ? string.Empty : string.Format("?access_token={0}", options.AccessToken);
			this.manifestUrl = string.Format("{0}{1}{2}", options.ContentBaseUrl, options.ManifestPath, arg);
			this.AttemptManifestRequest(0u, null);
		}

		private void AttemptManifestRequest(uint id, object cookie)
		{
			if (++this.loadRetryCounter > 3)
			{
				IFileManifest manifest = new RemoteFileManifest();
				this.onFailure(manifest);
			}
			else
			{
				this.webManager.Fetch(this.manifestUrl, WebAssetType.StandaloneText, 0, new OnFetchDelegate(this.OnManifestFetched), null);
			}
		}

		private void RetryRequest()
		{
			double delayJitter = GameUtils.GetDelayJitter(10.0, 0.2, this.loadRetryCounter - 1);
			Service.ViewTimerManager.CreateViewTimer((float)delayJitter, false, new TimerDelegate(this.AttemptManifestRequest), null);
		}

		private void OnManifestFetched(object asset, string error, object cookie)
		{
			IFileManifest fileManifest = new RemoteFileManifest();
			if (!string.IsNullOrEmpty(error))
			{
				Service.Logger.ErrorFormat("Unable to request manifest file [{0}] on attempt #{1} with the following error: {2}", new object[]
				{
					this.manifestUrl,
					this.loadRetryCounter,
					error
				});
				this.RetryRequest();
				return;
			}
			if (object.ReferenceEquals(asset, null))
			{
				Service.Logger.ErrorFormat("Manifest file request [{0}] attempt #{1} yielded an empty manifest.", new object[]
				{
					this.manifestUrl,
					this.loadRetryCounter
				});
				this.RetryRequest();
				return;
			}
			string input = (string)asset;
			if (!fileManifest.Prepare(this.options, input))
			{
				this.onFailure(fileManifest);
				return;
			}
			this.onSuccess(fileManifest);
		}
	}
}
