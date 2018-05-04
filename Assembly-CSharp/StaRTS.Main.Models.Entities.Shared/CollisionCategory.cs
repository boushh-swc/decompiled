using System;

namespace StaRTS.Main.Models.Entities.Shared
{
	[Flags]
	public enum CollisionCategory
	{
		None = 0,
		Building = 1,
		BuildingGhost = 2,
		Troop = 4,
		Projectile = 8,
		Wall = 16,
		WallGhost = 32,
		Clearable = 64,
		Blocker = 128,
		BlockerGhost = 256,
		Trap = 512,
		TrapGhost = 1024,
		Platform = 2048,
		PlatformGhost = 4096,
		AllMask = 8191
	}
}
