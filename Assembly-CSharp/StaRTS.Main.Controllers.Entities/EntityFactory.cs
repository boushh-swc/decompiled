using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.Externals.Manimal;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Controllers.Entities.StateMachines.Attack;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.Entities
{
	public class EntityFactory : IEventObserver
	{
		public const float BUILDING_ROTATION = 1.57079637f;

		private const string EMPIRE_HEALER_COMPANION = "civilianTechnician01";

		private const string REBEL_HEALER_COMPANION = "civilianMedic01";

		private uint entityCount;

		private Dictionary<uint, Entity> entities;

		public uint EntityCount
		{
			get
			{
				return this.entityCount;
			}
		}

		public EntityFactory()
		{
			Service.EntityFactory = this;
			Service.EventManager.RegisterObserver(this, EventId.BuildingViewReady, EventPriority.Default);
			this.entities = new Dictionary<uint, Entity>();
			this.entityCount = 0u;
		}

		public SmartEntity NewEntity()
		{
			SmartEntity smartEntity = new SmartEntity();
			smartEntity.ID = this.entityCount;
			smartEntity.Add<StateComponent>(new StateComponent(smartEntity));
			this.entities.Add(smartEntity.ID, smartEntity);
			this.entityCount += 1u;
			return smartEntity;
		}

		private SmartEntity CreateWalkerBaseEntity(IntPosition boardPosition, int sizeX, int sizeY)
		{
			Entity entity = this.NewEntity();
			int num = sizeX / 2;
			TransformComponent component = new TransformComponent(boardPosition.x - num, boardPosition.z - num, 0f, false, sizeX, sizeX);
			entity.Add(component);
			SizeComponent component2 = Units.SizeCompFromGrid(sizeX, sizeY);
			entity.Add(component2);
			BoardItem boardItem = new BoardItem(Units.SizeCompFromGrid(1, 1), entity, CollisionFilters.TROOP);
			BoardItemComponent component3 = new BoardItemComponent(boardItem);
			entity.Add(component3);
			return (SmartEntity)entity;
		}

		public SmartEntity CreateTroopEntity(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition, Entity spawnBuilding, TroopSpawnMode spawnMode, bool isShooter, bool requestAsset)
		{
			SmartEntity smartEntity = this.CreateWalkerBaseEntity(boardPosition, troopType.SizeX, troopType.SizeY);
			TeamComponent component = new TeamComponent(teamType);
			smartEntity.Add(component);
			if (teamType == TeamType.Defender)
			{
				DefenderComponent component2;
				if (spawnMode == TroopSpawnMode.LeashedToBuilding)
				{
					DamageableComponent damageableComponent = spawnBuilding.Get<DamageableComponent>();
					component2 = new DefenderComponent(boardPosition.x, boardPosition.z, damageableComponent, true, damageableComponent.GetLastSpawnIndex());
				}
				else
				{
					component2 = new DefenderComponent(boardPosition.x, boardPosition.z, null, spawnMode == TroopSpawnMode.LeashedToSpawnPoint, 0);
				}
				smartEntity.Add(component2);
			}
			else
			{
				smartEntity.Add(new AttackerComponent());
			}
			TroopComponent troopComponent = new TroopComponent(troopType);
			smartEntity.Add(troopComponent);
			smartEntity.Add(new BuffComponent());
			smartEntity.Add(new HealthViewComponent());
			if (isShooter)
			{
				ShooterComponent component3 = new ShooterComponent(troopType);
				smartEntity.Add(component3);
			}
			Service.EventManager.SendEvent(EventId.TroopCreated, smartEntity);
			if (isShooter)
			{
				smartEntity.ShooterComp.TargetingDelayed = (teamType == TeamType.Attacker);
				bool flag = TroopController.IsEntityHealer(smartEntity);
				HealthType healthType = (!flag) ? HealthType.Damaging : HealthType.Healing;
				smartEntity.ShooterComp.AttackFSM = new AttackFSM(Service.BattleController, smartEntity, smartEntity.StateComp, smartEntity.ShooterComp, smartEntity.TransformComp, healthType);
				if (flag)
				{
					smartEntity.Add(new PathingComponent());
					smartEntity.Add(new HealerComponent());
				}
				else
				{
					smartEntity.Add(new KillerComponent());
				}
				SecondaryTargetsComponent component4 = new SecondaryTargetsComponent();
				smartEntity.Add(component4);
			}
			HealthComponent component5 = new HealthComponent(troopType.Health, troopComponent.TroopType.ArmorType);
			smartEntity.Add(component5);
			if (troopType.ShieldHealth > 0)
			{
				TroopShieldHealthComponent component6 = new TroopShieldHealthComponent(troopType.ShieldHealth, ArmorType.Shield);
				smartEntity.Add(component6);
			}
			if (requestAsset)
			{
				Service.EntityViewManager.LoadEntityAsset(smartEntity);
			}
			return smartEntity;
		}

		public Entity CreateTransportEntity(TransportTypeVO transportType)
		{
			Entity entity = this.NewEntity();
			SizeComponent component = Units.SizeCompFromGrid(transportType.SizeX, transportType.SizeY);
			entity.Add(component);
			TransportComponent component2 = new TransportComponent(transportType);
			entity.Add(component2);
			Service.EntityViewManager.LoadEntityAsset(entity);
			return entity;
		}

		public void DestroyTransportEntity(Entity entity)
		{
			entity.Remove<SizeComponent>();
			entity.Remove<TransportComponent>();
			this.DestroyEntity(entity, false, false);
		}

		public Entity CreateDroidEntity(CivilianTypeVO droidType, IntPosition position)
		{
			Entity entity = this.CreateWalkerBaseEntity(position, droidType.SizeX, droidType.SizeY);
			DroidComponent component = new DroidComponent();
			entity.Add(component);
			entity.Add(new CivilianComponent(droidType));
			Service.EntityViewManager.LoadEntityAsset(entity);
			return entity;
		}

		public SmartEntity CreateChampionEntity(TroopTypeVO championType, IntPosition position)
		{
			SmartEntity smartEntity = this.CreateWalkerBaseEntity(position, championType.SizeX, championType.SizeY);
			smartEntity.TransformComp.Rotation = 90f;
			smartEntity.Add(new ChampionComponent(championType));
			smartEntity.Add(new WalkerComponent(championType.AssetName, championType));
			Service.EntityViewManager.LoadEntityAsset(smartEntity);
			return smartEntity;
		}

		public Entity CreateBuildingEntity(Building building, bool createCollider, bool requestAsset, bool addSupport)
		{
			BuildingTypeVO buildingType = Service.StaticDataController.Get<BuildingTypeVO>(building.Uid);
			return this.CreateBuildingEntity(buildingType, building, createCollider, requestAsset, addSupport);
		}

		public Entity CreateBuildingEntity(BuildingTypeVO buildingType, bool createCollider, bool requestAsset, bool addSupport)
		{
			Building building = Building.FromBuildingTypeVO(buildingType);
			return this.CreateBuildingEntity(buildingType, building, createCollider, requestAsset, addSupport);
		}

		private SmartEntity CreateBuildingEntity(BuildingTypeVO buildingType, Building building, bool createCollider, bool requestAsset, bool addSupport)
		{
			int x = building.X;
			int z = building.Z;
			bool flag = buildingType.Type == BuildingType.Blocker;
			SmartEntity smartEntity = this.NewEntity();
			TeamComponent component = new TeamComponent(TeamType.Defender);
			smartEntity.Add(component);
			int num = 0;
			if (!flag && buildingType.SizeX > 1 && buildingType.SizeY > 1)
			{
				num = 1;
			}
			TransformComponent transformComponent = new TransformComponent(Units.GridToBoardX(x), Units.GridToBoardZ(z), 1.57079637f, true, Units.GridToBoardX(buildingType.SizeX - num), Units.GridToBoardZ(buildingType.SizeY - num));
			smartEntity.Add(transformComponent);
			SizeComponent sizeComponent = Units.SizeCompFromGrid(buildingType.SizeX, buildingType.SizeY);
			smartEntity.Add(sizeComponent);
			if (buildingType.Type != BuildingType.Clearable)
			{
				smartEntity.Add(new DamageableComponent(transformComponent));
			}
			BuildingComponent buildingComponent = new BuildingComponent(buildingType, building);
			smartEntity.Add(buildingComponent);
			smartEntity.Add(new BuffComponent());
			smartEntity.Add(new HealthViewComponent());
			FilterComponent filter = CollisionFilters.BUILDING;
			if (buildingType.Type == BuildingType.Trap)
			{
				filter = CollisionFilters.TRAP;
			}
			else if (buildingType.Type == BuildingType.Wall)
			{
				filter = CollisionFilters.WALL;
			}
			else if (buildingType.Type == BuildingType.Clearable)
			{
				filter = CollisionFilters.CLEARABLE;
			}
			else if (buildingType.Type == BuildingType.ChampionPlatform)
			{
				filter = CollisionFilters.PLATFORM;
			}
			BoardItem boardItem = new BoardItem(sizeComponent, smartEntity, filter);
			smartEntity.Add(new BoardItemComponent(boardItem));
			if (buildingType.Type == BuildingType.Turret)
			{
				TurretTypeVO turretType = Service.StaticDataController.Get<TurretTypeVO>(buildingType.TurretUid);
				this.AddTurretComponentsToEntity(smartEntity, turretType);
			}
			if (buildingType.Type == BuildingType.Trap)
			{
				this.AddTrapComponentsToEntity(smartEntity, buildingType);
			}
			if (buildingType.Type == BuildingType.ShieldGenerator)
			{
				this.AddShieldComponentsToEntity(smartEntity, buildingType);
			}
			HealthComponent component2 = new HealthComponent(buildingType.Health, buildingComponent.BuildingType.ArmorType);
			smartEntity.Add(component2);
			if (addSupport)
			{
				smartEntity.Add<SupportComponent>(new SupportComponent());
			}
			if (buildingType.Type == BuildingType.ChampionPlatform)
			{
				smartEntity.Add<ChampionPlatformComponent>(new ChampionPlatformComponent());
			}
			if (buildingType.Type == BuildingType.Housing)
			{
				smartEntity.Add<HousingComponent>(new HousingComponent());
			}
			switch (buildingType.Type)
			{
			case BuildingType.HQ:
				smartEntity.Add<HQComponent>(new HQComponent());
				break;
			case BuildingType.Barracks:
				smartEntity.Add<BarracksComponent>(new BarracksComponent());
				break;
			case BuildingType.Factory:
				smartEntity.Add<FactoryComponent>(new FactoryComponent());
				break;
			case BuildingType.FleetCommand:
				smartEntity.Add<FleetCommandComponent>(new FleetCommandComponent());
				break;
			case BuildingType.HeroMobilizer:
				smartEntity.Add<TacticalCommandComponent>(new TacticalCommandComponent());
				break;
			case BuildingType.Squad:
				smartEntity.Add<SquadBuildingComponent>(new SquadBuildingComponent());
				break;
			case BuildingType.Starport:
				smartEntity.Add<StarportComponent>(new StarportComponent());
				break;
			case BuildingType.DroidHut:
				smartEntity.Add<DroidHutComponent>(new DroidHutComponent());
				break;
			case BuildingType.Wall:
				smartEntity.Add<WallComponent>(new WallComponent());
				break;
			case BuildingType.Turret:
				smartEntity.Add<TurretBuildingComponent>(new TurretBuildingComponent());
				break;
			case BuildingType.TroopResearch:
				smartEntity.Add<OffenseLabComponent>(new OffenseLabComponent());
				break;
			case BuildingType.DefenseResearch:
				smartEntity.Add<DefenseLabComponent>(new DefenseLabComponent());
				break;
			case BuildingType.Resource:
				if (building.LastCollectTime == 0u)
				{
					building.LastCollectTime = ServerTime.Time;
				}
				smartEntity.Add<GeneratorComponent>(new GeneratorComponent());
				break;
			case BuildingType.Storage:
				smartEntity.Add<StorageComponent>(new StorageComponent());
				break;
			case BuildingType.Clearable:
				smartEntity.Add<ClearableComponent>(new ClearableComponent());
				break;
			case BuildingType.Cantina:
				smartEntity.Add<CantinaComponent>(new CantinaComponent());
				break;
			case BuildingType.NavigationCenter:
				smartEntity.Add<NavigationCenterComponent>(new NavigationCenterComponent());
				break;
			case BuildingType.ScoutTower:
				smartEntity.Add<ScoutTowerComponent>(new ScoutTowerComponent());
				break;
			case BuildingType.Armory:
				smartEntity.Add<ArmoryComponent>(new ArmoryComponent());
				break;
			}
			if (buildingType.IsLootable)
			{
				LootComponent component3 = new LootComponent();
				smartEntity.Add<LootComponent>(component3);
			}
			if (requestAsset)
			{
				Service.EntityViewManager.LoadEntityAsset(smartEntity);
			}
			return smartEntity;
		}

		public Entity GetEntityByID(uint id)
		{
			if (!this.entities.ContainsKey(id))
			{
				return null;
			}
			return this.entities[id];
		}

		public void AddTrapComponentsToEntity(Entity buildingEntity, BuildingTypeVO buildingType)
		{
			TrapTypeVO type = Service.StaticDataController.Get<TrapTypeVO>(buildingType.TrapUid);
			TrapState currentStorage = (TrapState)buildingEntity.Get<BuildingComponent>().BuildingTO.CurrentStorage;
			TrapComponent component = new TrapComponent(type, currentStorage);
			buildingEntity.Add(component);
		}

		public void AddShieldComponentsToEntity(Entity buildingEntity, BuildingTypeVO buildingType)
		{
			Entity entity = this.NewEntity();
			HealthComponent component = new HealthComponent(Service.ShieldController.PointsToHealth[buildingType.ShieldHealthPoints], ArmorType.Shield);
			entity.Add(component);
			entity.Add(new ShieldBorderComponent
			{
				ShieldGeneratorEntity = buildingEntity
			});
			TeamComponent component2 = new TeamComponent(buildingEntity.Get<TeamComponent>().TeamType);
			entity.Add(component2);
			Service.EntityController.AddEntity(entity);
			ShieldGeneratorComponent shieldGeneratorComponent = new ShieldGeneratorComponent();
			shieldGeneratorComponent.PointsRange = buildingType.ShieldRangePoints;
			shieldGeneratorComponent.CurrentRadius = Service.ShieldController.PointsToRange[shieldGeneratorComponent.PointsRange];
			shieldGeneratorComponent.ShieldBorderEntity = entity;
			buildingEntity.Add(shieldGeneratorComponent);
		}

		public void AddTurretComponentsToEntity(SmartEntity buildingEntity, TurretTypeVO turretType)
		{
			if (buildingEntity.TrackingComp == null)
			{
				buildingEntity.Add(new TrackingComponent(turretType, turretType.ProjectileType.Arcs));
			}
			TransformComponent transformComp = buildingEntity.TransformComp;
			ShooterComponent shooterComponent = new ShooterComponent(turretType);
			shooterComponent.AttackFSM = new AttackFSM(Service.BattleController, buildingEntity, buildingEntity.StateComp, shooterComponent, transformComp, HealthType.Damaging);
			buildingEntity.Add(shooterComponent);
			TurretShooterComponent component = new TurretShooterComponent();
			buildingEntity.Add(component);
		}

		public void RemoveEntity(Entity entity, bool removeSpawnProtection)
		{
			BoardItemComponent boardItemComponent = entity.Get<BoardItemComponent>();
			if (boardItemComponent != null)
			{
				Service.BoardController.RemoveEntity(entity, removeSpawnProtection);
			}
		}

		public void DestroyEntity(Entity entity, bool destroyView, bool removeSpawnProtection)
		{
			SmartEntity smartEntity = (SmartEntity)entity;
			if (smartEntity.BuildingComp != null)
			{
				Service.BuildingController.ClearBuildingHighlight(entity);
			}
			Service.EntityViewManager.UnloadEntityAsset(entity);
			this.RemoveEntity(entity, removeSpawnProtection);
			if (destroyView)
			{
				Service.EntityViewManager.DestroyView(entity);
			}
			uint iD = entity.ID;
			Service.EntityController.RemoveEntity(entity);
			this.entities.Remove(iD);
			Service.EventManager.SendEvent(EventId.EntityDestroyed, iD);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id != EventId.BuildingViewReady)
			{
				return EatResponse.NotEaten;
			}
			EntityViewParams entityViewParams = cookie as EntityViewParams;
			Entity entity = entityViewParams.Entity;
			BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return EatResponse.NotEaten;
			}
			GameObjectViewComponent gameObjectViewComponent = entity.Get<GameObjectViewComponent>();
			TurretTypeVO turretTypeVO = null;
			if (buildingComponent.BuildingType.Type == BuildingType.Turret)
			{
				turretTypeVO = Service.StaticDataController.Get<TurretTypeVO>(buildingComponent.BuildingType.TurretUid);
			}
			if (turretTypeVO == null || string.IsNullOrEmpty(turretTypeVO.TrackerName))
			{
				Animation component = gameObjectViewComponent.MainGameObject.GetComponent<Animation>();
				if (component != null)
				{
					AssetMeshDataMonoBehaviour component2 = gameObjectViewComponent.MainGameObject.GetComponent<AssetMeshDataMonoBehaviour>();
					entity.Add<BuildingAnimationComponent>(new BuildingAnimationComponent(component, (!component2) ? null : component2.ListOfParticleSystems));
				}
			}
			else
			{
				TrackingComponent trackingComp = entity.Get<TrackingComponent>();
				TrackingGameObjectViewComponent component3 = new TrackingGameObjectViewComponent(gameObjectViewComponent.MainGameObject, turretTypeVO, trackingComp);
				entity.Add<TrackingGameObjectViewComponent>(component3);
			}
			return EatResponse.NotEaten;
		}
	}
}
