using StaRTS.Main.Controllers.Entities.StateMachines.Attack;
using StaRTS.Main.Models.Entities;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers
{
	public abstract class AbstractAttackController
	{
		protected ShooterController shooterController;

		protected BoardController boardController;

		public AbstractAttackController()
		{
			this.shooterController = Service.ShooterController;
			this.boardController = Service.BoardController;
		}

		protected void UpdateShooter(SmartEntity entity)
		{
			this.TryStartAttack(entity);
			this.UpdateAttackFSM(entity.ShooterComp.AttackFSM);
		}

		private void TryStartAttack(SmartEntity entity)
		{
			if (entity.ShooterComp.AttackFSM.IsAttacking)
			{
				return;
			}
			this.OnBeforeAttack(entity);
			bool flag = entity.ShooterComp.AttackFSM.StartAttack();
			if (flag)
			{
				this.shooterController.Reload(entity.ShooterComp);
				this.OnAttackBegin(entity);
			}
		}

		private void UpdateAttackFSM(AttackFSM attackFSM)
		{
			if (!attackFSM.IsAttacking)
			{
				return;
			}
			if (attackFSM.IsUnlocked())
			{
				attackFSM.Update();
			}
		}

		protected abstract void OnTargetIsNull(SmartEntity entity);

		protected abstract void OnTargetIsDead(SmartEntity entity);

		protected abstract void OnBeforeAttack(SmartEntity entity);

		protected abstract void OnAttackBegin(SmartEntity entity);

		protected abstract void StartSearch(SmartEntity entity);
	}
}
