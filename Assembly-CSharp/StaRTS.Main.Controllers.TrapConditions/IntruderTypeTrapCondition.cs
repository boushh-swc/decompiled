using StaRTS.Main.Models;
using System;

namespace StaRTS.Main.Controllers.TrapConditions
{
	public class IntruderTypeTrapCondition : TrapCondition
	{
		public TroopType IntruderType
		{
			get;
			private set;
		}

		public IntruderTypeTrapCondition(TroopType intruderType)
		{
			this.IntruderType = intruderType;
		}
	}
}
