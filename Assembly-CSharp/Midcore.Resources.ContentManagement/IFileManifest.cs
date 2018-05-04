using System;

namespace Midcore.Resources.ContentManagement
{
	public interface IFileManifest
	{
		bool Prepare(ContentManagerOptions options, string input);

		string GetFileUrl(string relativePath);

		int GetFileVersion(string relativePath);
	}
}
