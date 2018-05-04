using System;

namespace StaRTS.Main.Models.Entities.Shared
{
	public static class CellFlags
	{
		public const uint NONE = 0u;

		public const uint UNWALKABLE = 1u;

		public const uint UNWALKABLE_WALL = 2u;

		public const uint SPAWN_PROTECTED = 4u;

		public const uint SHIELD_BORDER = 8u;

		public const uint SHIELD_SPAWN_PROTECTED = 16u;

		public const uint DEFENSIVE_DEPLOYABLE = 32u;

		public const uint UNDESTRUCTIBLE = 64u;
	}
}
