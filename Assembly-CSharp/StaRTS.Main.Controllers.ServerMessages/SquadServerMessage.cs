using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public class SquadServerMessage : AbstractMessage
	{
		public List<object> Messages
		{
			get;
			private set;
		}

		public override object MessageCookie
		{
			get
			{
				return this;
			}
		}

		public override EventId MessageEventId
		{
			get
			{
				return EventId.SquadServerMessage;
			}
		}

		public override ISerializable FromObject(object obj)
		{
			List<object> list = obj as List<object>;
			if (list == null)
			{
				return this;
			}
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
				if (dictionary != null && dictionary.ContainsKey("message"))
				{
					if (this.Messages == null)
					{
						this.Messages = new List<object>();
					}
					this.Messages.Add(dictionary["message"]);
				}
				i++;
			}
			return this;
		}
	}
}
