using System;

namespace StaRTS.Main.Controllers.TrapConditions
{
	public class RadiusTrapCondition : TrapCondition
	{
		public uint Radius
		{
			get;
			private set;
		}

		public RadiusTrapCondition(uint radius)
		{
			this.Radius = radius;
		}
	}
}
