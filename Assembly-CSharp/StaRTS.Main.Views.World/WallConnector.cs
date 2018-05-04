using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Views.World
{
	public class WallConnector : IEventObserver
	{
		public WallConnector()
		{
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.BuildingPurchaseSuccess);
			eventManager.RegisterObserver(this, EventId.BuildingReplaced);
			eventManager.RegisterObserver(this, EventId.PreEntityKilled);
			eventManager.RegisterObserver(this, EventId.UserLiftedBuilding);
			eventManager.RegisterObserver(this, EventId.UserLoweredBuilding);
			eventManager.RegisterObserver(this, EventId.UserStashedBuilding);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.UserLoweredBuilding:
				goto IL_64;
			case EventId.UserLoweredBuildingAudio:
				IL_18:
				if (id == EventId.BuildingPurchaseSuccess)
				{
					goto IL_64;
				}
				if (id == EventId.PreEntityKilled)
				{
					goto IL_40;
				}
				if (id == EventId.BuildingReplaced)
				{
					goto IL_64;
				}
				if (id != EventId.UserLiftedBuilding)
				{
					return EatResponse.NotEaten;
				}
				this.DisconnectAllNeighbors(cookie as Entity, false);
				return EatResponse.NotEaten;
			case EventId.UserStashedBuilding:
				goto IL_40;
			}
			goto IL_18;
			IL_40:
			this.DisconnectAllNeighbors(cookie as Entity, true);
			return EatResponse.NotEaten;
			IL_64:
			this.ConnectAllNeighbors(cookie as Entity);
			return EatResponse.NotEaten;
		}

		private BoardCell CanConnect(Entity wall)
		{
			if (wall == null)
			{
				return null;
			}
			BuildingComponent buildingComponent = wall.Get<BuildingComponent>();
			if (buildingComponent == null)
			{
				return null;
			}
			BuildingTypeVO buildingType = buildingComponent.BuildingType;
			if (buildingType == null)
			{
				return null;
			}
			if (buildingType.Connectors == null)
			{
				return null;
			}
			BoardItemComponent boardItemComponent = wall.Get<BoardItemComponent>();
			if (boardItemComponent == null)
			{
				return null;
			}
			return boardItemComponent.BoardItem.CurrentCell;
		}

		public string GetConnectorAssetName(Entity wall, bool ignoreNE, bool ignoreNW)
		{
			BoardCell boardCell = this.CanConnect(wall);
			if (boardCell == null)
			{
				return null;
			}
			Entity entity = (!ignoreNE) ? this.GetWallGridNeighbor(boardCell, 1, 0) : null;
			Entity entity2 = (!ignoreNW) ? this.GetWallGridNeighbor(boardCell, 0, 1) : null;
			if (entity != null && entity2 != null)
			{
				return wall.Get<BuildingComponent>().BuildingType.Connectors.AssetNameBoth;
			}
			if (entity != null)
			{
				return wall.Get<BuildingComponent>().BuildingType.Connectors.AssetNameNE;
			}
			if (entity2 != null)
			{
				return wall.Get<BuildingComponent>().BuildingType.Connectors.AssetNameNW;
			}
			return wall.Get<BuildingComponent>().BuildingType.AssetName;
		}

		private void LoadWallAssetExplicit(Entity wall, bool ignoreNE, bool ignoreNW)
		{
			string text;
			if (!ignoreNE && !ignoreNW)
			{
				text = wall.Get<BuildingComponent>().BuildingType.Connectors.AssetNameBoth;
			}
			else if (!ignoreNE)
			{
				text = wall.Get<BuildingComponent>().BuildingType.Connectors.AssetNameNE;
			}
			else if (!ignoreNW)
			{
				text = wall.Get<BuildingComponent>().BuildingType.Connectors.AssetNameNW;
			}
			else
			{
				text = wall.Get<BuildingComponent>().BuildingType.AssetName;
			}
			if (text != null)
			{
				Service.EntityViewManager.LoadEntityAsset(wall, text, true);
			}
		}

		private void LoadWallAsset(Entity wall, bool ignoreNE, bool ignoreNW)
		{
			string connectorAssetName = this.GetConnectorAssetName(wall, ignoreNE, ignoreNW);
			if (connectorAssetName != null)
			{
				Service.EntityViewManager.LoadEntityAsset(wall, connectorAssetName, true);
			}
		}

		private SmartEntity GetNeighborFromTransform(SmartEntity wall, List<SmartEntity> wallList, int x, int z)
		{
			int x2 = wall.TransformComp.X;
			int z2 = wall.TransformComp.Z;
			for (int i = 0; i < wallList.Count; i++)
			{
				int x3 = wallList[i].TransformComp.X;
				int z3 = wallList[i].TransformComp.Z;
				if (x2 + x == x3 && z2 + z == z3)
				{
					return wallList[i];
				}
			}
			return null;
		}

		public void ConnectWallsInExclusiveSet(List<SmartEntity> wallList, bool connectWithSetOnly)
		{
			for (int i = 0; i < wallList.Count; i++)
			{
				SmartEntity wall = wallList[i];
				BoardCell boardCell = this.CanConnect(wall);
				if (boardCell != null)
				{
					if (connectWithSetOnly)
					{
						bool ignoreNE = !wallList.Contains(this.GetNeighborFromTransform(wall, wallList, 1, 0));
						bool ignoreNW = !wallList.Contains(this.GetNeighborFromTransform(wall, wallList, 0, 1));
						this.LoadWallAssetExplicit(wall, ignoreNE, ignoreNW);
					}
					else
					{
						bool ignoreNE2 = !wallList.Contains(this.GetWallGridNeighbor(boardCell, 1, 0));
						bool ignoreNW2 = !wallList.Contains(this.GetWallGridNeighbor(boardCell, 0, 1));
						this.LoadWallAsset(wall, ignoreNE2, ignoreNW2);
						SmartEntity wallGridNeighbor = this.GetWallGridNeighbor(boardCell, -1, 0);
						SmartEntity wallGridNeighbor2 = this.GetWallGridNeighbor(boardCell, 0, -1);
						bool flag = wallGridNeighbor == null || wallList.Contains(wallGridNeighbor);
						bool flag2 = wallGridNeighbor2 == null || wallList.Contains(wallGridNeighbor2);
						if (!flag)
						{
							this.LoadWallAsset(wallGridNeighbor, true, false);
						}
						if (!flag2)
						{
							this.LoadWallAsset(wallGridNeighbor2, false, true);
						}
					}
				}
			}
		}

		private void ConnectAllNeighbors(Entity wall)
		{
			BoardCell boardCell = this.CanConnect(wall);
			if (boardCell == null)
			{
				return;
			}
			this.LoadWallAsset(wall, false, false);
			Entity wallGridNeighbor = this.GetWallGridNeighbor(boardCell, -1, 0);
			Entity wallGridNeighbor2 = this.GetWallGridNeighbor(boardCell, 0, -1);
			this.LoadWallAsset(wallGridNeighbor, false, false);
			this.LoadWallAsset(wallGridNeighbor2, false, false);
		}

		private void DisconnectAllNeighbors(Entity wall, bool targetDestroyed)
		{
			BoardCell boardCell = this.CanConnect(wall);
			if (boardCell == null)
			{
				return;
			}
			if (!targetDestroyed)
			{
				this.LoadWallAsset(wall, true, true);
			}
			Entity wallGridNeighbor = this.GetWallGridNeighbor(boardCell, -1, 0);
			Entity wallGridNeighbor2 = this.GetWallGridNeighbor(boardCell, 0, -1);
			this.LoadWallAsset(wallGridNeighbor, true, false);
			this.LoadWallAsset(wallGridNeighbor2, false, true);
		}

		private SmartEntity GetWallGridNeighbor(BoardCell cell, int gridDeltaX, int gridDeltaZ)
		{
			int num = Units.GridToBoardX(gridDeltaX);
			int num2 = Units.GridToBoardZ(gridDeltaZ);
			BoardCell cellAt = cell.ParentBoard.GetCellAt(cell.X + num, cell.Z + num2);
			if (cellAt != null && cellAt.Children != null)
			{
				foreach (BoardItem current in cellAt.Children)
				{
					SmartEntity smartEntity = (SmartEntity)current.Data;
					BuildingComponent buildingComp = smartEntity.BuildingComp;
					if (buildingComp != null && buildingComp.BuildingType.Connectors != null)
					{
						return smartEntity;
					}
				}
			}
			return null;
		}

		public List<SmartEntity> GetWallChains(SmartEntity rootWall, int xDir, int zDir)
		{
			List<SmartEntity> list = new List<SmartEntity>();
			BoardCell boardCell = this.CanConnect(rootWall);
			BoardCell cell = boardCell;
			SmartEntity smartEntity = rootWall;
			while (cell != null)
			{
				if (smartEntity != rootWall)
				{
					list.Add(smartEntity);
				}
				smartEntity = this.GetWallGridNeighbor(cell, xDir, zDir);
				cell = this.CanConnect(smartEntity);
			}
			return list;
		}
	}
}
