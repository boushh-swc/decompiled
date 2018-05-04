using SwrveUnityMiniJSON;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SwrveUnity.Helpers
{
	public class UnityWwwHelper
	{
		public static WwwDeducedError DeduceWwwError(WWW request)
		{
			if (request.responseHeaders.Count > 0)
			{
				string text = null;
				Dictionary<string, string>.Enumerator enumerator = request.responseHeaders.GetEnumerator();
				while (enumerator.MoveNext())
				{
					KeyValuePair<string, string> current = enumerator.Current;
					string key = current.Key;
					if (string.Equals(key, "X-Swrve-Error", StringComparison.OrdinalIgnoreCase))
					{
						request.responseHeaders.TryGetValue(key, out text);
						break;
					}
				}
				if (text != null)
				{
					SwrveLog.LogError("Request response headers [\"X-Swrve-Error\"]: " + text + " at " + request.url);
					try
					{
						if (!string.IsNullOrEmpty(request.text))
						{
							SwrveLog.LogError("Request response headers [\"X-Swrve-Error\"]: " + ((IDictionary<string, object>)Json.Deserialize(request.text))["message"]);
						}
					}
					catch (Exception ex)
					{
						SwrveLog.LogError(ex.Message);
					}
					return WwwDeducedError.ApplicationErrorHeader;
				}
			}
			if (!string.IsNullOrEmpty(request.error))
			{
				SwrveLog.LogError("Request error: " + request.error + " in " + request.url);
				return WwwDeducedError.NetworkError;
			}
			return WwwDeducedError.NoError;
		}
	}
}
