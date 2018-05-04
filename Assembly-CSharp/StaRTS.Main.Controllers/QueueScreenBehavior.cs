using System;

namespace StaRTS.Main.Controllers
{
	public enum QueueScreenBehavior
	{
		Default = 0,
		Queue = 1,
		DeferTillClosed = 2,
		QueueAndDeferTillClosed = 3
	}
}
