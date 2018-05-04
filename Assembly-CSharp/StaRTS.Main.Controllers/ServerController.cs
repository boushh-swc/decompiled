using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class ServerController : IEventObserver
	{
		public ServerController()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ApplicationPauseToggled, EventPriority.ServerAfterOthers);
			eventManager.RegisterObserver(this, EventId.ApplicationQuit, EventPriority.ServerAfterOthers);
			Service.ServerController = this;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ApplicationPauseToggled)
			{
				if (id == EventId.ApplicationQuit)
				{
					Service.ServerAPI.Sync();
				}
			}
			else
			{
				bool flag = (bool)cookie;
				if (flag)
				{
					Service.ServerAPI.Sync();
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
