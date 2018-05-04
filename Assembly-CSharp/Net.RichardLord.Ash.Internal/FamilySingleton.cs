using Net.RichardLord.Ash.Core;
using System;

namespace Net.RichardLord.Ash.Internal
{
	public static class FamilySingleton<TNode> where TNode : Node<TNode>, new()
	{
		public static IFamily Family;
	}
}
