using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public abstract class AbstractMessage : IMessage, ISerializable
	{
		public abstract EventId MessageEventId
		{
			get;
		}

		public abstract object MessageCookie
		{
			get;
		}

		public abstract ISerializable FromObject(object obj);

		public string ToJson()
		{
			return string.Empty;
		}
	}
}
