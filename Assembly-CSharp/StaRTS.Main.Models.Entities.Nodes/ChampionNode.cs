using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class ChampionNode : Node<ChampionNode>
	{
		public ChampionComponent ChampionComp
		{
			get;
			set;
		}
	}
}
