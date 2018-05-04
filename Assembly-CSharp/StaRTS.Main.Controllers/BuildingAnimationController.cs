using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Main.Views.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class BuildingAnimationController : IEventObserver
	{
		private const string IDLE_ANIMATION_TRACK = "Idle";

		private const string ACTIVE_ANIM = "Active";

		private const string FULL_ANIM = "Full";

		private const string INTRO_ANIM = "Intro";

		private const float FACTORY_SPARK_DELAY1 = 0.6f;

		private const float FACTORY_SPARK_DELAY2 = 1f;

		private const string TRAP_IDLE_LOOP = "InactiveIdle";

		private const string TRAP_ACTIVATE_TRACK = "Activate";

		private const string TRAP_ACTIVATED_LOOP = "ActivatedIdle";

		private const string TRAP_DEACTIVATE_TRACK = "Deactivate";

		private const string TRAP_DEACTIVATED_LOOP = "DeactivatedIdle";

		private NodeList<BuildingRenderNode> nodeList;

		private string[] barracksOpenCloseAnims;

		private string[] storageOpenCloseAnims;

		public BuildingAnimationController()
		{
			Service.BuildingAnimationController = this;
			this.nodeList = Service.EntityController.GetNodeList<BuildingRenderNode>();
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.ShuttleAnimStateChanged);
			eventManager.RegisterObserver(this, EventId.GameStateChanged);
			eventManager.RegisterObserver(this, EventId.WorldLoadComplete);
			eventManager.RegisterObserver(this, EventId.ContractStarted);
			eventManager.RegisterObserver(this, EventId.ContractContinued);
			eventManager.RegisterObserver(this, EventId.ContractStopped);
			eventManager.RegisterObserver(this, EventId.GeneratorJustFilled);
			eventManager.RegisterObserver(this, EventId.CurrencyCollected);
			eventManager.RegisterObserver(this, EventId.BuildingViewReady);
			eventManager.RegisterObserver(this, EventId.EntityPostBattleRepairStarted);
			eventManager.RegisterObserver(this, EventId.EntityPostBattleRepairFinished);
			eventManager.RegisterObserver(this, EventId.TroopRecruited);
			eventManager.RegisterObserver(this, EventId.StorageDoorEvent);
			eventManager.RegisterObserver(this, EventId.ScreenClosing);
			eventManager.RegisterObserver(this, EventId.EquipmentDeactivated);
			this.barracksOpenCloseAnims = new string[]
			{
				"OpenDoors",
				"CloseDoors",
				"IdleClosed"
			};
			this.storageOpenCloseAnims = new string[]
			{
				"OpenDoor",
				"CloseDoor"
			};
		}

		private bool BuildingEligibleForActiveAnimation(Entity entity, IState gameState, BuildingAnimationComponent animComp)
		{
			if (entity == null)
			{
				return false;
			}
			if (gameState is EditBaseState)
			{
				return false;
			}
			if (animComp.BuildingUpgrading)
			{
				return false;
			}
			if (Service.PostBattleRepairController.IsEntityInRepair(entity))
			{
				return false;
			}
			if (animComp.Manufacturing)
			{
				return true;
			}
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return false;
			}
			BuildingTypeVO buildingType = buildingComponent.BuildingType;
			if (buildingType.Type == BuildingType.HQ)
			{
				return true;
			}
			if (buildingType.Type == BuildingType.ShieldGenerator && buildingType.SubType == BuildingSubType.OutpostDefenseGenerator)
			{
				return true;
			}
			if (!(gameState is HomeState))
			{
				return false;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.Barracks || buildingComponent.BuildingType.Type == BuildingType.Cantina)
			{
				return true;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.DroidHut)
			{
				return true;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.ScoutTower)
			{
				return Service.RaidDefenseController.IsRaidAvailable();
			}
			if (buildingComponent.BuildingType.Type == BuildingType.Resource && buildingComponent.BuildingTO.AccruedCurrency < buildingType.Storage)
			{
				return true;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.Storage && buildingComponent.BuildingTO.CurrentStorage < buildingType.Storage)
			{
				return true;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.Armory)
			{
				ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
				return ArmoryUtils.IsAnyEquipmentActive(activeArmory);
			}
			return false;
		}

		private bool BuildingEligibleForIdleAnimation(Entity entity, IState gameState, BuildingAnimationComponent animComp)
		{
			if (entity == null)
			{
				return false;
			}
			if (gameState is EditBaseState)
			{
				return false;
			}
			if (animComp.BuildingUpgrading)
			{
				return false;
			}
			if (Service.PostBattleRepairController.IsEntityInRepair(entity))
			{
				return false;
			}
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return false;
			}
			if (!(gameState is HomeState))
			{
				return false;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.ScoutTower)
			{
				return true;
			}
			if (buildingComponent.BuildingType.Type == BuildingType.Armory)
			{
				ActiveArmory activeArmory = Service.CurrentPlayer.ActiveArmory;
				return !ArmoryUtils.IsAnyEquipmentActive(activeArmory);
			}
			return false;
		}

		public void PlayAnimation(Animation anim)
		{
			this.PlayAnimation(anim, "Active");
		}

		public void PlayAnimation(Animation anim, string animName)
		{
			anim.Stop();
			if (anim.GetClip(animName) != null)
			{
				anim[animName].time = Service.Rand.ViewRangeFloat(0f, anim[animName].length);
				anim.Play(animName);
			}
			else
			{
				anim.Play();
			}
		}

		private void StopAnimation(Animation anim)
		{
			anim.Stop();
			if (anim.GetClip("Idle") != null)
			{
				anim["Idle"].normalizedTime = 1f;
				anim.Play("Idle");
				anim.Sample();
				anim.Stop();
			}
		}

		private void PlayFXs(List<ParticleSystem> fx)
		{
			if (fx == null)
			{
				return;
			}
			for (int i = 0; i < fx.Count; i++)
			{
				fx[i].Play();
			}
		}

		private void StopFXs(List<ParticleSystem> fx)
		{
			if (fx == null)
			{
				return;
			}
			for (int i = 0; i < fx.Count; i++)
			{
				fx[i].Stop();
			}
		}

		private void UpdateAnimation(Entity entity, IState gameMode, BuildingAnimationComponent animComp, bool updateContraband)
		{
			TrapComponent trapComp = ((SmartEntity)entity).TrapComp;
			if (trapComp != null)
			{
				return;
			}
			if (this.BuildingEligibleForActiveAnimation(entity, gameMode, animComp))
			{
				this.PlayAnimation(animComp.Anim, "Active");
				this.PlayFXs(animComp.ListOfParticleSystems);
			}
			else if (this.BuildingEligibleForIdleAnimation(entity, gameMode, animComp))
			{
				this.PlayAnimation(animComp.Anim, "Idle");
				this.StopFXs(animComp.ListOfParticleSystems);
			}
			else
			{
				this.StopAnimation(animComp.Anim);
				this.StopFXs(animComp.ListOfParticleSystems);
			}
			this.UpdateArmoryAnimation((SmartEntity)entity);
			if (updateContraband)
			{
				this.UpdateContraBandShipAnimation((SmartEntity)entity);
			}
		}

		private void EnqueueAnimation(BuildingAnimationComponent animComp, string animationID)
		{
			animComp.Anim.PlayQueued(animationID);
		}

		private void UpdateAnimations(IState gameMode)
		{
			for (BuildingRenderNode buildingRenderNode = this.nodeList.Head; buildingRenderNode != null; buildingRenderNode = buildingRenderNode.Next)
			{
				this.UpdateAnimation(buildingRenderNode.Entity, gameMode, buildingRenderNode.AnimComp, true);
				BuildingComponent buildingComp = buildingRenderNode.BuildingComp;
				BuildingTypeVO buildingType = buildingComp.BuildingType;
				if (buildingType.Type == BuildingType.Resource && buildingComp.BuildingTO.CurrentStorage >= buildingType.Storage)
				{
					this.UpdateAnimationOnGeneratorFull((SmartEntity)buildingRenderNode.Entity, gameMode);
				}
			}
			if (gameMode is HomeState)
			{
				StorageSpreadUtils.UpdateAllStarportFullnessMeters();
			}
		}

		private void StartAnimationOnContractStarted(Entity entity, Contract contract, IState currentState)
		{
			if (Service.ISupportController.IsBuildingFrozen(contract.ContractTO.BuildingKey))
			{
				return;
			}
			BuildingAnimationComponent buildingAnimationComponent = entity.Get<BuildingAnimationComponent>();
			if (buildingAnimationComponent == null)
			{
				return;
			}
			bool flag = contract.DeliveryType == DeliveryType.UpgradeBuilding || contract.DeliveryType == DeliveryType.Building;
			if (flag)
			{
				buildingAnimationComponent.BuildingUpgrading = true;
			}
			else
			{
				buildingAnimationComponent.BuildingUpgrading = false;
				buildingAnimationComponent.Manufacturing = true;
			}
			this.UpdateAnimation(entity, currentState, buildingAnimationComponent, true);
		}

		public void FactorySpark(int message, GameObjectViewComponent gameObjViewComp)
		{
			if (gameObjViewComp != null)
			{
				this.PrepareFactorySparkWithEvent(message, gameObjViewComp);
			}
		}

		public void DepotSpark(int message, GameObjectViewComponent gameObjViewComp)
		{
			if (gameObjViewComp != null)
			{
				this.PrepareFactorySparkWithEvent(message, gameObjViewComp);
			}
		}

		private void PrepareFactorySparkWithEvent(int message, GameObjectViewComponent viewComp)
		{
			if (viewComp.EffectGameObjects.Count > message)
			{
				GameObject gameObject = viewComp.EffectGameObjects[message];
				if (gameObject != null)
				{
					Transform transform = gameObject.transform;
					Transform transform2 = null;
					if (transform.childCount > 0)
					{
						transform2 = transform.GetChild(0);
					}
					if (transform2 != null)
					{
						ParticleSystem component = transform2.GetComponent<ParticleSystem>();
						component.simulationSpace = ParticleSystemSimulationSpace.World;
						if (component != null)
						{
							this.PlayStopParticle(0u, component);
						}
					}
				}
			}
		}

		private void UpdateAnimationOnContractStopped(Entity entity, IState currentState)
		{
			BuildingAnimationComponent buildingAnimationComponent = entity.Get<BuildingAnimationComponent>();
			if (buildingAnimationComponent == null)
			{
				return;
			}
			if (buildingAnimationComponent.BuildingUpgrading)
			{
				buildingAnimationComponent.BuildingUpgrading = false;
			}
			else if (buildingAnimationComponent.Manufacturing)
			{
				buildingAnimationComponent.Manufacturing = false;
			}
			this.UpdateAnimation(entity, currentState, buildingAnimationComponent, false);
		}

		private void UpdateContraBandGeneratorAnimation(SmartEntity entity, ShuttleAnim currentState)
		{
			if (entity.BuildingComp.BuildingType.Type != BuildingType.Resource || entity.BuildingComp.BuildingType.Currency != CurrencyType.Contraband)
			{
				return;
			}
			if (currentState != null)
			{
				BuildingAnimationComponent buildingAnimationComp = entity.BuildingAnimationComp;
				Animation anim = buildingAnimationComp.Anim;
				switch (currentState.State)
				{
				case ShuttleState.Landing:
				case ShuttleState.LiftOff:
					if (anim.GetClip("Full") != null)
					{
						anim.Stop();
						anim.Play("Full");
					}
					break;
				case ShuttleState.Idle:
					if (anim.GetClip("Active") != null && anim.GetClip("Intro") != null)
					{
						anim.Stop();
						anim.Play("Intro");
						this.EnqueueAnimation(buildingAnimationComp, "Active");
					}
					break;
				}
			}
		}

		private void UpdateArmoryAnimation(SmartEntity entity)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			BuildingComponent buildingComp = entity.BuildingComp;
			if (!(currentState is HomeState) || buildingComp == null || buildingComp.BuildingType.Type != BuildingType.Armory)
			{
				return;
			}
			BuildingAnimationComponent buildingAnimationComp = entity.BuildingAnimationComp;
			if (buildingAnimationComp == null)
			{
				return;
			}
			Animation anim = buildingAnimationComp.Anim;
			if (!ArmoryUtils.IsAnyEquipmentActive(currentPlayer.ActiveArmory) && anim.GetClip("Idle") != null)
			{
				anim.Stop();
				anim.Play("Idle");
				Service.ShuttleController.DestroyArmoryShuttle(entity);
				return;
			}
			if (entity.StateComp.CurState == EntityState.Idle && anim.IsPlaying("Idle") && anim.GetClip("Active") != null && anim.GetClip("Intro") != null)
			{
				anim.Stop();
				anim.Play("Intro");
				this.EnqueueAnimation(buildingAnimationComp, "Active");
				Service.ShuttleController.UpdateArmoryShuttle(entity);
			}
		}

		private void UpdateContraBandShipAnimation(SmartEntity entity)
		{
			if (entity == null || entity.BuildingComp.BuildingType.Type != BuildingType.Resource || entity.BuildingComp.BuildingType.Currency != CurrencyType.Contraband)
			{
				return;
			}
			IState currentState = Service.GameStateMachine.CurrentState;
			if (!(currentState is HomeState) || ContractUtils.IsBuildingUpgrading(entity) || ContractUtils.IsBuildingConstructing(entity))
			{
				Service.ShuttleController.RemoveStarportShuttle(entity);
			}
			else
			{
				Service.ShuttleController.UpdateContrabandShuttle(entity);
			}
			ShuttleAnim shuttleForStarport = Service.ShuttleController.GetShuttleForStarport(entity);
			if (shuttleForStarport != null)
			{
				this.UpdateContraBandGeneratorAnimation(entity, shuttleForStarport);
			}
		}

		private void UpdateAnimationOnGeneratorFull(SmartEntity entity, IState currentState)
		{
			if (!(currentState is HomeState))
			{
				return;
			}
			BuildingAnimationComponent buildingAnimationComp = entity.BuildingAnimationComp;
			Animation anim = buildingAnimationComp.Anim;
			if (anim.GetClip("Full") != null)
			{
				anim.Stop();
				anim.Play("Full");
			}
			this.UpdateContraBandShipAnimation(entity);
		}

		public void PlayStopParticle(uint id, ParticleSystem particle)
		{
			if (particle == null)
			{
				return;
			}
			if (particle.isPlaying)
			{
				particle.Stop();
			}
			particle.Play();
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			switch (id)
			{
			case EventId.CurrencyCollected:
			{
				CurrencyCollectionTag currencyCollectionTag = cookie as CurrencyCollectionTag;
				SmartEntity smartEntity = (SmartEntity)currencyCollectionTag.Building;
				if (smartEntity == null)
				{
					return EatResponse.NotEaten;
				}
				BuildingAnimationComponent buildingAnimationComp = smartEntity.BuildingAnimationComp;
				if (buildingAnimationComp == null)
				{
					return EatResponse.NotEaten;
				}
				this.UpdateAnimation(smartEntity, currentState, buildingAnimationComp, true);
				return EatResponse.NotEaten;
			}
			case EventId.AudibleCurrencySpent:
			case EventId.ViewObjectsPurged:
				IL_32:
				switch (id)
				{
				case EventId.EntityPostBattleRepairStarted:
				case EventId.EntityPostBattleRepairFinished:
				{
					SmartEntity smartEntity = (SmartEntity)cookie;
					if (smartEntity == null)
					{
						return EatResponse.NotEaten;
					}
					BuildingAnimationComponent buildingAnimationComp = smartEntity.BuildingAnimationComp;
					if (buildingAnimationComp == null)
					{
						return EatResponse.NotEaten;
					}
					this.UpdateAnimation(smartEntity, currentState, buildingAnimationComp, true);
					return EatResponse.NotEaten;
				}
				case EventId.AllPostBattleRepairFinished:
					IL_4F:
					switch (id)
					{
					case EventId.ContractStarted:
					case EventId.ContractContinued:
					{
						ContractEventData contractEventData = (ContractEventData)cookie;
						this.StartAnimationOnContractStarted(contractEventData.Entity, contractEventData.Contract, currentState);
						return EatResponse.NotEaten;
					}
					case EventId.ContractStopped:
						this.UpdateAnimationOnContractStopped((Entity)cookie, currentState);
						return EatResponse.NotEaten;
					default:
						if (id != EventId.BuildingViewReady)
						{
							if (id == EventId.TroopRecruited)
							{
								ContractEventData contractEventData2 = cookie as ContractEventData;
								if (contractEventData2.Contract.DeliveryType == DeliveryType.Infantry)
								{
									SmartEntity smartEntity = (SmartEntity)contractEventData2.Entity;
									if (smartEntity != null)
									{
										BuildingAnimationComponent buildingAnimationComp = smartEntity.BuildingAnimationComp;
										if (buildingAnimationComp != null)
										{
											if (this.BuildingEligibleForActiveAnimation(smartEntity, currentState, buildingAnimationComp))
											{
												buildingAnimationComp.Anim.Stop();
												for (int i = 0; i < this.barracksOpenCloseAnims.Length; i++)
												{
													this.EnqueueAnimation(buildingAnimationComp, this.barracksOpenCloseAnims[i]);
												}
											}
										}
									}
								}
								return EatResponse.NotEaten;
							}
							if (id == EventId.WorldLoadComplete)
							{
								this.UpdateAnimations(currentState);
								return EatResponse.NotEaten;
							}
							if (id == EventId.GameStateChanged)
							{
								this.UpdateAnimations(currentState);
								return EatResponse.NotEaten;
							}
							if (id != EventId.ScreenClosing)
							{
								if (id != EventId.EquipmentDeactivated)
								{
									return EatResponse.NotEaten;
								}
								NodeList<ArmoryNode> armoryNodeList = Service.BuildingLookupController.ArmoryNodeList;
								for (ArmoryNode armoryNode = armoryNodeList.Head; armoryNode != null; armoryNode = armoryNode.Next)
								{
									this.UpdateArmoryAnimation((SmartEntity)armoryNode.Entity);
								}
								return EatResponse.NotEaten;
							}
							else
							{
								if (!(cookie is ArmoryScreen))
								{
									return EatResponse.NotEaten;
								}
								NodeList<ArmoryNode> armoryNodeList2 = Service.BuildingLookupController.ArmoryNodeList;
								for (ArmoryNode armoryNode2 = armoryNodeList2.Head; armoryNode2 != null; armoryNode2 = armoryNode2.Next)
								{
									this.UpdateArmoryAnimation((SmartEntity)armoryNode2.Entity);
								}
								return EatResponse.NotEaten;
							}
						}
						else
						{
							EntityViewParams entityViewParams = cookie as EntityViewParams;
							SmartEntity smartEntity = entityViewParams.Entity;
							GameObject mainGameObject = smartEntity.GameObjectViewComp.MainGameObject;
							Animation component = mainGameObject.GetComponent<Animation>();
							if (component == null)
							{
								return EatResponse.NotEaten;
							}
							AssetMeshDataMonoBehaviour component2 = mainGameObject.GetComponent<AssetMeshDataMonoBehaviour>();
							this.UpdateAnimation(smartEntity, currentState, new BuildingAnimationComponent(component, (!component2) ? null : component2.ListOfParticleSystems), true);
							return EatResponse.NotEaten;
						}
						break;
					}
					break;
				case EventId.ShuttleAnimStateChanged:
				{
					ShuttleAnim shuttleAnim = (ShuttleAnim)cookie;
					SmartEntity smartEntity = (SmartEntity)shuttleAnim.Starport;
					if (smartEntity.BuildingComp.BuildingType.Type == BuildingType.Armory)
					{
						Service.ShuttleController.DestroyArmoryShuttle(smartEntity);
					}
					else
					{
						this.UpdateContraBandGeneratorAnimation(smartEntity, shuttleAnim);
					}
					return EatResponse.NotEaten;
				}
				}
				goto IL_4F;
			case EventId.GeneratorJustFilled:
				this.UpdateAnimationOnGeneratorFull((SmartEntity)cookie, currentState);
				return EatResponse.NotEaten;
			case EventId.StorageDoorEvent:
			{
				SmartEntity smartEntity = (SmartEntity)cookie;
				if (smartEntity == null || smartEntity.BuildingAnimationComp == null || smartEntity.StorageComp == null)
				{
					return EatResponse.NotEaten;
				}
				if (smartEntity.StorageComp.CurrentFullnessPercentage < 1f)
				{
					BuildingAnimationComponent buildingAnimationComp = smartEntity.BuildingAnimationComp;
					if (this.BuildingEligibleForActiveAnimation(smartEntity, currentState, buildingAnimationComp))
					{
						buildingAnimationComp.Anim.Stop();
						int num = this.storageOpenCloseAnims.Length;
						for (int j = 0; j < num; j++)
						{
							this.EnqueueAnimation(buildingAnimationComp, this.storageOpenCloseAnims[j]);
						}
					}
				}
				return EatResponse.NotEaten;
			}
			}
			goto IL_32;
		}
	}
}
