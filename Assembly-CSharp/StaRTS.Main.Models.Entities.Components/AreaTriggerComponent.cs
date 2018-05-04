using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class AreaTriggerComponent : ComponentBase
	{
		public uint RangeSquared
		{
			get;
			set;
		}

		public AreaTriggerComponent(uint range)
		{
			this.RangeSquared = range * range;
		}
	}
}
