using StaRTS.DataStructures;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Utils.Events
{
	public class EventManager
	{
		private EventObservers[] eventIdToObservers;

		public EventManager()
		{
			Service.EventManager = this;
			int num = 532;
			this.eventIdToObservers = new EventObservers[num];
			for (int i = 0; i < num; i++)
			{
				this.eventIdToObservers[i] = null;
			}
		}

		public void RegisterObserver(IEventObserver observer, EventId id)
		{
			this.RegisterObserver(observer, id, EventPriority.Default);
		}

		public void RegisterObserver(IEventObserver observer, EventId id, EventPriority priority)
		{
			if (observer == null)
			{
				return;
			}
			EventObservers eventObservers = this.eventIdToObservers[(int)id];
			if (eventObservers == null)
			{
				eventObservers = new EventObservers();
				this.eventIdToObservers[(int)id] = eventObservers;
			}
			EventObserverPriorityList list = eventObservers.List;
			if (list.IndexOf(observer) < 0)
			{
				list.Add(observer, (int)priority);
			}
		}

		public void UnregisterObserver(IEventObserver observer, EventId id)
		{
			EventObservers eventObservers = this.eventIdToObservers[(int)id];
			if (eventObservers != null)
			{
				EventObserverPriorityList list = eventObservers.List;
				MutableIterator iter = eventObservers.Iter;
				int num = list.IndexOf(observer);
				if (num >= 0)
				{
					list.RemoveAt(num);
					iter.OnRemove(num);
					if (list.Count == 0)
					{
						this.eventIdToObservers[(int)id] = null;
					}
				}
			}
		}

		public bool IsEventListenerRegistered(IEventObserver observer, EventId id)
		{
			EventObservers eventObservers = this.eventIdToObservers[(int)id];
			if (eventObservers == null)
			{
				return false;
			}
			EventObserverPriorityList list = eventObservers.List;
			return list.IndexOf(observer) >= 0;
		}

		public void SendEvent(EventId id, object cookie)
		{
			EventObservers eventObservers = this.eventIdToObservers[(int)id];
			if (eventObservers != null)
			{
				EventObserverPriorityList list = eventObservers.List;
				MutableIterator iter = eventObservers.Iter;
				iter.Init(list.Count);
				while (iter.Active())
				{
					IEventObserver element = list.GetElement(iter.Index);
					if (element.OnEvent(id, cookie) == EatResponse.Eaten)
					{
						break;
					}
					iter.Next();
				}
				iter.Reset();
			}
		}
	}
}
