using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Utils;
using System;
using System.Collections.Generic;

namespace StaRTS.GameBoard
{
	public class BoardUtils
	{
		public static BoardCell WhereDoesLineCrossFlag(Board board, int x0, int y0, int x1, int y1, uint crossFlag)
		{
			int num = (x1 <= x0) ? -1 : 1;
			int num2 = (x1 - x0) * num;
			int num3 = (y1 <= y0) ? -1 : 1;
			int num4 = (y1 - y0) * num3;
			if (num2 > num4)
			{
				int num5 = num4 * 2 - num2;
				int num6 = num4 * 2;
				int num7 = (num4 - num2) * 2;
				int num8 = x0;
				int num9 = y0;
				while (num8 != x1)
				{
					if (num5 <= 0)
					{
						num5 += num6;
						num8 += num;
					}
					else
					{
						num5 += num7;
						num8 += num;
						num9 += num3;
					}
					BoardCell cellAt = board.GetCellAt(num8, num9);
					if ((cellAt.Flags & crossFlag) != 0u)
					{
						return cellAt;
					}
				}
			}
			else
			{
				int num10 = num2 * 2 - num4;
				int num11 = num2 * 2;
				int num12 = (num2 - num4) * 2;
				int num13 = x0;
				int num14 = y0;
				while (num14 != y1)
				{
					if (num10 <= 0)
					{
						num10 += num11;
						num14 += num3;
					}
					else
					{
						num10 += num12;
						num13 += num;
						num14 += num3;
					}
					BoardCell cellAt = board.GetCellAt(num13, num14);
					if ((cellAt.Flags & crossFlag) != 0u)
					{
						return cellAt;
					}
				}
			}
			return null;
		}

