using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Shared;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.World.Deploying;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class TroopController : AbstractDeployableController
	{
		private const int DROPSHIP_SEARCH_RADIUS = 2;

		private BoardController boardController;

		public TroopController()
		{
			Service.TroopController = this;
			this.boardController = Service.BoardController;
		}

		public SmartEntity SpawnTroop(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition, TroopSpawnMode spawnMode, bool sendPlacedEvent)
		{
			return this.SpawnTroop(troopType, teamType, boardPosition, spawnMode, sendPlacedEvent, false);
		}

		public SmartEntity SpawnTroop(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition, TroopSpawnMode spawnMode, bool sendPlacedEvent, bool forceAllow)
		{
			return this.SpawnTroop(troopType, teamType, boardPosition, spawnMode, sendPlacedEvent, forceAllow, VisitorType.Invalid);
		}

		public SmartEntity SpawnTroop(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition, TroopSpawnMode spawnMode, bool sendPlacedEvent, bool forceAllow, VisitorType visitorType)
		{
			Entity spawnBuilding = null;
			BoardCell boardCell = null;
			if (!this.FinalizeSafeBoardPosition(troopType, ref spawnBuilding, ref boardPosition, ref boardCell, teamType, spawnMode, forceAllow))
			{
				return null;
			}
			SmartEntity smartEntity = Service.EntityFactory.CreateTroopEntity(troopType, teamType, boardPosition, spawnBuilding, spawnMode, true, true);
			if (smartEntity == null)
			{
				return null;
			}
			SpawnComponent component = new SpawnComponent(visitorType);
			smartEntity.Add(component);
			BoardItemComponent boardItemComp = smartEntity.BoardItemComp;
			BoardItem boardItem = boardItemComp.BoardItem;
			if (Service.BoardController.Board.AddChild(boardItem, boardCell.X, boardCell.Z, null, false, !forceAllow && troopType.Type != TroopType.Champion) == null)
			{
				return null;
			}
			Service.EntityController.AddEntity(smartEntity);
			Service.TroopAbilityController.OnTroopSpawned(smartEntity);
			if (troopType.Type != TroopType.Champion || teamType == TeamType.Attacker)
			{
				base.EnsureBattlePlayState();
			}
			if (sendPlacedEvent)
			{
				Service.EventManager.SendEvent(EventId.TroopPlacedOnBoard, smartEntity);
			}
			return smartEntity;
		}

		public bool FinalizeSafeBoardPosition(TroopTypeVO troopType, ref Entity spawnBuilding, ref IntPosition boardPosition, ref BoardCell targetCell, TeamType teamType, TroopSpawnMode spawnMode, bool forceAllow)
		{
			targetCell = this.boardController.Board.GetClampedDeployableCellAt(boardPosition.x, boardPosition.z, troopType.SizeX);
			boardPosition = new IntPosition(targetCell.X, targetCell.Z);
			BoardCell boardCell = null;
			if (spawnMode == TroopSpawnMode.LeashedToBuilding)
			{
				if (targetCell.Children == null)
				{
					return false;
				}
				LinkedListNode<BoardItem> linkedListNode = targetCell.Children.First;
				while (linkedListNode != null)
				{
					spawnBuilding = linkedListNode.Value.Data;
					BuildingComponent buildingComponent = spawnBuilding.Get<BuildingComponent>();
					DamageableComponent damageableComponent = spawnBuilding.Get<DamageableComponent>();
					if (buildingComponent != null && (forceAllow || buildingComponent.BuildingType.AllowDefensiveSpawn) && damageableComponent != null)
					{
						if (forceAllow && troopType.Type == TroopType.Champion)
						{
							boardPosition = new IntPosition(targetCell.X, targetCell.Z);
							break;
						}
						targetCell = damageableComponent.FindASafeSpawnSpot(troopType.SizeX, out boardCell);
						if (targetCell == null)
						{
							return false;
						}
						boardPosition = new IntPosition(targetCell.X, targetCell.Z);
						break;
					}
					else
					{
						linkedListNode = linkedListNode.Next;
					}
				}
				if (linkedListNode == null)
				{
					return false;
				}
			}
			else if (!this.ValidateTroopPlacement(boardPosition, teamType, troopType.SizeX, true, out boardCell, forceAllow))
			{
				return false;
			}
			return true;
		}

		public Entity DeployTroopWithOffset(TroopTypeVO troopVO, ref int currentOffsetIndex, IntPosition spawnPosition, bool forceAllow, TeamType teamType)
		{
			TroopController troopController = Service.TroopController;
			IntPosition ip = TroopDeployer.OFFSETS[currentOffsetIndex] * troopVO.AutoSpawnSpreadingScale;
			IntPosition boardPosition = spawnPosition + ip;
			if (!troopController.ValidateAttackerTroopPlacement(boardPosition, troopVO.SizeX, false))
			{
				boardPosition = spawnPosition;
			}
			if (++currentOffsetIndex == TroopDeployer.OFFSETS.Length)
			{
				currentOffsetIndex = 0;
			}
			return troopController.SpawnTroop(troopVO, teamType, boardPosition, TroopSpawnMode.Unleashed, true, forceAllow);
		}

		public SmartEntity SpawnChampion(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition)
		{
			TroopSpawnMode spawnMode = (teamType != TeamType.Defender) ? TroopSpawnMode.Unleashed : TroopSpawnMode.LeashedToBuilding;
			SmartEntity smartEntity = this.SpawnTroop(troopType, teamType, boardPosition, spawnMode, true, teamType == TeamType.Defender);
			if (smartEntity != null)
			{
				smartEntity.Add(new ChampionComponent(troopType));
			}
			return smartEntity;
		}

		public SmartEntity SpawnHero(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition)
		{
			return this.SpawnHero(troopType, teamType, boardPosition, teamType == TeamType.Defender);
		}

		public SmartEntity SpawnHero(TroopTypeVO troopType, TeamType teamType, IntPosition boardPosition, bool leashed)
		{
			return this.SpawnTroop(troopType, teamType, boardPosition, (!leashed) ? TroopSpawnMode.Unleashed : TroopSpawnMode.LeashedToBuilding, true);
		}

		public bool ValidateTroopPlacement(IntPosition boardPosition, TeamType teamType, int troopWidth, bool sendEventsForInvalidPlacement, out BoardCell pathCell, bool forceAllow)
		{
			Board board = this.boardController.Board;
			BoardCell cellAt = board.GetCellAt(boardPosition.x, boardPosition.z);
			pathCell = null;
			if (!forceAllow && (cellAt == null || cellAt.CollidesWith(CollisionFilters.TROOP)))
			{
				if (sendEventsForInvalidPlacement)
				{
					Service.EventManager.SendEvent(EventId.TroopNotPlacedInvalidArea, boardPosition);
				}
				return false;
			}
			int num = boardPosition.x - troopWidth / 2;
			int num2 = boardPosition.z - troopWidth / 2;
			pathCell = board.GetCellAt(num, num2);
			if (!forceAllow && (num > 23 - troopWidth || num2 > 23 - troopWidth || pathCell == null || pathCell.Clearance < troopWidth))
			{
				if (sendEventsForInvalidPlacement)
				{
					Service.EventManager.SendEvent(EventId.TroopNotPlacedInvalidArea, new IntPosition(boardPosition.x - Units.GridToBoardX(troopWidth), boardPosition.z - Units.GridToBoardX(troopWidth)));
				}
				return false;
			}
			if (teamType == TeamType.Attacker)
			{
				uint num3 = cellAt.Flags & 20u;
				if (num3 != 0u && !forceAllow)
				{
					if (sendEventsForInvalidPlacement)
					{
						Service.EventManager.SendEvent(EventId.TroopNotPlacedInvalidArea, boardPosition);
					}
					return false;
				}
			}
			return true;
		}

		public bool FindValidDropShipTroopPlacementCell(IntPosition boardPosition, TeamType teamType, int troopWidth, out IntPosition newBoardPosition)
		{
			BoardCellDynamicArray boardCellDynamicArray = GameUtils.TraverseSpiral(2, boardPosition.x, boardPosition.z);
			newBoardPosition = IntPosition.Zero;
			this.boardController.Board.RefreshClearanceMap();
			for (int i = 0; i < boardCellDynamicArray.Length; i++)
			{
				BoardCell boardCell = boardCellDynamicArray.Array[i];
				if (boardCell != null && boardCell.Clearance >= troopWidth && !boardCell.CollidesWith(CollisionFilters.TROOP))
				{
					newBoardPosition = new IntPosition(boardCell.X, boardCell.Z);
					return true;
				}
			}
			return false;
		}

		public bool ValidateAttackerTroopPlacement(IntPosition boardPosition, int troopWidth, bool sendEventsForInvalidPlacement)
		{
			BoardCell boardCell;
			return this.ValidateTroopPlacement(boardPosition, TeamType.Attacker, troopWidth, sendEventsForInvalidPlacement, out boardCell, false);
		}

		public static bool IsEntityHealer(SmartEntity troop)
		{
			bool result = false;
			if (troop != null && troop.ShooterComp != null && troop.ShooterComp.ShooterVO != null)
			{
				result = (troop.ShooterComp.ShooterVO.TroopRole == TroopRole.Healer);
			}
			return result;
		}

		public static bool IsEntityPhantom(SmartEntity troop)
		{
			bool result = false;
			if (troop != null && troop.TroopComp != null && troop.TroopComp.TroopType != null)
			{
				result = (troop.TroopComp.TroopType.Type == TroopType.Phantom);
			}
			return result;
		}

		public static bool CanEntityCrushWalls(SmartEntity troop)
		{
			bool result = false;
			if (troop != null && troop.ShooterComp != null && troop.ShooterComp.ShooterVO != null)
			{
				result = troop.ShooterComp.ShooterVO.CrushesWalls;
			}
			return result;
		}

		public static bool CanEntityIgnoreWalls(SmartEntity troop)
		{
			bool result = false;
			if (troop != null)
			{
				if (troop.TeamComp != null && troop.TeamComp.TeamType == TeamType.Defender)
				{
					return true;
				}
				if (troop.ShooterComp != null && troop.ShooterComp.ShooterVO != null)
				{
					result = troop.ShooterComp.ShooterVO.IgnoresWalls;
				}
			}
			return result;
		}
	}
}
