using System;

namespace Midcore.Resources.ContentManagement
{
	public class LocalManifestLoader : IManifestLoader
	{
		public void Load(ContentManagerOptions options, ManifestLoadDelegate onSuccess, ManifestLoadDelegate onFailure)
		{
			IFileManifest fileManifest = new LocalFileManifest();
			if (!fileManifest.Prepare(options, string.Empty))
			{
				onFailure(fileManifest);
				return;
			}
			onSuccess(fileManifest);
		}
	}
}
