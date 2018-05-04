using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.DataStructures.PriorityQueue;
using StaRTS.GameBoard.Pathfinding.InternalClasses;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.GameBoard.Pathfinding
{
	public class Path
	{
		public const int DIST_STRAIGHT = 1000;

		public const int DIST_DIAGONAL = 1414;

		public const int MAX_SPEED_NORMALIZER = 10;

		private const int DIR_LEN = 8;

		private const int CELLS_LIST_INITIAL_CAPACITY = 64;

		private BoardCellDynamicArray pathCells;

		private BoardCellDynamicArray scratchCells;

		private BoardCellDynamicArray turns;

		private List<int> turnDistances;

		private BoardCell startCell;

		private BoardCell destCell;

		private BoardCell targetCell;

		private PathfindingCellInfo curPathingCell;

		private Board board;

		private PathingManager pathingManager;

		private int damagePerSecond;

		private uint maxShooterRange;

		private uint minShooterRange;

		private int maxSpeed;

		private int heristicMultiplier;

		private bool melee;

		private bool overWalls;

		private ProjectileTypeVO projectileType;

		private int maxPathLength;

		private bool isHealer;

		private uint targetInRangeModifier;

		private static readonly int[] dirX;

		private static readonly int[] dirY;

		private int priorityQueueSize;

		private HeapPriorityQueue openCells;

		private bool destructible;

		public int TroopWidth;

		public bool NoWall;

		private bool crushesWalls;

		private bool isTargetShield;

		public int CellCount
		{
			get
			{
				return this.pathCells.Length;
			}
		}

		public int TurnCount
		{
			get
			{
				return this.turns.Length;
			}
		}

		public Path(BoardCell fromCell, BoardCell toCell, BoardCell targetAt, int maxLength, PathTroopParams troopParams, PathBoardParams boardParams)
		{
			this.pathCells = new BoardCellDynamicArray(64);
			this.scratchCells = new BoardCellDynamicArray(64);
			this.turns = new BoardCellDynamicArray(64);
			this.turnDistances = new List<int>(64);
			BoardController boardController = Service.BoardController;
			this.board = boardController.Board;
			this.pathingManager = Service.PathingManager;
			this.startCell = fromCell;
			this.destCell = toCell;
			this.targetCell = targetAt;
			this.NoWall = (boardParams.IgnoreWall || troopParams.CrushesWalls);
			this.crushesWalls = troopParams.CrushesWalls;
			this.destructible = boardParams.Destructible;
			this.isHealer = troopParams.IsHealer;
			this.TroopWidth = troopParams.TroopWidth;
			this.damagePerSecond = troopParams.DPS;
			this.maxShooterRange = troopParams.MaxRange;
			this.targetInRangeModifier = troopParams.TargetInRangeModifier;
			if (this.isHealer && this.maxShooterRange > troopParams.SupportRange)
			{
				this.maxShooterRange = troopParams.SupportRange;
			}
			this.minShooterRange = troopParams.MinRange;
			this.maxSpeed = troopParams.MaxSpeed;
			this.heristicMultiplier = (int)troopParams.PathSearchWidth;
			this.maxPathLength = ((!this.isHealer) ? maxLength : -1);
			this.melee = troopParams.IsMelee;
			this.overWalls = troopParams.IsOverWall;
			this.projectileType = troopParams.ProjectileType;
			this.isTargetShield = troopParams.IsTargetShield;
			this.openCells = new HeapPriorityQueue(boardController.GetPriorityQueueSize());
			this.curPathingCell = this.pathingManager.GetPathingCell();
			this.curPathingCell.Cell = this.startCell;
			this.startCell.PathInfo = this.curPathingCell;
			this.curPathingCell.InRange = this.InRangeOfTarget(this.startCell);
			this.curPathingCell.RemainingCost = this.HeuristicDiagonal(this.startCell, this.destCell);
			this.curPathingCell.PathLength = 0;
			this.curPathingCell.PastCost = 0;
			this.curPathingCell.InClosedSet = true;
		}

		static Path()
		{
			Path.dirX = new int[]
			{
				-1,
				-1,
				-1,
				0,
				0,
				1,
				1,
				1
			};
			Path.dirY = new int[]
			{
				-1,
				0,
				1,
				-1,
				1,
				-1,
				0,
				1
			};
			for (int i = 0; i < 8; i++)
			{
				Path.dirX[i] += 23;
				Path.dirY[i] += 23;
			}
		}

		public BoardCell GetDestCell()
		{
			return this.destCell;
		}

		public BoardCell GetCell(int index)
		{
			if (index < this.pathCells.Length)
			{
				return this.pathCells.Array[index];
			}
			return null;
		}

		private bool InRangeOfTarget(BoardCell curCell)
		{
			if (curCell == this.destCell && this.minShooterRange == 0u)
			{
				return true;
			}
			int x;
			int z;
			if (this.melee && this.destCell != null)
			{
				x = this.destCell.X;
				z = this.destCell.Z;
			}
			else
			{
				if (this.targetCell == null)
				{
					return false;
				}
				x = this.targetCell.X;
				z = this.targetCell.Z;
			}
			int halfWidthForOffset = BoardUtils.GetHalfWidthForOffset(this.TroopWidth);
			int num = curCell.X + halfWidthForOffset;
			int num2 = curCell.Z + halfWidthForOffset;
			int num3 = num - x;
			int num4 = num3 + ((num3 <= 0) ? -1 : 1);
			int num5 = num2 - z;
			int num6 = num5 + ((num5 <= 0) ? -1 : 1);
			int num7 = num4 * num4 + num6 * num6;
			num7 *= 1;
			int num8 = num3 * num3 + num5 * num5;
			num8 *= 1;
			uint num9 = this.maxShooterRange * this.maxShooterRange;
			uint num10 = this.minShooterRange * this.minShooterRange;
			if (this.isHealer)
			{
				num7 += (int)(num9 / 2u);
			}
			else if (this.targetInRangeModifier > 0u)
			{
				num9 /= this.targetInRangeModifier;
			}
			if ((long)num7 < (long)((ulong)num9) && (long)num8 >= (long)((ulong)num10))
			{
				BoardCell boardCell = BoardUtils.WhereDoesLineCrossFlag(Service.BoardController.Board, num, num2, x, z, 64u);
				return boardCell == null;
			}
			return false;
		}

		private BoardCellDynamicArray FindTheTurns(ref BoardCellDynamicArray path)
		{
			BoardCellDynamicArray result = new BoardCellDynamicArray(64);
			int length = path.Length;
			result.Add(path.Array[length - 1]);
			for (int i = length - 2; i > 0; i--)
			{
				BoardCell boardCell = path.Array[i];
				BoardCell boardCell2 = path.Array[i + 1];
				BoardCell boardCell3 = path.Array[i - 1];
				if (boardCell.X - boardCell2.X != boardCell3.X - boardCell.X || boardCell.Z - boardCell2.Z != boardCell3.Z - boardCell.Z)
				{
					result.Add(boardCell);
				}
			}
			if (length >= 2)
			{
				result.Add(path.Array[0]);
			}
			return result;
		}

		private void AddTurn(BoardCell legStart, BoardCell legEnd, ref BoardCellDynamicArray pathCells, ref BoardCellDynamicArray turns, List<int> turnDistances)
		{
			turns.Add(legEnd);
			int num = 0;
			int num2 = 0;
			this.RasterLine(legStart.X, legStart.Z, legEnd.X, legEnd.Z, ref pathCells, out num, out num2);
			turnDistances.Add(num * 1000 + num2 * 1414);
		}

		public void EndCurrentPath(PathingComponent pathing)
		{
			int nextTileIndex = pathing.NextTileIndex;
			if (nextTileIndex < this.pathCells.Length)
			{
				this.pathCells.RemoveRange(nextTileIndex, this.pathCells.Length - nextTileIndex);
			}
		}

		private void SmoothThePath(ref BoardCellDynamicArray rawTurns, ref BoardCellDynamicArray pathCells, ref BoardCellDynamicArray turns, List<int> turnDistances)
		{
			if (rawTurns.Length == 0)
			{
				Service.Logger.Error("SmoothThePath: Not expecting empty path!");
				return;
			}
			pathCells.Add(rawTurns.Array[0]);
			turns.Add(rawTurns.Array[0]);
			turnDistances.Add(0);
			if (rawTurns.Length == 1)
			{
				return;
			}
			if (rawTurns.Length == 2)
			{
				this.AddTurn(rawTurns.Array[0], rawTurns.Array[1], ref pathCells, ref turns, turnDistances);
				return;
			}
			BoardCell boardCell = rawTurns.Array[0];
			BoardCell boardCell2 = rawTurns.Array[1];
			for (int i = 2; i < rawTurns.Length; i++)
			{
				BoardCell boardCell3 = rawTurns.Array[i];
				if (!BoardUtils.HasLineOfClearance(this.board, boardCell.X, boardCell.Z, boardCell3.X, boardCell3.Z, this.TroopWidth))
				{
					this.AddTurn(boardCell, boardCell2, ref pathCells, ref turns, turnDistances);
					boardCell = boardCell2;
				}
				boardCell2 = boardCell3;
			}
			this.AddTurn(boardCell, boardCell2, ref pathCells, ref turns, turnDistances);
		}

		public void CalculatePath(out bool found)
		{
			this.CalculatePath(out found, ref this.scratchCells);
		}

		private void CalculatePath(out bool found, ref BoardCellDynamicArray scratchCells)
		{
			int num = 46 - this.TroopWidth;
			while (!this.curPathingCell.InRange)
			{
				if (this.maxPathLength < 0 || this.curPathingCell.PathLength < this.maxPathLength)
				{
					BoardCell cell = this.curPathingCell.Cell;
					int x = cell.X;
					int z = cell.Z;
					for (int i = 0; i < 8; i++)
					{
						int num2 = x + Path.dirX[i];
						if (num2 >= 0 && num2 <= num)
						{
							int num3 = z + Path.dirY[i];
							if (num3 >= 0 && num3 <= num)
							{
								BoardCell boardCell = this.board.Cells[num2, num3];
								if ((boardCell.Flags & 64u) == 0u)
								{
									if (!this.destructible)
									{
										int num4 = (!this.NoWall) ? boardCell.Clearance : boardCell.ClearanceNoWall;
										if (this.TroopWidth > num4)
										{
											goto IL_29A;
										}
									}
									int num5 = this.CostToNeighbor(cell, boardCell, ref scratchCells);
									if (num5 != 2147483647)
									{
										int num6 = this.curPathingCell.PastCost + num5;
										int pathLength = this.curPathingCell.PathLength + 1;
										PathfindingCellInfo pathfindingCellInfo = boardCell.PathInfo;
										if (pathfindingCellInfo != null && pathfindingCellInfo.PoolIndex < this.pathingManager.FreeCellIndex)
										{
											if (!pathfindingCellInfo.InClosedSet)
											{
												if (!this.openCells.Contains(pathfindingCellInfo))
												{
													Service.Logger.ErrorFormat("Allocated cell not in close/open sets,PoolIndex:{0}, FreeIndex:{1}", new object[]
													{
														pathfindingCellInfo.PoolIndex,
														this.pathingManager.FreeCellIndex
													});
												}
												else if (num6 < pathfindingCellInfo.PastCost)
												{
													pathfindingCellInfo.PastCost = num6;
													pathfindingCellInfo.PathLength = pathLength;
													this.openCells.UpdatePriority(pathfindingCellInfo, pathfindingCellInfo.PastCost + pathfindingCellInfo.RemainingCost);
												}
											}
										}
										else
										{
											pathfindingCellInfo = this.pathingManager.GetPathingCell();
											pathfindingCellInfo.PrevCell = this.curPathingCell;
											if (boardCell.PathInfo != null)
											{
												PathfindingCellInfo pathInfo = boardCell.PathInfo;
												pathInfo.Cell = null;
											}
											pathfindingCellInfo.Cell = boardCell;
											boardCell.PathInfo = pathfindingCellInfo;
											pathfindingCellInfo.InRange = this.InRangeOfTarget(boardCell);
											pathfindingCellInfo.RemainingCost = this.HeuristicDiagonal(boardCell, this.destCell);
											pathfindingCellInfo.PastCost = num6;
											pathfindingCellInfo.PathLength = pathLength;
											this.openCells.Enqueue(pathfindingCellInfo, pathfindingCellInfo.PastCost + pathfindingCellInfo.RemainingCost);
										}
									}
								}
							}
						}
						IL_29A:;
					}
				}
				if (this.openCells.Count == 0)
				{
					this.pathingManager.RecycleAllPathingCells();
					this.openCells = null;
					found = false;
					scratchCells.Clear();
					return;
				}
				this.curPathingCell = this.openCells.Dequeue();
				this.curPathingCell.InClosedSet = true;
			}
			if (!this.curPathingCell.InRange)
			{
				this.pathingManager.RecycleAllPathingCells();
				this.openCells = null;
				found = false;
				scratchCells.Clear();
				return;
			}
			do
			{
				scratchCells.Add(this.curPathingCell.Cell);
				this.curPathingCell = this.curPathingCell.PrevCell;
			}
			while (this.curPathingCell != null);
			int length = scratchCells.Length;
			if (length == 0)
			{
				Service.Logger.ErrorFormat("Empth Path from {0} to {1} within range {2}", new object[]
				{
					this.startCell,
					this.destCell,
					this.maxShooterRange
				});
			}
			if (scratchCells.Array[length - 1] != this.startCell)
			{
				Service.Logger.ErrorFormat("First cell doesn't match: {0} and {1}", new object[]
				{
					this.startCell,
					scratchCells.Array[length - 1]
				});
			}
			BoardCellDynamicArray boardCellDynamicArray = this.FindTheTurns(ref scratchCells);
			this.SmoothThePath(ref boardCellDynamicArray, ref this.pathCells, ref this.turns, this.turnDistances);
			this.pathingManager.RecycleAllPathingCells();
			this.openCells = null;
			found = true;
			scratchCells.Clear();
		}

		private int CostToNeighbor(BoardCell fromCell, BoardCell toCell, ref BoardCellDynamicArray cells)
		{
			int num;
			if (fromCell.X != toCell.X && fromCell.Z != toCell.Z)
			{
				num = 1414;
			}
			else
			{
				num = 1000;
			}
			int num2 = num * 10 / this.maxSpeed;
			int num3 = this.TroopWidth - ((!this.NoWall) ? toCell.Clearance : toCell.ClearanceNoWall);
			if (num3 > 0)
			{
				int num4 = this.RasterCrossSection(fromCell.X, fromCell.Z, toCell.X, toCell.Z, this.TroopWidth, ref cells);
				for (int i = 0; i < num4; i++)
				{
					BoardCell boardCell = cells.Array[i];
					uint flags = boardCell.Flags;
					if ((flags & 3u) != 0u)
					{
						if (!this.NoWall || (flags & 1u) != 0u)
						{
							if ((flags & 64u) != 0u || this.isHealer)
							{
								num2 = 2147483647;
								break;
							}
							HealthComponent buildingHealth = boardCell.BuildingHealth;
							if (buildingHealth != null && this.damagePerSecond != 0)
							{
								ArmorType armorType = buildingHealth.ArmorType;
								int num5 = 100;
								if (this.projectileType != null)
								{
									int num6 = this.projectileType.DamageMultipliers[(int)armorType];
									if (num6 >= 0)
									{
										num5 = num6;
									}
									else
									{
										Service.Logger.ErrorFormat("ArmorType {0} not found in ProjectileType {1}", new object[]
										{
											armorType,
											this.projectileType.Uid
										});
									}
								}
								int num7 = this.damagePerSecond * num5 / 100;
								if (num7 <= 0)
								{
									num2 = 2147483647;
									break;
								}
								num2 += buildingHealth.Health * 1000 / num7;
							}
						}
					}
				}
				cells.Clear();
			}
			return num2;
		}

		private int HeuristicDiagonal(BoardCell fromCell, BoardCell toCell)
		{
			uint num = (uint)(IntMath.FastDist(fromCell.X, fromCell.Z, toCell.X, toCell.Z) / 1024);
			uint num2 = 0u;
			if (num > this.maxShooterRange)
			{
				num2 = num - this.maxShooterRange;
			}
			else if (num < this.minShooterRange)
			{
				num2 = this.minShooterRange - num;
			}
			return this.heristicMultiplier * (int)num2 * 10 / this.maxSpeed;
		}

		public BoardCell GetTurn(int turnIndex)
		{
			if (turnIndex >= 0 && turnIndex < this.turns.Length)
			{
				return this.turns.Array[turnIndex];
			}
			return null;
		}

		public int GetTurnDistance(int turnIndex)
		{
			return this.turnDistances[turnIndex];
		}

		private void AddEntitiesOnCellToBlockingList(BoardCell cell, uint targetId, HashSet<uint> entityIds, LinkedList<Entity> list, LinkedList<Entity> wallList, bool isPathingCell)
		{
			if (cell.Children == null)
			{
				return;
			}
			if (!cell.IsWalkableNoWall() || ((!this.NoWall || (this.crushesWalls && isPathingCell)) && (isPathingCell || !this.overWalls) && !cell.IsWalkable()))
			{
				foreach (BoardItem current in cell.Children)
				{
					if (current.Data.Has(typeof(HealthComponent)))
					{
						if (entityIds.Add(current.Data.ID))
						{
							if (current.Data.ID != targetId)
							{
								SmartEntity smartEntity = (SmartEntity)current.Data;
								if (smartEntity.BuildingComp != null && smartEntity.BuildingComp.BuildingType.Type == BuildingType.Wall && this.crushesWalls)
								{
									wallList.AddLast(current.Data);
								}
								else
								{
									list.AddLast(current.Data);
								}
							}
						}
					}
				}
			}
		}

		private void AddCell(int x, int y, ref BoardCellDynamicArray cells)
		{
			x += 23;
			if (x >= 0 && x < 46)
			{
				y += 23;
				if (y >= 0 && y < 46)
				{
					BoardCell element = this.board.Cells[x, y];
					cells.Add(element);
				}
			}
		}

		private int RasterCrossSection(int x0, int y0, int x1, int y1, int size, ref BoardCellDynamicArray cells)
		{
			if (size == 1)
			{
				this.AddCell(x1, y1, ref cells);
				return 1;
			}
			int num = x1 - x0;
			int num2 = y1 - y0;
			if (num > 0 && num2 == 0)
			{
				int i = y1;
				int num3 = y1 + size;
				while (i < num3)
				{
					this.AddCell(x1 + 1, i, ref cells);
					i++;
				}
				return size;
			}
			if (num < 0 && num2 == 0)
			{
				int j = y1;
				int num4 = y1 + size;
				while (j < num4)
				{
					this.AddCell(x1, j, ref cells);
					j++;
				}
				return size;
			}
			if (num == 0 && num2 > 0)
			{
				int k = x1;
				int num5 = x1 + size;
				while (k < num5)
				{
					this.AddCell(k, y1 + 1, ref cells);
					k++;
				}
				return size;
			}
			if (num == 0 && num2 < 0)
			{
				int l = x1;
				int num6 = x1 + size;
				while (l < num6)
				{
					this.AddCell(l, y1, ref cells);
					l++;
				}
				return size;
			}
			if (num > 0 && num2 < 0)
			{
				int num7 = 1;
				int num8 = x1;
				int num9 = y1 + size - 1;
				int num10 = x1 + size - 1;
				while (num8 < num10 && num9 > y1)
				{
					this.AddCell(num8, y1, ref cells);
					this.AddCell(x1 + 1, num9, ref cells);
					num7 += 2;
					num8++;
					num9--;
				}
				this.AddCell(x1 + 1, y1, ref cells);
				return num7;
			}
			if (num < 0 && num2 < 0)
			{
				int num7 = 1;
				int num11 = x1 + size - 1;
				int num12 = y1 + size - 1;
				while (num11 > x1 && num12 > y1)
				{
					this.AddCell(num11, y1, ref cells);
					this.AddCell(x1, num12, ref cells);
					num7 += 2;
					num11--;
					num12--;
				}
				this.AddCell(x1, y1, ref cells);
				return num7;
			}
			if (num < 0 && num2 > 0)
			{
				int num7 = 1;
				int num13 = x1 + size - 1;
				int num14 = y1;
				int num15 = y1 + size - 1;
				while (num13 > x1 && num14 < num15)
				{
					this.AddCell(num13, y1 + 1, ref cells);
					this.AddCell(x1, num14, ref cells);
					num7 += 2;
					num13--;
					num14++;
				}
				this.AddCell(x1, y1 + 1, ref cells);
				return num7;
			}
			if (num > 0 && num2 > 0)
			{
				int num7 = 1;
				int num16 = x1;
				int num17 = y1;
				int num18 = y1 + size - 1;
				while (num16 < x1 + size - 1 && num17 < num18)
				{
					this.AddCell(num16, y1 + 1, ref cells);
					this.AddCell(x1 + 1, num17, ref cells);
					num7 += 2;
					num16++;
					num17++;
				}
				this.AddCell(x1 + 1, y1 + 1, ref cells);
				return num7;
			}
			return 0;
		}

		public LinkedList<Entity> GetBlockingEntities(uint targetId, out LinkedList<Entity> wallListForCrushing)
		{
			HashSet<uint> entityIds = new HashSet<uint>();
			LinkedList<Entity> linkedList = new LinkedList<Entity>();
			LinkedList<Entity> linkedList2 = new LinkedList<Entity>();
			BoardCellDynamicArray boardCellDynamicArray = new BoardCellDynamicArray(64);
			for (int i = 0; i < this.pathCells.Length; i++)
			{
				BoardCell boardCell = this.pathCells.Array[i];
				int num = (!this.NoWall || this.crushesWalls) ? boardCell.Clearance : boardCell.ClearanceNoWall;
				if (num < this.TroopWidth)
				{
					int x;
					int y;
					if (i == 0)
					{
						if (this.pathCells.Length <= 1)
						{
							break;
						}
						x = boardCell.X * 2 - this.pathCells.Array[1].X;
						y = boardCell.Z * 2 - this.pathCells.Array[1].Z;
					}
					else
					{
						x = this.pathCells.Array[i - 1].X;
						y = this.pathCells.Array[i - 1].Z;
					}
					this.RasterCrossSection(x, y, boardCell.X, boardCell.Z, this.TroopWidth, ref boardCellDynamicArray);
				}
			}
			for (int j = 0; j < boardCellDynamicArray.Length; j++)
			{
				this.AddEntitiesOnCellToBlockingList(boardCellDynamicArray.Array[j], targetId, entityIds, linkedList, linkedList2, true);
			}
			if (!this.melee)
			{
				boardCellDynamicArray.Clear();
				int num2 = 0;
				int num3 = 0;
				int halfWidthForOffset = BoardUtils.GetHalfWidthForOffset(this.TroopWidth);
				BoardCell boardCell2 = this.pathCells.Array[this.pathCells.Length - 1];
				int num4 = boardCell2.X + halfWidthForOffset;
				int num5 = boardCell2.Z + halfWidthForOffset;
				SmartEntity smartEntity = null;
				bool flag = true;
				if (this.isTargetShield)
				{
					foreach (BoardItem current in this.targetCell.Children)
					{
						if (GameUtils.IsEntityShieldGenerator((SmartEntity)current.Data))
						{
							smartEntity = (SmartEntity)current.Data;
							break;
						}
					}
					if (smartEntity == null)
					{
						Service.Logger.Error("Pathing believes target is shield generator, however targetCell does not have shield generator entity.");
					}
					else
					{
						flag = Service.ShieldController.IsPositionUnderShield(num4, num5, smartEntity);
					}
				}
				this.RasterLine(num4, num5, this.targetCell.X, this.targetCell.Z, ref boardCellDynamicArray, out num2, out num3);
				for (int k = 0; k < boardCellDynamicArray.Length - 1; k++)
				{
					if (this.isTargetShield && !flag && Service.ShieldController.IsPositionUnderShield(boardCellDynamicArray.Array[k].X, boardCellDynamicArray.Array[k].Z, smartEntity))
					{
						break;
					}
					this.AddEntitiesOnCellToBlockingList(boardCellDynamicArray.Array[k], targetId, entityIds, linkedList, linkedList2, false);
				}
			}
			wallListForCrushing = linkedList2;
			return linkedList;
		}

		private void RasterLine(int x0, int y0, int x1, int y1, ref BoardCellDynamicArray cells, out int flatDist, out int diagDist)
		{
			int num = (x1 <= x0) ? -1 : 1;
			int num2 = (x1 - x0) * num;
			int num3 = (y1 <= y0) ? -1 : 1;
			int num4 = (y1 - y0) * num3;
			flatDist = 0;
			diagDist = 0;
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
						flatDist++;
					}
					else
					{
						num5 += num7;
						num8 += num;
						num9 += num3;
						diagDist++;
					}
					this.AddCell(num8, num9, ref cells);
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
						flatDist++;
					}
					else
					{
						num10 += num12;
						num13 += num;
						num14 += num3;
						diagDist++;
					}
					this.AddCell(num13, num14, ref cells);
				}
			}
		}
	}
}
