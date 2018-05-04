using System;

namespace Net.RichardLord.Ash.Core
{
	public class EntityList
	{
		private Entity _head;

		private Entity _tail;

		public Entity Head
		{
			get
			{
				return this._head;
			}
		}

		public Entity Tail
		{
			get
			{
				return this._tail;
			}
		}

		public void Add(Entity entity)
		{
			if (this.Head == null)
			{
				this._tail = entity;
				this._head = entity;
				Entity entity2 = null;
				entity.Previous = entity2;
				entity.Next = entity2;
			}
			else
			{
				this._tail.Next = entity;
				entity.Previous = this._tail;
				entity.Next = null;
				this._tail = entity;
			}
		}

		public void Remove(Entity entity)
		{
			if (this._head == entity)
			{
				this._head = this._head.Next;
			}
			if (this._tail == entity)
			{
				this._tail = this._tail.Previous;
			}
			if (entity.Previous != null)
			{
				entity.Previous.Next = entity.Next;
			}
			if (entity.Next != null)
			{
				entity.Next.Previous = entity.Previous;
			}
		}

		public void RemoveAll()
		{
			while (this.Head != null)
			{
				Entity head = this.Head;
				this._head = this.Head.Next;
				head.Previous = null;
				head.Next = null;
			}
			this._tail = null;
		}
	}
}
