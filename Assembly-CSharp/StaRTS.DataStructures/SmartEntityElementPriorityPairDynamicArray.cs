using System;

namespace StaRTS.DataStructures
{
	public struct SmartEntityElementPriorityPairDynamicArray
	{
		public SmartEntityElementPriorityPair[] Array;

		public int Length;

		public int Capacity;

		public SmartEntityElementPriorityPairDynamicArray(int initialCapacity)
		{
			this.Array = new SmartEntityElementPriorityPair[initialCapacity];
			this.Length = 0;
			this.Capacity = initialCapacity;
		}

		public void Add(SmartEntityElementPriorityPair element)
		{
			SmartEntityElementPriorityPair[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				SmartEntityElementPriorityPair[] array2 = new SmartEntityElementPriorityPair[num];
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

		public void Insert(int index, SmartEntityElementPriorityPair element)
		{
			SmartEntityElementPriorityPair[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				SmartEntityElementPriorityPair[] array2 = new SmartEntityElementPriorityPair[num];
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
				SmartEntityElementPriorityPair smartEntityElementPriorityPair = array[index];
				array[index] = element;
				for (int j = index + 1; j < length; j++)
				{
					SmartEntityElementPriorityPair smartEntityElementPriorityPair2 = array[j];
					array[j] = smartEntityElementPriorityPair;
					smartEntityElementPriorityPair = smartEntityElementPriorityPair2;
				}
				array[length] = smartEntityElementPriorityPair;
				this.Length++;
			}
		}

		public void RemoveAt(int index)
		{
			SmartEntityElementPriorityPair[] array = this.Array;
			int num = this.Length;
			for (int i = index + 1; i < num; i++)
			{
				array[i - 1] = array[i];
			}
			num--;
			array[num] = default(SmartEntityElementPriorityPair);
			this.Length = num;
		}

		public void RemoveRange(int startIndex, int count)
		{
			if (count == 0)
			{
				return;
			}
			SmartEntityElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = startIndex + count; i < length; i++)
			{
				array[i - count] = array[i];
			}
			for (int j = length - count; j < length; j++)
			{
				array[j] = default(SmartEntityElementPriorityPair);
			}
			this.Length = length - count;
		}

		public void Clear()
		{
			SmartEntityElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				array[i] = default(SmartEntityElementPriorityPair);
			}
			this.Length = 0;
		}

		public int IndexOf(SmartEntityElementPriorityPair element)
		{
			SmartEntityElementPriorityPair[] array = this.Array;
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
