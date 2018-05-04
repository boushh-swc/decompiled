using Net.RichardLord.Ash.Core;
using StaRTS.FX;
using StaRTS.Main.Controllers.CombatTriggers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using StaRTS.Utils.State;
using System;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class TrapController : IEventObserver
	{
		private const string DISARM_EVENT_SUCCESS = "EventSuccess";

		private const string TRAP_DESTRUCTION = "TrapDestruction";

		private CombatTriggerManager ctm;

		public TrapController()
		{
			Service.TrapController = this;
			this.ctm = Service.CombatTriggerManager;
			Service.EventManager.RegisterObserver(this, EventId.EntityKilled, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.BattleEndProcessing, EventPriority.Default);
		}

		public void RegisterTraps(CurrentBattle currentBattle)
		{
			NodeList<TrapNode> nodeList = Service.EntityController.GetNodeList<TrapNode>();
			for (TrapNode trapNode = nodeList.Head; trapNode != null; trapNode = trapNode.Next)
			{
				if (currentBattle.DisabledBuildings == null || !currentBattle.DisabledBuildings.Contains(trapNode.BuildingComp.BuildingTO.Key))
				{
					TrapCombatTrigger combatTrigger = new TrapCombatTrigger(trapNode);
					this.ctm.RegisterTrigger(combatTrigger);
				}
			}
		}

		public void SwapTrapToTurret(SmartEntity entity)
		{
			entity.HealthComp.ArmorType = ArmorType.Turret;
			string turretUid = entity.TrapComp.Type.TurretTED.TurretUid;
			TurretTypeVO turretTypeVO = Service.StaticDataController.Get<TurretTypeVO>(turretUid);
			Service.EntityFactory.AddTurretComponentsToEntity(entity, turretTypeVO);
			if (entity.TrackingGameObjectViewComp == null)
			{
				TrackingGameObjectViewComponent component = new TrackingGameObjectViewComponent(entity.GameObjectViewComp.MainGameObject, turretTypeVO, entity.TrackingComp);
				entity.Add<TrackingGameObjectViewComponent>(component);
			}
			Service.SpatialIndexController.ResetTurretScannedFlagForBoard();
		}

		private void SwapTurretToTrap(SmartEntity entity)
		{
			if (entity.HealthComp != null)
			{
				entity.HealthComp.ArmorType = entity.BuildingComp.BuildingType.ArmorType;
			}
			this.SetTrapState(entity.TrapComp, TrapState.Spent);
			entity.Remove<ShooterComponent>();
			entity.Remove<TurretShooterComponent>();
			entity.Remove<TrackingComponent>();
			entity.Remove<TrackingGameObjectViewComponent>();
			Service.TrapViewController.UpdateTrapVisibility(entity);
		}

		public int GetTrapDamageForUIDisplay(TrapTypeVO trapType)
		{
			TrapEventType eventType = trapType.EventType;
			if (eventType == TrapEventType.SpecialAttack)
			{
				SpecialAttackTypeVO specialAttackTypeVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(trapType.ShipTED.SpecialAttackName);
				return specialAttackTypeVO.DPS;
			}
			if (eventType != TrapEventType.Turret)
			{
				return 0;
			}
			TurretTypeVO turretTypeVO = Service.StaticDataController.Get<TurretTypeVO>(trapType.TurretTED.TurretUid);
			return turretTypeVO.DPS;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.EntityKilled)
			{
				if (id == EventId.BattleEndProcessing)
				{
					NodeList<TrapNode> nodeList = Service.EntityController.GetNodeList<TrapNode>();
					for (TrapNode trapNode = nodeList.Head; trapNode != null; trapNode = trapNode.Next)
					{
						if (trapNode.TrapComp.CurrentState == TrapState.Active && trapNode.TrapComp.Type.EventType == TrapEventType.Turret)
						{
							this.SwapTurretToTrap((SmartEntity)trapNode.Entity);
						}
					}
				}
			}
			else
			{
				SmartEntity smartEntity = cookie as SmartEntity;
				if (smartEntity.TrapComp != null)
				{
					TrapTypeVO type = smartEntity.TrapComp.Type;
					this.SetTrapState(smartEntity.TrapComp, TrapState.Spent);
					TrapEventType eventType = type.EventType;
					if (eventType == TrapEventType.Turret)
					{
						this.SwapTurretToTrap(smartEntity);
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public void ExecuteTrap(TrapComponent comp, int boardX, int boardZ)
		{
			TrapEventType eventType = comp.Type.EventType;
			if (eventType != TrapEventType.SpecialAttack)
			{
				if (eventType == TrapEventType.Turret)
				{
					this.SwapTrapToTurret((SmartEntity)comp.Entity);
					this.SetTrapState(comp, TrapState.Active);
				}
			}
			else
			{
				Vector3 zero = Vector3.zero;
				zero.x = Units.BoardToWorldX(boardX);
				zero.z = Units.BoardToWorldX(boardZ);
				SpecialAttackTypeVO specialAttackTypeVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(comp.Type.ShipTED.SpecialAttackName);
				Service.SpecialAttackController.DeploySpecialAttack(specialAttackTypeVO, TeamType.Defender, zero);
				if (comp.Type.DisarmConditions == "EventSuccess")
				{
					this.SetTrapState(comp, TrapState.Active);
					this.SetTrapState(comp, TrapState.Spent);
				}
				if (!specialAttackTypeVO.IsDropship)
				{
					Service.ViewTimerManager.CreateViewTimer((specialAttackTypeVO.AnimationDelay + specialAttackTypeVO.HitDelay) * 0.001f, false, new TimerDelegate(this.OnStarshipStrike), comp.Entity);
				}
			}
		}

		public void DestroyTrap(SmartEntity trap)
		{
			this.SetTrapState(trap.TrapComp, TrapState.Destroyed);
		}

		private void OnStarshipStrike(uint uid, object cookie)
		{
			SmartEntity smartEntity = (SmartEntity)cookie;
			string assetName = string.Format(smartEntity.BuildingComp.BuildingType.DestructFX, smartEntity.BuildingComp.BuildingType.SizeX, smartEntity.BuildingComp.BuildingType.SizeY);
			FXManager fXManager = Service.FXManager;
			fXManager.CreateAndAttachFXToEntity(smartEntity, assetName, "TrapDestruction", null, false, Vector3.zero, false);
		}

		public void SetTrapState(TrapComponent comp, TrapState state)
		{
			SmartEntity smartEntity = (SmartEntity)comp.Entity;
			TrapViewComponent trapViewComp = smartEntity.TrapViewComp;
			TrapState currentState = comp.CurrentState;
			if (currentState != state)
			{
				comp.CurrentState = state;
				IState currentState2 = Service.GameStateMachine.CurrentState;
				if (TrapUtils.IsCurrentPlayerInDefensiveBattle(currentState2))
				{
					smartEntity.BuildingComp.BuildingTO.CurrentStorage = ((state != TrapState.Armed) ? 0 : 1);
				}
				Service.TrapViewController.UpdateTrapVisibility((SmartEntity)comp.Entity);
				switch (state)
				{
				case TrapState.Spent:
					Service.EventManager.SendEvent(EventId.TrapDisarmed, comp);
					break;
				case TrapState.Active:
					Service.EventManager.SendEvent(EventId.TrapTriggered, comp);
					break;
				case TrapState.Destroyed:
					comp.CurrentState = TrapState.Spent;
					Service.EventManager.SendEvent(EventId.TrapDestroyed, comp);
					break;
				}
			}
			Service.TrapViewController.SetTrapViewState(trapViewComp, state);
		}
	}
}
