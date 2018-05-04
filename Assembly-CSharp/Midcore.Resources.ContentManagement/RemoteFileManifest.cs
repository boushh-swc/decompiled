using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using System;
using System.Collections.Generic;

namespace Midcore.Resources.ContentManagement
{
	public class RemoteFileManifest : IFileManifest
	{
		private const string URL_FORMAT = "{0}{1}/{2}{3}";

		private const string TOKEN_PARAM = "?access_token={0}";

		private const string PATHS_KEY = "\"paths\":{";

		private ContentManagerOptions options;

		private Dictionary<string, string[]> files;

		private Logger logger;

		public bool Prepare(ContentManagerOptions options, string input)
		{
			this.options = options;
			this.files = new Dictionary<string, string[]>();
			this.logger = Service.Logger;
			return this.ParseHashes(input);
		}

		private bool ParseHashes(string input)
		{
			if (string.IsNullOrEmpty(input))
			{
				this.logger.Error("Could not build manifest from empty json");
				return false;
			}
			int num = input.IndexOf("\"paths\":{");
			if (num < 0)
			{
				this.logger.Error("Could not build manifest from invalid json");
				return false;
			}
			num += "\"paths\":{".Length;
			int num2 = 0;
			int num3 = -1;
			string key = null;
			string text = null;
			int i = num;
			int length = input.Length;
			while (i < length)
			{
				char c = input[i];
				if (c == '"')
				{
					switch (num2)
					{
					case 0:
					case 4:
						num3 = i + 1;
						num2++;
						break;
					case 1:
						key = input.Substring(num3, i - num3);
						num2++;
						break;
					case 2:
					case 3:
					case 6:
						num2++;
						break;
					case 5:
						text = input.Substring(num3, i - num3);
						num2++;
						break;
					case 7:
						num3 = i + 2;
						break;
					}
				}
				else if (c == '}')
				{
					if (num2 != 7)
					{
						return true;
					}
					string text2 = input.Substring(num3, i - num3);
					this.files.Add(key, new string[]
					{
						text,
						text2
					});
					num2 = 0;
				}
				i++;
			}
			return this.files.Count > 0;
		}

		private bool FilesContainsKey(ref string key)
		{
			if (this.files.ContainsKey(key))
			{
				return true;
			}
			key = key.ToLower();
			return this.files.ContainsKey(key);
		}

		public string GetFileUrl(string relativePath)
		{
			if (!this.FilesContainsKey(ref relativePath))
			{
				this.logger.Error(this.FormatErrorMessage(relativePath));
				return string.Empty;
			}
			string text = string.IsNullOrEmpty(this.options.AccessToken) ? string.Empty : string.Format("?access_token={0}", this.options.AccessToken);
			return string.Format("{0}{1}/{2}{3}", new object[]
			{
				this.options.ContentBaseUrl,
				this.files[relativePath][1],
				relativePath,
				text
			});
		}

		public string GetFileCrc(string relativePath)
		{
			if (!this.FilesContainsKey(ref relativePath))
			{
				this.logger.Error(this.FormatErrorMessage(relativePath));
				return string.Empty;
			}
			return this.files[relativePath][0];
		}

		public int GetFileVersion(string relativePath)
		{
			if (!this.FilesContainsKey(ref relativePath))
			{
				this.logger.Error(this.FormatErrorMessage(relativePath));
				return 0;
			}
			return Convert.ToInt32(this.files[relativePath][1]);
		}

		private string FormatErrorMessage(string relativePath)
		{
			return string.Format("Unable to find {0} in the file manifest.", relativePath);
		}
	}
}
