using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;

namespace StaRTS.Main.Controllers.CombatTriggers
{
	public class DefendedBuildingCombatTrigger : ICombatTrigger
	{
		private bool triggered;

		public object Owner
		{
			get;
			set;
		}

		public CombatTriggerType Type
		{
			get;
			set;
		}

		public TroopTypeVO Troop
		{
			get;
			private set;
		}

		public int TroopCount
		{
			get;
			private set;
		}

		public bool Leashed
		{
			get;
			set;
		}

		public uint Stagger
		{
			get;
			private set;
		}

		public uint InitialDelay
		{
			get;
			set;
		}

		public uint LastDitchDelayMillis
		{
			get;
			private set;
		}

		public bool TroopsHurtable
		{
			get;
			private set;
		}

		public bool TroopsHurt
		{
			get;
			set;
		}

		public DefendedBuildingCombatTrigger(Entity buildingEntity, CombatTriggerType type, bool troopsHurtable, TroopTypeVO troop, int troopCount, bool leashed, uint stagger, uint lastDitchDelaySeconds)
		{
			this.Owner = buildingEntity;
			this.Type = type;
			this.Troop = troop;
			this.TroopCount = troopCount;
			this.Leashed = leashed;
			this.Stagger = stagger;
			this.LastDitchDelayMillis = lastDitchDelaySeconds * 1000u;
			this.TroopsHurt = false;
			this.TroopsHurtable = troopsHurtable;
		}

		private void ActivateChampion()
		{
			SmartEntity defensiveChampion = ((SmartEntity)this.Owner).ChampionPlatformComp.DefensiveChampion;
			if (defensiveChampion != null)
			{
				defensiveChampion.ShooterComp.AttackFSM.Activate();
			}
		}

		public void Trigger(Entity intruder)
		{
			if (this.Type == CombatTriggerType.Area && intruder != null && intruder.Get<TeamComponent>() != null && intruder.Get<TeamComponent>().TeamType != TeamType.Attacker)
			{
				return;
			}
			this.triggered = true;
			if (this.Troop.Type == TroopType.Champion)
			{
				this.ActivateChampion();
				return;
			}
			IntPosition position = this.DetermineSpawnPosition();
			TroopSpawnData troopSpawnData = new TroopSpawnData(this.Troop, position, (!this.Leashed) ? TroopSpawnMode.Unleashed : TroopSpawnMode.LeashedToBuilding, this.TroopCount);
			Service.SimTimerManager.CreateSimTimer(this.InitialDelay, false, new TimerDelegate(this.OnSpawnTimer), troopSpawnData);
		}

		private IntPosition DetermineSpawnPosition()
		{
			IntPosition zero = IntPosition.Zero;
			switch (this.Type)
			{
			case CombatTriggerType.Area:
			case CombatTriggerType.Load:
				if (this.Leashed)
				{
					TransformComponent transformComponent = ((Entity)this.Owner).Get<TransformComponent>();
					if (transformComponent != null)
					{
						zero = new IntPosition(transformComponent.X, transformComponent.Z);
					}
				}
				else
				{
					DamageableComponent damageableComponent = ((Entity)this.Owner).Get<DamageableComponent>();
					if (damageableComponent != null)
					{
						BoardCell boardCell2;
						BoardCell boardCell = damageableComponent.FindASafeSpawnSpot(this.Troop.SizeX, out boardCell2);
						zero = new IntPosition(boardCell.X, boardCell.Z);
					}
				}
				break;
			case CombatTriggerType.Death:
			{
				TransformComponent transformComponent = ((Entity)this.Owner).Get<TransformComponent>();
				if (transformComponent != null)
				{
					zero = new IntPosition(transformComponent.CenterGridX(), transformComponent.CenterGridZ());
				}
				break;
			}
			}
			return zero;
		}

		private void OnSpawnTimer(uint id, object cookie)
		{
			TroopSpawnData troopSpawnData = (TroopSpawnData)cookie;
			SmartEntity smartEntity = Service.TroopController.SpawnTroop(troopSpawnData.TroopType, TeamType.Defender, troopSpawnData.BoardPosition, troopSpawnData.SpawnMode, true, true);
			if (smartEntity == null)
			{
				return;
			}
			if (--troopSpawnData.Amount > 0)
			{
				Service.SimTimerManager.CreateSimTimer(this.Stagger, false, new TimerDelegate(this.OnSpawnTimer), troopSpawnData);
			}
			if (this.TroopsHurt)
			{
				HealthComponent healthComp = smartEntity.HealthComp;
				int quantity = IntMath.Normalize(0, 100, 100 - GameConstants.SPAWN_HEALTH_PERCENT, 0, healthComp.MaxHealth);
				Service.HealthController.ApplyHealthFragment(healthComp, new HealthFragment(null, HealthType.Damaging, quantity), true);
			}
			if (smartEntity != null)
			{
				Service.EventManager.SendEvent(EventId.DefenderTriggeredInBattle, new DefenderTroopDeployedData(smartEntity, (Entity)this.Owner));
			}
		}

		public bool IsAlreadyTriggered()
		{
			return this.triggered;
		}
	}
}
