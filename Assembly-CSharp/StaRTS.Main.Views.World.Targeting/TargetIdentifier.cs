using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Pooling;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Views.World.Targeting
{
	public class TargetIdentifier : TargetReticlePool, IEventObserver
	{
		public const float RETICLE_LIFE_TIME = 1.5f;

		private const float STARSHIP_RETICLE_OFFSET = 0.5f;

		public TargetIdentifier() : base(new TargetReticlePool.CreatePoolObjectDelegate(TargetReticle.CreateTargetReticlePoolObject), null, null, new TargetReticlePool.DeactivatePoolObjectDelegate(TargetReticle.DeactivateTargetReticlePoolObject))
		{
			Service.EventManager.RegisterObserver(this, EventId.TroopAcquiredFirstTarget, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.SpecialAttackDeployed, EventPriority.Default);
			base.EnsurePoolCapacity(10);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			Entity entity = null;
			if (id != EventId.SpecialAttackDeployed)
			{
				if (id == EventId.TroopAcquiredFirstTarget)
				{
					Entity entity2 = cookie as Entity;
					TeamComponent teamComponent = entity2.Get<TeamComponent>();
					if (teamComponent != null && teamComponent.TeamType != TeamType.Attacker)
					{
						return EatResponse.NotEaten;
					}
					TroopComponent troopComponent = entity2.Get<TroopComponent>();
					if (troopComponent != null && troopComponent.TroopShooterVO.TargetSelf)
					{
						return EatResponse.NotEaten;
					}
					ShooterComponent shooterComponent = entity2.Get<ShooterComponent>();
					if (shooterComponent != null)
					{
						entity = Service.ShooterController.GetPrimaryTarget(shooterComponent);
					}
					if (entity == null)
					{
						return EatResponse.NotEaten;
					}
					TargetReticle fromPool = this.GetFromPool(true);
					fromPool.SetTarget(entity);
					Service.ViewTimerManager.CreateViewTimer(1.5f, false, new TimerDelegate(this.DeactivateTargetReticle), fromPool);
				}
			}
			else
			{
				SpecialAttack specialAttack = cookie as SpecialAttack;
				TargetReticle fromPool2 = this.GetFromPool(true);
				fromPool2.SetWorldTarget((float)specialAttack.TargetBoardX + 0.5f, (float)specialAttack.TargetBoardZ + 0.5f, specialAttack.VO.ReticleScale, specialAttack.VO.ReticleScale);
				Service.ViewTimerManager.CreateViewTimer(specialAttack.VO.ReticleDuration, false, new TimerDelegate(this.DeactivateTargetReticle), fromPool2);
			}
			return EatResponse.NotEaten;
		}

		public void DeactivateTargetReticle(uint id, object cookie)
		{
			this.ReturnToPool(cookie as TargetReticle);
		}
	}
}
