using System;

namespace StaRTS.Main.Controllers.VictoryConditions
{
	public interface IConditionParent
	{
		void ChildSatisfied(AbstractCondition child);

		void ChildFailed(AbstractCondition child);

		void ChildUpdated(AbstractCondition child, int delta);
	}
}
