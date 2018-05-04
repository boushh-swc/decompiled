using System;

namespace Net.RichardLord.Ash.Core
{
	internal class SystemList<T> where T : SystemBase<T>
	{
		internal T Head
		{
			get;
			set;
		}

		internal T Tail
		{
			get;
			set;
		}

		internal void Add(T system)
		{
			if (this.Head == null)
			{
				T t = system;
				this.Tail = t;
				this.Head = t;
				t = (T)((object)null);
				system.Previous = t;
				system.Next = t;
			}
			else
			{
				T t2;
				for (t2 = this.Tail; t2 != null; t2 = t2.Previous)
				{
					if (t2.Priority <= system.Priority)
					{
						break;
					}
				}
				if (t2 == this.Tail)
				{
					T t = this.Tail;
					t.Next = system;
					system.Previous = this.Tail;
					system.Next = (T)((object)null);
					this.Tail = system;
				}
				else if (t2 == null)
				{
					system.Next = this.Head;
					system.Previous = (T)((object)null);
					T head = this.Head;
					head.Previous = system;
					this.Head = system;
				}
				else
				{
					system.Next = t2.Next;
					system.Previous = t2;
					T next = t2.Next;
					next.Previous = system;
					t2.Next = system;
				}
			}
		}

		internal void Remove(T system)
		{
			if (this.Head == system)
			{
				T head = this.Head;
				this.Head = head.Next;
			}
			if (this.Tail == system)
			{
				T tail = this.Tail;
				this.Tail = tail.Previous;
			}
			if (system.Previous != null)
			{
				T previous = system.Previous;
				previous.Next = system.Next;
			}
			if (system.Next != null)
			{
				T next = system.Next;
				next.Previous = system.Previous;
			}
		}

		internal void RemoveAll()
		{
			while (this.Head != null)
			{
				T head = this.Head;
				T head2 = this.Head;
				this.Head = head2.Next;
				head.Previous = (T)((object)null);
				head.Next = (T)((object)null);
			}
			this.Tail = (T)((object)null);
		}

		internal T Get(Type type)
		{
			for (T t = this.Head; t != null; t = t.Next)
			{
				if (type.IsAssignableFrom(t.GetType()))
				{
					return t;
				}
			}
			return (T)((object)null);
		}
	}
}
