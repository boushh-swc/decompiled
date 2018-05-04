using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Perks
{
	public class PerksData : ISerializable
	{
		public List<ActivatedPerkData> ActivatedPerks
		{
			get;
			private set;
		}

		public Dictionary<string, uint> Cooldowns
		{
			get;
			private set;
		}

		public bool HasActivatedFirstPerk
		{
			get;
			private set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("activatedPerks"))
			{
				List<object> list = dictionary["activatedPerks"] as List<object>;
				this.ActivatedPerks = new List<ActivatedPerkData>();
				int i = 0;
				int count = list.Count;
				while (i < count)
				{
					ActivatedPerkData activatedPerkData = new ActivatedPerkData();
					activatedPerkData.FromObject(list[i]);
					this.ActivatedPerks.Add(activatedPerkData);
					i++;
				}
			}
			if (dictionary.ContainsKey("cooldowns"))
			{
				this.Cooldowns = new Dictionary<string, uint>();
				Dictionary<string, object> dictionary2 = dictionary["cooldowns"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						this.Cooldowns.Add(current.Key, Convert.ToUInt32(current.Value));
					}
				}
			}
			if (dictionary.ContainsKey("hasActivatedFirstPerk"))
			{
				this.HasActivatedFirstPerk = (bool)dictionary["hasActivatedFirstPerk"];
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize PerksData");
			return string.Empty;
		}
	}
}
