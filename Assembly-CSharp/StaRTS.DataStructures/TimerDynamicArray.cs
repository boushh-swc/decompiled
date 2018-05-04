using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.DataStructures
{
	public struct TimerDynamicArray
	{
		public Timer[] Array;

		public int Length;

		public int Capacity;

		public TimerDynamicArray(int initialCapacity)
		{
			this.Array = new Timer[initialCapacity];
			this.Length = 0;
			this.Capacity = initialCapacity;
		}

		public void Add(Timer element)
		{
			Timer[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				Timer[] array2 = new Timer[num];
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

		public void Insert(int index, Timer element)
		{
			Timer[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				Timer[] array2 = new Timer[num];
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
				Timer timer = array[index];
				array[index] = element;
				for (int j = index + 1; j < length; j++)
				{
					Timer timer2 = array[j];
					array[j] = timer;
					timer = timer2;
				}
				array[length] = timer;
				this.Length++;
			}
		}

		public void RemoveAt(int index)
		{
			Timer[] array = this.Array;
			int num = this.Length;
			for (int i = index + 1; i < num; i++)
			{
				array[i - 1] = array[i];
			}
			num--;
			array[num] = null;
			this.Length = num;
		}

		public void RemoveRange(int startIndex, int count)
		{
			if (count == 0)
			{
				return;
			}
			Timer[] array = this.Array;
			int length = this.Length;
			for (int i = startIndex + count; i < length; i++)
			{
				array[i - count] = array[i];
			}
			for (int j = length - count; j < length; j++)
			{
				array[j] = null;
			}
			this.Length = length - count;
		}

		public void Clear()
		{
			Timer[] array = this.Array;
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				array[i] = null;
			}
			this.Length = 0;
		}

		public int IndexOf(Timer element)
		{
			Timer[] array = this.Array;
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
