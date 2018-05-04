using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public abstract class AbstractTimedEvent : ISerializable
	{
		public string Uid
		{
			get;
			set;
		}

		public bool Collected
		{
			get;
			set;
		}

		public string ToJson()
		{
			return "{}";
		}

		public virtual ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Uid = dictionary["uid"].ToString();
			if (dictionary.ContainsKey("collected"))
			{
				this.Collected = (bool)dictionary["collected"];
			}
			return this;
		}
	}
}
