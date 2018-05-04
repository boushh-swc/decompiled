using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Episodes
{
	public class EpisodeGrindProgressInfo : ISerializable
	{
		public uint LastStartTime
		{
			get;
			private set;
		}

		public int Started
		{
			get;
			private set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("started"))
			{
				this.Started = Convert.ToInt32(dictionary["started"]);
			}
			if (dictionary.ContainsKey("lastStartTime"))
			{
				this.LastStartTime = Convert.ToUInt32(dictionary["lastStartTime"]);
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize EpisodeGrindProgressInfo");
			return string.Empty;
		}
	}
}