		private static bool CheckIfCellContainsTarget(SmartEntity target, BoardCell cell)
		{
			if (target != null)
			{
				OrderedSet children = cell.Children;
				if (children != null)
				{
					for (LinkedListNode<BoardItem> linkedListNode = children.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
					{
						if (linkedListNode.Value.Data == target)
						{
							return true;
						}
					}
				}
			}
			return false;
		}

		private static bool CheckIfCellShieldBorderIsFromShieldTarget(SmartEntity target, BoardCell cell)
		{
			if (GameUtils.IsEntityShieldGenerator(target))
			{
				SmartEntity smartEntity = (SmartEntity)target.ShieldGeneratorComp.ShieldBorderEntity;
				List<Entity> obstacles = cell.Obstacles;
				if (obstacles != null)
				{
					int i = 0;
					int count = obstacles.Count;
					while (i < count)
					{
						if (obstacles[i] == smartEntity)
						{
							return true;
						}
						i++;
					}
				}
			}
			return false;
		}

		private static bool UpdateHitShieldCellAndReturnIfItIsFromTarget(SmartEntity target, BoardCell cell, ref BoardCell hitShieldCell)
		{
			if ((cell.Flags & 8u) != 0u)
			{
				if (hitShieldCell == null)
				{
					hitShieldCell = cell;
				}
				if (BoardUtils.CheckIfCellShieldBorderIsFromShieldTarget(target, cell))
				{
					return true;
				}
			}
			return false;
		}

		private static bool IsCellBlockingLineOfSight(BoardCell cell, bool isOverWalls, bool isFlying)
		{
			return !cell.IsWalkableNoWall() || (!isOverWalls && !isFlying && !cell.IsWalkable());
		}

		public static bool HasLineOfSight(Board board, int x0, int y0, int x1, int y1, SmartEntity troop, SmartEntity target, out BoardCell hitShieldCell)
		{
			bool overWalls = troop.ShooterComp.ShooterVO.OverWalls;
			bool isFlying = troop.TroopComp.TroopType.IsFlying;
			hitShieldCell = null;
			int num = (x1 <= x0) ? -1 : 1;
			int num2 = (x1 - x0) * num;
			int num3 = (y1 <= y0) ? -1 : 1;
			int num4 = (y1 - y0) * num3;
			if (num2 > num4)
			{
				int num5 = num4 * 2 - num2;
				int num6 = num4 * 2;
				int num7 = (num4 - num2) * 2;
				int num8 = x0;
				int num9 = y0;
				while (num8 != x1)
				{
					if (num5 <= 0)
					{
						num5 += num6;
						num8 += num;
					}
					else
					{
						num5 += num7;
						num8 += num;
						num9 += num3;
					}
					BoardCell cellAt = board.GetCellAt(num8, num9);
					if (BoardUtils.UpdateHitShieldCellAndReturnIfItIsFromTarget(target, cellAt, ref hitShieldCell))
					{
						return true;
					}
					if (num8 == x1)
					{
						return true;
					}
					if (BoardUtils.IsCellBlockingLineOfSight(cellAt, overWalls, isFlying))
					{
						return BoardUtils.CheckIfCellContainsTarget(target, cellAt);
					}
				}
			}
			else
			{
				int num10 = num2 * 2 - num4;
				int num11 = num2 * 2;
				int num12 = (num2 - num4) * 2;
				int num13 = x0;
				int num14 = y0;
				while (num14 != y1)
				{
					if (num10 <= 0)
					{
						num10 += num11;
						num14 += num3;
					}
					else
					{
						num10 += num12;
						num13 += num;
						num14 += num3;
					}
					BoardCell cellAt = board.GetCellAt(num13, num14);
					if (BoardUtils.UpdateHitShieldCellAndReturnIfItIsFromTarget(target, cellAt, ref hitShieldCell))
					{
						return true;
					}
					if (num14 == y1)
					{
						return true;
					}
					if (BoardUtils.IsCellBlockingLineOfSight(cellAt, overWalls, isFlying))
					{
						return BoardUtils.CheckIfCellContainsTarget(target, cellAt);
					}
				}
			}
			return true;
		}

		public static bool HasLineOfClearance(Board board, int x0, int y0, int x1, int y1, int troopWidth)
		{
			int num = (x1 <= x0) ? -1 : 1;
			int num2 = (x1 - x0) * num;
			int num3 = (y1 <= y0) ? -1 : 1;
			int num4 = (y1 - y0) * num3;
			if (num2 > num4)
			{
				int num5 = num4 * 2 - num2;
				int num6 = num4 * 2;
				int num7 = (num4 - num2) * 2;
				int num8 = x0;
				int num9 = y0;
				while (num8 != x1)
				{
					if (num5 <= 0)
					{
						num5 += num6;
						num8 += num;
					}
					else
					{
						num5 += num7;
						num8 += num;
						num9 += num3;
					}
					if (num8 == x1)
					{
						return true;
					}
					BoardCell cellAt = board.GetCellAt(num8, num9);
					if (cellAt.Clearance < troopWidth)
					{
						return false;
					}
				}
			}
			else
			{
				int num10 = num2 * 2 - num4;
				int num11 = num2 * 2;
				int num12 = (num2 - num4) * 2;
				int num13 = x0;
				int num14 = y0;
				while (num14 != y1)
				{
					if (num10 <= 0)
					{
						num10 += num11;
						num14 += num3;
					}
					else
					{
						num10 += num12;
						num13 += num;
						num14 += num3;
					}
					if (num14 == y1)
					{
						return true;
					}
					BoardCell cellAt = board.GetCellAt(num13, num14);
					if (cellAt.Clearance < troopWidth)
					{
						return false;
					}
				}
			}
			return true;
		}

		public static int GetChessboardDistance(int x0, int z0, int x1, int z1)
		{
			int num = (x0 <= x1) ? (x1 - x0) : (x0 - x1);
			int num2 = (z0 <= z1) ? (z1 - z0) : (z0 - z1);
			return (num <= num2) ? num2 : num;
		}

		public static int GetHalfWidthForOffset(int width)
		{
			return (width - 1) / 2;
		}
	}
}
