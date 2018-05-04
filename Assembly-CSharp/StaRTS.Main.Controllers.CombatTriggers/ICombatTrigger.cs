using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Controllers.CombatTriggers
{
	public interface ICombatTrigger
	{
		object Owner
		{
			get;
		}

		CombatTriggerType Type
		{
			get;
		}

		uint LastDitchDelayMillis
		{
			get;
		}

		void Trigger(Entity intruder);

		bool IsAlreadyTriggered();
	}
}
