using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.DataStructures
{
	public struct EntityElementPriorityPair
	{
		public Entity Element;

		public int Priority;

		public EntityElementPriorityPair(Entity element, int priority)
		{
			this.Element = element;
			this.Priority = priority;
		}

		public bool Equals(EntityElementPriorityPair other)
		{
			return this.Element == other.Element && this.Priority == other.Priority;
		}
	}
}
