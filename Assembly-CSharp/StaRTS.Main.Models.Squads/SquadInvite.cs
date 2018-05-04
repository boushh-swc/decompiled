using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads
{
	public class SquadInvite : ISerializable
	{
		public string SquadId
		{
			get;
			set;
		}

		public string SenderId
		{
			get;
			set;
		}

		public string SenderName
		{
			get;
			set;
		}

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
			if (dictionary.ContainsKey("guildId"))
			{
				this.SquadId = (dictionary["guildId"] as string);
			}
			if (dictionary.ContainsKey("senderId"))
			{
				this.SenderId = (dictionary["senderId"] as string);
			}
			if (dictionary.ContainsKey("senderName"))
			{
				this.SenderName = (dictionary["senderName"] as string);
			}
			return this;
		}
	}
}
