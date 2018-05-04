using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TurretShooterComponent : ComponentBase
	{
		public int TargetWeight
		{
			get;
			set;
		}

		public TurretShooterComponent()
		{
			this.TargetWeight = 0;
		}
	}
}
