using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class GetSquadChatKeyResponse : AbstractResponse
	{
		public string ChatMessageEncryptionKey
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("chatMessageEncryptionKey"))
			{
				this.ChatMessageEncryptionKey = Convert.ToString(dictionary["chatMessageEncryptionKey"]);
			}
			return this;
		}
	}
}
