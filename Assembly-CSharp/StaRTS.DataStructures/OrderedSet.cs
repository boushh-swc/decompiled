using StaRTS.GameBoard;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.DataStructures
{
	public class OrderedSet
	{
		private readonly IDictionary<BoardItem, LinkedListNode<BoardItem>> dictionary;

		private readonly LinkedList<BoardItem> linkedList;

		public int Count
		{
			get
			{
				return this.dictionary.Count;
			}
		}

		public virtual bool IsReadOnly
		{
			get
			{
				return this.dictionary.IsReadOnly;
			}
		}

		public LinkedListNode<BoardItem> First
		{
			get
			{
				return this.linkedList.First;
			}
		}

		public LinkedListNode<BoardItem> Last
		{
			get
			{
				return this.linkedList.Last;
			}
		}

		public OrderedSet() : this(EqualityComparer<BoardItem>.Default)
		{
		}

		public OrderedSet(IEqualityComparer<BoardItem> comparer)
		{
			this.dictionary = new Dictionary<BoardItem, LinkedListNode<BoardItem>>(comparer);
			this.linkedList = new LinkedList<BoardItem>();
		}

		public void Clear()
		{
			this.linkedList.Clear();
			this.dictionary.Clear();
		}

		public bool Remove(BoardItem item)
		{
			if (item != null)
			{
				LinkedListNode<BoardItem> linkedListNode;
				if (!this.dictionary.TryGetValue(item, out linkedListNode))
				{
					return false;
				}
				this.dictionary.Remove(item);
				if (linkedListNode == null)
				{
					Service.Logger.Error("OrderedSet Node is NULL");
				}
				else if (!this.linkedList.Contains(item))
				{
					Service.Logger.Error("OrderedSet list does not contain item");
				}
				else if (linkedListNode.List != this.linkedList)
				{
					Service.Logger.Error("OrderedSet node list does not match");
					linkedListNode = this.linkedList.Find(item);
					if (linkedListNode != null)
					{
						this.linkedList.Remove(linkedListNode);
					}
				}
				else
				{
					this.linkedList.Remove(linkedListNode);
				}
			}
			else
			{
				Service.Logger.Error("OrderedSet Item is NULL");
			}
			return true;
		}

		public IEnumerator<BoardItem> GetEnumerator()
		{
			return this.linkedList.GetEnumerator();
		}

		public bool Contains(BoardItem item)
		{
			return this.dictionary.ContainsKey(item);
		}

		public void CopyTo(BoardItem[] array, int arrayIndex)
		{
			this.linkedList.CopyTo(array, arrayIndex);
		}

		public bool Add(BoardItem item)
		{
			if (this.dictionary.ContainsKey(item))
			{
				return false;
			}
			LinkedListNode<BoardItem> linkedListNode = this.linkedList.AddLast(item);
			this.dictionary.Add(item, linkedListNode);
			if (linkedListNode == null)
			{
				Service.Logger.Error("OrderedSet Added NULL node " + item);
			}
			return true;
		}
	}
}
