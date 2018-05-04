using System;

namespace StaRTS.DataStructures
{
	public class Int32PriorityList
	{
		private Int32ElementPriorityPairDynamicArray list;

		public int Count
		{
			get
			{
				return this.list.Length;
			}
		}

		public Int32PriorityList()
		{
			this.list = new Int32ElementPriorityPairDynamicArray(4);
		}

		public virtual int Add(int element, int priority)
		{
			int i = 0;
			int length = this.list.Length;
			while (i < length)
			{
				Int32ElementPriorityPair int32ElementPriorityPair = this.list.Array[i];
				if (int32ElementPriorityPair.Element == element)
				{
					return -1;
				}
				if (priority > int32ElementPriorityPair.Priority)
				{
					this.list.Insert(i, new Int32ElementPriorityPair(element, priority));
					return i;
				}
				i++;
			}
			this.list.Add(new Int32ElementPriorityPair(element, priority));
			return this.list.Length - 1;
		}

		public Int32ElementPriorityPair Get(int i)
		{
			return this.list.Array[i];
		}

		public int GetElement(int i)
		{
			return this.list.Array[i].Element;
		}

		public int GetPriority(int i)
		{
			return this.list.Array[i].Priority;
		}

		public void GetElementPriority(int i, out int element, out int priority)
		{
			Int32ElementPriorityPair int32ElementPriorityPair = this.list.Array[i];
			element = int32ElementPriorityPair.Element;
			priority = int32ElementPriorityPair.Priority;
		}

		public int IndexOf(int element)
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
