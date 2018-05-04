using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads.War
{
	public class SquadWarBuffBaseData : ISerializable
	{
		public string BuffBaseId;

		public string OwnerId;

		public int BaseLevel;

		public uint AttackExpirationDate;

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("buffUid"))
			{
				this.BuffBaseId = Convert.ToString(dictionary["buffUid"]);
			}
			if (dictionary.ContainsKey("level"))
			{
				this.BaseLevel = Convert.ToInt32(dictionary["level"]);
			}
			if (dictionary.ContainsKey("currentlyDefending"))
			{
				Dictionary<string, object> dictionary2 = dictionary["currentlyDefending"] as Dictionary<string, object>;
				if (dictionary2 != null && dictionary2.ContainsKey("expiration"))
				{
					this.AttackExpirationDate = Convert.ToUInt32(dictionary2["expiration"]);
				}
			}
			if (dictionary.ContainsKey("ownerId"))
			{
				this.OwnerId = Convert.ToString(dictionary["ownerId"]);
			}
			return this;
		}

		public int GetDisplayBaseLevel()
		{
			int[] wAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING = GameConstants.WAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING;
			int num = wAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING.Length;
			if (this.BaseLevel >= num)
			{
				return wAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING[num - 1];
			}
			if (this.BaseLevel < 0)
			{
				return wAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING[0];
			}
			return wAR_BUFF_BASE_HQ_TO_LEVEL_MAPPING[this.BaseLevel];
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}
	}
}
