using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class LevelMap : ISerializable
	{
		private Dictionary<string, int> levels;

		public IDictionary<string, int> Levels
		{
			get
			{
				return this.levels;
			}
		}

		public LevelMap()
		{
			this.levels = new Dictionary<string, int>();
		}

		public bool Has(IUpgradeableVO vo)
		{
			return this.Has(vo.UpgradeGroup);
		}

		public bool Has(string groupId)
		{
			return this.levels.ContainsKey(groupId);
		}

		public void SetLevel(IUpgradeableVO upgradeable)
		{
			this.SetLevel(upgradeable.UpgradeGroup, upgradeable.Lvl);
		}

		public void SetLevel(string groupName, int level)
		{
			if (this.levels.ContainsKey(groupName))
			{
				this.levels[groupName] = level;
			}
			else
			{
				this.levels.Add(groupName, level);
			}
		}

		public int GetLevel(string groupName)
		{
			if (!this.levels.ContainsKey(groupName))
			{
				return 1;
			}
			return this.levels[groupName];
		}

		public int GetNextLevel(string groupName)
		{
			if (!this.levels.ContainsKey(groupName))
			{
				return 2;
			}
			return this.levels[groupName] + 1;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			return serializer.End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					this.SetLevel(current.Key, Convert.ToInt32(current.Value));
				}
			}
			return this;
		}
	}
}
