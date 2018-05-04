using StaRTS.Main.Utils.Events;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public delegate bool VisualReadyDelegate(EventId id, object cookie, SmartEntity target);
}
