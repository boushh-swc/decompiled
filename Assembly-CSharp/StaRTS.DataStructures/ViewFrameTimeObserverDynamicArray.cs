using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.DataStructures
{
	public struct ViewFrameTimeObserverDynamicArray
	{
		public IViewFrameTimeObserver[] Array;

		public int Length;

		public int Capacity;

		public ViewFrameTimeObserverDynamicArray(int initialCapacity)
		{
			this.Array = new IViewFrameTimeObserver[initialCapacity];
			this.Length = 0;
			this.Capacity = initialCapacity;
		}

		public void Add(IViewFrameTimeObserver element)
		{
			IViewFrameTimeObserver[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				IViewFrameTimeObserver[] array2 = new IViewFrameTimeObserver[num];
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

		public void Insert(int index, IViewFrameTimeObserver element)
		{
			IViewFrameTimeObserver[] array = this.Array;
			int length = this.Length;
			int capacity = this.Capacity;
			if (length == capacity)
			{
				int num = capacity * 2;
				IViewFrameTimeObserver[] array2 = new IViewFrameTimeObserver[num];
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
				IViewFrameTimeObserver viewFrameTimeObserver = array[index];
				array[index] = element;
				for (int j = index + 1; j < length; j++)
				{
					IViewFrameTimeObserver viewFrameTimeObserver2 = array[j];
					array[j] = viewFrameTimeObserver;
					viewFrameTimeObserver = viewFrameTimeObserver2;
				}
				array[length] = viewFrameTimeObserver;
				this.Length++;
			}
		}

		public void RemoveAt(int index)
		{
			IViewFrameTimeObserver[] array = this.Array;
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
			IViewFrameTimeObserver[] array = this.Array;
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
			IViewFrameTimeObserver[] array = this.Array;
			int length = this.Length;
			for (int i = 0; i < length; i++)
			{
				array[i] = null;
			}
			this.Length = 0;
		}

		public int IndexOf(IViewFrameTimeObserver element)
		{
			IViewFrameTimeObserver[] array = this.Array;
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
