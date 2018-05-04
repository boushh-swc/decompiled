using StaRTS.Main.Models;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.TrapConditions
{
	public class ArmorNotTrapCondition : TrapCondition
	{
		public List<ArmorType> IntruderArmorTypes
		{
			get;
			private set;
		}

		public ArmorNotTrapCondition(List<ArmorType> intruderArmorTypes)
		{
			this.IntruderArmorTypes = intruderArmorTypes;
		}
	}
}
