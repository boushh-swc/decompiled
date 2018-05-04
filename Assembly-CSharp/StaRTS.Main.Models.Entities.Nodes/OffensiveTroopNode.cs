using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class OffensiveTroopNode : Node<OffensiveTroopNode>
	{
		public KillerComponent KillerComp
		{
			get;
			set;
		}

		public AttackerComponent AttackerComp
		{
			get;
			set;
		}

		public ShooterComponent ShooterComp
		{
			get;
			set;
		}

		public TransformComponent Transform
		{
			get;
			set;
		}

		public SizeComponent Size
		{
			get;
			set;
		}

		public StateComponent StateComp
		{
			get;
			set;
		}

		public SecondaryTargetsComponent SecondaryTargets
		{
			get;
			set;
		}

		public TroopComponent TroopComp
		{
			get;
			set;
		}

		public TeamComponent TeamComp
		{
			get;
			set;
		}
	}
}
