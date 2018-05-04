using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.DataStructures
{
	public struct SmartEntityElementPriorityPair
	{
		public SmartEntity Element;

		public int Priority;

		public SmartEntityElementPriorityPair(SmartEntity element, int priority)
		{
			this.Element = element;
			this.Priority = priority;
		}

		public bool Equals(SmartEntityElementPriorityPair other)
		{
			return this.Element == other.Element && this.Priority == other.Priority;
		}
	}
}
