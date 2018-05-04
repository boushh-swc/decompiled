using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;

namespace AhoCorasick
{
	public class Trie : Trie<string>
	{
		public void Add(string s)
		{
			base.Add(s, s);
		}

		public void Add(IEnumerable<string> strings)
		{
			foreach (string current in strings)
			{
				this.Add(current);
			}
		}
	}
	public class Trie<TValue> : Trie<char, TValue>
	{
	}
	public class Trie<T, TValue>
	{
		private class Node<TNode, TNodeValue> : IEnumerable<Trie<T, TValue>.Node<TNode, TNodeValue>>, IEnumerable
		{
			private readonly TNode word;

			private readonly Trie<T, TValue>.Node<TNode, TNodeValue> parent;

			private readonly Dictionary<TNode, Trie<T, TValue>.Node<TNode, TNodeValue>> children = new Dictionary<TNode, Trie<T, TValue>.Node<TNode, TNodeValue>>();

			private readonly List<TNodeValue> values = new List<TNodeValue>();

			public TNode Word
			{
				get
				{
					return this.word;
				}
			}

			public Trie<T, TValue>.Node<TNode, TNodeValue> Parent
			{
				get
				{
					return this.parent;
				}
			}

			public Trie<T, TValue>.Node<TNode, TNodeValue> Fail
			{
				get;
				set;
			}

			public Trie<T, TValue>.Node<TNode, TNodeValue> this[TNode c]
			{
				get
				{
					return (!this.children.ContainsKey(c)) ? null : this.children[c];
				}
				set
				{
					this.children[c] = value;
				}
			}

			public List<TNodeValue> Values
			{
				get
				{
					return this.values;
				}
			}

			public Node()
			{
			}

			public Node(TNode word, Trie<T, TValue>.Node<TNode, TNodeValue> parent)
			{
				this.word = word;
				this.parent = parent;
			}

			public IEnumerator<Trie<T, TValue>.Node<TNode, TNodeValue>> GetEnumerator()
			{
				return this.children.Values.GetEnumerator();
			}

			IEnumerator IEnumerable.GetEnumerator()
			{
				return this.GetEnumerator();
			}

			public override string ToString()
			{
				TNode tNode = this.Word;
				return tNode.ToString();
			}
		}

		private readonly Trie<T, TValue>.Node<T, TValue> root = new Trie<T, TValue>.Node<T, TValue>();

		public void Add(IEnumerable<T> word, TValue value)
		{
			Trie<T, TValue>.Node<T, TValue> node = this.root;
			foreach (T current in word)
			{
				Trie<T, TValue>.Node<T, TValue> node2 = node[current];
				if (node2 == null)
				{
					Trie<T, TValue>.Node<T, TValue> node3 = new Trie<T, TValue>.Node<T, TValue>(current, node);
					node[current] = node3;
					node2 = node3;
				}
				node = node2;
			}
			node.Values.Add(value);
		}

		public void Build()
		{
			Queue<Trie<T, TValue>.Node<T, TValue>> queue = new Queue<Trie<T, TValue>.Node<T, TValue>>();
			queue.Enqueue(this.root);
			while (queue.Count > 0)
			{
				Trie<T, TValue>.Node<T, TValue> node = queue.Dequeue();
				foreach (Trie<T, TValue>.Node<T, TValue> current in node)
				{
					queue.Enqueue(current);
				}
				if (node == this.root)
				{
					this.root.Fail = this.root;
				}
				else
				{
					Trie<T, TValue>.Node<T, TValue> fail = node.Parent.Fail;
					while (fail[node.Word] == null && fail != this.root)
					{
						fail = fail.Fail;
					}
					node.Fail = (fail[node.Word] ?? this.root);
					if (node.Fail == node)
					{
						node.Fail = this.root;
					}
				}
			}
		}

		[DebuggerHidden]
		public IEnumerable<TValue> Find(IEnumerable<T> text)
		{
			bool flag = false;
			Trie<T, TValue>.Node<T, TValue> node = this.root;
			IEnumerator<T> enumerator = text.GetEnumerator();
			while (enumerator.MoveNext())
			{
				T current = enumerator.Current;
				while (node[current] == null && node != this.root)
				{
					node = node.Fail;
				}
				node = (node[current] ?? this.root);
				for (Trie<T, TValue>.Node<T, TValue> node2 = node; node2 != this.root; node2 = node2.Fail)
				{
					List<TValue>.Enumerator enumerator2 = node2.Values.GetEnumerator();
					try
					{
						uint num;
						switch (num)
						{
						}
						if (enumerator2.MoveNext())
						{
							TValue current2 = enumerator2.Current;
							bool flag2;
							if (!flag2)
							{
							}
							flag = true;
							return;
						}
					}
					finally
					{
						if (!flag)
						{
							((IDisposable)enumerator2).Dispose();
						}
					}
				}
			}
			yield break;
		}
	}
}
