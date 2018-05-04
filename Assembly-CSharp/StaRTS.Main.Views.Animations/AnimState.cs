using System;

namespace StaRTS.Main.Views.Animations
{
	public enum AnimState
	{
		Idle = 0,
		Walk = 1,
		Die = 2,
		Shoot = 3,
		ShootWarmup = 4,
		AbilityWarmup1 = 5,
		AbilityShoot = 6,
		AbilityWarmup2 = 7,
		Repair = 8,
		Run = 9,
		Disable = 11,
		AbilityPose = 95,
		Closed = 96,
		Open = 97,
		Unlocked = 98
	}
}
