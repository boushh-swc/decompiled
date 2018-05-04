using System;

namespace StaRTS.Main.Models.Entities.Shared
{
	public enum EntityState
	{
		Disable = 0,
		Idle = 1,
		Moving = 2,
		Tracking = 3,
		Turning = 4,
		WarmingUp = 5,
		Attacking = 6,
		AttackingReset = 7,
		CoolingDown = 8,
		Dying = 9
	}
}
