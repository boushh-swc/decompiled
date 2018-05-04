using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class ProjectileController
	{
		private const float FLASH_FX_DURATION = 0.3f;

		private EntityFlasher entityFlasher;

		public ProjectileController()
		{
			Service.ProjectileController = this;
			this.entityFlasher = new EntityFlasher();
		}

		public Bullet SpawnProjectileForTargetPosition(uint travelTime, Vector3 spawnWorldLocation, int targetBoardX, int targetBoardZ, HealthFragment payload, TeamType ownerTeam, Entity attacker, ProjectileTypeVO projectileType, List<Buff> appliedBuffs, FactionType faction)
		{
			return this.SpawnProjectileForTargetPosition(travelTime, spawnWorldLocation, targetBoardX, targetBoardZ, payload, ownerTeam, attacker, projectileType, appliedBuffs, faction, false);
		}

		public Bullet SpawnProjectileForTargetPosition(uint travelTime, Vector3 spawnWorldLocation, int targetBoardX, int targetBoardZ, HealthFragment payload, TeamType ownerTeam, Entity attacker, ProjectileTypeVO projectileType, List<Buff> appliedBuffs, FactionType faction, bool preventProjectileView)
		{
			Bullet bullet = new Bullet();
			bullet.InitWithTargetPositionAndTravelTime(travelTime, spawnWorldLocation, targetBoardX, targetBoardZ, ownerTeam, attacker, payload, projectileType, appliedBuffs, faction);
			this.SpawnProjectileInternal(bullet, true, preventProjectileView);
			return bullet;
		}

		public Bullet SpawnProjectileForTarget(uint travelTime, int spawnBoardX, int spawnBoardZ, Vector3 spawnWorldLocation, Target target, HealthFragment payload, TeamType ownerTeam, Entity attacker, ProjectileTypeVO projectileType, bool allowSplash, List<Buff> appliedBuffs, FactionType faction, GameObject gunLocator)
		{
			Bullet newProjectileForTarget = this.GetNewProjectileForTarget(travelTime, spawnBoardX, spawnBoardZ, spawnWorldLocation, target, ownerTeam, attacker, projectileType, payload, appliedBuffs, faction, gunLocator);
			if (newProjectileForTarget != null)
			{
				this.SpawnProjectileInternal(newProjectileForTarget, allowSplash, false);
			}
			return newProjectileForTarget;
		}

		public Bullet SpawnProjectileForDeath(Vector3 spawnWorldLocation, SmartEntity attacker, ProjectileTypeVO deathProjectileType, uint deathProjectileDelay, int deathProjectileDistance, int deathProjectileDamage, out bool useRotation, ref int rotateDegrees)
		{
			useRotation = false;
			TransformComponent transformComp = attacker.TransformComp;
			int num = transformComp.CenterGridX();
			int num2 = transformComp.CenterGridZ();
			if (spawnWorldLocation.y < 0f)
			{
				spawnWorldLocation = new Vector3(Units.BoardToWorldX(num), 0f, Units.BoardToWorldZ(num2));
			}
			Vector3 vector = spawnWorldLocation;
			if (deathProjectileDistance > 0)
			{
				this.ChooseTargetLocationForDeathProjectile(attacker, deathProjectileDistance, ref num, ref num2, out rotateDegrees);
				useRotation = true;
				Vector3 point = Vector3.right * (float)deathProjectileDistance;
				vector += Quaternion.Euler(0f, (float)rotateDegrees, 0f) * point;
			}
			HealthFragment healthFrag = new HealthFragment(attacker, HealthType.Damaging, deathProjectileDamage);
			TeamType teamType = attacker.TeamComp.TeamType;
			Bullet bullet = new Bullet();
			bullet.InitWithTargetPositionAndTravelTime(deathProjectileDelay, spawnWorldLocation, num, num2, teamType, attacker, healthFrag, deathProjectileType, null, FactionType.Invalid);
			bullet.SetTargetWorldLocation(vector);
			bullet.FlashTarget = (deathProjectileType.ApplyBuffs == null || deathProjectileType.ApplyBuffs.Length == 0);
			this.SpawnProjectileInternal(bullet, true, false);
			return bullet;
		}

		private void ChooseTargetLocationForDeathProjectile(SmartEntity attacker, int distance, ref int boardX, ref int boardZ, out int rotateDegrees)
		{
			int num = boardX;
			int num2 = boardZ;
			PathingComponent pathingComp = attacker.PathingComp;
			if (pathingComp != null)
			{
				BoardCell targetCell = pathingComp.TargetCell;
				if (targetCell != null && targetCell.Children != null)
				{
					bool flag = false;
					LinkedListNode<BoardItem> linkedListNode = targetCell.Children.First;
					while (linkedListNode != null && !flag)
					{
						SmartEntity target = (SmartEntity)linkedListNode.Value.Data;
						flag = this.TryGetBoardPositionFromTarget(target, ref num, ref num2);
						linkedListNode = linkedListNode.Next;
					}
				}
			}
			if (num == boardX && num2 == boardZ)
			{
				num = 0;
				num2 = 0;
			}
			if (num == boardX && num2 == boardZ)
			{
				num++;
				num2++;
			}
			int num3 = num - boardX;
			int num4 = num2 - boardZ;
			int num5 = IntMath.FastDist(boardX, boardZ, num, num2);
			int num6 = 1024 * distance * 1;
			int num7 = num5 * 3;
			boardX += num3 * num6 / num7;
			boardZ += num4 * num6 / num7;
			rotateDegrees = -IntMath.Atan2Lookup(num3, num4) * 180 / 16384;
		}

		private bool TryGetBoardPositionFromTarget(SmartEntity target, ref int targetBoardX, ref int targetBoardZ)
		{
			if (target != null)
			{
				TransformComponent transformComp = target.TransformComp;
				if (transformComp != null)
				{
					targetBoardX = transformComp.CenterGridX();
					targetBoardZ = transformComp.CenterGridZ();
					return true;
				}
			}
			return false;
		}

		private void SpawnProjectileInternal(Bullet bullet, bool allowSplash, bool preventProjectileView)
		{
			if (allowSplash && bullet.ProjectileType.SplashRadius > 0)
			{
				bullet.AddSplash(bullet.ProjectileType);
			}
			if (bullet.ProjectileType.IsBeam)
			{
				if (this.ImpactBeamTargets(bullet))
				{
					int maxSpeed = bullet.ProjectileType.MaxSpeed;
					if (maxSpeed > 0)
					{
						uint num = (uint)(1000 / maxSpeed);
						if (num != 0u)
						{
							Service.SimTimerManager.CreateSimTimer(num, true, new TimerDelegate(this.OnBeamProjectileImpact), bullet);
						}
					}
				}
			}
			else
			{
				Service.SimTimerManager.CreateSimTimer(bullet.TravelTime, false, new TimerDelegate(this.OnProjectileImpact), bullet);
			}
			if (!preventProjectileView)
			{
				Service.ProjectileViewManager.SpawnProjectile(bullet);
			}
		}

		private bool ImpactBeamTargets(Bullet bullet)
		{
			BoardCellDynamicArray beamNearbyCells = bullet.GetBeamNearbyCells();
			if (beamNearbyCells.Array == null)
			{
				return false;
			}
			for (int i = 0; i < beamNearbyCells.Length; i++)
			{
				BoardCell boardCell = beamNearbyCells.Array[i];
				if (boardCell.Children != null)
				{
					int beamDamagePercent = bullet.GetBeamDamagePercent(boardCell.X, boardCell.Z);
					if (beamDamagePercent != 0)
					{
						for (LinkedListNode<BoardItem> linkedListNode = boardCell.Children.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
						{
							SmartEntity target = (SmartEntity)linkedListNode.Value.Data;
							bullet.ApplyBeamDamagePercent(target, beamDamagePercent);
						}
					}
				}
			}
			foreach (BeamTarget current in bullet.BeamTargets.Values)
			{
				if (current.HitThisSegment)
				{
					bullet.SetTarget(current.Target);
					this.ImpactSingleTarget(bullet, current.CurDamagePercent, false, true);
					bullet.SetTarget(null);
				}
				current.OnBeamAdvance();
			}
			bullet.AdvanceBeam();
			return true;
		}

		private void ImpactAreaWithSplash(Bullet bullet)
		{
			int targetBoardX = bullet.TargetBoardX;
			int targetBoardZ = bullet.TargetBoardZ;
			int splashRadius = bullet.SplashVO.SplashRadius;
			BoardCellDynamicArray cellsInSquare = Service.BoardController.Board.GetCellsInSquare(splashRadius, targetBoardX, targetBoardZ);
			Dictionary<Entity, bool> dictionary = new Dictionary<Entity, bool>();
			Vector3 targetWorldLocation = bullet.TargetWorldLocation;
			for (int i = 0; i < cellsInSquare.Length; i++)
			{
				BoardCell boardCell = cellsInSquare.Array[i];
				int chessboardDistance = BoardUtils.GetChessboardDistance(boardCell.X, boardCell.Z, targetBoardX, targetBoardZ);
				int splashDamagePercent = bullet.SplashVO.GetSplashDamagePercent(chessboardDistance);
				if (splashDamagePercent != 0)
				{
					ShieldGeneratorComponent activeShieldAffectingBoardPos = Service.ShieldController.GetActiveShieldAffectingBoardPos(boardCell.X, boardCell.Z);
					if (activeShieldAffectingBoardPos != null && !bullet.ProjectileType.PassThroughShield && activeShieldAffectingBoardPos.Entity.Get<TeamComponent>().TeamType != bullet.OwnerTeam)
					{
						TransformComponent transformComponent = activeShieldAffectingBoardPos.Entity.Get<TransformComponent>();
						Vector3 targetPos = new Vector3(Units.BoardToWorldX(transformComponent.CenterX()), transformComponent.CenterX(), Units.BoardToWorldZ(transformComponent.CenterZ()));
						Vector3 zero = Vector3.zero;
						if (Service.ShieldController.GetRayShieldIntersection(targetWorldLocation, targetPos, activeShieldAffectingBoardPos, out zero))
						{
							bullet.SetTargetWorldLocation(zero);
						}
						this.ImpactTargetFromSplashDamage((SmartEntity)activeShieldAffectingBoardPos.ShieldBorderEntity, bullet, splashDamagePercent, ref dictionary);
					}
					else if (boardCell.Children != null)
					{
						LinkedListNode<BoardItem> next;
						for (LinkedListNode<BoardItem> linkedListNode = boardCell.Children.First; linkedListNode != null; linkedListNode = next)
						{
							next = linkedListNode.Next;
							this.ImpactTargetFromSplashDamage((SmartEntity)linkedListNode.Value.Data, bullet, splashDamagePercent, ref dictionary);
						}
					}
				}
			}
		}

		private void ImpactTargetFromSplashDamage(SmartEntity target, Bullet bullet, int effectiveDamagePercentage, ref Dictionary<Entity, bool> entitiesHit)
		{
			if (!entitiesHit.ContainsKey(target))
			{
				entitiesHit.Add(target, true);
				bullet.SetTarget(target);
				this.ImpactSingleTarget(bullet, effectiveDamagePercentage, true, false);
			}
		}

		private void ImpactSingleTarget(Bullet bullet, int effectiveDamagePercentage, bool fromSplash, bool fromBeam)
		{
			Service.EventManager.SendEvent(EventId.EntityHit, bullet);
			SmartEntity target = bullet.Target;
			TeamComponent teamComp = target.TeamComp;
			if (bullet.HealthFrag.Type == HealthType.Damaging && teamComp != null && teamComp.TeamType == bullet.OwnerTeam)
			{
				return;
			}
			if (bullet.HealthFrag.Type == HealthType.Healing)
			{
				if (teamComp != null && teamComp.TeamType != bullet.OwnerTeam)
				{
					return;
				}
				TargetingController targetingController = Service.TargetingController;
				if (!targetingController.CanBeHealed(target, bullet.Owner))
				{
					return;
				}
			}
			IHealthComponent healthComponent;
			if (target.TroopShieldComp != null && target.TroopShieldComp.IsActive())
			{
				healthComponent = target.TroopShieldHealthComp;
			}
			else
			{
				healthComponent = target.HealthComp;
			}
			if (healthComponent != null)
			{
				Service.HealthController.ApplyHealthFragment(healthComponent, bullet.HealthFrag, bullet.ProjectileType.DamageMultipliers, effectiveDamagePercentage, fromSplash, fromBeam, bullet.Owner);
				bool flag = fromBeam && bullet.IsBeamFirstHit(target);
				if (flag)
				{
					Service.EventManager.SendEvent(EventId.EntityHitByBeam, bullet);
				}
				if (bullet.FlashTarget && !healthComponent.IsDead() && (!fromBeam || flag))
				{
					this.entityFlasher.AddFlashing(target, 0.3f, 0f);
				}
			}
		}

		private void ImpactProjectile(Bullet bullet)
		{
			if (bullet.SplashVO != null && bullet.SplashVO.SplashRadius > 0)
			{
				this.ImpactAreaWithSplash(bullet);
			}
			else if (bullet.Target != null)
			{
				this.ImpactSingleTarget(bullet, 100, false, false);
			}
			Service.EventManager.SendEvent(EventId.ProjectileImpacted, bullet);
		}

		private void OnProjectileImpact(uint timerId, object cookie)
		{
			Bullet bullet = (Bullet)cookie;
			this.ImpactProjectile(bullet);
		}

		private void OnBeamProjectileImpact(uint timerId, object cookie)
		{
			Bullet bullet = (Bullet)cookie;
			if (!this.ImpactBeamTargets(bullet))
			{
				Service.SimTimerManager.KillSimTimer(timerId);
			}
		}

		private Bullet GetNewProjectileForTarget(uint travelTime, int spawnBoardX, int spawnBoardZ, Vector3 spawnWorldLocation, Target target, TeamType teamType, Entity attacker, ProjectileTypeVO projectileType, HealthFragment healthFrag, List<Buff> appliedBuffs, FactionType faction, GameObject gunLocator)
		{
			Bullet bullet = new Bullet();
			if (!bullet.InitWithTarget(spawnBoardX, spawnBoardZ, spawnWorldLocation, target, teamType, attacker, healthFrag, projectileType, appliedBuffs, faction, gunLocator))
			{
				return null;
			}
			if (travelTime > 0u)
			{
				bullet.SetTravelTime(travelTime);
			}
			else
			{
				this.SetProjectileTravelTime(bullet, bullet.TargetBoardX, bullet.TargetBoardZ);
			}
			return bullet;
		}

		private void SetProjectileTravelTime(Bullet bullet, int targetBoardX, int targetBoardZ)
		{
			if (bullet.ProjectileType.IsMultiStage)
			{
				bullet.SetTravelTime(bullet.ProjectileType.Stage1Duration + bullet.ProjectileType.StageTransitionDuration + bullet.ProjectileType.Stage2Duration);
			}
			else
			{
				int num = (!bullet.ProjectileType.IsBeam) ? IntMath.FastDist(bullet.SpawnBoardX, bullet.SpawnBoardZ, targetBoardX, targetBoardZ) : ((bullet.ProjectileType.BeamLifeLength - bullet.ProjectileType.BeamInitialZeroes) * 1024);
				uint travelTime = 0u;
				if (bullet.ProjectileType.MaxSpeed > 0)
				{
					int val = num / bullet.ProjectileType.MaxSpeed;
					travelTime = (uint)IntMath.Normalize(0, 1024, val, 0, 1000);
				}
				bullet.SetTravelTime(travelTime);
			}
		}
	}
}
