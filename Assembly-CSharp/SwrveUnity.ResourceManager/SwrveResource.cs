using System;
using System.Collections.Generic;

namespace SwrveUnity.ResourceManager
{
	public class SwrveResource
	{
		public readonly Dictionary<string, string> Attributes;

		public SwrveResource(Dictionary<string, string> attributes)
		{
			this.Attributes = attributes;
		}

		public T GetAttribute<T>(string attributeName, T defaultValue)
		{
			if (this.Attributes.ContainsKey(attributeName))
			{
				string text = this.Attributes[attributeName];
				if (text != null)
				{
					return (T)((object)Convert.ChangeType(text, typeof(T)));
				}
			}
			return defaultValue;
		}
	}
}
