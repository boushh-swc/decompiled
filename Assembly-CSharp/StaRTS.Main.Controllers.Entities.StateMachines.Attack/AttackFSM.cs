using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.Entities.StateMachines.Attack
{
	public class AttackFSM : TimeLockedStateMachine
	{
		public delegate void StateChangeCallback(AttackFSM attackFSM, IState prevState, IState curState);

		public const uint MAX_STATE_CHANGES_PER_TICK = 100u;

		private ShooterController shooterController;

		private HealthType healthType;

		private TeamType ownerTeam;

		private int lastLookAtX;

		private int lastLookAtZ;

		private int shotIndex;

		private Dictionary<int, int> gunSequences;

		public SmartEntity Entity
		{
			get;
			protected set;
		}

		public ShooterComponent ShooterComp
		{
			get;
			protected set;
		}

		public StateComponent StateComponent
		{
			get;
			protected set;
		}

		public TransformComponent TransformComponent
		{
			get;
			protected set;
		}

		public IdleState IdleState
		{
			get;
			protected set;
		}

		public TurnState TurnState
		{
			get;
			protected set;
		}

		public WarmupState WarmupState
		{
			get;
			protected set;
		}

		public PreFireState PreFireState
		{
			get;
			protected set;
		}

		public PostFireState PostFireState
		{
			get;
			protected set;
		}

		public CooldownState CooldownState
		{
			get;
			protected set;
		}

		public StunState StunState
		{
			get;
			protected set;
		}

		public bool IsAttacking
		{
			get
			{
				return base.CurrentState != this.IdleState;
			}
		}

		public AttackFSM(ISimTimeProvider timeProvider, SmartEntity entity, StateComponent stateComponent, ShooterComponent shooterComp, TransformComponent transformComponent, HealthType healthType) : base(timeProvider)
		{
			this.Initialize(entity, shooterComp, stateComponent, transformComponent, healthType);
		}

		public uint getTurnDelayInMs()
		{
			return this.TurnState.GetDuration();
		}

		private void Initialize(SmartEntity entity, ShooterComponent shooterComp, StateComponent stateComponent, TransformComponent transformComponent, HealthType healthType)
		{
			this.Entity = entity;
			this.shooterController = Service.ShooterController;
			this.ShooterComp = shooterComp;
			this.StateComponent = stateComponent;
			this.TransformComponent = transformComponent;
			this.IdleState = new IdleState(this);
			this.TurnState = new TurnState(this);
			this.WarmupState = new WarmupState(this);
			this.PreFireState = new PreFireState(this);
			this.PostFireState = new PostFireState(this);
			this.CooldownState = new CooldownState(this);
			this.StunState = new StunState(this);
			this.shooterController.Reload(shooterComp);
			base.SetLegalTransition<IdleState, StunState>();
			base.SetLegalTransition<IdleState, TurnState>();
			base.SetLegalTransition<IdleState, WarmupState>();
			base.SetLegalTransition<TurnState, IdleState>();
			base.SetLegalTransition<TurnState, StunState>();
			base.SetLegalTransition<TurnState, WarmupState>();
			base.SetLegalTransition<WarmupState, IdleState>();
			base.SetLegalTransition<WarmupState, PreFireState>();
			base.SetLegalTransition<WarmupState, StunState>();
			base.SetLegalTransition<PreFireState, IdleState>();
			base.SetLegalTransition<PreFireState, PostFireState>();
			base.SetLegalTransition<PreFireState, StunState>();
			base.SetLegalTransition<PostFireState, CooldownState>();
			base.SetLegalTransition<PostFireState, IdleState>();
			base.SetLegalTransition<PostFireState, PreFireState>();
			base.SetLegalTransition<PostFireState, StunState>();
			base.SetLegalTransition<CooldownState, IdleState>();
			base.SetLegalTransition<CooldownState, StunState>();
			base.SetLegalTransition<CooldownState, WarmupState>();
			base.SetLegalTransition<StunState, IdleState>();
			if (entity.TroopComp != null && entity.TroopComp.TroopType.Type == TroopType.Champion && entity.TeamComp.TeamType == TeamType.Defender)
			{
				entity.StateComp.CurState = EntityState.Disable;
				entity.StateComp.ForceUpdateAnimation = true;
			}
			this.SetState(this.IdleState);
			this.healthType = healthType;
			this.ownerTeam = this.Entity.TeamComp.TeamType;
			this.lastLookAtX = (this.lastLookAtZ = 0);
			this.shotIndex = 0;
			TroopComponent troopComp = this.Entity.TroopComp;
			BuildingComponent buildingComp = this.Entity.BuildingComp;
			if (troopComp != null)
			{
				this.gunSequences = troopComp.TroopShooterVO.Sequences;
			}
			else if (buildingComp != null)
			{
				string uid = null;
				BuildingType type = buildingComp.BuildingType.Type;
				if (type != BuildingType.Turret)
				{
					if (type == BuildingType.Trap)
					{
						uid = this.Entity.TrapComp.Type.TurretTED.TurretUid;
					}
				}
				else
				{
					uid = buildingComp.BuildingType.TurretUid;
				}
				TurretTypeVO turretTypeVO = Service.StaticDataController.Get<TurretTypeVO>(uid);
				this.gunSequences = turretTypeVO.Sequences;
			}
			else
			{
				Service.Logger.Error("Attaching AttackFSM to Unsupported Entity. No Troop, Building, or Trap Componenet found.");
			}
		}

		public bool IsGunSequenceDone()
		{
			return this.shotIndex == 0;
		}

		public bool CanSwitchAbility()
		{
			return base.CurrentState == this.IdleState || base.CurrentState == this.TurnState;
		}

		public void Update()
		{
			if (!this.IsAttacking)
			{
				return;
			}
			if (base.CurrentState == this.TurnState)
			{
				this.SetState(this.WarmupState);
			}
			else if (base.CurrentState == this.WarmupState)
			{
				this.SetState(this.PreFireState);
			}
			else if (base.CurrentState == this.PreFireState)
			{
				this.SetState(this.PostFireState);
			}
			else if (base.CurrentState == this.PostFireState)
			{
				if (this.shooterController.NeedsReload(this.ShooterComp))
				{
					this.SetState(this.CooldownState);
				}
				else
				{
					if (this.IsGunSequenceDone())
					{
						this.StateComponent.CurState = EntityState.AttackingReset;
					}
					this.SetState(this.PreFireState);
				}
			}
			else if (base.CurrentState == this.CooldownState)
			{
				this.SetState(this.WarmupState);
			}
		}

		public bool StartAttack()
		{
			int num = 0;
			Target targetToAttack = this.shooterController.GetTargetToAttack(this.Entity);
			if (this.Entity.TroopComp != null && this.Entity.TroopComp.TroopShooterVO.TargetSelf)
			{
				return this.SetState(this.WarmupState);
			}
			if (this.StateComponent.CurState == EntityState.Moving)
			{
				PathView pathView = this.Entity.PathingComp.PathView;
				BoardCell nextTurn = pathView.GetNextTurn();
				BoardCell prevTurn = pathView.GetPrevTurn();
				if (prevTurn != null)
				{
					this.lastLookAtX = nextTurn.X - prevTurn.X;
					this.lastLookAtZ = nextTurn.Z - prevTurn.Z;
				}
			}
			int x = targetToAttack.TargetBoardX - this.TransformComponent.CenterGridX();
			int y = targetToAttack.TargetBoardZ - this.TransformComponent.CenterGridZ();
			if (this.lastLookAtX != 0 || this.lastLookAtZ != 0)
			{
				int num2 = IntMath.Atan2Lookup(this.lastLookAtX, this.lastLookAtZ);
				int num3 = IntMath.Atan2Lookup(x, y);
				int num4 = (num2 <= num3) ? (num3 - num2) : (num2 - num3);
				if (num4 > 16384)
				{
					num4 = 32768 - num4;
				}
				WalkerComponent walkerComp = this.Entity.WalkerComp;
				if (walkerComp != null)
				{
					long num5 = (long)walkerComp.SpeedVO.RotationSpeed * 16384L;
					if (num5 > 0L)
					{
						num = (int)((long)num4 * 3142L * 1000L / num5);
						num *= 2;
					}
				}
			}
			this.lastLookAtX = x;
			this.lastLookAtZ = y;
			bool result;
			if (num > 0)
			{
				this.TurnState.SetDefaultLockDuration((uint)num);
				result = this.SetState(this.TurnState);
			}
			else
			{
				result = this.SetState(this.WarmupState);
			}
			return result;
		}

		public bool InStrictCoolDownState()
		{
			return base.CurrentState == this.CooldownState && this.ShooterComp.ShooterVO.StrictCooldown;
		}

		public bool StopAttacking(bool isTroop)
		{
			if (isTroop)
			{
				Service.ShooterController.StopAttacking(this.Entity.StateComp);
			}
			if (!this.IsAttacking)
			{
				return false;
			}
			if (this.InStrictCoolDownState())
			{
				return false;
			}
			this.SetState(this.IdleState);
			return true;
		}

		public bool IsUnlocked()
		{
			return base.CurrentState.IsUnlocked();
		}

		private void FireAShot(int spawnBoardX, int spawnBoardZ, Vector3 startPos, Target target, GameObject gunLocator)
		{
			FactionType faction = FactionType.Invalid;
			if (this.Entity != null)
			{
				BuildingComponent buildingComp = this.Entity.BuildingComp;
				TroopComponent troopComp = this.Entity.TroopComp;
				if (buildingComp != null)
				{
					faction = buildingComp.BuildingType.Faction;
				}
				else if (troopComp != null)
				{
					faction = troopComp.TroopType.Faction;
				}
			}
			HealthFragment payload = new HealthFragment(this.Entity, this.healthType, this.ShooterComp.ShooterVO.Damage);
			Service.ProjectileController.SpawnProjectileForTarget(0u, spawnBoardX, spawnBoardZ, startPos, target, payload, this.ownerTeam, this.Entity, this.ShooterComp.ShooterVO.ProjectileType, true, this.Entity.BuffComp.Buffs, faction, gunLocator);
			Service.EventManager.SendEvent(EventId.EntityAttackedTarget, this.Entity);
			this.shooterController.DecreaseShotsRemainingInClip(this.ShooterComp);
		}

		public void Stun(int millisecondsToStun)
		{
			this.StunState.SetDuration(millisecondsToStun);
			this.SetState(this.StunState);
		}

		public void Fire()
		{
			TransformComponent transformComp = this.Entity.TransformComp;
			Vector3 startPos = Vector3.zero;
			Target targetToAttack = this.shooterController.GetTargetToAttack(this.Entity);
			this.shotIndex = (this.shotIndex + 1) % this.gunSequences.Count;
			GameObjectViewComponent gameObjectViewComp = this.Entity.GameObjectViewComp;
			List<GameObject> list;
			if (gameObjectViewComp != null && gameObjectViewComp.GunLocators.Count != 0)
			{
				list = gameObjectViewComp.GunLocators[this.shotIndex];
			}
			else
			{
				list = null;
			}
			for (int i = 0; i < this.gunSequences[this.shotIndex + 1]; i++)
			{
				if (this.ShooterComp.ShotsRemainingInClip > 0u)
				{
					if (list != null)
					{
						startPos = list[i].transform.position;
						this.FireAShot(transformComp.CenterGridX(), transformComp.CenterGridZ(), startPos, targetToAttack, list[i]);
					}
					else
					{
						Vector3 vector = (gameObjectViewComp != null) ? gameObjectViewComp.MainGameObject.transform.position : new Vector3(transformComp.CenterX(), 0f, transformComp.CenterZ());
						startPos = new Vector3(vector.x, 2f, vector.z);
						this.FireAShot(transformComp.CenterGridX(), transformComp.CenterGridZ(), startPos, targetToAttack, null);
					}
				}
			}
		}

		public void SetEntityState(EntityState entityState)
		{
			this.StateComponent.CurState = entityState;
		}

		public void Activate()
		{
			this.StateComponent.CurState = EntityState.Idle;
			this.StateComponent.ForceUpdateAnimation = true;
		}
	}
}
