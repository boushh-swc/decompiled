using System;

namespace StaRTS.DataStructures
{
	public struct Int32ElementPriorityPair
	{
		public int Element;

		public int Priority;

		public Int32ElementPriorityPair(int element, int priority)
		{
			this.Element = element;
			this.Priority = priority;
		}

		public bool Equals(Int32ElementPriorityPair other)
		{
			return this.Element == other.Element && this.Priority == other.Priority;
		}
	}
}
