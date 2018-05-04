using System;

namespace StaRTS.Main.Controllers
{
	public enum DefensiveCameraEventType
	{
		BuildingDestroyed = 0,
		WallDestroyed = 1,
		TroopDestroyed = 2,
		TroopSpawned = 3,
		EntityDamaged = 4,
		None = 5
	}
}
