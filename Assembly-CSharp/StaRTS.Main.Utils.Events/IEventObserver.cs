using StaRTS.Utils;
using System;

namespace StaRTS.Main.Utils.Events
{
	public interface IEventObserver
	{
		EatResponse OnEvent(EventId id, object cookie);
	}
}
