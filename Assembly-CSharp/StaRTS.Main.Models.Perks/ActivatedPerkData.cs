using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Perks
{
	public class ActivatedPerkData : ISerializable
	{
		public string PerkId
		{
			get;
			set;
		}

		public uint StartTime
		{
			get;
			set;
		}

		public uint EndTime
		{
			get;
			set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("perkId"))
			{
				this.PerkId = (dictionary["perkId"] as string);
			}
			if (dictionary.ContainsKey("startTime"))
			{
				this.StartTime = Convert.ToUInt32(dictionary["startTime"]);
			}
			if (dictionary.ContainsKey("endTime"))
			{
				this.EndTime = Convert.ToUInt32(dictionary["endTime"]);
			}
			return this;
		}

		public string ToJson()
		{
			Service.Logger.Warn("Attempting to inappropriately serialize ActivatedPerkData");
			return string.Empty;
		}
	}
}
