using System;

namespace StaRTS.FX
{
	public enum ShieldLoadReason
	{
		CreateEffect = 0,
		ApplyHitEffect = 1,
		UpdateShieldScale = 2,
		PlayEffect = 3,
		StopEffect = 4,
		PowerDownShieldEffect = 5,
		PlayDestructionEffect = 6
	}
}
