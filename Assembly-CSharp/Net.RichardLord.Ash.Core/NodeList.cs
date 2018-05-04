using Net.RichardLord.Ash.Internal;
using System;

namespace Net.RichardLord.Ash.Core
{
	public class NodeList<TNode> : INodeList where TNode : Node<TNode>, new()
	{
		public event Action<TNode> NodeAdded;

		public event Action<TNode> NodeRemoved;

		public TNode Head
		{
			get;
			set;
		}

		public TNode Tail
		{
			get;
			set;
		}

		public bool Empty
		{
			get
			{
				return this.Head == null;
			}
		}

		internal void Add(TNode node)
		{
			if (this.Head == null)
			{
				TNode tNode = node;
				this.Tail = tNode;
				this.Head = tNode;
				tNode = (TNode)((object)null);
				node.Previous = tNode;
				node.Next = tNode;
			}
			else
			{
				TNode tNode = this.Tail;
				tNode.Next = node;
				node.Previous = this.Tail;
				node.Next = (TNode)((object)null);
				this.Tail = node;
			}
			if (this.NodeAdded != null)
			{
				this.NodeAdded(node);
			}
		}

		internal void Remove(TNode node)
		{
			if (this.Head == node)
			{
				TNode head = this.Head;
				this.Head = head.Next;
			}
			if (this.Tail == node)
			{
				TNode tail = this.Tail;
				this.Tail = tail.Previous;
			}
			if (node.Previous != null)
			{
				TNode previous = node.Previous;
				previous.Next = node.Next;
			}
			if (node.Next != null)
			{
				TNode next = node.Next;
				next.Previous = node.Previous;
			}
			if (this.NodeRemoved != null)
			{
				this.NodeRemoved(node);
			}
		}

		internal void RemoveAll()
		{
			while (this.Head != null)
			{
				TNode head = this.Head;
				this.Head = head.Next;
				head.Previous = (TNode)((object)null);
				head.Next = (TNode)((object)null);
				if (this.NodeRemoved != null)
				{
					this.NodeRemoved(head);
				}
			}
			this.Tail = (TNode)((object)null);
		}

		public void Swap(TNode node1, TNode node2)
		{
			if (node1.Previous == node2)
			{
				node1.Previous = node2.Previous;
				node2.Previous = node1;
				node2.Next = node1.Next;
				node1.Next = node2;
			}
			else if (node2.Previous == node1)
			{
				node2.Previous = node1.Previous;
				node1.Previous = node2;
				node1.Next = node2.Next;
				node2.Next = node1;
			}
			else
			{
				TNode tNode = node1.Previous;
				node1.Previous = node2.Previous;
				node2.Previous = tNode;
				tNode = node1.Next;
				node1.Next = node2.Next;
				node2.Next = tNode;
			}
			if (this.Head == node1)
			{
				this.Head = node2;
			}
			else if (this.Head == node2)
			{
				this.Head = node1;
			}
			if (this.Tail == node1)
			{
				this.Tail = node2;
			}
			else if (this.Tail == node2)
			{
				this.Tail = node1;
			}
			if (node1.Previous != null)
			{
				TNode previous = node1.Previous;
				previous.Next = node1;
			}
			if (node2.Previous != null)
			{
				TNode previous2 = node2.Previous;
				previous2.Next = node2;
			}
			if (node1.Next != null)
			{
				TNode next = node1.Next;
				next.Previous = node1;
			}
			if (node2.Next != null)
			{
				TNode next2 = node2.Next;
				next2.Previous = node2;
			}
		}

		public int CalculateCount()
		{
			int num = 0;
			for (TNode tNode = this.Head; tNode != null; tNode = tNode.Next)
			{
				num++;
			}
			return num;
		}

		public void CleanUp()
		{
			NodeListSingleton<TNode>.NodeList = null;
		}
	}
}
