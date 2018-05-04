using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public class TroopAttackController : AbstractAttackController
	{
		private BoardCell hitShieldCell;

		public TroopAttackController()
		{
			Service.TroopAttackController = this;
		}

		private void EnsurePrimaryTarget(SmartEntity entity)
		{
			if (entity.ShooterComp.Target != null)
			{
				ShooterComponent shooterComp = entity.ShooterComp;
				HealthComponent healthComp = shooterComp.Target.HealthComp;
				if (healthComp == null || healthComp.IsDead())
				{
					Service.TargetingController.InvalidateCurrentTarget(entity);
				}
			}
		}

		private void EnsureTetheredDistance(SmartEntity entity)
		{
			DefenderComponent defenderComp = entity.DefenderComp;
			TransformComponent transformComp = entity.TransformComp;
			if (defenderComp == null || transformComp == null)
			{
				return;
			}
			ShooterComponent shooterComp = entity.ShooterComp;
			if (shooterComp.Target != null && (long)GameUtils.SquaredDistance(transformComp.CenterGridX(), transformComp.CenterGridZ(), defenderComp.SpawnX, defenderComp.SpawnZ) > (long)((ulong)(shooterComp.ShooterVO.ViewRange * shooterComp.ShooterVO.ViewRange)))
			{
				Service.TargetingController.InvalidateCurrentTarget(entity);
			}
		}

		private void GetAttackEndPoints(SmartEntity troop, SmartEntity target, out int selfX, out int selfZ, out int targetX, out int targetZ)
		{
			TransformComponent transformComp = troop.TransformComp;
			selfX = transformComp.CenterGridX();
			selfZ = transformComp.CenterGridZ();
			TransformComponent transformComp2 = target.TransformComp;
			if (troop.ShooterComp.IsMelee)
			{
				targetX = GameUtils.NearestPointOnRect(selfX, transformComp2.MinX(), transformComp2.MaxX());
				targetZ = GameUtils.NearestPointOnRect(selfZ, transformComp2.MinZ(), transformComp2.MaxZ());
			}
			else
			{
				targetX = transformComp2.CenterGridX();
				targetZ = transformComp2.CenterGridZ();
			}
		}

		private bool HasLineOfSight(SmartEntity troop, SmartEntity target)
		{
			int x;
			int y;
			int x2;
			int y2;
			this.GetAttackEndPoints(troop, target, out x, out y, out x2, out y2);
			BoardCell boardCell = null;
			bool result = TroopController.IsEntityHealer(troop) || troop.TeamComp.TeamType == TeamType.Defender || BoardUtils.HasLineOfSight(Service.BoardController.Board, x, y, x2, y2, troop, target, out boardCell);
			if (this.ShouldSeekObstacleTarget(troop))
			{
				this.hitShieldCell = boardCell;
			}
			return result;
		}

		private bool WillCrushNearTarget(SmartEntity troop, SmartEntity target)
		{
			if (!TroopController.CanEntityCrushWalls(troop))
			{
				return false;
			}
			BuildingComponent buildingComp = target.BuildingComp;
			return buildingComp != null && buildingComp.BuildingType.Type == BuildingType.Wall;
		}

		private bool CanCrushNearTargetNow(SmartEntity troop, SmartEntity target)
		{
			TransformComponent transformComp = target.TransformComp;
			if (transformComp == null)
			{
				return false;
			}
			TransformComponent transformComp2 = troop.TransformComp;
			int num = transformComp.CenterGridX() - transformComp2.CenterGridX();
			int num2 = transformComp.CenterGridZ() - transformComp2.CenterGridZ();
			if (num < 0)
			{
				num = -num;
			}
			if (num2 < 0)
			{
				num2 = -num2;
			}
			int num3 = (transformComp.BoardWidth + transformComp2.BoardWidth) / 2;
			return num <= num3 && num2 <= num3;
		}

		private void UpdateTroopShield(SmartEntity Entity)
		{
			if (Entity.TroopShieldComp == null && Entity.TroopShieldHealthComp != null && !Entity.TroopShieldHealthComp.IsDead() && Entity.StateComp.CurState != EntityState.Dying && !Entity.StateComp.IsRunning)
			{
				Entity.Add(new TroopShieldComponent(Entity, Entity.TroopComp.TroopType.ShieldCooldown));
			}
		}

		public void UpdateTroop(SmartEntity troop)
		{
			this.EnsurePrimaryTarget(troop);
			this.UpdateTroopShield(troop);
			SmartEntity troopTarget = this.shooterController.GetTroopTarget(troop);
			for (SmartEntity troopWallCrushingTarget = this.shooterController.GetTroopWallCrushingTarget(troop); troopWallCrushingTarget != null; troopWallCrushingTarget = this.shooterController.GetTroopWallCrushingTarget(troop))
			{
				if (!this.CanCrushNearTargetNow(troop, troopWallCrushingTarget))
				{
					break;
				}
				this.OnTargetWallIsDestroyed(troop, troopWallCrushingTarget);
			}
			while (troopTarget != null)
			{
				HealthComponent healthComp = troopTarget.HealthComp;
				if (healthComp != null && !healthComp.IsDead())
				{
					break;
				}
				this.OnTargetIsDead(troop);
				troopTarget = this.shooterController.GetTroopTarget(troop);
			}
			ShooterComponent shooterComp = troop.ShooterComp;
			bool flag = shooterComp.PrimaryTargetMoved();
			PathingComponent pathingComp = troop.PathingComp;
			bool flag2 = pathingComp == null || pathingComp.CurrentPath == null || pathingComp.GetNextTile() == null;
			if (troopTarget == null)
			{
				this.OnTargetIsNull(troop);
			}
			else if (!this.IsTargetInRangeForAttack(troop, troopTarget, flag2) || (flag && shooterComp.Target != troopTarget))
			{
				this.OnTargetIsOutOfRange(troop, flag);
			}
			else
			{
				if (troopTarget.TransformComp == null)
				{
					base.UpdateShooter(troop);
					return;
				}
				this.hitShieldCell = null;
				if (flag2)
				{
					this.UpdateShieldInLineOfShoot(troop, troopTarget);
					base.UpdateShooter(troop);
					return;
				}
				if (this.HasLineOfSight(troop, troopTarget))
				{
					if (!TroopController.IsEntityHealer(troop) || flag2)
					{
						base.UpdateShooter(troop);
					}
					return;
				}
				this.OnTargetIsOutOfRange(troop, flag2 || flag);
			}
		}

		protected bool IsTargetInRangeForAttack(SmartEntity troop, SmartEntity target, bool isLastTile)
		{
			if (target.ShieldBorderComp != null)
			{
				return true;
			}
			if (this.WillCrushNearTarget(troop, target))
			{
				return false;
			}
			if (isLastTile && GameUtils.IsEntityShieldGenerator(troop.ShooterComp.Target) && troop.ShooterComp.Target != target)
			{
				return true;
			}
			ShooterComponent shooterComp = troop.ShooterComp;
			int num = GameUtils.GetSquaredDistanceToTarget(shooterComp, target);
			TroopComponent troopComp = troop.TroopComp;
			uint num2 = 0u;
			if (troopComp != null)
			{
				if (TroopController.IsEntityHealer(troop))
				{
					num += (int)((shooterComp.MaxAttackRangeSquared - shooterComp.MinAttackRangeSquared) / 2u);
				}
				if (!shooterComp.IsMelee)
				{
					num2 = Service.PathingManager.GetMaxAttackRange(troop, target);
				}
			}
			if (num2 != 0u)
			{
				if (!this.shooterController.InRange(num, shooterComp, num2))
				{
					return false;
				}
			}
			else if (!this.shooterController.InRange(num, shooterComp))
			{
				return false;
			}
			return true;
		}

		private bool StopAttackingIfAttacking(SmartEntity troop)
		{
			if (troop.ShooterComp.AttackFSM.IsAttacking)
			{
				troop.ShooterComp.AttackFSM.StopAttacking(true);
				return true;
			}
			return false;
		}

		protected override void OnTargetIsNull(SmartEntity troop)
		{
			this.StopAttackingIfAttacking(troop);
			if (!troop.ShooterComp.AttackFSM.InStrictCoolDownState())
			{
				this.StartSearch(troop);
			}
			else
			{
				base.UpdateShooter(troop);
			}
		}

		protected void OnTargetIsInvalid(SmartEntity troop)
		{
			Service.TargetingController.InvalidateCurrentTarget(troop);
		}

		protected void OnTargetWallIsDestroyed(SmartEntity troop, SmartEntity wallTarget)
		{
			if (wallTarget != null)
			{
				Service.HealthController.KillEntity(wallTarget);
				SecondaryTargetsComponent secondaryTargetsComp = troop.SecondaryTargetsComp;
				if (secondaryTargetsComp.CurrentWallTarget != null)
				{
					secondaryTargetsComp.CurrentWallTarget = null;
					return;
				}
			}
		}

		protected override void OnTargetIsDead(SmartEntity troop)
		{
			ShooterComponent shooterComp = troop.ShooterComp;
			SecondaryTargetsComponent secondaryTargetsComp = troop.SecondaryTargetsComp;
			this.StopAttackingIfAttacking(troop);
			if (secondaryTargetsComp.ObstacleTarget != null)
			{
				secondaryTargetsComp.ObstacleTarget = null;
				return;
			}
			if (secondaryTargetsComp.CurrentAlternateTarget != null)
			{
				secondaryTargetsComp.CurrentAlternateTarget = null;
				return;
			}
			if (secondaryTargetsComp.CurrentWallTarget != null)
			{
				secondaryTargetsComp.CurrentWallTarget = null;
				return;
			}
			shooterComp.Target = null;
		}

		public void RefreshTarget(SmartEntity troop)
		{
			this.OnTargetIsNull(troop);
		}

		protected void OnTargetIsOutOfRange(SmartEntity troop, bool forceRetarget)
		{
			if (!this.StopAttackingIfAttacking(troop) && !forceRetarget)
			{
				if (troop.StateComp.CurState == EntityState.Idle)
				{
					this.StartSearch(troop);
				}
				return;
			}
			if (troop.TroopComp.TroopShooterVO.TargetLocking)
			{
				PathingComponent pathingComp = troop.PathingComp;
				if (pathingComp != null && !troop.ShooterComp.AttackFSM.InStrictCoolDownState() && GameUtils.IsEligibleToFindTarget(troop.ShooterComp))
				{
					bool flag;
					Service.PathingManager.RestartPathing(troop, out flag, false);
					if (flag)
					{
						this.shooterController.StartMoving(troop);
					}
					else
					{
						GameUtils.UpdateMinimumFrameCountForNextTargeting(troop.ShooterComp);
						Service.Logger.Debug("Could not find a path for healer!");
					}
				}
			}
			else
			{
				this.OnTargetIsInvalid(troop);
				this.StartSearch(troop);
			}
		}

		private void UpdateShieldInLineOfShoot(SmartEntity troop, SmartEntity target)
		{
			if (troop.TeamComp.TeamType == TeamType.Defender || TroopController.IsEntityHealer(troop))
			{
				return;
			}
			int x;
			int y;
			int x2;
			int y2;
			this.GetAttackEndPoints(troop, target, out x, out y, out x2, out y2);
			this.hitShieldCell = BoardUtils.WhereDoesLineCrossFlag(Service.BoardController.Board, x, y, x2, y2, 8u);
		}

		protected override void OnBeforeAttack(SmartEntity troop)
		{
			if (this.hitShieldCell == null)
			{
				return;
			}
			if (this.hitShieldCell.Obstacles != null)
			{
				int i = 0;
				int count = this.hitShieldCell.Obstacles.Count;
				while (i < count)
				{
					if (this.hitShieldCell.Obstacles[i] != null)
					{
						HealthComponent healthComp = ((SmartEntity)this.hitShieldCell.Obstacles[i]).HealthComp;
						if (healthComp != null && !healthComp.IsDead())
						{
							SecondaryTargetsComponent secondaryTargetsComp = troop.SecondaryTargetsComp;
							secondaryTargetsComp.ObstacleTarget = this.hitShieldCell.Obstacles[i];
							secondaryTargetsComp.ObstacleTargetPoint = new Point(this.hitShieldCell.X, this.hitShieldCell.Z);
							break;
						}
					}
					i++;
				}
			}
		}

		protected override void OnAttackBegin(SmartEntity troop)
		{
		}

		protected override void StartSearch(SmartEntity troop)
		{
			troop.ShooterComp.Searching = true;
		}

		private bool ShouldSeekObstacleTarget(SmartEntity troop)
		{
			return !TroopController.IsEntityHealer(troop) && !troop.ShooterComp.ShooterVO.ProjectileType.PassThroughShield && troop.DefenderComp == null;
		}
	}
}
