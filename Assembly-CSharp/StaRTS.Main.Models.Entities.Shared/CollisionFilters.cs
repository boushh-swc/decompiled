using StaRTS.GameBoard.Components;
using System;

namespace StaRTS.Main.Models.Entities.Shared
{
	public static class CollisionFilters
	{
		public static FilterComponent BUILDING;

		public static FilterComponent BUILDING_SKIRT;

		public static FilterComponent BUILDING_GHOST;

		public static FilterComponent WALL;

		public static FilterComponent WALL_SKIRT;

		public static FilterComponent WALL_GHOST;

		public static FilterComponent TRAP;

		public static FilterComponent TRAP_GHOST;

		public static FilterComponent PLATFORM;

		public static FilterComponent PLATFORM_GHOST;

		public static FilterComponent BUILDABLE_AREA;

		public static FilterComponent TROOP;

		public static FilterComponent CLEARABLE;

		public static FilterComponent BLOCKER;

		public static FilterComponent BLOCKER_GHOST;

		public static void StaticInit()
		{
			CollisionFilters.BUILDING = new FilterComponent(1, 2769);
			CollisionFilters.BUILDING_SKIRT = new FilterComponent(0, 2641);
			CollisionFilters.BUILDING_GHOST = new FilterComponent(2, 2769);
			CollisionFilters.WALL = new FilterComponent(16, 2769);
			CollisionFilters.WALL_SKIRT = new FilterComponent(0, 2113);
			CollisionFilters.WALL_GHOST = new FilterComponent(32, 2769);
			CollisionFilters.TRAP = new FilterComponent(512, 2769);
			CollisionFilters.TRAP_GHOST = new FilterComponent(1024, 2769);
			CollisionFilters.PLATFORM = new FilterComponent(2048, 2769);
			CollisionFilters.PLATFORM_GHOST = new FilterComponent(4096, 2769);
			CollisionFilters.TROOP = new FilterComponent(4, 145);
			CollisionFilters.BUILDABLE_AREA = new FilterComponent(0, 8179);
			CollisionFilters.CLEARABLE = new FilterComponent(64, 2769);
			CollisionFilters.BLOCKER = new FilterComponent(128, 2769);
			CollisionFilters.BLOCKER_GHOST = new FilterComponent(256, 2769);
		}

		public static void StaticReset()
		{
			CollisionFilters.BUILDING = null;
			CollisionFilters.BUILDING_SKIRT = null;
			CollisionFilters.WALL = null;
			CollisionFilters.WALL_SKIRT = null;
			CollisionFilters.BUILDING_GHOST = null;
			CollisionFilters.WALL_GHOST = null;
			CollisionFilters.TROOP = null;
			CollisionFilters.BUILDABLE_AREA = null;
			CollisionFilters.CLEARABLE = null;
			CollisionFilters.TRAP = null;
			CollisionFilters.TRAP_GHOST = null;
			CollisionFilters.BLOCKER = null;
			CollisionFilters.BLOCKER_GHOST = null;
			CollisionFilters.PLATFORM = null;
			CollisionFilters.PLATFORM_GHOST = null;
		}
	}
}
