using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public interface IMessage : ISerializable
	{
		EventId MessageEventId
		{
			get;
		}

		object MessageCookie
		{
			get;
		}
	}
}
