using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Perks
{
	public class PerksInfo : ISerializable
	{
		public PerksData Perks
		{
			get;
			private set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("perks"))
			{
				this.UpdatePerksData(dictionary["perks"]);
			}
			return this;
		}

		public void UpdatePerksData(object obj)
		{
			if (this.Perks != null)
			{
				this.Perks = null;
			}
			if (obj != null)
			{
				this.Perks = new PerksData();
				this.Perks.FromObject(obj);
			}
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize PerksInfo");
			return string.Empty;
		}
	}
}
