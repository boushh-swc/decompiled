using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.DataStructures;
using StaRTS.FX;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Pathfinding;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class TransportController : IEventObserver
	{
		private static readonly Vector3 SPAWN_POSITION = new Vector3(60f, 12f, 60f);

		private static readonly Vector3 FACTORY_ORIENTATION = new Vector3(-1f, 0f, 0f);

		private static readonly Vector3 STARPORT_ORIENTATION = new Vector3(-1f, 0f, 0f);

		private static readonly Vector3 DOCK_OFFSET = new Vector3(0f, 1f, 3.8f);

		private static readonly float FACTORY_WALL_HEIGHT = 1.7f;

		private string TRANSPORT_SHIP_EMPIRE = "ThetaClassBarge1";

		private string TRANSPORT_SHIP_REBEL = "Cr25transport1";

		private const string FACTORY_PRODUCT = "FactoryProduct";

		private const float VEHICLE_SCALE = 0.8f;

		private const float FADE_OUT_TIME = 1f;

		private const int TARGET_PROXIMITY = 2;

		private const string FACTORY_VEHICLE_LOCATOR = "locator_vehicle";

		private const string LANDING_EFFECT = "fx_landing_lrg";

		private const string TAKEOFF_EFFECT = "fx_takeoff_lrg";

		private const float LANDING_EFFECT_DELAY = 0.2f;

		private const float TAKEOFF_EFFECT_DELAY = 5f;

		private const float VEHICLE_SPAWN_DELAY = 3f;

		private Dictionary<int, GameObject> landingFxObjects;

		private Dictionary<int, GameObject> takeOffFxObjects;

		private Dictionary<int, AssetHandle> landingFxHandles;

		private Dictionary<int, AssetHandle> takeOffFxHandles;

		private int activeEffectsCount;

		private const int MAX_TROOP_EFFECTS_PER_STARPORT = 10;

		private EntityController entityController;

		private StaticDataController sdc;

		private FXManager fxManager;

		private ViewFader entityFader;

		private Vector3 factoryOffset;

		private Dictionary<Entity, TroopTypeVO> busyEntities;

		private Dictionary<Entity, TransportTroopEffect> troopEffectsByEntity;

		private Dictionary<Entity, int> numTroopEffectsByStarport;

		private List<Entity> busyTransportByFactory;

		public TransportController()
		{
			this.entityController = Service.EntityController;
			this.sdc = Service.StaticDataController;
			this.fxManager = Service.FXManager;
			this.entityFader = new ViewFader();
			Service.TransportController = this;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.TroopRecruited, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractStarted, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractContinued, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.ContractCanceled, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.TroopReachedPathEnd, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.WorldReset, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BuildingReplaced, EventPriority.Default);
			this.landingFxHandles = new Dictionary<int, AssetHandle>();
			this.takeOffFxHandles = new Dictionary<int, AssetHandle>();
			this.landingFxObjects = new Dictionary<int, GameObject>();
			this.takeOffFxObjects = new Dictionary<int, GameObject>();
			this.busyEntities = new Dictionary<Entity, TroopTypeVO>();
		}

		private void FactoryReached(object cookie)
		{
			ContractEventData contractEventData = cookie as ContractEventData;
			Entity entity = contractEventData.Entity;
			this.DespawnVehicle(entity);
			Contract contract = Service.ISupportController.FindCurrentContract(entity.Get<BuildingComponent>().BuildingTO.Key);
			if (contract != null)
			{
				KeyValuePair<Contract, Entity> keyValuePair = new KeyValuePair<Contract, Entity>(contract, entity);
				Service.ViewTimerManager.CreateViewTimer(3f, false, new TimerDelegate(this.CallbackSpawnVehicle), keyValuePair);
			}
			Service.EventManager.SendEvent(EventId.TransportDeparted, null);
		}

		private void CallbackSpawnVehicle(uint id, object cookie)
		{
			KeyValuePair<Contract, Entity> keyValuePair = (KeyValuePair<Contract, Entity>)cookie;
			Contract key = keyValuePair.Key;
			Entity value = keyValuePair.Value;
			this.SpawnVehicle(key, value);
		}

		private void StarportReached(object cookie)
		{
			KeyValuePair<Entity, ContractEventData> keyValuePair = (KeyValuePair<Entity, ContractEventData>)cookie;
			SmartEntity starport = (SmartEntity)keyValuePair.Key;
			ContractEventData value = keyValuePair.Value;
			this.RemoveTransportRequest(value);
			TroopTypeVO troop = this.sdc.Get<TroopTypeVO>(value.Contract.ProductUid);
			StorageSpreadUtils.AddTroopToStarportVisually(starport, troop);
			Service.EventManager.SendEvent(EventId.TransportDeparted, null);
		}

		private void ArrivingAtBuilding(object cookie)
		{
			Service.EventManager.SendEvent(EventId.TransportArrived, null);
			if (cookie != null)
			{
				this.LoadAndPlayEffects(cookie);
			}
		}

		private void LoadAndPlayEffects(object cookie)
		{
			int key = ((KeyValuePair<int, Vector3>)cookie).Key;
			if (!this.landingFxHandles.ContainsKey(key))
			{
				AssetHandle value = AssetHandle.Invalid;
				Service.AssetManager.Load(ref value, "fx_landing_lrg", new AssetSuccessDelegate(this.OnLandingFxSuccess), null, cookie);
				this.landingFxHandles.Add(key, value);
			}
			if (!this.takeOffFxHandles.ContainsKey(key))
			{
				AssetHandle value2 = AssetHandle.Invalid;
				Service.AssetManager.Load(ref value2, "fx_takeoff_lrg", new AssetSuccessDelegate(this.OnTakeOffFxSuccess), null, cookie);
				this.takeOffFxHandles.Add(key, value2);
			}
		}

		private void OnLandingFxSuccess(object asset, object cookie)
		{
			KeyValuePair<int, Vector3> keyValuePair = (KeyValuePair<int, Vector3>)cookie;
			int key = keyValuePair.Key;
			GameObject gameObject = (GameObject)asset;
			gameObject = Service.AssetManager.CloneGameObject(gameObject);
			gameObject.transform.position = keyValuePair.Value;
			Service.ViewTimerManager.CreateViewTimer(0.2f, false, new TimerDelegate(this.CallbackPlayParticle), gameObject);
			if (!this.landingFxObjects.ContainsKey(key))
			{
				this.landingFxObjects.Add(key, gameObject);
			}
		}

		private void OnTakeOffFxSuccess(object asset, object cookie)
		{
			KeyValuePair<int, Vector3> keyValuePair = (KeyValuePair<int, Vector3>)cookie;
			int key = keyValuePair.Key;
			GameObject gameObject = (GameObject)asset;
			gameObject = Service.AssetManager.CloneGameObject(gameObject);
			gameObject.transform.position = keyValuePair.Value;
			Service.ViewTimerManager.CreateViewTimer(5f, false, new TimerDelegate(this.CallbackPlayParticle), gameObject);
			if (!this.takeOffFxObjects.ContainsKey(key))
			{
				this.takeOffFxObjects.Add(key, gameObject);
			}
		}

		private void CallbackPlayParticle(uint id, object cookie)
		{
			GameObject effectObject = (GameObject)cookie;
			this.PlayParticle(effectObject);
		}

		private void PlayParticle(GameObject effectObject)
		{
			if (effectObject != null)
			{
				ParticleSystem component = effectObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.Play();
				}
			}
		}

		private void UnloadEffects(object cookie)
		{
			int key = ((KeyValuePair<int, Vector3>)cookie).Key;
			if (this.landingFxObjects.ContainsKey(key))
			{
				UnityEngine.Object.Destroy(this.landingFxObjects[key]);
				this.landingFxObjects.Remove(key);
			}
			if (this.takeOffFxObjects.ContainsKey(key))
			{
				UnityEngine.Object.Destroy(this.takeOffFxObjects[key]);
				this.takeOffFxObjects.Remove(key);
			}
			if (this.landingFxHandles.ContainsKey(key))
			{
				AssetHandle assetHandle = this.landingFxHandles[key];
				if (assetHandle != AssetHandle.Invalid)
				{
					Service.AssetManager.Unload(assetHandle);
					this.landingFxHandles.Remove(key);
				}
			}
			if (this.takeOffFxHandles.ContainsKey(key))
			{
				AssetHandle assetHandle2 = this.takeOffFxHandles[key];
				if (assetHandle2 != AssetHandle.Invalid)
				{
					Service.AssetManager.Unload(assetHandle2);
					this.takeOffFxHandles.Remove(key);
				}
			}
			this.activeEffectsCount--;
		}

		private void BuildSpline(LinearSpline spline, Vector3 startPosition, Vector3 pickupPosition, Vector3 dropoffPosition, Entity starportEntity, ContractEventData contractData)
		{
			spline.Start();
			KeyValuePair<int, Vector3> keyValuePair = new KeyValuePair<int, Vector3>(this.activeEffectsCount, dropoffPosition);
			this.activeEffectsCount++;
			Vector3 vector = pickupPosition + new Vector3(0f, 12f, 0f);
			Quaternion rotation = Quaternion.LookRotation(vector - startPosition);
			spline.AddWayPoint(startPosition, rotation);
			Vector3 position = Vector3.Lerp(startPosition, vector, 0.6f);
			spline.AddWayPoint(position, rotation, 0.9f, 0f);
			position = Vector3.Lerp(startPosition, vector, 0.7f);
			spline.AddWayPoint(position, rotation, 0.8f, 0f);
			position = Vector3.Lerp(startPosition, vector, 0.8f);
			spline.AddWayPoint(position, rotation, 0.7f, 0f);
			position = Vector3.Lerp(startPosition, vector, 0.9f);
			spline.AddWayPoint(position, rotation, 0.6f, 0f);
			Quaternion rotation2 = Quaternion.LookRotation(TransportController.FACTORY_ORIENTATION);
			spline.AddWayPoint(vector, rotation2, 0.3f, 0f, new WaypointReached(this.ArrivingAtBuilding), null);
			spline.AddWayPoint(pickupPosition, rotation2, 0.3f, 2f, new WaypointReached(this.FactoryReached), contractData);
			Quaternion rotation3 = Quaternion.LookRotation(TransportController.FACTORY_ORIENTATION + new Vector3(0f, -0.2f, 0f));
			spline.AddWayPoint(vector + new Vector3(0f, -5f, 0f), rotation3, 0.25f, 0f);
			Vector3 vector2 = dropoffPosition + new Vector3(0f, 12f, 0f);
			Vector3 vector3 = vector2 - vector;
			vector3.Normalize();
			Quaternion rotation4 = Quaternion.LookRotation(vector3 + new Vector3(0f, -0.2f, 0f));
			spline.AddWayPoint(vector, rotation4, 0.7f, 0f);
			Vector3 position2 = Vector3.Lerp(vector, vector2, 0.2f);
			Quaternion rotation5 = Quaternion.LookRotation(vector3);
			spline.AddWayPoint(position2, rotation5, 0.8f, 0f);
			Vector3 position3 = Vector3.Lerp(vector, vector2, 0.3f);
			spline.AddWayPoint(position3, rotation5);
			position = Vector3.Lerp(vector, vector2, 0.6f);
			spline.AddWayPoint(position, rotation5, 0.8f, 0f);
			position = Vector3.Lerp(vector, vector2, 0.7f);
			spline.AddWayPoint(position, rotation5, 0.6f, 0f);
			position = Vector3.Lerp(vector, vector2, 0.8f);
			spline.AddWayPoint(position, rotation5, 0.5f, 0f);
			position = Vector3.Lerp(vector, vector2, 0.9f);
			spline.AddWayPoint(position, rotation5, 0.4f, 0f);
			rotation2 = Quaternion.LookRotation(TransportController.STARPORT_ORIENTATION);
			spline.AddWayPoint(vector2, rotation2, 0.3f, 0f, new WaypointReached(this.ArrivingAtBuilding), keyValuePair);
			spline.AddWayPoint(dropoffPosition, rotation2, 0.3f, 2f, new WaypointReached(this.StarportReached), new KeyValuePair<Entity, ContractEventData>(starportEntity, contractData));
			rotation3 = Quaternion.LookRotation(TransportController.STARPORT_ORIENTATION + new Vector3(0f, -0.2f, 0f));
			spline.AddWayPoint(vector2 + new Vector3(0f, -5f, 0f), rotation3, 0.25f, 0f);
			Vector3 vector4 = startPosition - vector2;
			vector4.Normalize();
			Quaternion rotation6 = Quaternion.LookRotation(vector4 + new Vector3(0f, -0.2f, 0f));
			spline.AddWayPoint(vector2, rotation6, 0.7f, 0f, new WaypointReached(this.UnloadEffects), keyValuePair);
			position2 = Vector3.Lerp(vector2, startPosition, 0.2f);
			Quaternion rotation7 = Quaternion.LookRotation(vector4);
			spline.AddWayPoint(position2, rotation7, 0.8f, 0f);
			position3 = Vector3.Lerp(vector2, startPosition, 0.3f);
			spline.AddWayPoint(position3, rotation7);
			spline.AddWayPoint(startPosition, rotation7);
		}

		private Entity FindIdleTransport(TransportTypeVO transportType)
		{
			NodeList<TransportNode> nodeList = this.entityController.GetNodeList<TransportNode>();
			for (TransportNode transportNode = nodeList.Head; transportNode != null; transportNode = transportNode.Next)
			{
				if (transportNode.State.CurState == EntityState.Idle)
				{
					transportNode.Transport.GameObj.SetActive(true);
					return transportNode.Entity;
				}
			}
			Entity entity = Service.EntityFactory.CreateTransportEntity(transportType);
			Service.EntityController.AddEntity(entity);
			return entity;
		}

		private SmartEntity FindIdleStarport(ContractEventData contractData)
		{
			SmartEntity smartEntity = StorageSpreadUtils.FindLeastFullStarport();
			if (smartEntity != null)
			{
				TroopTypeVO troop = this.sdc.Get<TroopTypeVO>(contractData.Contract.ProductUid);
				StorageSpreadUtils.AddTroopToStarportReserve(smartEntity, troop);
			}
			return smartEntity;
		}

		private bool DespawnVehicle(Entity factoryEntity)
		{
			if (this.busyEntities.ContainsKey(factoryEntity))
			{
				this.fxManager.RemoveAttachedFXFromEntity(factoryEntity, "FactoryProduct");
				this.busyEntities.Remove(factoryEntity);
				return true;
			}
			return false;
		}

		private void OnVehicleAssetLoadSuccess(GameObject instance, Entity parentEntity, float value)
		{
			Animator component = instance.GetComponent<Animator>();
			if (component != null)
			{
				component.SetInteger("Motivation", 4);
			}
			if (this.busyEntities.ContainsKey(parentEntity))
			{
				float factoryRotation = this.busyEntities[parentEntity].FactoryRotation;
				Transform transform = instance.transform;
				transform.rotation = Quaternion.Euler(0f, -90f + factoryRotation, 0f);
				transform.localScale *= 0.8f * this.busyEntities[parentEntity].FactoryScaleFactor;
			}
		}

		private bool SpawnVehicle(Contract contract, Entity contractEntity)
		{
			if (this.busyEntities.ContainsKey(contractEntity))
			{
				return true;
			}
			TroopTypeVO optional = this.sdc.GetOptional<TroopTypeVO>(contract.ProductUid);
			if (optional == null)
			{
				Service.Logger.Error("Could not find troop with uid " + contract.ProductUid);
				return true;
			}
			bool flag = this.DespawnVehicle(contractEntity);
			string troopID = optional.TroopID;
			string assetName;
			if (troopID == "ATAT")
			{
				assetName = "atatfactory_emp-mod";
			}
			else if (troopID == "MHC")
			{
				assetName = "umhcfactory_emp-mod";
			}
			else
			{
				assetName = optional.AssetName;
			}
			GameObjectViewComponent gameObjectViewComponent = contractEntity.Get<GameObjectViewComponent>();
			if (gameObjectViewComponent == null)
			{
				return true;
			}
			GameObject mainGameObject = gameObjectViewComponent.MainGameObject;
			if (mainGameObject == null)
			{
				return true;
			}
			Transform transform = mainGameObject.transform.Find("locator_vehicle");
			if (transform == null)
			{
				return true;
			}
			Vector3 offset = Vector3.zero;
			offset = transform.position + new Vector3(0f, 0.2f, 0f) - mainGameObject.transform.position;
			this.fxManager.CreateAndAttachFXToEntity(contractEntity, assetName, "FactoryProduct", new FXManager.AttachedFXLoadedCallback(this.OnVehicleAssetLoadSuccess), false, offset, true);
			this.busyEntities.Add(contractEntity, optional);
			return !flag;
		}

		private void SpawnInfantry(ContractEventData contractData)
		{
			SmartEntity smartEntity = this.FindIdleStarport(contractData);
			if (smartEntity == null)
			{
				return;
			}
			Entity entity = contractData.Entity;
			TransformComponent transformComponent = entity.Get<TransformComponent>();
			BoardCell boardCell = null;
			IntPosition boardPosition = new IntPosition(transformComponent.X, transformComponent.Z);
			TroopTypeVO troopTypeVO = this.sdc.Get<TroopTypeVO>(contractData.Contract.ProductUid);
			Service.TroopController.FinalizeSafeBoardPosition(troopTypeVO, ref entity, ref boardPosition, ref boardCell, TeamType.Defender, TroopSpawnMode.Unleashed, true);
			SmartEntity smartEntity2 = Service.EntityFactory.CreateTroopEntity(troopTypeVO, TeamType.Defender, boardPosition, entity, TroopSpawnMode.Unleashed, false, true);
			BoardItemComponent boardItemComp = smartEntity2.BoardItemComp;
			Service.BoardController.Board.AddChild(boardItemComp.BoardItem, boardCell.X, boardCell.Z, null, false);
			Service.EntityController.AddEntity(smartEntity2);
			TroopComponent troopComp = smartEntity2.TroopComp;
			TeamComponent teamComp = smartEntity2.TeamComp;
			bool flag = false;
			PathingManager pathingManager = Service.PathingManager;
			pathingManager.StartPathing(smartEntity2, smartEntity, smartEntity2.TransformComp, false, out flag, 0, new PathTroopParams
			{
				TroopWidth = smartEntity2.SizeComp.Width,
				DPS = 0,
				MinRange = 0u,
				MaxRange = 2u,
				MaxSpeed = troopComp.SpeedVO.MaxSpeed,
				PathSearchWidth = troopComp.TroopType.PathSearchWidth,
				IsMelee = true,
				IsOverWall = false,
				IsHealer = false,
				CrushesWalls = false,
				IsTargetShield = false,
				TargetInRangeModifier = troopComp.TroopType.TargetInRangeModifier
			}, new PathBoardParams
			{
				IgnoreWall = teamComp != null && teamComp.IsDefender(),
				Destructible = false
			}, false, true);
			if (!flag)
			{
				pathingManager.StartPathing(smartEntity2, smartEntity, smartEntity2.TransformComp, false, out flag, 0, new PathTroopParams
				{
					TroopWidth = smartEntity2.SizeComp.Width,
					DPS = 0,
					MinRange = 0u,
					MaxRange = 2u,
					MaxSpeed = troopComp.SpeedVO.MaxSpeed,
					PathSearchWidth = troopComp.TroopType.PathSearchWidth,
					IsMelee = true,
					IsOverWall = false,
					IsHealer = false,
					CrushesWalls = false,
					IsTargetShield = false,
					TargetInRangeModifier = troopComp.TroopType.TargetInRangeModifier
				}, new PathBoardParams
				{
					IgnoreWall = true,
					Destructible = false
				}, false, true);
			}
			smartEntity2.StateComp.CurState = EntityState.Moving;
			bool showFullEffect = true;
			if (this.numTroopEffectsByStarport == null)
			{
				this.numTroopEffectsByStarport = new Dictionary<Entity, int>();
			}
			if (this.numTroopEffectsByStarport.ContainsKey(smartEntity))
			{
				Dictionary<Entity, int> dictionary;
				SmartEntity key;
				int num;
				(dictionary = this.numTroopEffectsByStarport)[key = smartEntity] = (num = dictionary[key]) + 1;
				if (num >= 10)
				{
					showFullEffect = false;
				}
			}
			else
			{
				this.numTroopEffectsByStarport.Add(smartEntity, 1);
			}
			if (this.troopEffectsByEntity == null)
			{
				this.troopEffectsByEntity = new Dictionary<Entity, TransportTroopEffect>();
			}
			this.troopEffectsByEntity.Add(smartEntity2, new TransportTroopEffect(smartEntity2, troopTypeVO, smartEntity, this.entityFader, new TransportTroopEffect.OnEffectFinished(this.OnTroopEffectFinished), showFullEffect));
		}

		private void OnTroopEffectFinished(Entity troopEntity, Entity starportEntity)
		{
			this.troopEffectsByEntity.Remove(troopEntity);
			Dictionary<Entity, int> dictionary;
			(dictionary = this.numTroopEffectsByStarport)[starportEntity] = dictionary[starportEntity] - 1;
		}

		private bool CountTransportRequest(ContractEventData contractData)
		{
			if (this.busyTransportByFactory == null)
			{
				this.busyTransportByFactory = new List<Entity>();
			}
			if (this.busyTransportByFactory.Contains(contractData.Entity))
			{
				return false;
			}
			this.busyTransportByFactory.Add(contractData.Entity);
			return true;
		}

		private void RemoveTransportRequest(ContractEventData contractData)
		{
			if (this.busyTransportByFactory != null && this.busyTransportByFactory.Contains(contractData.Entity))
			{
				this.busyTransportByFactory.Remove(contractData.Entity);
			}
		}

		private void SpawnTransport(ContractEventData contractData)
		{
			if (!this.CountTransportRequest(contractData))
			{
				return;
			}
			Entity entity = contractData.Entity;
			BuildingTypeVO buildingVO = contractData.BuildingVO;
			string uid;
			if (buildingVO.Faction == FactionType.Empire)
			{
				uid = this.TRANSPORT_SHIP_EMPIRE;
			}
			else
			{
				if (buildingVO.Faction != FactionType.Rebel)
				{
					return;
				}
				uid = this.TRANSPORT_SHIP_REBEL;
			}
			TransportTypeVO transportType = Service.StaticDataController.Get<TransportTypeVO>(uid);
			Entity entity2 = this.FindIdleTransport(transportType);
			if (entity2 == null)
			{
				return;
			}
			Entity entity3 = this.FindIdleStarport(contractData);
			if (entity3 == null)
			{
				return;
			}
			TransformComponent transformComponent = entity.Get<TransformComponent>();
			Vector3 vector = new Vector3(Units.BoardToWorldX(transformComponent.CenterX()), 0f, Units.BoardToWorldZ(transformComponent.CenterZ()));
			GameObject vehicleLocator = entity.Get<GameObjectViewComponent>().VehicleLocator;
			if (vehicleLocator != null)
			{
				this.factoryOffset = new Vector3(0f, vehicleLocator.transform.position.y + TransportController.FACTORY_WALL_HEIGHT, 0f);
			}
			else
			{
				this.factoryOffset = new Vector3(0f, TransportController.FACTORY_WALL_HEIGHT, 0f);
			}
			vector += this.factoryOffset;
			transformComponent = entity3.Get<TransformComponent>();
			Vector3 vector2 = new Vector3(Units.BoardToWorldX(transformComponent.CenterX()), 0f, Units.BoardToWorldZ(transformComponent.CenterZ()));
			vector2 += TransportController.DOCK_OFFSET;
			TransportComponent transportComponent = entity2.Get<TransportComponent>();
			this.BuildSpline(transportComponent.Spline, TransportController.SPAWN_POSITION, vector, vector2, entity3, contractData);
			entity2.Get<StateComponent>().CurState = EntityState.Moving;
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.ContractStarted && id != EventId.ContractContinued)
			{
				if (id != EventId.TroopRecruited)
				{
					if (id != EventId.BuildingReplaced)
					{
						if (id != EventId.WorldReset)
						{
							if (id != EventId.TroopReachedPathEnd)
							{
								if (id == EventId.ContractCanceled)
								{
									ContractEventData contractEventData = cookie as ContractEventData;
									DeliveryType deliveryType = contractEventData.Contract.DeliveryType;
									if (deliveryType == DeliveryType.Vehicle)
									{
										this.DespawnVehicle(contractEventData.Entity);
									}
								}
							}
							else
							{
								Entity entity = cookie as Entity;
								if (entity.Get<ShooterComponent>() == null && entity.Get<DroidComponent>() == null && this.troopEffectsByEntity.ContainsKey(entity))
								{
									TransportTroopEffect transportTroopEffect = this.troopEffectsByEntity[entity];
									transportTroopEffect.OnTroopReachedPathEnd();
								}
							}
						}
						else
						{
							if (this.troopEffectsByEntity != null)
							{
								foreach (TransportTroopEffect current in this.troopEffectsByEntity.Values)
								{
									current.Cleanup();
								}
								this.troopEffectsByEntity.Clear();
							}
							if (this.busyTransportByFactory != null)
							{
								this.busyTransportByFactory.Clear();
							}
							if (this.numTroopEffectsByStarport != null)
							{
								this.numTroopEffectsByStarport.Clear();
							}
						}
					}
					else
					{
						SmartEntity smartEntity = (SmartEntity)cookie;
						StarportComponent starportComp = smartEntity.StarportComp;
						if (starportComp != null && this.numTroopEffectsByStarport != null)
						{
							foreach (KeyValuePair<Entity, int> current2 in this.numTroopEffectsByStarport)
							{
								if (current2.Key.Get<StarportComponent>() == null)
								{
									this.numTroopEffectsByStarport[smartEntity] = current2.Value;
									this.numTroopEffectsByStarport.Remove(current2.Key);
									break;
								}
							}
						}
					}
				}
				else
				{
					ContractEventData contractEventData2 = cookie as ContractEventData;
					DeliveryType deliveryType2 = contractEventData2.Contract.DeliveryType;
					if (deliveryType2 != DeliveryType.Vehicle)
					{
						if (deliveryType2 == DeliveryType.Infantry || deliveryType2 == DeliveryType.Mercenary)
						{
							this.SpawnInfantry(contractEventData2);
						}
					}
					else
					{
						this.SpawnTransport(contractEventData2);
					}
				}
			}
			else
			{
				ContractEventData contractEventData3 = cookie as ContractEventData;
				if (contractEventData3.Contract.DeliveryType == DeliveryType.Vehicle)
				{
					this.SpawnVehicle(contractEventData3.Contract, contractEventData3.Entity);
				}
			}
			return EatResponse.NotEaten;
		}
	}
}
