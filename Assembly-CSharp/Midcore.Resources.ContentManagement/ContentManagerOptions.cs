using System;

namespace Midcore.Resources.ContentManagement
{
	public class ContentManagerOptions
	{
		public ContentManagerMode Mode
		{
			get;
			set;
		}

		public int ManifestVersion
		{
			get;
			set;
		}

		public string ManifestPath
		{
			get;
			set;
		}

		public string ContentBaseUrl
		{
			get;
			set;
		}

		public string AccessToken
		{
			get;
			set;
		}

		public string Env
		{
			get;
			set;
		}
	}
}
