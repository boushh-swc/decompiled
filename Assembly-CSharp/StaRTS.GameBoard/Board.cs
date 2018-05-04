using StaRTS.DataStructures;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.GameBoard
{
	public class Board
	{
		public delegate void CellTraverseCallback(BoardCell cell);

		public delegate void CellTraverseWithCookieCallback<TCookie>(BoardCell cell, TCookie cookie);

		public delegate void NeighborTraverseCallback(BoardCell center, BoardCell neighbor);

		public delegate void CellReader(ref StringBuilder sb, BoardCell cell, int cellBufferSize);

		public delegate void FlagStampAction(BoardCell cell, FlagStamp flagStamp);

		private int centerOffset;

		private int boardSize;

		private int flaggableMinAbs;

		private int flaggableMaxAbs;

		public BoardCell[,] Cells;

		private bool clearanceMapDirty;

		private bool clearanceMapNoWallDirty;

		private int upperRightX;

		private int upperRightZ;

		private int upperRightXNoWall;

		private int upperRightZNoWall;

		private LinkedList<ConstraintRegion> constraintRegions;

		private LinkedList<BoardItem> children;

		public int CenterOffset
		{
			get
			{
				return this.centerOffset;
			}
		}

		public int BoardSize
		{
			get
			{
				return this.boardSize;
			}
		}

		public LinkedList<BoardItem> Children
		{
			get
			{
				return this.children;
			}
		}

		public Board(int boardSize, int flaggableBoardSize, uint defaultCellFlags)
		{
			this.boardSize = boardSize;
			this.flaggableMinAbs = (boardSize - flaggableBoardSize) / 2;
			this.flaggableMaxAbs = boardSize - this.flaggableMinAbs;
			this.centerOffset = boardSize / 2;
			this.Cells = new BoardCell[boardSize, boardSize];
			for (int i = 0; i < boardSize; i++)
			{
				for (int j = 0; j < boardSize; j++)
				{
					this.Cells[i, j] = new BoardCell(this, this.AbsToRel(i), this.AbsToRel(j), null, defaultCellFlags);
				}
			}
			this.upperRightX = (this.upperRightZ = (this.upperRightXNoWall = (this.upperRightZNoWall = boardSize - 1)));
			this.Cells[this.upperRightX, this.upperRightZ].Clearance = 1;
			this.Cells[this.upperRightXNoWall, this.upperRightZNoWall].ClearanceNoWall = 1;
			this.clearanceMapDirty = true;
			this.clearanceMapNoWallDirty = true;
			this.RefreshClearanceMap();
			this.RefreshClearanceMapNoWall();
			this.children = new LinkedList<BoardItem>();
		}

		public int GetMaxSquaredDistance()
		{
			return this.boardSize * this.boardSize * 2;
		}

		public void DirtyClearanceMap(int cellX, int cellZ)
		{
			this.clearanceMapDirty = true;
			this.MakeCoordinatesAbsolute(ref cellX, ref cellZ);
			if (cellX > this.upperRightX)
			{
				this.upperRightX = cellX;
			}
			if (cellZ > this.upperRightZ)
			{
				this.upperRightZ = cellZ;
			}
		}

		public void DirtyClearanceMapNoWall(int cellX, int cellZ)
		{
			this.clearanceMapNoWallDirty = true;
			this.MakeCoordinatesAbsolute(ref cellX, ref cellZ);
			if (cellX > this.upperRightXNoWall)
			{
				this.upperRightXNoWall = cellX;
			}
			if (cellZ > this.upperRightZNoWall)
			{
				this.upperRightZNoWall = cellZ;
			}
		}

		public void RefreshClearanceMap()
		{
			if (!this.clearanceMapDirty)
			{
				return;
			}
			for (int i = this.upperRightZ; i >= 0; i--)
			{
				for (int j = this.upperRightX; j >= 0; j--)
				{
					if (this.Cells[j, i].IsWalkable())
					{
						if (i == this.boardSize - 1 || j == this.boardSize - 1)
						{
							this.Cells[j, i].Clearance = 1;
						}
						else
						{
							int clearance = this.Cells[j + 1, i + 1].Clearance;
							if (this.Cells[j + 1, i].Clearance < clearance)
							{
								clearance = this.Cells[j + 1, i].Clearance;
							}
							if (this.Cells[j, i + 1].Clearance < clearance)
							{
								clearance = this.Cells[j, i + 1].Clearance;
							}
							this.Cells[j, i].Clearance = clearance + 1;
						}
					}
					else
					{
						this.Cells[j, i].Clearance = 0;
					}
				}
			}
			this.clearanceMapDirty = false;
		}

		public void RefreshClearanceMapNoWall()
		{
			if (!this.clearanceMapNoWallDirty)
			{
				return;
			}
			for (int i = this.upperRightZNoWall; i >= 0; i--)
			{
				for (int j = this.upperRightXNoWall; j >= 0; j--)
				{
					if (this.Cells[j, i].IsWalkableNoWall())
					{
						if (i == this.boardSize - 1 || j == this.boardSize - 1)
						{
							this.Cells[j, i].ClearanceNoWall = 1;
						}
						else
						{
							int clearanceNoWall = this.Cells[j + 1, i + 1].ClearanceNoWall;
							if (this.Cells[j + 1, i].ClearanceNoWall < clearanceNoWall)
							{
								clearanceNoWall = this.Cells[j + 1, i].ClearanceNoWall;
							}
							if (this.Cells[j, i + 1].ClearanceNoWall < clearanceNoWall)
							{
								clearanceNoWall = this.Cells[j, i + 1].ClearanceNoWall;
							}
							this.Cells[j, i].ClearanceNoWall = clearanceNoWall + 1;
						}
					}
					else
					{
						this.Cells[j, i].ClearanceNoWall = 0;
					}
				}
			}
			this.clearanceMapNoWallDirty = false;
		}

		public void MakeCoordinatesAbsolute(ref int x, ref int z)
		{
			x += this.centerOffset;
			z += this.centerOffset;
		}

		public void MakeCoordinatesRelative(ref int x, ref int z)
		{
			x -= this.centerOffset;
			z -= this.centerOffset;
		}

		public int RelToAbs(int value)
		{
			return value + this.centerOffset;
		}

		public int AbsToRel(int value)
		{
			return value - this.centerOffset;
		}

		public BoardCell GetCellAt(int x, int z, bool absolute)
		{
			if (!absolute)
			{
				this.MakeCoordinatesAbsolute(ref x, ref z);
			}
			if (x < 0 || x >= this.boardSize || z < 0 || z >= this.boardSize)
			{
				return null;
			}
			return this.Cells[x, z];
		}

		public BoardCell GetCellAt(int x, int z)
		{
			return this.GetCellAt(x, z, false);
		}

		public BoardCell GetClampedToBoardCellAt(int x, int z, int troopWidth)
		{
			this.MakeCoordinatesAbsolute(ref x, ref z);
			return this.Cells[this.ClampToBoard(x, troopWidth), this.ClampToBoard(z, troopWidth)];
		}

		public BoardCell GetClampedDeployableCellAt(int x, int z, int troopWidth)
		{
			this.MakeCoordinatesAbsolute(ref x, ref z);
			return this.Cells[this.ClampToDeployableBoard(x, troopWidth), this.ClampToDeployableBoard(z, troopWidth)];
		}

		public bool DoesBoardCellHasBlockerAround(int x, int z, int troopWidth)
		{
			for (int i = 0; i < troopWidth; i++)
			{
				for (int j = 0; j < troopWidth; j++)
				{
					if (x + i >= this.boardSize || z + j >= this.boardSize)
					{
						return true;
					}
					BoardCell cellAt = this.GetCellAt(x + i, z + j);
					if (cellAt != null && (cellAt.Flags & 64u) != 0u)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CanOccupy(BoardItem item, int x, int z, bool checkSkirt)
		{
			int walkableGap = 0;
			if (checkSkirt && item.Width > 1 && item.Depth > 1)
			{
				walkableGap = 1;
			}
			return this.FitsAt(item, x, z, walkableGap) && !this.CollidesWith(item, x, z, walkableGap) && (!checkSkirt || !this.SkirtCollidesWith(item, x, z, walkableGap));
		}

		public bool SkirtCollidesWith(BoardItem item, int x, int z, int walkableGap)
		{
			FilterComponent filter = null;
			if (item.Filter == CollisionFilters.BUILDING || item.Filter == CollisionFilters.BUILDING_GHOST || item.Filter == CollisionFilters.PLATFORM || item.Filter == CollisionFilters.PLATFORM_GHOST)
			{
				filter = CollisionFilters.BUILDING_SKIRT;
			}
			else if (item.Filter == CollisionFilters.WALL || item.Filter == CollisionFilters.WALL_GHOST || item.Filter == CollisionFilters.TRAP || item.Filter == CollisionFilters.TRAP_GHOST)
			{
				filter = CollisionFilters.WALL_SKIRT;
			}
			int num = x + item.Width - walkableGap;
			int num2 = z + item.Depth - walkableGap;
			for (int i = x - 1; i <= num; i++)
			{
				BoardCell cellAt = this.GetCellAt(i, z - 1);
				if (cellAt != null && cellAt.CollidesWith(filter))
				{
					return true;
				}
				cellAt = this.GetCellAt(i, num2);
				if (cellAt != null && cellAt.CollidesWith(filter))
				{
					return true;
				}
			}
			for (int j = z; j < num2; j++)
			{
				BoardCell cellAt = this.GetCellAt(x - 1, j);
				if (cellAt != null && cellAt.CollidesWith(filter))
				{
					return true;
				}
				cellAt = this.GetCellAt(num, j);
				if (cellAt != null && cellAt.CollidesWith(filter))
				{
					return true;
				}
			}
			return false;
		}

		public bool CollidesWith(BoardItem item, int x, int z, int walkableGap)
		{
			for (LinkedListNode<ConstraintRegion> linkedListNode = this.constraintRegions.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				if (linkedListNode.Value.Blocks(item, x, z, item.Width - walkableGap, item.Depth - walkableGap))
				{
					return true;
				}
			}
			int num = x + item.Width - walkableGap;
			int num2 = z + item.Depth - walkableGap;
			for (int i = x; i < num; i++)
			{
				for (int j = z; j < num2; j++)
				{
					if (this.GetCellAt(i, j).CollidesWithItem(item))
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool FitsAt(BoardItem item, int x, int z, int walkableGap)
		{
			this.MakeCoordinatesAbsolute(ref x, ref z);
			return x >= 0 && x + item.Width - walkableGap <= this.BoardSize && z >= 0 && z + item.Depth - walkableGap <= this.BoardSize;
		}

		private int ClampToFlaggableArea(int absValue)
		{
			if (absValue > this.flaggableMaxAbs)
			{
				return this.flaggableMaxAbs;
			}
			if (absValue < this.flaggableMinAbs)
			{
				return this.flaggableMinAbs;
			}
			return absValue;
		}

		public bool IsCellOutsideFlaggableArea(int x, int y)
		{
			x = this.RelToAbs(x);
			y = this.RelToAbs(y);
			return x > this.flaggableMaxAbs - 1 || x < this.flaggableMinAbs || y > this.flaggableMaxAbs - 1 || y < this.flaggableMinAbs;
		}

		private int ClampToBoard(int absValue, int troopWidth)
		{
			if (absValue >= this.boardSize - troopWidth)
			{
				return this.boardSize - troopWidth;
			}
			if (absValue < 0)
			{
				return 0;
			}
			return absValue;
		}

		private int ClampToDeployableBoard(int absValue, int troopWidth)
		{
			int halfWidthForOffset = BoardUtils.GetHalfWidthForOffset(troopWidth);
			int num = this.boardSize - 1 - halfWidthForOffset;
			if (absValue > num)
			{
				return num;
			}
			if (absValue < halfWidthForOffset)
			{
				return halfWidthForOffset;
			}
			return absValue;
		}

		public BoardCell AddChild(BoardItem child, int x, int z, HealthComponent buildingHealth, bool checkSkirt)
		{
			return this.AddChild(child, x, z, buildingHealth, checkSkirt, true);
		}

		public BoardCell AddChild(BoardItem child, int x, int z, HealthComponent buildingHealth, bool checkSkirt, bool checkOccupancy)
		{
			if (checkOccupancy && !this.CanOccupy(child, x, z, checkSkirt))
			{
				return null;
			}
			int num = 0;
			if (checkSkirt && child.Width > 1 && child.Depth > 1)
			{
				num = 1;
			}
			int num2 = x + child.Width - num;
			int num3 = z + child.Depth - num;
			BoardCell boardCell = null;
			for (int i = x; i < num2; i++)
			{
				for (int j = z; j < num3; j++)
				{
					BoardCell cellAt = this.GetCellAt(i, j);
					bool flag = cellAt.AddChild(child, x, z, true);
					if (boardCell == null && flag)
					{
						boardCell = cellAt;
					}
					if (flag && buildingHealth != null)
					{
						cellAt.BuildingHealth = buildingHealth;
					}
				}
			}
			child.LinkedListNode = this.children.AddLast(child);
			child.Internal_InformAddedToBoard(this, x, z);
			if (child.FlagStamp != null)
			{
				child.FlagStamp.CenterTo(x + (child.Width - num) / 2, z + (child.Depth - num) / 2);
				this.AddFlagStamp(child.FlagStamp);
			}
			return boardCell;
		}

		public void RemoveChild(BoardItem child, bool checkSkirt, bool isBuilding)
		{
			if (child.CurrentCell == null)
			{
				return;
			}
			int x = child.CurrentCell.X;
			int z = child.CurrentCell.Z;
			int num = 0;
			if (checkSkirt && child.Width > 1 && child.Depth > 1)
			{
				num = 1;
			}
			int num2 = x + child.Width - num;
			int num3 = z + child.Depth - num;
			for (int i = x; i < num2; i++)
			{
				for (int j = z; j < num3; j++)
				{
					BoardCell cellAt = this.GetCellAt(i, j);
					cellAt.RemoveChild(child);
					if (isBuilding)
					{
						cellAt.BuildingHealth = null;
					}
				}
			}
			if (child.FlagStamp != null)
			{
				this.RemoveFlagStamp(child.FlagStamp);
			}
			if (child.LinkedListNode == null)
			{
				Service.Logger.Error("Error removing board child - LinkedListNode is null.");
			}
			else if (child.LinkedListNode.List != this.children)
			{
				Service.Logger.Error("Error removing board child - LinkedList does not match.");
			}
			else
			{
				this.children.Remove(child.LinkedListNode);
			}
			child.Internal_InformRemovedFromBoard();
		}

		public BoardCell MoveChild(BoardItem child, int x, int z, HealthComponent buildingHealth, bool leaveChildIfCollision, bool checkSkirt)
		{
			int x2 = 0;
			int z2 = 0;
			bool flag = child.CurrentCell != null;
			if (flag)
			{
				if (leaveChildIfCollision)
				{
					x2 = child.BoardX;
					z2 = child.BoardZ;
				}
				this.RemoveChild(child, checkSkirt, buildingHealth != null);
			}
			bool flag2 = this.CanOccupy(child, x, z, checkSkirt);
			BoardCell result = null;
			if (!flag2)
			{
				if (leaveChildIfCollision)
				{
					if (!flag)
					{
						return null;
					}
					result = this.AddChild(child, x2, z2, buildingHealth, checkSkirt);
				}
			}
			else
			{
				result = this.AddChild(child, x, z, buildingHealth, checkSkirt);
			}
			return result;
		}

		private void ForEachCellInFlagStamp(FlagStamp flagStamp, Board.FlagStampAction callback)
		{
			int num = this.RelToAbs(flagStamp.X);
			int num2 = this.RelToAbs(flagStamp.Z);
			int num3 = this.RelToAbs(flagStamp.Right);
			int num4 = this.RelToAbs(flagStamp.Bottom);
			if (!flagStamp.IsShieldFlag)
			{
				num = this.ClampToFlaggableArea(num);
				num2 = this.ClampToFlaggableArea(num2);
				num3 = this.ClampToFlaggableArea(num3);
				num4 = this.ClampToFlaggableArea(num4);
			}
			for (int i = num; i < num3; i++)
			{
				for (int j = num2; j < num4; j++)
				{
					BoardCell cellAt = this.GetCellAt(i, j, true);
					if (cellAt != null)
					{
						callback(cellAt, flagStamp);
					}
				}
			}
		}

		public void AddFlagStamp(FlagStamp flagStamp)
		{
			this.ForEachCellInFlagStamp(flagStamp, new Board.FlagStampAction(this.AddFlagStampToCell));
		}

		private void AddFlagStampToCell(BoardCell cell, FlagStamp flagStamp)
		{
			cell.AddFlagStamp(flagStamp);
		}

		public void RemoveFlagStamp(FlagStamp flagStamp)
		{
			this.ForEachCellInFlagStamp(flagStamp, new Board.FlagStampAction(this.RemoveFlagStampFromCell));
		}

		private void RemoveFlagStampFromCell(BoardCell cell, FlagStamp flagStamp)
		{
			cell.RemoveFlagStamp(flagStamp);
		}

		public void AddConstraintRegion(ConstraintRegion region)
		{
			if (this.constraintRegions == null)
			{
				this.constraintRegions = new LinkedList<ConstraintRegion>();
			}
			this.constraintRegions.AddLast(region);
		}

		public string GetDebugString(Board.CellReader cellReader, int cellBufferSize)
		{
			int num = this.boardSize;
			if (cellBufferSize > 1)
			{
				num *= 2;
			}
			int num2 = this.boardSize * cellBufferSize * num;
			if (cellBufferSize > 1)
			{
				num2 += num;
			}
			StringBuilder stringBuilder = new StringBuilder(num2);
			for (int i = 0; i < this.boardSize; i++)
			{
				for (int j = 0; j < this.boardSize; j++)
				{
					BoardCell cellAt = this.GetCellAt(this.AbsToRel(j), this.AbsToRel(i));
					cellReader(ref stringBuilder, cellAt, cellBufferSize);
				}
				stringBuilder.Append("\n");
				if (cellBufferSize > 1)
				{
					for (int k = 0; k < this.boardSize * cellBufferSize; k++)
					{
						stringBuilder.Append("-");
					}
					stringBuilder.Append("\n");
				}
			}
			return stringBuilder.ToString();
		}

		public void TraverseCellNeighbors(BoardCell cell, Board.NeighborTraverseCallback callback)
		{
			callback(cell, this.GetCellAt(cell.X - 1, cell.Z - 1));
			callback(cell, this.GetCellAt(cell.X, cell.Z - 1));
			callback(cell, this.GetCellAt(cell.X + 1, cell.Z - 1));
			callback(cell, this.GetCellAt(cell.X - 1, cell.Z));
			callback(cell, this.GetCellAt(cell.X + 1, cell.Z));
			callback(cell, this.GetCellAt(cell.X - 1, cell.Z + 1));
			callback(cell, this.GetCellAt(cell.X, cell.Z + 1));
			callback(cell, this.GetCellAt(cell.X + 1, cell.Z + 1));
		}

		public void TraverseCellNeighbors(int x, int z, Board.NeighborTraverseCallback callback)
		{
			BoardCell cellAt = this.GetCellAt(x, z);
			this.TraverseCellNeighbors(cellAt, callback);
		}

		public void TraverseAllCells(Board.CellTraverseCallback callback)
		{
			for (int i = 0; i < this.boardSize; i++)
			{
				for (int j = 0; j < this.boardSize; j++)
				{
					callback(this.GetCellAt(i, j, true));
				}
			}
		}

		public void TraverseCircle<TCookie>(int radius, int centerX, int centerZ, Board.CellTraverseWithCookieCallback<TCookie> callback, TCookie cookie)
		{
			if (radius == 0)
			{
				callback(this.GetCellAt(centerX, centerZ), cookie);
				return;
			}
			if (radius == 1)
			{
				callback(this.GetCellAt(centerX - 1, centerZ - 1), cookie);
				callback(this.GetCellAt(centerX, centerZ - 1), cookie);
				callback(this.GetCellAt(centerX + 1, centerZ - 1), cookie);
				callback(this.GetCellAt(centerX - 1, centerZ), cookie);
				callback(this.GetCellAt(centerX, centerZ), cookie);
				callback(this.GetCellAt(centerX + 1, centerZ), cookie);
				callback(this.GetCellAt(centerX - 1, centerZ + 1), cookie);
				callback(this.GetCellAt(centerX, centerZ + 1), cookie);
				callback(this.GetCellAt(centerX + 1, centerZ + 1), cookie);
				return;
			}
			int i = radius;
			int num = 0;
			int num2 = 1 - i;
			while (i >= num)
			{
				if (i > num)
				{
					for (int j = i; j >= -i; j--)
					{
						callback(this.GetCellAt(j + centerX, num + centerZ), cookie);
					}
				}
				if (num > 0 && num < i)
				{
					for (int k = i; k >= -i; k--)
					{
						callback(this.GetCellAt(k + centerX, -num + centerZ), cookie);
					}
				}
				if (num2 < 0)
				{
					num++;
					num2 += 2 * num + 1;
				}
				else
				{
					for (int l = num; l >= -num; l--)
					{
						callback(this.GetCellAt(l + centerX, i + centerZ), cookie);
					}
					for (int m = num; m >= -num; m--)
					{
						callback(this.GetCellAt(m + centerX, -i + centerZ), cookie);
					}
					num++;
					i--;
					num2 += 2 * (num - i + 1);
				}
			}
		}

		private void GetChildrenInCircleCallback(BoardCell cell, List<BoardItem> cookie)
		{
			if (cell == null || cell.Children == null)
			{
				return;
			}
			for (LinkedListNode<BoardItem> linkedListNode = cell.Children.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
			{
				cookie.Add(linkedListNode.Value);
			}
		}

		public List<BoardItem> GetChildrenInCircle(int radius, int centerX, int centerZ)
		{
			List<BoardItem> list = new List<BoardItem>();
			this.TraverseCircle<List<BoardItem>>(radius, centerX, centerZ, new Board.CellTraverseWithCookieCallback<List<BoardItem>>(this.GetChildrenInCircleCallback), list);
			return list;
		}

		public void TraverseCellsWithinSquare<TCookie>(int distFromCenter, int centerX, int centerZ, Board.CellTraverseWithCookieCallback<TCookie> callback, TCookie cookie)
		{
			int num = centerX - distFromCenter;
			int num2 = centerX + distFromCenter;
			int num3 = centerZ - distFromCenter;
			int num4 = centerZ + distFromCenter;
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					BoardCell cellAt = this.GetCellAt(i, j);
					if (cellAt != null)
					{
						callback(cellAt, cookie);
					}
				}
			}
		}

		public BoardCellDynamicArray GetCellsInSquare(int distFromCenter, int centerX, int centerZ)
		{
			BoardCellDynamicArray result = new BoardCellDynamicArray(distFromCenter * distFromCenter);
			int num = centerX - distFromCenter;
			int num2 = centerX + distFromCenter;
			int num3 = centerZ - distFromCenter;
			int num4 = centerZ + distFromCenter;
			for (int i = num; i <= num2; i++)
			{
				for (int j = num3; j <= num4; j++)
				{
					BoardCell cellAt = this.GetCellAt(i, j);
					if (cellAt != null)
					{
						result.Add(cellAt);
					}
				}
			}
			return result;
		}
	}
}
