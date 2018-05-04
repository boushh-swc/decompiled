using StaRTS.GameBoard.Pathfinding.InternalClasses;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.GameBoard.Pathfinding
{
	public class PathingManager : IEventObserver
	{
		public int FreeCellIndex;

		private List<PathfindingCellInfo> pathingCellPool;

		private BoardController boardController;

		private PathingComponent pathingComp;

		private Path path;

		private TransformComponent transform;

		private SmartEntity troop;

		private SmartEntity target;

		private bool noWall;

		private bool crushesWalls;

		public PathingManager()
		{
			Service.PathingManager = this;
			this.pathingCellPool = new List<PathfindingCellInfo>();
			this.FreeCellIndex = 0;
			this.boardController = Service.BoardController;
			Service.EventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.EntityDestroyed, EventPriority.Default);
		}

		public PathfindingCellInfo GetPathingCell()
		{
			PathfindingCellInfo pathfindingCellInfo;
			if (this.FreeCellIndex == this.pathingCellPool.Count)
			{
				pathfindingCellInfo = new PathfindingCellInfo();
				pathfindingCellInfo.PoolIndex = this.FreeCellIndex;
				this.pathingCellPool.Add(pathfindingCellInfo);
			}
			else
			{
				pathfindingCellInfo = this.pathingCellPool[this.FreeCellIndex];
				if (pathfindingCellInfo.Cell != null)
				{
					pathfindingCellInfo.Cell.PathInfo = null;
					pathfindingCellInfo.Cell = null;
				}
				pathfindingCellInfo.InClosedSet = false;
				pathfindingCellInfo.InRange = false;
				pathfindingCellInfo.PrevCell = null;
			}
			this.FreeCellIndex++;
			return pathfindingCellInfo;
		}

		public void RecycleAllPathingCells()
		{
			this.FreeCellIndex = 0;
		}

		public bool StartPathing(SmartEntity troop, SmartEntity target, TransformComponent transform, bool clampPathLength, out bool found, int rangeOverride, PathTroopParams troopParams, PathBoardParams boardParams, bool isRepathing, bool allowTroopFanning)
		{
			if (troop == null || target == null)
			{
				found = false;
				return true;
			}
			if (this.IsPathingOngoing())
			{
				found = false;
				return false;
			}
			boardParams.IgnoreWall = (boardParams.IgnoreWall || troop.TroopComp.TroopType.IsFlying);
			this.troop = troop;
			this.target = target;
			this.noWall = boardParams.IgnoreWall;
			this.crushesWalls = troopParams.CrushesWalls;
			this.transform = transform;
			SmartEntity smartEntity = (SmartEntity)transform.Entity;
			int x = transform.X;
			int z = transform.Z;
			BoardCell cellAt = this.boardController.Board.GetCellAt(x, z);
			BoardCell boardCell = null;
			DamageableComponent damageableComp = target.DamageableComp;
			ShooterComponent shooterComp = troop.ShooterComp;
			this.pathingComp = smartEntity.PathingComp;
			BoardCell boardCell2;
			if (this.pathingComp != null && isRepathing && this.pathingComp.TargetCell != null && this.pathingComp.EndCell != null && damageableComp != null)
			{
				boardCell2 = this.pathingComp.TargetCell;
				boardCell = this.pathingComp.EndCell;
			}
			else if (damageableComp != null)
			{
				uint maxRange = (rangeOverride < 0) ? shooterComp.ShooterVO.MaxAttackRange : ((uint)rangeOverride);
				boardCell = damageableComp.FindAttackCell(maxRange, troopParams.TroopWidth, x, z, allowTroopFanning);
				int x2;
				int z2;
				damageableComp.GetCenterPosition(out x2, out z2);
				boardCell2 = this.boardController.Board.GetCellAt(x2, z2);
			}
			else
			{
				TransformComponent transformComp = target.TransformComp;
				boardCell2 = this.boardController.Board.GetCellAt(transformComp.CenterGridX(), transformComp.CenterGridZ());
				int squaredDistanceToTarget = GameUtils.GetSquaredDistanceToTarget(shooterComp, target);
				bool flag = (long)squaredDistanceToTarget > (long)((ulong)shooterComp.MaxAttackRangeSquared);
				if (boardCell2 != cellAt && !troopParams.IsHealer)
				{
					if (flag)
					{
						int x3 = boardCell2.X - cellAt.X;
						int y = boardCell2.Z - cellAt.Z;
						int num = IntMath.Atan2Lookup(x3, y) * 180 / 16384;
						int targetCount = target.TroopComp.TargetCount;
						int num2 = (int)((troopParams.MaxRange <= 1u) ? 1u : (troopParams.MaxRange - 1u));
						int num3 = num2;
						while (num3 > 0 && boardCell == null)
						{
							int num4 = num3 * 3 + 1;
							int num5 = 180 / num4;
							for (int i = 0; i < num4; i++)
							{
								int num6 = (i + targetCount) % num4;
								int twiceAngle = (num + 90 + num6 * num5) % 360 * 2;
								int x4 = boardCell2.X + IntMath.cosLookup(twiceAngle) * num3 / 1024;
								int z3 = boardCell2.Z + IntMath.sinLookup(twiceAngle) * num3 / 1024;
								boardCell = this.boardController.Board.GetClampedToBoardCellAt(x4, z3, troopParams.TroopWidth);
								if ((boardCell.Children == null || boardCell.Children.Count == 0) && boardCell.Clearance >= troopParams.TroopWidth)
								{
									break;
								}
								boardCell = null;
							}
							num3--;
						}
					}
					else
					{
						boardCell = cellAt;
					}
				}
				if (boardCell == null)
				{
					boardCell = boardCell2;
					if (boardCell.Clearance < troopParams.TroopWidth)
					{
						this.pathingComp = null;
						found = false;
						return false;
					}
				}
			}
			WalkerComponent walkerComp = smartEntity.WalkerComp;
			if (this.pathingComp == null)
			{
				this.pathingComp = new PathingComponent(walkerComp.SpeedVO.MaxSpeed, target);
			}
			else
			{
				this.pathingComp.Reset();
				this.pathingComp.MaxSpeed = walkerComp.SpeedVO.MaxSpeed;
				this.pathingComp.Target = target;
			}
			this.pathingComp.TargetCell = boardCell2;
			this.pathingComp.EndCell = boardCell;
			int maxLength = -1;
			BattleController battleController = Service.BattleController;
			if (clampPathLength && (battleController == null || battleController.GetCurrentBattle().Type == BattleType.Pvp || battleController.GetCurrentBattle().Type == BattleType.ClientBattle))
			{
				int num7 = IntMath.FastDist(cellAt.X, cellAt.Z, boardCell2.X, boardCell2.Z);
				maxLength = 2 * num7 / 1024 + 2;
			}
			this.path = new Path(cellAt, boardCell, boardCell2, maxLength, troopParams, boardParams);
			this.UpdatePathing(out found);
			return true;
		}

		public bool StartPathingWorkerOrPatrol(SmartEntity entity, SmartEntity target, BoardCell startCell, BoardCell endCell, int size, bool attemptNoWall)
		{
			if (this.IsPathingOngoing())
			{
				return false;
			}
			this.boardController.Board.RefreshClearanceMap();
			this.boardController.Board.RefreshClearanceMapNoWall();
			WalkerComponent walkerComp = entity.WalkerComp;
			this.pathingComp = entity.PathingComp;
			if (this.pathingComp == null)
			{
				this.pathingComp = new PathingComponent(walkerComp.SpeedVO.MaxSpeed, target);
			}
			else
			{
				this.pathingComp.Reset();
				this.pathingComp.MaxSpeed = walkerComp.SpeedVO.MaxSpeed;
				this.pathingComp.Target = target;
			}
			bool ignoreWall = false;
			uint pathSearchWidth = 1u;
			if (entity.TroopComp != null)
			{
				pathSearchWidth = entity.TroopComp.TroopType.PathSearchWidth;
			}
			if (entity.DroidComp != null)
			{
				ignoreWall = true;
			}
			if (entity.TeamComp != null)
			{
				ignoreWall = entity.TeamComp.IsDefender();
			}
			this.pathingComp.CurrentPath = new Path(startCell, endCell, endCell, -1, new PathTroopParams
			{
				TroopWidth = size,
				DPS = 0,
				MinRange = 0u,
				MaxRange = 1u,
				MaxSpeed = walkerComp.SpeedVO.MaxSpeed,
				PathSearchWidth = pathSearchWidth,
				IsMelee = true,
				IsOverWall = false,
				IsHealer = false,
				CrushesWalls = false,
				IsTargetShield = false,
				TargetInRangeModifier = 1u
			}, new PathBoardParams
			{
				IgnoreWall = ignoreWall,
				Destructible = false
			});
			bool flag;
			this.pathingComp.CurrentPath.CalculatePath(out flag);
			if (!flag && attemptNoWall)
			{
				this.boardController.Board.RefreshClearanceMapNoWall();
				this.pathingComp.CurrentPath = new Path(startCell, endCell, endCell, -1, new PathTroopParams
				{
					TroopWidth = size,
					DPS = 0,
					MinRange = 0u,
					MaxRange = 1u,
					MaxSpeed = walkerComp.SpeedVO.MaxSpeed,
					PathSearchWidth = pathSearchWidth,
					IsMelee = true,
					IsOverWall = false,
					IsHealer = false,
					CrushesWalls = false,
					IsTargetShield = false,
					TargetInRangeModifier = 1u
				}, new PathBoardParams
				{
					IgnoreWall = true,
					Destructible = false
				});
				this.pathingComp.CurrentPath.CalculatePath(out flag);
			}
			if (flag)
			{
				entity.Add<PathingComponent>(this.pathingComp);
				this.pathingComp.InitializePathView();
			}
			else
			{
				this.pathingComp.CurrentPath = null;
			}
			this.pathingComp = null;
			return flag;
		}

		public bool IsPathingOngoing()
		{
			return this.pathingComp != null;
		}

		private void SetPathToTarget()
		{
			SmartEntity smartEntity = (SmartEntity)this.transform.Entity;
			this.pathingComp.CurrentPath = this.path;
			smartEntity.Add<PathingComponent>(this.pathingComp);
			this.pathingComp.InitializePathView();
			this.pathingComp = null;
			this.path = null;
		}

		public void UpdatePathing(out bool found)
		{
			SmartEntity smartEntity = null;
			this.UpdatePathing(out found, out smartEntity);
		}

		public void UpdatePathing(out bool found, out SmartEntity walker)
		{
			walker = null;
			if (this.noWall)
			{
				this.boardController.Board.RefreshClearanceMapNoWall();
				if (this.crushesWalls)
				{
					this.boardController.Board.RefreshClearanceMap();
				}
			}
			else
			{
				this.boardController.Board.RefreshClearanceMap();
			}
			if (this.path == null)
			{
				string text = string.Empty;
				if (this.pathingComp == null)
				{
					text = "nullPathingComp";
				}
				else if (this.pathingComp.Entity == null)
				{
					text = "nullEntity";
				}
				else
				{
					SmartEntity smartEntity = (SmartEntity)this.pathingComp.Entity;
					if (smartEntity.TroopComp == null)
					{
						text = "nullTroopComp";
					}
					else
					{
						text = smartEntity.TroopComp.TroopType.Uid;
					}
				}
				Service.Logger.WarnFormat("UpdatePathing to null path for {0}", new object[]
				{
					text
				});
				found = false;
				walker = null;
				return;
			}
			this.path.CalculatePath(out found);
			if (found)
			{
				this.SetPathToTarget();
				walker = this.troop;
			}
			this.pathingComp = null;
		}

		public uint GetMaxAttackRange(SmartEntity troopEntity, SmartEntity targetEntity)
		{
			uint num = troopEntity.ShooterComp.ShooterVO.MaxAttackRange;
			if (targetEntity.ShieldGeneratorComp != null && troopEntity.TroopComp.TroopType.AttackShieldBorder)
			{
				num += (uint)targetEntity.ShieldGeneratorComp.CurrentRadius;
			}
			return num;
		}

		public bool RestartPathing(SmartEntity troopEntity, out bool found, bool ignoreWall)
		{
			ShooterComponent shooterComp = troopEntity.ShooterComp;
			IShooterVO shooterVO = shooterComp.ShooterVO;
			TroopComponent troopComp = troopEntity.TroopComp;
			TeamComponent teamComp = troopEntity.TeamComp;
			ITroopDeployableVO troopType = troopComp.TroopType;
			uint maxAttackRange = this.GetMaxAttackRange(troopEntity, shooterComp.Target);
			PathTroopParams troopParams = new PathTroopParams
			{
				TroopWidth = troopEntity.SizeComp.Width,
				DPS = shooterVO.DPS,
				MinRange = shooterVO.MinAttackRange,
				MaxRange = maxAttackRange,
				MaxSpeed = troopComp.SpeedVO.MaxSpeed,
				PathSearchWidth = troopType.PathSearchWidth,
				IsMelee = shooterComp.IsMelee,
				IsOverWall = shooterVO.OverWalls,
				IsHealer = TroopController.IsEntityHealer(troopEntity),
				SupportRange = troopType.SupportFollowDistance,
				ProjectileType = shooterVO.ProjectileType,
				CrushesWalls = TroopController.CanEntityCrushWalls(troopEntity),
				IsTargetShield = GameUtils.IsEntityShieldGenerator(troopEntity.ShooterComp.Target),
				TargetInRangeModifier = troopType.TargetInRangeModifier
			};
			PathBoardParams boardParams = new PathBoardParams
			{
				IgnoreWall = ignoreWall || TroopController.CanEntityIgnoreWalls(troopEntity),
				Destructible = teamComp.CanDestructBuildings()
			};
			return this.StartPathing(troopEntity, shooterComp.Target, troopEntity.TransformComp, true, out found, -1, troopParams, boardParams, true, false);
		}

		public virtual EatResponse OnEvent(EventId id, object cookie)
		{
			if (this.pathingComp == null)
			{
				return EatResponse.NotEaten;
			}
			if (id != EventId.EntityDestroyed)
			{
				if (id == EventId.EntityKilled)
				{
					SmartEntity smartEntity = (SmartEntity)cookie;
					if (smartEntity == this.target || smartEntity == this.troop)
					{
						this.pathingComp = null;
					}
				}
			}
			else
			{
				uint num = (uint)cookie;
				if (num == this.target.ID || num == this.troop.ID)
				{
					this.pathingComp = null;
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
