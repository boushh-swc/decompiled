using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class ShieldGeneratorComponent : ComponentBase
	{
		private int currentRadius;

		public int PointsRange
		{
			get;
			set;
		}

		public Entity ShieldBorderEntity
		{
			get;
			set;
		}

		public int CurrentRadius
		{
			get
			{
				return this.currentRadius;
			}
			set
			{
				this.currentRadius = value;
				this.RadiusSquared = value * value;
			}
		}

		public int RadiusSquared
		{
			get;
			private set;
		}
	}
}
