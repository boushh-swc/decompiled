using StaRTS.Main.Utils.Events;
using System;

namespace StaRTS.DataStructures
{
	public class EventObserverPriorityList
	{
		private EventObserverElementPriorityPairDynamicArray list;

		public int Count
		{
			get
			{
				return this.list.Length;
			}
		}

		public EventObserverPriorityList()
		{
			this.list = new EventObserverElementPriorityPairDynamicArray(4);
		}

		public virtual int Add(IEventObserver element, int priority)
		{
			if (element == null)
			{
				return -1;
			}
			int i = 0;
			int length = this.list.Length;
			while (i < length)
			{
				EventObserverElementPriorityPair eventObserverElementPriorityPair = this.list.Array[i];
				if (eventObserverElementPriorityPair.Element == element)
				{
					return -1;
				}
				if (priority > eventObserverElementPriorityPair.Priority)
				{
					this.list.Insert(i, new EventObserverElementPriorityPair(element, priority));
					return i;
				}
				i++;
			}
			this.list.Add(new EventObserverElementPriorityPair(element, priority));
			return this.list.Length - 1;
		}

		public EventObserverElementPriorityPair Get(int i)
		{
			return this.list.Array[i];
		}

		public IEventObserver GetElement(int i)
		{
			return this.list.Array[i].Element;
		}

		public int GetPriority(int i)
		{
			return this.list.Array[i].Priority;
		}

		public void GetElementPriority(int i, out IEventObserver element, out int priority)
		{
			EventObserverElementPriorityPair eventObserverElementPriorityPair = this.list.Array[i];
			element = eventObserverElementPriorityPair.Element;
			priority = eventObserverElementPriorityPair.Priority;
		}

		public int IndexOf(IEventObserver element)
		{
			int i = 0;
			int length = this.list.Length;
			while (i < length)
			{
				if (this.list.Array[i].Element == element)
				{
					return i;
				}
				i++;
			}
			return -1;
		}

		public void RemoveAt(int i)
		{
			this.list.RemoveAt(i);
		}
	}
}
