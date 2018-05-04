using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.Main.Controllers;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class DamageableComponent : ComponentBase
	{
		private const int NUM_OF_QUADRANTS = 4;

		private static readonly int[,] rotation = new int[,]
		{
			{
				1,
				1
			},
			{
				-1,
				1
			},
			{
				-1,
				-1
			},
			{
				1,
				-1
			}
		};

		private static readonly int[][,] attackOffsets;

		private static readonly int[] neighborsToEvaluateEachSide;

		private const int SMALL_BUILDING_AREA = 9;

		private const int NEIGHBORS_TO_EVALUATE_FOR_MELEES_VS_SMALL_BUILDINGS = 1;

		private bool isSmallBuilding;

		private int[][][] attackPositions;

		private int[][] attackWeights;

		private TransformComponent transform;

		private BoardController board;

		private int lastSpawnIndex;

		private int lastAttackIndex;

		public DamageableComponent(TransformComponent transform)
		{
			this.transform = transform;
			this.isSmallBuilding = (transform.BoardDepth * transform.BoardWidth < 9);
			this.board = Service.BoardController;
		}

		public BoardCell FindAttackCell(uint maxRange, int troopWidth, int startX, int startZ)
		{
			return this.FindAttackCell(maxRange, troopWidth, startX, startZ, false);
		}

		public BoardCell FindAttackCell(uint maxRange, int troopWidth, int startX, int startZ, bool allowTroopFanning)
		{
			Service.BoardController.Board.RefreshClearanceMap();
			bool flag = false;
			BoardCell boardCellWithMinimumWeight = this.GetBoardCellWithMinimumWeight(maxRange, startX, startZ, troopWidth, out flag, allowTroopFanning);
			if (!flag && boardCellWithMinimumWeight.Clearance < troopWidth && this.FindNextSafeSpot(troopWidth, out boardCellWithMinimumWeight, ref this.lastAttackIndex) == null)
			{
				Service.Logger.WarnFormat("Failed to find safe attack spot for {0}", new object[]
				{
					this.Entity.Get<BuildingComponent>().BuildingType.Uid
				});
				return null;
			}
			return boardCellWithMinimumWeight;
		}

		private BoardCell FindNextSafeSpot(int troopWidth, out BoardCell pathCell, ref int lastIndex)
		{
			int num = lastIndex;
			while (true)
			{
				pathCell = this.AdvanceIndex(troopWidth, ref lastIndex);
				if (pathCell == null)
				{
					break;
				}
				if (pathCell.Clearance >= troopWidth)
				{
					goto IL_28;
				}
				if (lastIndex == num)
				{
					goto Block_3;
				}
			}
			return null;
			IL_28:
			return this.board.Board.GetCellAt(pathCell.X + troopWidth / 2, pathCell.Z + troopWidth / 2);
			Block_3:
			return null;
		}

		public BoardCell FindASafeSpawnSpot(int troopWidth, out BoardCell pathCell)
		{
			return this.FindNextSafeSpot(troopWidth, out pathCell, ref this.lastSpawnIndex);
		}

		public BoardCell FindNextPatrolPoint(int troopWidth, ref int locIndex)
		{
			this.AdvanceQuadrant(ref locIndex);
			BoardCell result;
			if (this.FindNextSafeSpot(troopWidth, out result, ref locIndex) != null)
			{
				return result;
			}
			return null;
		}

		public void GetCenterPosition(out int targetX, out int targetZ)
		{
			targetX = this.transform.CenterGridX();
			targetZ = this.transform.CenterGridZ();
		}

		public int GetLastSpawnIndex()
		{
			return this.lastSpawnIndex;
		}

		public void Init()
		{
			int num = DamageableComponent.attackOffsets.Length - 4 + 1;
			this.attackPositions = new int[num][][];
			this.attackWeights = new int[num][];
			int num2 = 0;
			int num3 = 0;
			int num4 = this.transform.BoardDepth * 2 + this.transform.BoardWidth * 2 + 4;
			this.attackPositions[num2] = new int[num4][];
			this.attackWeights[num2] = new int[num4];
			int num5 = this.transform.MinX() - 1;
			int num6 = this.transform.MaxX() + 1;
			int num7 = this.transform.MinZ() - 1;
			int num8 = this.transform.MaxZ() + 1;
			int num9 = this.transform.CenterGridX();
			int num10 = this.transform.CenterGridZ();
			int i = num6;
			int j;
			for (j = num7; j <= num8; j++)
			{
				this.attackPositions[num2][num3] = new int[]
				{
					i,
					j,
					GameUtils.SquaredDistance(i, j, num9, num10)
				};
				num3++;
			}
			j = num8;
			for (i = num6 - 1; i >= num5; i--)
			{
				this.attackPositions[num2][num3] = new int[]
				{
					i,
					j,
					GameUtils.SquaredDistance(i, j, num9, num10)
				};
				num3++;
			}
			i = num5;
			for (j = num8 - 1; j >= num7; j--)
			{
				this.attackPositions[num2][num3] = new int[]
				{
					i,
					j,
					GameUtils.SquaredDistance(i, j, num9, num10)
				};
				num3++;
			}
			j = num7;
			for (i = num5 + 1; i < num6; i++)
			{
				this.attackPositions[num2][num3] = new int[]
				{
					i,
					j,
					GameUtils.SquaredDistance(i, j, num9, num10)
				};
				num3++;
			}
			int[,] expr_1ED = new int[4, 2];
			expr_1ED[0, 0] = num9;
			expr_1ED[0, 1] = num10;
			expr_1ED[1, 0] = num9;
			expr_1ED[1, 1] = num10;
			expr_1ED[2, 0] = num9;
			expr_1ED[2, 1] = num10;
			expr_1ED[3, 0] = num9;
			expr_1ED[3, 1] = num10;
			int[,] array = expr_1ED;
			if (this.transform.BoardWidth % 2 == 0)
			{
				array[1, 0]--;
				array[2, 0]--;
			}
			if (this.transform.BoardDepth % 2 == 0)
			{
				array[2, 1]--;
				array[3, 1]--;
			}
			for (int k = 4; k < DamageableComponent.attackOffsets.Length; k++)
			{
				num3 = 0;
				num2 = k - 4 + 1;
				this.attackPositions[num2] = new int[DamageableComponent.attackOffsets[k].GetLength(0) * 4][];
				this.attackWeights[num2] = new int[DamageableComponent.attackOffsets[k].GetLength(0) * 4];
				for (int l = 0; l < 4; l++)
				{
					for (int m = 0; m < DamageableComponent.attackOffsets[k].GetLength(0); m++)
					{
						this.attackPositions[num2][num3] = this.CalculateAttackPosition(array[l, 0], array[l, 1], k, l, m);
						num3++;
					}
				}
			}
			this.lastSpawnIndex = 0;
			this.lastAttackIndex = 0;
		}

		private int[] CalculateAttackPosition(int centerX, int centerZ, int rangeIndex, int quadrant, int positionIndex)
		{
			int num = quadrant & 1;
			int num2 = num ^ 1;
			int num3 = DamageableComponent.attackOffsets[rangeIndex][positionIndex, num] * DamageableComponent.rotation[quadrant, 0];
			int num4 = DamageableComponent.attackOffsets[rangeIndex][positionIndex, num2] * DamageableComponent.rotation[quadrant, 1];
			return new int[]
			{
				centerX + num3,
				centerZ + num4,
				num3 * num3 + num4 * num4
			};
		}

		private int FindClosetPointIndex(int range, int startX, int startZ, uint maxRange)
		{
			int[][] array = this.attackPositions[range];
			uint num = maxRange * maxRange;
			int num2 = GameUtils.SquaredDistance(startX, startZ, array[0][0], array[0][1]);
			int result = 0;
			for (int i = 0; i < array.Length; i++)
			{
				int num3 = GameUtils.SquaredDistance(startX, startZ, array[i][0], array[i][1]);
				if (num3 < num2 && ((ulong)num >= (ulong)((long)array[i][2]) || maxRange < 4u))
				{
					num2 = num3;
					result = i;
				}
			}
			return result;
		}

		private BoardCell GetBoardCellWithMinimumWeight(uint maxRange, int startX, int startZ, int troopWidth, out bool isIdealCell, bool allowTroopFanning)
		{
			int num = (int)(maxRange - 4u + 1u);
			uint num2 = maxRange * maxRange;
			if (num < 0)
			{
				num = 0;
			}
			if (num > this.attackPositions.Length - 1)
			{
				num = this.attackPositions.Length - 1;
			}
			int i = 0;
			int num3 = this.FindClosetPointIndex(i, startX, startZ, maxRange);
			BoardCell boardCellFromIndex = this.GetBoardCellFromIndex(i, num3, troopWidth);
			int num4 = this.attackWeights[i][num3];
			int[][] array = this.attackPositions[i];
			if (boardCellFromIndex.Clearance >= troopWidth && !this.board.Board.DoesBoardCellHasBlockerAround(boardCellFromIndex.X, boardCellFromIndex.Z, troopWidth) && (!allowTroopFanning || num4 <= 0) && (ulong)num2 >= (ulong)((long)array[num3][2]))
			{
				this.attackWeights[i][num3]++;
				isIdealCell = true;
				return boardCellFromIndex;
			}
			for (int j = troopWidth; j >= 0; j--)
			{
				for (i = 0; i <= num; i++)
				{
					array = this.attackPositions[i];
					int num5;
					if (i == 0)
					{
						num5 = num3;
					}
					else
					{
						num5 = this.FindClosetPointIndex(i, startX, startZ, maxRange);
						BoardCell boardCellFromIndex2 = this.GetBoardCellFromIndex(i, num5, troopWidth);
						if (this.attackWeights[i][num5] <= num4 && boardCellFromIndex2.Clearance >= j && (ulong)num2 >= (ulong)((long)array[num5][2]))
						{
							this.attackWeights[i][num5]++;
							isIdealCell = false;
							return boardCellFromIndex2;
						}
					}
					int numNeightborsToEvaluate = this.GetNumNeightborsToEvaluate(i);
					for (int k = 1; k <= numNeightborsToEvaluate; k++)
					{
						int num6 = num5 + k;
						if (num6 >= this.attackWeights[i].Length)
						{
							num6 -= this.attackWeights[i].Length;
						}
						if (this.attackWeights[i][num6] <= num4 && (ulong)num2 >= (ulong)((long)array[num6][2]))
						{
							BoardCell boardCellFromIndex3 = this.GetBoardCellFromIndex(i, num6, troopWidth);
							if (boardCellFromIndex3.Clearance >= j)
							{
								this.attackWeights[i][num6]++;
								isIdealCell = false;
								return boardCellFromIndex3;
							}
						}
						int num7 = num5 - k;
						if (num7 < 0)
						{
							num7 = this.attackWeights[i].Length + num7;
						}
						if (this.attackWeights[i][num7] <= num4 && (ulong)num2 >= (ulong)((long)array[num7][2]))
						{
							BoardCell boardCellFromIndex3 = this.GetBoardCellFromIndex(i, num7, troopWidth);
							if (boardCellFromIndex3.Clearance >= j)
							{
								this.attackWeights[i][num7]++;
								isIdealCell = false;
								return boardCellFromIndex3;
							}
						}
					}
				}
			}
			this.attackWeights[0][num3]++;
			isIdealCell = true;
			return boardCellFromIndex;
		}

		private int GetNumNeightborsToEvaluate(int rangeIndex)
		{
			int num = rangeIndex + 4 - 1;
			if (num < 4 && this.isSmallBuilding)
			{
				return 1;
			}
			if (num < 0)
			{
				return DamageableComponent.neighborsToEvaluateEachSide[num];
			}
			if (num > DamageableComponent.neighborsToEvaluateEachSide.Length - 1)
			{
				return DamageableComponent.neighborsToEvaluateEachSide[this.attackPositions.Length - 1];
			}
			return DamageableComponent.neighborsToEvaluateEachSide[num];
		}

		private BoardCell GetBoardCellFromIndex(int rangeIndex, int positionIndex, int troopWidth)
		{
			return this.board.Board.GetClampedToBoardCellAt(this.attackPositions[rangeIndex][positionIndex][0], this.attackPositions[rangeIndex][positionIndex][1], troopWidth);
		}

		private BoardCell AdvanceIndex(int troopWidth, ref int lastIndex)
		{
			int num = 0;
			for (int i = 0; i < this.attackPositions.Length; i++)
			{
				int num2 = this.attackPositions[i].Length;
				if (num <= lastIndex && lastIndex < num + num2)
				{
					int num3 = lastIndex - num + 1;
					if (num3 >= num2)
					{
						if (++i >= this.attackPositions.Length)
						{
							num3 = (i = 0);
						}
						else
						{
							num3 = 0;
						}
					}
					lastIndex++;
					return this.board.Board.GetClampedToBoardCellAt(this.attackPositions[i][num3][0], this.attackPositions[i][num3][1], troopWidth);
				}
				num += this.attackPositions[i].Length;
			}
			Service.Logger.ErrorFormat("Failed to AdvanceIndex: lastIndex {0}", new object[]
			{
				this.lastSpawnIndex
			});
			return null;
		}

		private void AdvanceQuadrant(ref int lastIndex)
		{
			int num = 0;
			int i = 0;
			while (i < this.attackPositions.Length)
			{
				int num2 = this.attackPositions[i].Length;
				if (num <= lastIndex && lastIndex < num + num2)
				{
					int num3 = (lastIndex - num + num2 / 4) % num2;
					lastIndex = num + num3;
					return;
				}
				num += this.attackPositions[i++].Length;
			}
			Service.Logger.ErrorFormat("Failed to AdvanceQuadrant: lastIndex {0}", new object[]
			{
				lastIndex
			});
		}

		static DamageableComponent()
		{
			// Note: this type is marked as 'beforefieldinit'.
			int[][,] expr_1E = new int[13][,];
			expr_1E[0] = new int[1, 2];
			int arg_3A_1 = 1;
			int[,] expr_31 = new int[1, 2];
			expr_31[0, 0] = 1;
			expr_1E[arg_3A_1] = expr_31;
			expr_1E[2] = new int[,]
			{
				{
					2,
					0
				},
				{
					1,
					1
				}
			};
			expr_1E[3] = new int[,]
			{
				{
					3,
					0
				},
				{
					2,
					2
				}
			};
			expr_1E[4] = new int[,]
			{
				{
					4,
					0
				},
				{
					3,
					2
				},
				{
					2,
					3
				}
			};
			expr_1E[5] = new int[,]
			{
				{
					5,
					0
				},
				{
					4,
					2
				},
				{
					3,
					3
				},
				{
					2,
					4
				}
			};
			expr_1E[6] = new int[,]
			{
				{
					6,
					0
				},
				{
					5,
					3
				},
				{
					4,
					4
				},
				{
					3,
					5
				}
			};
			expr_1E[7] = new int[,]
			{
				{
					7,
					0
				},
				{
					6,
					2
				},
				{
					5,
					4
				},
				{
					4,
					5
				},
				{
					2,
					6
				}
			};
			expr_1E[8] = new int[,]
			{
				{
					8,
					0
				},
				{
					7,
					3
				},
				{
					6,
					5
				},
				{
					5,
					6
				},
				{
					3,
					7
				}
			};
			expr_1E[9] = new int[,]
			{
				{
					9,
					0
				},
				{
					9,
					1
				},
				{
					8,
					4
				},
				{
					6,
					6
				},
				{
					4,
					8
				},
				{
					1,
					9
				}
			};
			expr_1E[10] = new int[,]
			{
				{
					10,
					0
				},
				{
					10,
					2
				},
				{
					9,
					5
				},
				{
					7,
					7
				},
				{
					5,
					9
				},
				{
					2,
					10
				}
			};
			expr_1E[11] = new int[,]
			{
				{
					11,
					0
				},
				{
					11,
					3
				},
				{
					10,
					6
				},
				{
					8,
					8
				},
				{
					6,
					10
				},
				{
					3,
					11
				}
			};
			expr_1E[12] = new int[,]
			{
				{
					12,
					0
				},
				{
					12,
					3
				},
				{
					11,
					6
				},
				{
					10,
					8
				},
				{
					8,
					10
				},
				{
					6,
					11
				},
				{
					3,
					12
				}
			};
			DamageableComponent.attackOffsets = expr_1E;
			DamageableComponent.neighborsToEvaluateEachSide = new int[]
			{
				1,
				1,
				2,
				2,
				2,
				3,
				3,
				2,
				2,
				3,
				3,
				3,
				4
			};
		}
	}
}
