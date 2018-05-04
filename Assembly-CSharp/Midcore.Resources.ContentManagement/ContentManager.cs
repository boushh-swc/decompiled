using Midcore.Web;
using StaRTS.Utils.Core;
using System;

namespace Midcore.Resources.ContentManagement
{
	public class ContentManager
	{
		private ManagerLoadDelegate onSuccess;

		private ManagerLoadDelegate onFailure;

		private IFileManifest fileManifest;

		private int manifestVersion;

		private string environment;

		public ContentManagerOptions Options
		{
			get;
			private set;
		}

		public ContentManager(ContentManagerOptions options)
		{
			this.Options = options;
			this.manifestVersion = options.ManifestVersion;
			this.environment = options.Env;
			Service.ContentManager = this;
		}

		public void Load(ManagerLoadDelegate onSuccess, ManagerLoadDelegate onFailure, WebManager webManager)
		{
			this.onSuccess = onSuccess;
			this.onFailure = onFailure;
			IManifestLoader manifestLoader = null;
			ContentManagerMode mode = this.Options.Mode;
			if (mode != ContentManagerMode.Local)
			{
				if (mode == ContentManagerMode.Remote)
				{
					manifestLoader = new RemoteManifestLoader(webManager);
				}
			}
			else
			{
				manifestLoader = new LocalManifestLoader();
			}
			manifestLoader.Load(this.Options, new ManifestLoadDelegate(this.OnManifestSuccess), new ManifestLoadDelegate(this.OnManifestFailure));
		}

		private void OnManifestSuccess(IFileManifest fileManifest)
		{
			this.fileManifest = fileManifest;
			if (this.onSuccess != null)
			{
				this.onSuccess(this);
			}
		}

		private void OnManifestFailure(IFileManifest fileManifest)
		{
			this.fileManifest = fileManifest;
			if (this.onFailure != null)
			{
				this.onFailure(this);
			}
		}

		public string GetFileUrl(string relativePath)
		{
			return this.fileManifest.GetFileUrl(relativePath);
		}

		public int GetFileVersion(string relativePath)
		{
			return this.fileManifest.GetFileVersion(relativePath);
		}

		public int GetManifestVersion()
		{
			return this.manifestVersion;
		}

		public string GetManifestEnvironment()
		{
			return this.environment;
		}

		public void SetAccessToken(string accessToken)
		{
			this.Options.AccessToken = accessToken;
		}
	}
}
