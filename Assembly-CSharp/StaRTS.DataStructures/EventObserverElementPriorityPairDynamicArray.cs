using System;

namespace StaRTS.DataStructures
{
	public struct EventObserverElementPriorityPairDynamicArray
	{
		public EventObserverElementPriorityPair[] Array;

		public int Length;

		public int Capacity;

		public EventObserverElementPriorityPairDynamicArray(int initialCapacity)
		{
			this.Array = new EventObserverElementPriorityPair[initialCapacity];
			this.Length = 0;
			this.Capacity = initialCapacity;
		}

		public void Add(EventObserverElementPriorityPair element)
		{
			EventObserverElementPriorityPair[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				EventObserverElementPriorityPair[] array2 = new EventObserverElementPriorityPair[num];
				for (int i = 0; i < length; i++)
				{
					array2[i] = array[i];
				}
				this.Array = array2;
				this.Capacity = num;
				array = array2;
			}
			array[length] = element;
			this.Length++;
		}

		public void Insert(int index, EventObserverElementPriorityPair element)
		{
			EventObserverElementPriorityPair[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				EventObserverElementPriorityPair[] array2 = new EventObserverElementPriorityPair[num];
				for (int i = 0; i < length; i++)
				{
					array2[i] = array[i];
				}
				this.Array = array2;
				this.Capacity = num;
				array = array2;
			}
			if (index == length)
			{
				array[length] = element;
				this.Length++;
			}
			else
			{
				EventObserverElementPriorityPair eventObserverElementPriorityPair = array[index];
				array[index] = element;
				for (int j = index + 1; j < length; j++)
				{
					EventObserverElementPriorityPair eventObserverElementPriorityPair2 = array[j];
					array[j] = eventObserverElementPriorityPair;
					eventObserverElementPriorityPair = eventObserverElementPriorityPair2;
				}
				array[length] = eventObserverElementPriorityPair;
				this.Length++;
			}
		}

		public void RemoveAt(int index)
		{
			EventObserverElementPriorityPair[] array = this.Array;
			int num = this.Length;
			for (int i = index + 1; i < num; i++)
			{
				array[i - 1] = array[i];
			}
			num--;
			array[num] = default(EventObserverElementPriorityPair);
			this.Length = num;
		}

		public void RemoveRange(int startIndex, int count)
		{
			if (count == 0)
			{
				return;
			}
			EventObserverElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = startIndex + count; i < length; i++)
			{
				array[i - count] = array[i];
			}
			for (int j = length - count; j < length; j++)
			{
				array[j] = default(EventObserverElementPriorityPair);
			}
			this.Length = length - count;
		}

		public void Clear()
		{
			EventObserverElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				array[i] = default(EventObserverElementPriorityPair);
			}
			this.Length = 0;
		}

		public int IndexOf(EventObserverElementPriorityPair element)
		{
			EventObserverElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				if (array[i].Equals(element))
				{
					return i;
				}
			}
			return -1;
		}
	}
}
