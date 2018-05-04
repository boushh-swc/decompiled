using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class ArmoryInfo : ISerializable
	{
		public bool FirstCratePurchased;

		public string ToJson()
		{
			return "{}";
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("firstCratePurchased"))
			{
				this.FirstCratePurchased = Convert.ToBoolean(dictionary["firstCratePurchased"]);
			}
			return this;
		}
	}
}
