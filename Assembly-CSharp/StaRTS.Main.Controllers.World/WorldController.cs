using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Configs;
using StaRTS.Main.Controllers.Entities;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Move;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class WorldController
	{
		public const int DEFAULT_SPAWN_PROTECTION_SIZE = 1;

		public const int DEFAULT_WALKABLE_INSET_SIZE = 1;

		private EntityController entityController;

		public Entity DroidHut
		{
			get;
			private set;
		}

		public WorldController()
		{
			Service.WorldController = this;
			this.entityController = Service.EntityController;
		}

		public Entity ProcessWorldDataBuilding(Building building, out bool validPlacement)
		{
			return this.ProcessWorldDataBuilding(building, true, false, out validPlacement);
		}

		public Entity ProcessWorldDataBuilding(Building building, bool createCollider, bool requestAsset, out bool validPlacement)
		{
			Entity entity = Service.EntityFactory.CreateBuildingEntity(building, createCollider, requestAsset, Service.WorldTransitioner.IsCurrentWorldHome());
			int x = Units.GridToBoardX(building.X);
			int z = Units.GridToBoardX(building.Z);
			BoardCell boardCell = this.AddBuildingHelper(entity, x, z, false);
			validPlacement = (boardCell != null);
			return entity;
		}

		public BoardCell AddBuildingHelper(Entity building, int x, int z, bool isUpgrade)
		{
			BoardCell boardCell = this.AddBuildingToBoard(building, x, z, false);
			if (boardCell == null)
			{
				return null;
			}
			this.AddEntityToWorld(building);
			if (!isUpgrade)
			{
				Service.EventManager.SendEvent(EventId.BuildingPlacedOnBoard, building);
			}
			return boardCell;
		}

		public void AddEntityToWorld(Entity entity)
		{
			this.entityController.AddEntity(entity);
		}

		public BoardCell AddBuildingToBoard(Entity building, int boardX, int boardZ, bool sendEvent)
		{
			BoardItemComponent boardItemComponent = building.Get<BoardItemComponent>();
			BoardItem boardItem = boardItemComponent.BoardItem;
			SizeComponent size = boardItem.Size;
			BuildingComponent buildingComponent = building.Get<BuildingComponent>();
			BuildingTypeVO buildingType = buildingComponent.BuildingType;
			bool flag = buildingType.Type == BuildingType.Clearable || buildingType.Type == BuildingType.Trap || buildingType.Type == BuildingType.ChampionPlatform;
			bool flag2 = buildingType.Type == BuildingType.Blocker;
			int walkableGap = (!flag2) ? this.CalculateWalkableGap(size) : 0;
			FlagStamp flagStamp = this.CreateFlagStamp(building, buildingType, size, walkableGap);
			if (!flag)
			{
				this.AddUnWalkableUnDestructibleFlags(flagStamp, size, walkableGap, flag2);
			}
			boardItem.FlagStamp = flagStamp;
			BoardController boardController = Service.BoardController;
			BoardCell boardCell = boardController.Board.AddChild(boardItem, boardX, boardZ, building.Get<HealthComponent>(), !flag2);
			if (boardCell == null)
			{
				Service.Logger.ErrorFormat("Failed to add building {0}:{1} at ({2},{3})", new object[]
				{
					buildingComponent.BuildingTO.Key,
					buildingComponent.BuildingTO.Uid,
					boardX,
					boardZ
				});
				return null;
			}
			TransformComponent transformComponent = building.Get<TransformComponent>();
			transformComponent.X = boardX;
			transformComponent.Z = boardZ;
			DamageableComponent damageableComponent = building.Get<DamageableComponent>();
			if (damageableComponent != null)
			{
				damageableComponent.Init();
			}
			buildingComponent.BuildingTO.SyncWithTransform(transformComponent);
			if (sendEvent)
			{
				Service.EventManager.SendEvent(EventId.BuildingPlacedOnBoard, building);
			}
			if (buildingType.Type == BuildingType.DroidHut)
			{
				this.DroidHut = building;
			}
			return boardCell;
		}

		public int CalculateWalkableGap(SizeComponent size)
		{
			if (Units.BoardToGridX(size.Width) > 1 && Units.BoardToGridZ(size.Depth) > 1)
			{
				return 1;
			}
			return 0;
		}

		public FlagStamp CreateFlagStamp(Entity building, BuildingTypeVO buildingVO, SizeComponent size, int walkableGap)
		{
			ShieldGeneratorComponent shieldGeneratorComponent = null;
			BuildingComponent buildingComponent = null;
			if (building != null)
			{
				shieldGeneratorComponent = building.Get<ShieldGeneratorComponent>();
				buildingComponent = building.Get<BuildingComponent>();
			}
			if (shieldGeneratorComponent != null)
			{
				return Service.ShieldController.CreateFlagStampForShield(shieldGeneratorComponent, size, walkableGap);
			}
			uint num;
			if (buildingVO != null && buildingVO.Type == BuildingType.Trap)
			{
				num = 0u;
			}
			else
			{
				num = 4u;
			}
			if (buildingVO != null && buildingVO.AllowDefensiveSpawn)
			{
				num |= 32u;
			}
			if (buildingComponent != null && buildingComponent.BuildingType.SpawnProtection > 0)
			{
				int num2 = buildingComponent.BuildingType.SpawnProtection;
				int num3 = num2 - (size.Width - walkableGap);
				if (num3 % 2 == 1)
				{
					num2++;
				}
				return new FlagStamp(num2, num2, num, false);
			}
			return new FlagStamp(size.Width - walkableGap + 2, size.Depth - walkableGap + 2, num, false);
		}

		public void AddUnWalkableUnDestructibleFlags(FlagStamp flagStamp, SizeComponent size, int walkableGap, bool blocker)
		{
			flagStamp.SetFlagsInRectCenter(size.Width - walkableGap, size.Depth - walkableGap, ((walkableGap <= 0) ? 2u : 1u) | ((!blocker) ? 0u : 64u));
		}

		public BoardCell MoveBuildingWithinBoard(Entity building, int boardX, int boardZ)
		{
			BoardController boardController = Service.BoardController;
			BoardItemComponent boardItemComponent = building.Get<BoardItemComponent>();
			BoardItem boardItem = boardItemComponent.BoardItem;
			BuildingComponent buildingComponent = building.Get<BuildingComponent>();
			bool checkSkirt = buildingComponent.BuildingType.Type != BuildingType.Blocker;
			BoardCell boardCell = boardController.Board.MoveChild(boardItem, boardX, boardZ, building.Get<HealthComponent>(), true, checkSkirt);
			if (boardCell != null)
			{
				TransformComponent transformComponent = building.Get<TransformComponent>();
				transformComponent.X = boardCell.X;
				transformComponent.Z = boardCell.Z;
				DamageableComponent damageableComponent = building.Get<DamageableComponent>();
				if (damageableComponent != null)
				{
					damageableComponent.Init();
				}
				Building buildingTO = building.Get<BuildingComponent>().BuildingTO;
				buildingTO.SyncWithTransform(transformComponent);
				Service.EventManager.SendEvent(EventId.BuildingMovedOnBoard, building);
			}
			else
			{
				Service.Logger.ErrorFormat("Failed to move building {0}:{1} to ({2},{3})", new object[]
				{
					buildingComponent.BuildingTO.Key,
					buildingComponent.BuildingTO.Uid,
					boardX,
					boardZ
				});
			}
			return boardCell;
		}

		public void FindValidPositionsAndAddBuildings(List<Entity> entities)
		{
			PositionMap positionMap = new PositionMap();
			for (int i = 0; i < entities.Count; i++)
			{
				Entity entity = entities[i];
				BuildingComponent buildingComponent = entity.Get<BuildingComponent>();
				Building buildingTO = buildingComponent.BuildingTO;
				if (this.FindValidPositionAndUpdate(entity, buildingTO))
				{
					positionMap.AddPosition(buildingTO.Key, new Position
					{
						X = buildingTO.X,
						Z = buildingTO.Z
					});
				}
			}
			Service.ServerAPI.Enqueue(new BuildingMultiMoveCommand(new BuildingMultiMoveRequest
			{
				PositionMap = positionMap
			}));
		}

		private bool FindValidPositionAndUpdate(Entity entity, Building building)
		{
			int x = building.X;
			int z = building.Z;
			int num = 0;
			int num2 = 0;
			Service.BuildingController.FindStartingLocation(entity, out num, out num2, x, z, false);
			if (this.AddBuildingHelper(entity, num, num2, false) == null)
			{
				Service.Logger.ErrorFormat("Attempted to fix position for building {0} at ({1},{2}) and no valid location available", new object[]
				{
					building.Key,
					x,
					z
				});
				return false;
			}
			Service.Logger.ErrorFormat("Fixed invalid position for building {0} at ({1},{2}) to ({3},{4})", new object[]
			{
				building.Key,
				x,
				z,
				num,
				num2
			});
			building.X = num;
			building.Z = num2;
			return true;
		}

		public void ResetWorld()
		{
			EntityFactory entityFactory = Service.EntityFactory;
			EntityList allEntities = this.entityController.GetAllEntities();
			for (Entity entity = allEntities.Head; entity != null; entity = entity.Next)
			{
				entityFactory.DestroyEntity(entity, true, true);
			}
			this.DroidHut = null;
			Service.SpatialIndexController.Reset();
			Service.BoardController.ResetBoard();
			Service.FXManager.Reset();
			if (!HardwareProfile.IsLowEndDevice())
			{
				Service.TerrainBlendController.ResetTerrain();
			}
			Service.EventManager.SendEvent(EventId.WorldReset, null);
		}
	}
}
