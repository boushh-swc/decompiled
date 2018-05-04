using StaRTS.DataStructures.PriorityQueue;
using StaRTS.Main.Models.Entities.Components;
using System;

namespace StaRTS.GameBoard.Pathfinding.InternalClasses
{
	public class PathfindingCellInfo : PriorityQueueNode
	{
		public bool InClosedSet;

		public bool InRange;

		public PathfindingCellInfo PrevCell;

		public BoardCell Cell;

		public int RemainingCost;

		public int PathLength;

		public int PastCost;

		public int PoolIndex;

		public HealthComponent Health;

		public PathfindingCellInfo()
		{
			this.PrevCell = null;
			this.Cell = null;
			this.InClosedSet = false;
			this.InRange = false;
			this.Health = null;
		}
	}
}
