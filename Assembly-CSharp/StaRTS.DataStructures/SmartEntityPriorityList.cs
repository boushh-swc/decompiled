using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.DataStructures
{
	public class SmartEntityPriorityList
	{
		private SmartEntityElementPriorityPairDynamicArray list;

		public int Count
		{
			get
			{
				return this.list.Length;
			}
		}

		public SmartEntityPriorityList()
		{
			this.list = new SmartEntityElementPriorityPairDynamicArray(4);
		}

		public virtual int Add(SmartEntity element, int priority)
		{
			if (element == null)
			{
				return -1;
			}
			int i = 0;
			int length = this.list.Length;
			while (i < length)
			{
				SmartEntityElementPriorityPair smartEntityElementPriorityPair = this.list.Array[i];
				if (smartEntityElementPriorityPair.Element == element)
				{
					return -1;
				}
				if (priority > smartEntityElementPriorityPair.Priority)
				{
					this.list.Insert(i, new SmartEntityElementPriorityPair(element, priority));
					return i;
				}
				i++;
			}
			this.list.Add(new SmartEntityElementPriorityPair(element, priority));
			return this.list.Length - 1;
		}

		public SmartEntityElementPriorityPair Get(int i)
		{
			return this.list.Array[i];
		}

		public SmartEntity GetElement(int i)
		{
			return this.list.Array[i].Element;
		}

		public int GetPriority(int i)
		{
			return this.list.Array[i].Priority;
		}

		public void GetElementPriority(int i, out SmartEntity element, out int priority)
		{
			SmartEntityElementPriorityPair smartEntityElementPriorityPair = this.list.Array[i];
			element = smartEntityElementPriorityPair.Element;
			priority = smartEntityElementPriorityPair.Priority;
		}

		public int IndexOf(SmartEntity element)
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
