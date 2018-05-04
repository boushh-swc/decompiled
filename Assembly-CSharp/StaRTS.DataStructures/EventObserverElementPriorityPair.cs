using StaRTS.Main.Utils.Events;
using System;

namespace StaRTS.DataStructures
{
	public struct EventObserverElementPriorityPair
	{
		public IEventObserver Element;

		public int Priority;

		public EventObserverElementPriorityPair(IEventObserver element, int priority)
		{
			this.Element = element;
			this.Priority = priority;
		}

		public bool Equals(EventObserverElementPriorityPair other)
		{
			return this.Element == other.Element && this.Priority == other.Priority;
		}
	}
}
