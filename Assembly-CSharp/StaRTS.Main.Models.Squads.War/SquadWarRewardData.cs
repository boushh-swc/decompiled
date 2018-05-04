using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads.War
{
	public class SquadWarRewardData : ISerializable
	{
		public string WarId
		{
			get;
			private set;
		}

		public string CrateId
		{
			get;
			private set;
		}

		public uint ExpireDate
		{
			get;
			private set;
		}

		public int RewardHqLevel
		{
			get;
			private set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("warId"))
			{
				this.WarId = Convert.ToString(dictionary["warId"]);
			}
			if (dictionary.ContainsKey("crateTier"))
			{
				this.CrateId = Convert.ToString(dictionary["crateTier"]);
			}
			else if (dictionary.ContainsKey("crateId"))
			{
				this.CrateId = Convert.ToString(dictionary["crateId"]);
			}
			if (dictionary.ContainsKey("expiry"))
			{
				this.ExpireDate = Convert.ToUInt32(dictionary["expiry"]);
			}
			if (dictionary.ContainsKey("hqLevel"))
			{
				this.RewardHqLevel = Convert.ToInt32(dictionary["hqLevel"]);
			}
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}
	}
}
