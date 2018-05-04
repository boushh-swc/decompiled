using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Episodes
{
	public class EpisodeTaskProgressInfo : ISerializable
	{
		public string uid;

		public int hq;

		public string actionUID;

		public string type;

		public int count;

		public int target;

		public bool completed;

		public uint endTimestamp;

		public ISerializable FromObject(object obj)
		{
			if (obj == null)
			{
				return this;
			}
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("uid"))
			{
				this.uid = (string)dictionary["uid"];
			}
			if (dictionary.ContainsKey("hq"))
			{
				this.hq = Convert.ToInt32(dictionary["hq"]);
			}
			if (dictionary.ContainsKey("actionUID"))
			{
				this.actionUID = (string)dictionary["actionUID"];
			}
			if (dictionary.ContainsKey("type"))
			{
				this.type = (string)dictionary["type"];
			}
			if (dictionary.ContainsKey("count"))
			{
				this.count = Convert.ToInt32(dictionary["count"]);
			}
			if (dictionary.ContainsKey("target"))
			{
				this.target = Convert.ToInt32(dictionary["target"]);
			}
			if (dictionary.ContainsKey("completed"))
			{
				this.completed = (bool)dictionary["completed"];
			}
			if (dictionary.ContainsKey("endTime"))
			{
				this.endTimestamp = Convert.ToUInt32(dictionary["endTime"]);
			}
			else
			{
				this.endTimestamp = 0u;
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize EpisodeTaskProgressInfo");
			return string.Empty;
		}
	}
}
