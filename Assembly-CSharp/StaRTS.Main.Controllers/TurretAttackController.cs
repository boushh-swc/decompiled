using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class TurretAttackController : AbstractAttackController
	{
		public const string ANIM_PROP_MOTIVATOR = "Motivation";

		public TurretAttackController()
		{
			Service.TurretAttackController = this;
		}

		protected bool IsTargetInRange(ShooterComponent shooterComp, SmartEntity target)
		{
			if (target.ShieldBorderComp != null)
			{
				return true;
			}
			int squaredDistanceToTarget = GameUtils.GetSquaredDistanceToTarget(shooterComp, target);
			return this.shooterController.InRange(squaredDistanceToTarget, shooterComp);
		}

		protected bool IsTargetAliveAndInRange(SmartEntity entity, SmartEntity target)
		{
			if (target == null)
			{
				this.OnTargetIsNull(entity);
				return false;
			}
			HealthComponent healthComp = target.HealthComp;
			if (healthComp == null || healthComp.IsDead())
			{
				this.OnTargetIsDead(entity);
				return false;
			}
			if (!this.IsTargetInRange(entity.ShooterComp, target))
			{
				this.OnTargetIsOutOfRange(entity);
				return false;
			}
			return true;
		}

		public void UpdateTurret(SmartEntity entity)
		{
			SmartEntity turretTarget = this.shooterController.GetTurretTarget(entity.ShooterComp);
			if (this.IsTargetAliveAndInRange(entity, turretTarget))
			{
				base.UpdateShooter(entity);
				Animator anim;
				if (entity.TrapViewComp != null && entity.TrapViewComp.TurretAnim != null)
				{
					anim = entity.TrapViewComp.TurretAnim;
				}
				else
				{
					anim = entity.GameObjectViewComp.MainGameObject.GetComponent<Animator>();
				}
				this.UpdateAnimationState(anim, entity.StateComp);
			}
		}

		private void StopAttackingAndStartSearching(SmartEntity entity)
		{
			if (entity.ShooterComp.AttackFSM.IsAttacking)
			{
				entity.ShooterComp.AttackFSM.StopAttacking(false);
			}
			this.StartSearch(entity);
		}

		protected override void OnTargetIsNull(SmartEntity entity)
		{
			this.StopAttackingAndStartSearching(entity);
		}

		protected override void OnTargetIsDead(SmartEntity entity)
		{
			this.StopAttackingAndStartSearching(entity);
		}

		protected void OnTargetIsOutOfRange(SmartEntity entity)
		{
			this.StopAttackingAndStartSearching(entity);
		}

		protected override void OnBeforeAttack(SmartEntity entity)
		{
		}

		protected override void OnAttackBegin(SmartEntity entity)
		{
		}

		protected override void StartSearch(SmartEntity entity)
		{
			entity.ShooterComp.Searching = true;
			entity.ShooterComp.Target = null;
			entity.TurretShooterComp.TargetWeight = -1;
		}

		public void UpdateAnimationState(Animator anim, StateComponent stateComp)
		{
			if (stateComp.Dirty)
			{
				if (anim == null)
				{
					return;
				}
				if (!anim.gameObject.activeInHierarchy)
				{
					return;
				}
				while (stateComp.Dirty)
				{
					EntityState entityState = stateComp.DequeuePrevState();
					if (entityState == EntityState.AttackingReset)
					{
						anim.Play(string.Empty, 0, 0f);
					}
				}
				switch (stateComp.CurState)
				{
				case EntityState.Idle:
					anim.SetInteger("Motivation", 0);
					break;
				case EntityState.Moving:
					anim.SetInteger("Motivation", 1);
					break;
				case EntityState.Turning:
					anim.SetInteger("Motivation", 1);
					break;
				case EntityState.WarmingUp:
					anim.SetInteger("Motivation", 4);
					break;
				case EntityState.Attacking:
				case EntityState.AttackingReset:
					anim.SetInteger("Motivation", 3);
					break;
				case EntityState.CoolingDown:
					anim.SetInteger("Motivation", 4);
					break;
				case EntityState.Dying:
					anim.SetInteger("Motivation", 2);
					break;
				}
			}
		}
	}
}
