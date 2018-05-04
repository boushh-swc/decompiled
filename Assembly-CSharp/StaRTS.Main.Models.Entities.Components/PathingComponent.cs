using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Pathfinding;
using StaRTS.Main.Views.Entities;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class PathingComponent : ComponentBase
	{
		private const uint KILOTILES_PER_MINUTE = 3u;

		private const uint MS_PER_MINUTE = 60000u;

		private int nextTileIndex;

		private int maxSpeed;

		public BoardCell EndCell;

		public BoardCell TargetCell;

		public Path CurrentPath;

		public SmartEntity Target;

		public PathView PathView;

		public int TimePerBoardCellMs
		{
			get;
			private set;
		}

		public int TimeToMove
		{
			get;
			set;
		}

		public uint TimeOnSegment
		{
			get;
			set;
		}

		public int NextTileIndex
		{
			get
			{
				return this.nextTileIndex;
			}
		}

		public bool StateDirty
		{
			get;
			set;
		}

		public int MaxSpeed
		{
			get
			{
				return this.maxSpeed;
			}
			set
			{
				this.maxSpeed = value;
				this.TimePerBoardCellMs = (int)(60000L / (3L * (long)this.maxSpeed));
			}
		}

		public PathingComponent()
		{
			this.Reset();
		}

		public PathingComponent(int maxSpeed, SmartEntity target)
		{
			this.Reset();
			this.MaxSpeed = maxSpeed;
			this.TimePerBoardCellMs = (int)(60000L / (3L * (long)maxSpeed));
			this.Target = target;
		}

		public BoardCell GetNextTile()
		{
			return this.CurrentPath.GetCell(this.nextTileIndex);
		}

		public BoardCell AdvanceNextTile()
		{
			return this.CurrentPath.GetCell(++this.nextTileIndex);
		}

		public void InitializePathView()
		{
			if (this.PathView == null)
			{
				this.PathView = new PathView(this);
			}
			else
			{
				this.PathView.Reset(this);
			}
			this.PathView.AdvanceNextTurn();
		}

		public void Reset()
		{
			this.TimeOnSegment = 0u;
			this.TimeToMove = 0;
			this.Target = null;
			this.CurrentPath = null;
			this.nextTileIndex = 0;
			this.EndCell = null;
			this.TargetCell = null;
		}
	}
}
