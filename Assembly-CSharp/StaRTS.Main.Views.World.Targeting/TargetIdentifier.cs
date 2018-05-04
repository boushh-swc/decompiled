using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Pooling;
using StaRTS.Utils.Scheduling;
using System;
using System.Runtime.CompilerServices;

namespace StaRTS.Main.Views.World.Targeting
{
	public class TargetIdentifier : TargetReticlePool, IEventObserver
	{
		public const float RETICLE_LIFE_TIME = 1.5f;

		private const float STARSHIP_RETICLE_OFFSET = 0.5f;

		[CompilerGenerated]
		private static TargetReticlePool.CreatePoolObjectDelegate <>f__mg$cache0;

		[CompilerGenerated]
		private static TargetReticlePool.DeactivatePoolObjectDelegate <>f__mg$cache1;

		public TargetIdentifier()
		{
			if (TargetIdentifier.<>f__mg$cache0 == null)
			{
				TargetIdentifier.<>f__mg$cache0 = new TargetReticlePool.CreatePoolObjectDelegate(TargetReticle.CreateTargetReticlePoolObject);
			}
			TargetReticlePool.CreatePoolObjectDelegate arg_3D_1 = TargetIdentifier.<>f__mg$cache0;
			TargetReticlePool.DestroyPoolObjectDelegate arg_3D_2 = null;
			TargetReticlePool.ActivatePoolObjectDelegate arg_3D_3 = null;
			if (TargetIdentifier.<>f__mg$cache1 == null)
			{
				TargetIdentifier.<>f__mg$cache1 = new TargetReticlePool.DeactivatePoolObjectDelegate(TargetReticle.DeactivateTargetReticlePoolObject);
			}
			base..ctor(arg_3D_1, arg_3D_2, arg_3D_3, TargetIdentifier.<>f__mg$cache1);
			Service.EventManager.RegisterObserver(this, EventId.TroopAcquiredFirstTarget, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.SpecialAttackDeployed, EventPriority.Default);
			base.EnsurePoolCapacity(10);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			Entity entity = null;
			if (id != EventId.TroopAcquiredFirstTarget)
			{
				if (id == EventId.SpecialAttackDeployed)
				{
					SpecialAttack specialAttack = cookie as SpecialAttack;
					TargetReticle fromPool = this.GetFromPool(true);
					fromPool.SetWorldTarget((float)specialAttack.TargetBoardX + 0.5f, (float)specialAttack.TargetBoardZ + 0.5f, specialAttack.VO.ReticleScale, specialAttack.VO.ReticleScale);
					Service.ViewTimerManager.CreateViewTimer(specialAttack.VO.ReticleDuration, false, new TimerDelegate(this.DeactivateTargetReticle), fromPool);
				}
			}
			else
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
				TargetReticle fromPool2 = this.GetFromPool(true);
				fromPool2.SetTarget(entity);
				Service.ViewTimerManager.CreateViewTimer(1.5f, false, new TimerDelegate(this.DeactivateTargetReticle), fromPool2);
			}
			return EatResponse.NotEaten;
		}

		public void DeactivateTargetReticle(uint id, object cookie)
		{
			this.ReturnToPool(cookie as TargetReticle);
		}
	}
}
