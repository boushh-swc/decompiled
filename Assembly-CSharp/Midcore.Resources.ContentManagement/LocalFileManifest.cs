using System;

namespace Midcore.Resources.ContentManagement
{
	public class LocalFileManifest : IFileManifest
	{
		private const string URL_FORMAT = "{0}/{1}";

		private ContentManagerOptions options;

		public bool Prepare(ContentManagerOptions options, string input)
		{
			this.options = options;
			return true;
		}

		public string GetFileUrl(string relativePath)
		{
			return string.Format("{0}/{1}", this.options.ContentBaseUrl, relativePath);
		}

		public int GetFileVersion(string relativePath)
		{
			return 0;
		}
	}
}
