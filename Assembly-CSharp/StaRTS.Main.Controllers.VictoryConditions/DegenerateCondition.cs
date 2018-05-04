using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public class DegenerateCondition : AbstractCondition
	{
		public DegenerateCondition(ConditionVO vo, IConditionParent parent) : base(vo, parent)
		{
		}
	}
}
