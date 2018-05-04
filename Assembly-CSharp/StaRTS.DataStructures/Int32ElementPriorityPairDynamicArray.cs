using System;

namespace StaRTS.DataStructures
{
	public struct Int32ElementPriorityPairDynamicArray
	{
		public Int32ElementPriorityPair[] Array;

		public int Length;

		public int Capacity;

		public Int32ElementPriorityPairDynamicArray(int initialCapacity)
		{
			this.Array = new Int32ElementPriorityPair[initialCapacity];
			this.Length = 0;
			this.Capacity = initialCapacity;
		}

		public void Add(Int32ElementPriorityPair element)
		{
			Int32ElementPriorityPair[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				Int32ElementPriorityPair[] array2 = new Int32ElementPriorityPair[num];
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

		public void Insert(int index, Int32ElementPriorityPair element)
		{
			Int32ElementPriorityPair[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				Int32ElementPriorityPair[] array2 = new Int32ElementPriorityPair[num];
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
				Int32ElementPriorityPair int32ElementPriorityPair = array[index];
				array[index] = element;
				for (int j = index + 1; j < length; j++)
				{
					Int32ElementPriorityPair int32ElementPriorityPair2 = array[j];
					array[j] = int32ElementPriorityPair;
					int32ElementPriorityPair = int32ElementPriorityPair2;
				}
				array[length] = int32ElementPriorityPair;
				this.Length++;
			}
		}

		public void RemoveAt(int index)
		{
			Int32ElementPriorityPair[] array = this.Array;
			int num = this.Length;
			for (int i = index + 1; i < num; i++)
			{
				array[i - 1] = array[i];
			}
			num--;
			array[num] = default(Int32ElementPriorityPair);
			this.Length = num;
		}

		public void RemoveRange(int startIndex, int count)
		{
			if (count == 0)
			{
				return;
			}
			Int32ElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = startIndex + count; i < length; i++)
			{
				array[i - count] = array[i];
			}
			for (int j = length - count; j < length; j++)
			{
				array[j] = default(Int32ElementPriorityPair);
			}
			this.Length = length - count;
		}

		public void Clear()
		{
			Int32ElementPriorityPair[] array = this.Array;
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				array[i] = default(Int32ElementPriorityPair);
			}
			this.Length = 0;
		}

		public int IndexOf(Int32ElementPriorityPair element)
		{
			Int32ElementPriorityPair[] array = this.Array;
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
