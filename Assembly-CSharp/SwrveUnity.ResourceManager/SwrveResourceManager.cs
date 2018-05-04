using System;
using System.Collections.Generic;

namespace SwrveUnity.ResourceManager
{
	public class SwrveResourceManager
	{
		public Dictionary<string, SwrveResource> UserResources;

		public void SetResourcesFromJSON(Dictionary<string, Dictionary<string, string>> userResourcesJson)
		{
			Dictionary<string, SwrveResource> dictionary = new Dictionary<string, SwrveResource>();
			Dictionary<string, Dictionary<string, string>>.Enumerator enumerator = userResourcesJson.GetEnumerator();
			while (enumerator.MoveNext())
			{
				KeyValuePair<string, Dictionary<string, string>> current = enumerator.Current;
				dictionary[current.Key] = new SwrveResource(current.Value);
			}
			this.UserResources = dictionary;
		}

		public SwrveResource GetResource(string resourceId)
		{
			if (this.UserResources != null)
			{
				if (this.UserResources.ContainsKey(resourceId))
				{
					return this.UserResources[resourceId];
				}
			}
			else
			{
				SwrveLog.LogWarning(string.Format("SwrveResourceManager::GetResource('{0}'): Resources are not available yet.", resourceId));
			}
			return null;
		}

		public T GetResourceAttribute<T>(string resourceId, string attributeName, T defaultValue)
		{
			SwrveResource resource = this.GetResource(resourceId);
			if (resource != null)
			{
				return resource.GetAttribute<T>(attributeName, defaultValue);
			}
			return defaultValue;
		}
	}
}
