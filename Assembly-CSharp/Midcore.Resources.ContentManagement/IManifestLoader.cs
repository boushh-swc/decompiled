using System;

namespace Midcore.Resources.ContentManagement
{
	public interface IManifestLoader
	{
		void Load(ContentManagerOptions options, ManifestLoadDelegate onSuccess, ManifestLoadDelegate onFailure);
	}
}
