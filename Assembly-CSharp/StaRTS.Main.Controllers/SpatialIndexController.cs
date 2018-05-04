using Net.RichardLord.Ash.Core;
using StaRTS.DataStructures;
using StaRTS.GameBoard;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class SpatialIndexController
	{
		private SpatialIndex[,] spatialIndices;

		private int width;

		private int depth;

		private int maxSquaredDistance;

		public SpatialIndexController()
		{
			Service.SpatialIndexController = this;
			this.Initialize();
		}

		private void Initialize()
		{
			Board board = Service.BoardController.Board;
			this.width = board.BoardSize;
			this.depth = board.BoardSize;
			this.maxSquaredDistance = board.GetMaxSquaredDistance();
			this.spatialIndices = new SpatialIndex[this.width, this.depth];
			this.Reset();
		}

		public void Reset()
		{
			for (int i = 0; i < this.width; i++)
			{
				for (int j = 0; j < this.depth; j++)
				{
					this.spatialIndices[i, j] = null;
				}
			}
		}

		public void ResetTurretScannedFlagForBoard()
		{
			for (int i = 0; i < this.width; i++)
			{
				for (int j = 0; j < this.depth; j++)
				{
					if (this.spatialIndices[i, j] != null)
					{
						this.spatialIndices[i, j].ResetTurretScanedFlag();
					}
				}
			}
		}

		private void SetBuildingsToAttack(int x, int z, SpatialIndex spatialIndex)
		{
			Board board = Service.BoardController.Board;
			BoardCell cellAt = board.GetCellAt(x, z, true);
			NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
			spatialIndex.AlreadyScannedBuildingsToAttack = true;
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)buildingNode.Entity;
				if (smartEntity.DamageableComp != null)
				{
					if (this.IsAliveHealthNode(smartEntity))
					{
						int squaredDistance = this.CalcSquredDistanceFromTransformToCell(smartEntity.TransformComp, cellAt);
						int nearness = this.CalcNearness(squaredDistance);
						spatialIndex.AddBuildingsToAttack(smartEntity, nearness);
					}
				}
			}
		}

		private void SetTurretsInRangeOf(int x, int z, SpatialIndex spatialIndex)
		{
			Board board = Service.BoardController.Board;
			BoardCell cellAt = board.GetCellAt(x, z, true);
			NodeList<TurretNode> nodeList = Service.EntityController.GetNodeList<TurretNode>();
			spatialIndex.AlreadyScannedTurretsInRange = true;
			for (TurretNode turretNode = nodeList.Head; turretNode != null; turretNode = turretNode.Next)
			{
				SmartEntity smartEntity = (SmartEntity)turretNode.Entity;
				if (this.IsAliveHealthNode(smartEntity))
				{
					TransformComponent transformComp = smartEntity.TransformComp;
					int num = this.CalcSquredDistanceFromTransformToCell(transformComp, cellAt);
					int nearness = this.CalcNearness(num);
					spatialIndex.AddTurretsInRangeOf(smartEntity, num, nearness);
				}
			}
		}

		private void SetAreaTriggerBuildingsInRangeOf(int x, int z, SpatialIndex spatialIndex)
		{
			Board board = Service.BoardController.Board;
			BoardCell cellAt = board.GetCellAt(x, z, true);
			spatialIndex.AlreadyScannedAreaTriggerBuildingsInRange = true;
			NodeList<AreaTriggerBuildingNode> nodeList = Service.EntityController.GetNodeList<AreaTriggerBuildingNode>();
			for (AreaTriggerBuildingNode areaTriggerBuildingNode = nodeList.Head; areaTriggerBuildingNode != null; areaTriggerBuildingNode = areaTriggerBuildingNode.Next)
			{
				Entity entity = areaTriggerBuildingNode.Entity;
				int num = this.CalcSquredDistanceFromTransformToCell(areaTriggerBuildingNode.TransformComp, cellAt);
				int nearness = this.CalcNearness(num);
				spatialIndex.AddAreaTriggerBuildingsInRangeOf(entity, num, nearness);
			}
		}

		private SpatialIndex EnsureSpatialIndex(int i, int j)
		{
			SpatialIndex spatialIndex = this.spatialIndices[i, j];
			if (spatialIndex == null)
			{
				spatialIndex = new SpatialIndex();
				this.spatialIndices[i, j] = spatialIndex;
			}
			return spatialIndex;
		}

		public SmartEntityPriorityList GetBuildingsToAttack(int x, int z)
		{
			Board board = Service.BoardController.Board;
			board.MakeCoordinatesAbsolute(ref x, ref z);
			if (this.IsPositionInvalid(x, z))
			{
				return null;
			}
			SpatialIndex spatialIndex = this.EnsureSpatialIndex(x, z);
			if (!spatialIndex.AlreadyScannedBuildingsToAttack)
			{
				this.SetBuildingsToAttack(x, z, spatialIndex);
			}
			return spatialIndex.GetBuildingsToAttack();
		}

		public List<EntityElementPriorityPair> GetTurretsInRangeOf(int x, int z)
		{
			Board board = Service.BoardController.Board;
			board.MakeCoordinatesAbsolute(ref x, ref z);
			if (this.IsPositionInvalid(x, z))
			{
				return null;
			}
			SpatialIndex spatialIndex = this.EnsureSpatialIndex(x, z);
			if (!spatialIndex.AlreadyScannedTurretsInRange)
			{
				this.SetTurretsInRangeOf(x, z, spatialIndex);
			}
			return spatialIndex.GetTurretsInRangeOf();
		}

		public List<EntityElementPriorityPair> GetAreaTriggerBuildingsInRangeOf(int x, int z)
		{
			Board board = Service.BoardController.Board;
			board.MakeCoordinatesAbsolute(ref x, ref z);
			if (this.IsPositionInvalid(x, z))
			{
				return null;
			}
			SpatialIndex spatialIndex = this.EnsureSpatialIndex(x, z);
			if (!spatialIndex.AlreadyScannedAreaTriggerBuildingsInRange)
			{
				this.SetAreaTriggerBuildingsInRangeOf(x, z, spatialIndex);
			}
			return spatialIndex.GetArareaTriggerBuildingsInRange();
		}

		public bool IsPositionInvalid(int x, int z)
		{
			return x < 0 || x >= this.width || z < 0 || z >= this.depth;
		}

		private bool IsAliveHealthNode(SmartEntity entity)
		{
			return entity.HealthComp != null && !entity.HealthComp.IsDead();
		}

		private int CalcSquredDistanceFromTransformToCell(TransformComponent transform, BoardCell cell)
		{
			return GameUtils.SquaredDistance(transform.CenterGridX(), transform.CenterGridZ(), cell.X, cell.Z);
		}

		public int CalcNearness(int squaredDistance)
		{
			return (this.maxSquaredDistance - squaredDistance) * 10000 / this.maxSquaredDistance;
		}
	}
}
