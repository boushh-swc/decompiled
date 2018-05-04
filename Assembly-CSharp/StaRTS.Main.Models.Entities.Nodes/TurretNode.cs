using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.Main.Models.Entities.Nodes
{
	public class TurretNode : Node<TurretNode>
	{
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

		public GameObjectViewComponent View
		{
			get;
			set;
		}

		public StateComponent StateComp
		{
			get;
			set;
		}

		public HealthComponent HealthComp
		{
			get;
			set;
		}

		public TurretShooterComponent TurretShooterComp
		{
			get;
			set;
		}
	}
}
