using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.GameBoard.Pathfinding
{
	public class PathTroopParams
	{
		public int TroopWidth;

		public int DPS;

		public uint MaxRange;

		public uint MinRange;

		public int MaxSpeed;

		public bool IsMelee;

		public bool IsOverWall;

		public bool CrushesWalls;

		public bool IsHealer;

		public ProjectileTypeVO ProjectileType;

		public uint PathSearchWidth;

		public bool IsTargetShield;

		public uint SupportRange;

		public uint TargetInRangeModifier;
	}
}
