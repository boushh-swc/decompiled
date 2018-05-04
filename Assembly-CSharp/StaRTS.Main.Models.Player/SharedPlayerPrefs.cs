using StaRTS.Main.Models.Commands;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player
{
	public class SharedPlayerPrefs
	{
		private const int MAX_SERIALIZED_VALUE_LENGTH = 64;

		private Dictionary<string, string> localCache;

		public SharedPlayerPrefs()
		{
			Service.SharedPlayerPrefs = this;
			this.localCache = new Dictionary<string, string>();
		}

		public void Populate(Dictionary<string, object> map)
		{
			if (map != null)
			{
				foreach (string current in map.Keys)
				{
					this.localCache.Add(current, map[current] as string);
				}
			}
		}

		public T GetPref<T>(string prefName)
		{
			if (!string.IsNullOrEmpty(prefName) && this.localCache.ContainsKey(prefName))
			{
				return (T)((object)Convert.ChangeType(this.localCache[prefName], typeof(T)));
			}
			return default(T);
		}

		public void SetPref(string prefName, string value)
		{
			if (value != null && value.Length > 64)
			{
				Service.Logger.ErrorFormat("Value not saved.  SharedPref value is too large.\r\nPref:{0} Value:{1}\r\nSerialized length ({2}) is greater than the max value ({3}).", new object[]
				{
					prefName,
					value,
					value.Length,
					64
				});
				return;
			}
			this.SetPrefInternal(prefName, value);
		}

		public void SetPrefUnlimitedLength(string prefName, string value)
		{
			this.SetPrefInternal(prefName, value);
		}

		private void SetPrefInternal(string prefName, string value)
		{
			bool flag = false;
			if (value != null)
			{
				string a;
				if (this.localCache.TryGetValue(prefName, out a))
				{
					if (a != value)
					{
						this.localCache[prefName] = value;
						flag = true;
					}
				}
				else
				{
					this.localCache.Add(prefName, value);
					flag = true;
				}
			}
			else if (this.localCache.Remove(prefName))
			{
				flag = true;
			}
			if (flag)
			{
				Service.ServerAPI.Enqueue(new SaveSharedPrefCommand(prefName, value));
			}
		}
	}
}
