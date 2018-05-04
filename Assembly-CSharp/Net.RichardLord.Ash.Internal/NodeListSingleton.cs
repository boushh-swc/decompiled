using Net.RichardLord.Ash.Core;
using System;

namespace Net.RichardLord.Ash.Internal
{
	public static class NodeListSingleton<TNode> where TNode : Node<TNode>, new()
	{
		public static NodeList<TNode> NodeList;
	}
}
