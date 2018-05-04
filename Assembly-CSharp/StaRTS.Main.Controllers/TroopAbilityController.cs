using Game.Behaviors;
using StaRTS.FX;
using StaRTS.Main.Controllers.Entities.StateMachines.Attack;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Diagnostics;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class TroopAbilityController : IEventObserver
	{
		private Dictionary<uint, DeployedTroop> deployedTroops;

		private List<uint> userActivatedEntityIDs;

		private List<uint> autoActivatedEntityIDs;

		private int numActivatedTroopsWithClipCounts;

		public TroopAbilityController()
		{
			Service.TroopAbilityController = this;
			this.deployedTroops = null;
			this.userActivatedEntityIDs = null;
			this.autoActivatedEntityIDs = null;
			this.numActivatedTroopsWithClipCounts = 0;
		}

		public void OnTroopSpawned(SmartEntity troopEntity)
		{
			if (troopEntity != null && troopEntity.TroopComp != null)
			{
				ITroopDeployableVO troopType = troopEntity.TroopComp.TroopType;
				TroopAbilityVO abilityVO = troopEntity.TroopComp.AbilityVO;
				if (abilityVO != null)
				{
					if (this.deployedTroops == null)
					{
						this.deployedTroops = new Dictionary<uint, DeployedTroop>();
					}
					if (this.deployedTroops.Count == 0)
					{
						this.EnsureEvents(true);
					}
					DeployedTroop deployedTroop = new DeployedTroop(troopType.Uid, troopEntity);
					this.deployedTroops.Add(troopEntity.ID, deployedTroop);
					if (abilityVO.Auto)
					{
						if (abilityVO.CooldownOnSpawn)
						{
							this.StartCoolDown(deployedTroop, abilityVO);
						}
						else
						{
							this.QueueAutoActivateAbility(troopEntity.ID);
						}
					}
				}
			}
		}

		public void UserActivateAbility(uint entityID)
		{
			if (this.userActivatedEntityIDs == null)
			{
				this.userActivatedEntityIDs = new List<uint>();
			}
			this.userActivatedEntityIDs.Add(entityID);
		}

		private void QueueAutoActivateAbility(uint entityID)
		{
			if (this.autoActivatedEntityIDs == null)
			{
				this.autoActivatedEntityIDs = new List<uint>();
			}
			this.autoActivatedEntityIDs.Add(entityID);
		}

		public void ProcessPendingAbilities()
		{
			if (this.userActivatedEntityIDs != null)
			{
				int i = 0;
				int count = this.userActivatedEntityIDs.Count;
				while (i < count)
				{
					this.ActivateAbility(this.userActivatedEntityIDs[i]);
					i++;
				}
				this.userActivatedEntityIDs.Clear();
			}
			if (this.autoActivatedEntityIDs != null)
			{
				for (int j = this.autoActivatedEntityIDs.Count - 1; j >= 0; j--)
				{
					uint entityID = this.autoActivatedEntityIDs[j];
					if (this.CanAutoActivateAbility(entityID))
					{
						this.ActivateAbility(entityID);
						this.autoActivatedEntityIDs.RemoveAt(j);
					}
				}
			}
		}

		private bool CanAutoActivateAbility(uint entityID)
		{
			bool result = false;
			if (this.deployedTroops.ContainsKey(entityID))
			{
				SmartEntity entity = this.deployedTroops[entityID].Entity;
				ShooterComponent shooterComp = entity.ShooterComp;
				if (shooterComp != null)
				{
					TroopComponent troopComp = entity.TroopComp;
					TroopAbilityVO troopAbilityVO = null;
					if (troopComp != null)
					{
						troopAbilityVO = troopComp.AbilityVO;
					}
					result = ((troopAbilityVO != null && troopAbilityVO.RecastAbility) || shooterComp.AttackFSM.CanSwitchAbility());
				}
			}
			return result;
		}

		public void ActivateHeroAbility(string heroUid)
		{
			foreach (KeyValuePair<uint, DeployedTroop> current in this.deployedTroops)
			{
				if (current.Value.Uid == heroUid)
				{
					this.ActivateAbility(current.Key);
					break;
				}
			}
		}

		public bool ActivateAbility(uint entityID)
		{
			if (!this.deployedTroops.ContainsKey(entityID))
			{
				return false;
			}
			DeployedTroop deployedTroop = this.deployedTroops[entityID];
			SmartEntity entity = deployedTroop.Entity;
			if (entity.StateComp.CurState == EntityState.Dying)
			{
				return false;
			}
			TroopComponent troopComp = entity.TroopComp;
			TroopAbilityVO abilityVO = troopComp.AbilityVO;
			if (abilityVO == null || (abilityVO.ClipCount == 0 && abilityVO.Duration == 0u))
			{
				return false;
			}
			deployedTroop.Activated = true;
			troopComp.SetVOData(abilityVO, abilityVO);
			troopComp.IsAbilityModeActive = true;
			troopComp.UpdateWallAttackerTroop = false;
			ShooterComponent shooterComp = entity.ShooterComp;
			if (shooterComp != null)
			{
				shooterComp.SetVOData(abilityVO);
			}
			this.ResetTargetAndSendEvent(entity, true);
			if (!deployedTroop.EffectsSetup)
			{
				this.SetupAbilityViewEffects(deployedTroop, abilityVO);
				deployedTroop.EffectsSetup = true;
			}
			this.ActivateAbilityViewEffects(deployedTroop, abilityVO);
			if (abilityVO.ClipCount > 0)
			{
				this.StartTrackingShooterClips(deployedTroop, abilityVO.ClipCount);
			}
			else
			{
				deployedTroop.AbilityTimer = Service.SimTimerManager.CreateSimTimer(abilityVO.Duration, false, new TimerDelegate(this.OnAbilityTimer), deployedTroop);
			}
			return true;
		}

		private void OnAbilityTimer(uint id, object cookie)
		{
			this.DeactivateAbility((DeployedTroop)cookie);
		}

		private void DeactivateAbility(DeployedTroop deployedTroop)
		{
			if (deployedTroop == null)
			{
				Service.Logger.Error("TroopAbilityConroller.DeactivateAbility: DeployedTroop Null");
				return;
			}
			deployedTroop.AbilityTimer = 0u;
			SmartEntity entity = deployedTroop.Entity;
			if (entity == null)
			{
				Service.Logger.Error("TroopAbilityConroller.DeactivateAbility: SmartEntity troop = Null");
				return;
			}
			TroopComponent troopComp = entity.TroopComp;
			if (troopComp == null)
			{
				Service.Logger.Error("TroopAbilityConroller.DeactivateAbility: troopComp = Null");
				return;
			}
			troopComp.SetVOData(troopComp.OriginalTroopShooterVO, troopComp.OriginalSpeedVO);
			troopComp.IsAbilityModeActive = false;
			ShooterComponent shooterComp = entity.ShooterComp;
			if (shooterComp != null)
			{
				shooterComp.SetVOData(shooterComp.OriginalShooterVO);
			}
			TroopAbilityVO abilityVO = troopComp.AbilityVO;
			if (abilityVO == null)
			{
				Service.Logger.Error("TroopAbilityConroller.DeactivateAbility: TroopAbilityVO abilityVO = Null");
				return;
			}
			this.ResetTargetAndSendEvent(entity, false);
			this.DeactivateAbilityViewEffects(deployedTroop);
			this.StopTrackingShooterClips(deployedTroop);
			deployedTroop.Activated = false;
			this.StartCoolDown(deployedTroop, abilityVO);
		}

		private void ResetTargetAndSendEvent(SmartEntity troop, bool activated)
		{
			Service.TargetingController.InvalidateCurrentTarget(troop);
			if (activated)
			{
				Service.EventManager.SendEvent(EventId.TroopAbilityActivate, troop);
			}
			else
			{
				Service.EventManager.SendEvent(EventId.TroopAbilityDeactivate, troop);
			}
			this.ResetAttackFSM(troop);
		}

		private void StartCoolDown(DeployedTroop deployedTroop, TroopAbilityVO abilityVO)
		{
			if (abilityVO.CoolDownTime == 0u)
			{
				this.OnCoolDownTimer(0u, deployedTroop);
			}
			else
			{
				deployedTroop.CoolDownTimer = Service.SimTimerManager.CreateSimTimer(abilityVO.CoolDownTime, false, new TimerDelegate(this.OnCoolDownTimer), deployedTroop);
			}
		}

		private void OnCoolDownTimer(uint id, object cookie)
		{
			DeployedTroop deployedTroop = (DeployedTroop)cookie;
			deployedTroop.CoolDownTimer = 0u;
			SmartEntity entity = deployedTroop.Entity;
			Service.EventManager.SendEvent(EventId.TroopAbilityCoolDownComplete, entity);
			TroopAbilityVO abilityVO = entity.TroopComp.AbilityVO;
			if (abilityVO.Auto)
			{
				this.QueueAutoActivateAbility(entity.ID);
			}
		}

		private void SetupAbilityViewEffects(DeployedTroop deployedTroop, TroopAbilityVO abilityVO)
		{
			GameObjectViewComponent gameObjectViewComp = deployedTroop.Entity.GameObjectViewComp;
			if (gameObjectViewComp == null)
			{
				return;
			}
			WeaponTrail componentInChildren = gameObjectViewComp.MainGameObject.GetComponentInChildren<WeaponTrail>();
			if (componentInChildren != null)
			{
				deployedTroop.WeaponTrail = componentInChildren;
				float[] weaponTrailFxParams = abilityVO.WeaponTrailFxParams;
				if (weaponTrailFxParams != null && weaponTrailFxParams.Length >= 2)
				{
					deployedTroop.WeaponTrailActivateLifetime = weaponTrailFxParams[0];
					deployedTroop.WeaponTrailDeactivateLifetime = weaponTrailFxParams[1];
				}
			}
		}

		private void ActivateAbilityViewEffects(DeployedTroop deployedTroop, TroopAbilityVO ability)
		{
			SmartEntity entity = deployedTroop.Entity;
			GameObjectViewComponent gameObjectViewComp = entity.GameObjectViewComp;
			if (gameObjectViewComp == null)
			{
				return;
			}
			if (ability.AltGunLocators != null && ability.AltGunLocators.Length > 0)
			{
				gameObjectViewComp.SwitchGunLocators(true);
			}
			GameObject mainGameObject = gameObjectViewComp.MainGameObject;
			mainGameObject.transform.localScale = Vector3.one * ability.PersistentScaling;
			if (!string.IsNullOrEmpty(ability.PersistentEffect))
			{
				ITroopDeployableVO troopType = entity.TroopComp.TroopType;
				TransformComponent transformComp = entity.TransformComp;
				deployedTroop.LightSaberHitFx = new LightSaberHitEffect(ability.ProjectileType.SplashRadius, transformComp.CenterGridX(), transformComp.CenterGridZ(), mainGameObject.transform, ability.PersistentEffect, troopType.Faction);
			}
			if (deployedTroop.WeaponTrail != null && deployedTroop.WeaponTrailActivateLifetime > 0f)
			{
				deployedTroop.WeaponTrail.ChangeLifeTime(deployedTroop.WeaponTrailActivateLifetime);
			}
		}

		private void DeactivateAbilityViewEffects(DeployedTroop deployedTroop)
		{
			SmartEntity entity = deployedTroop.Entity;
			GameObjectViewComponent gameObjectViewComp = entity.GameObjectViewComp;
			if (gameObjectViewComp == null)
			{
				return;
			}
			gameObjectViewComp.SwitchGunLocators(false);
			gameObjectViewComp.MainGameObject.transform.localScale = Vector3.one;
			if (deployedTroop.LightSaberHitFx != null)
			{
				deployedTroop.LightSaberHitFx.StopFxAndDestroy();
				deployedTroop.LightSaberHitFx = null;
			}
			if (deployedTroop.WeaponTrail != null && deployedTroop.WeaponTrailDeactivateLifetime > 0f)
			{
				deployedTroop.WeaponTrail.ChangeLifeTime(deployedTroop.WeaponTrailDeactivateLifetime);
			}
		}

		private void ResetAttackFSM(SmartEntity troopEntity)
		{
			Logger logger = Service.Logger;
			StateComponent stateComp = troopEntity.StateComp;
			if (stateComp == null)
			{
				logger.Error("ResetAttackFSM StateComp is null");
				return;
			}
			stateComp.Reset();
			TroopComponent troopComp = troopEntity.TroopComp;
			if (troopComp == null)
			{
				logger.Error("ResetAttackFSM TroopComp is null");
				return;
			}
			if (troopComp.TroopType == null)
			{
				logger.Error("ResetAttackFSM TroopVO is null");
				return;
			}
			ShooterComponent shooterComp = troopEntity.ShooterComp;
			if (shooterComp == null)
			{
				logger.Error("ResetAttackFSM ShooterComp is null");
				return;
			}
			TroopRole troopRole = shooterComp.ShooterVO.TroopRole;
			if (troopRole == TroopRole.None)
			{
				troopRole = shooterComp.OriginalShooterVO.TroopRole;
			}
			HealthType healthType = (troopRole != TroopRole.Healer) ? HealthType.Damaging : HealthType.Healing;
			shooterComp.Reset();
			shooterComp.AttackFSM = new AttackFSM(Service.BattleController, troopEntity, troopEntity.StateComp, shooterComp, troopEntity.TransformComp, healthType);
		}

		private void OnTroopDestroyed(uint entityID, bool remove)
		{
			DeployedTroop deployedTroop = this.deployedTroops[entityID];
			if (deployedTroop.AbilityTimer != 0u)
			{
				Service.SimTimerManager.KillSimTimer(deployedTroop.AbilityTimer);
				deployedTroop.AbilityTimer = 0u;
			}
			if (deployedTroop.CoolDownTimer != 0u)
			{
				Service.SimTimerManager.KillSimTimer(deployedTroop.CoolDownTimer);
				deployedTroop.CoolDownTimer = 0u;
			}
			this.StopTrackingShooterClips(deployedTroop);
			if (remove)
			{
				this.deployedTroops.Remove(entityID);
				if (this.deployedTroops.Count == 0)
				{
					this.EnsureEvents(false);
				}
				if (this.autoActivatedEntityIDs != null && this.autoActivatedEntityIDs.Contains(entityID))
				{
					this.autoActivatedEntityIDs.Remove(entityID);
				}
				if (this.userActivatedEntityIDs != null && this.userActivatedEntityIDs.Contains(entityID))
				{
					this.userActivatedEntityIDs.Remove(entityID);
				}
			}
		}

		private void RemoveAllDeployedTroops()
		{
			foreach (KeyValuePair<uint, DeployedTroop> current in this.deployedTroops)
			{
				this.OnTroopDestroyed(current.Key, false);
			}
			this.deployedTroops.Clear();
			if (this.autoActivatedEntityIDs != null)
			{
				this.autoActivatedEntityIDs.Clear();
			}
			if (this.userActivatedEntityIDs != null)
			{
				this.userActivatedEntityIDs.Clear();
			}
			this.EnsureEvents(false);
		}

		private void StartTrackingShooterClips(DeployedTroop deployedTroop, int clipCount)
		{
			if (deployedTroop.Activated && clipCount > 0)
			{
				deployedTroop.AbilityClipCount = clipCount;
				deployedTroop.Entity.ShooterComp.ShouldCountClips = true;
				if (this.numActivatedTroopsWithClipCounts++ == 0)
				{
					Service.EventManager.RegisterObserver(this, EventId.ShooterClipUsed);
				}
			}
		}

		private void StopTrackingShooterClips(DeployedTroop deployedTroop)
		{
			if (deployedTroop.Activated && deployedTroop.AbilityClipCount > 0)
			{
				SmartEntity entity = deployedTroop.Entity;
				if (entity != null && entity.ShooterComp != null)
				{
					entity.ShooterComp.ShouldCountClips = false;
				}
				if (--this.numActivatedTroopsWithClipCounts == 0)
				{
					Service.EventManager.UnregisterObserver(this, EventId.ShooterClipUsed);
				}
			}
		}

		private void EnsureEvents(bool register)
		{
			EventManager eventManager = Service.EventManager;
			if (register)
			{
				eventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.EntityDestroyed, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.BattleEndProcessing, EventPriority.Default);
			}
			else
			{
				eventManager.UnregisterObserver(this, EventId.EntityKilled);
				eventManager.UnregisterObserver(this, EventId.EntityDestroyed);
				eventManager.UnregisterObserver(this, EventId.BattleEndProcessing);
			}
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.EntityDestroyed)
			{
				if (id != EventId.BattleEndProcessing)
				{
					if (id == EventId.ShooterClipUsed)
					{
						ShooterComponent shooterComponent = (ShooterComponent)cookie;
						if (shooterComponent != null)
						{
							SmartEntity smartEntity = (SmartEntity)shooterComponent.Entity;
							uint iD = smartEntity.ID;
							if (this.deployedTroops.ContainsKey(iD))
							{
								DeployedTroop deployedTroop = this.deployedTroops[iD];
								if ((ulong)shooterComponent.NumClipsUsed >= (ulong)((long)deployedTroop.AbilityClipCount))
								{
									this.DeactivateAbility(deployedTroop);
								}
							}
						}
					}
				}
				else
				{
					this.RemoveAllDeployedTroops();
				}
			}
			else
			{
				uint num = (uint)cookie;
				if (this.deployedTroops.ContainsKey(num))
				{
					this.OnTroopDestroyed(num, true);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
