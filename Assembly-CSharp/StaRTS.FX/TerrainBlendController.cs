using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.FX
{
	public class TerrainBlendController : IEventObserver
	{
		public const string TERRAIN_MESH_NAME = "terrainInnerMesh";

		public const string TERRAIN_MESH_OUTER = "terrainOuterMesh";

		public const float TERRAIN_BLEND_OPACITY_MIN = 0.1f;

		public const float TERRAIN_BLEND_OPACITY_MAX = 0.8f;

		private const float RADIUS_OF_INFLUENCE = 3f;

		private const int ACTIVE_AREA_X = 46;

		private const int ACTIVE_AREA_Z = 46;

		private const int WORLD_SCALE = 3;

		public static readonly Color TERRAIN_PLACEMENT_COLOR = Color.red;

		private const int INITIAL_LIST_CAPACITY = 16384;

		private Mesh currentTerrainMesh;

		private Color32[] originalVertexColors;

		private Color32[] targetColors;

		private List<int> vertexIndexes;

		private int[,] vertexTable;

		private int vertexTableStartX;

		private int vertexTableStartZ;

		private int vertexTableCountX;

		private int vertexTableCountZ;

		public TerrainBlendController()
		{
			Service.TerrainBlendController = this;
			Service.EventManager.RegisterObserver(this, EventId.WorldLoadComplete, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.UserLiftedBuilding, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.UserLoweredBuilding, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.UserStashedBuilding, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.BuildingPlacedOnBoard, EventPriority.Default);
			Service.EventManager.RegisterObserver(this, EventId.BuildingRemovedFromBoard, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.BuildingPlacedOnBoard:
				this.PaintTerrainPlacedArea((Entity)cookie);
				return EatResponse.NotEaten;
			case EventId.BuildingMovedOnBoard:
				IL_14:
				switch (id)
				{
				case EventId.UserLoweredBuilding:
					this.PaintTerrainPlacedArea((Entity)cookie);
					return EatResponse.NotEaten;
				case EventId.UserLoweredBuildingAudio:
					IL_2C:
					if (id == EventId.WorldLoadComplete)
					{
						this.RefreshTerrainBlendingNewMap();
						return EatResponse.NotEaten;
					}
					if (id != EventId.UserLiftedBuilding)
					{
						return EatResponse.NotEaten;
					}
					goto IL_54;
				case EventId.UserStashedBuilding:
					goto IL_54;
				}
				goto IL_2C;
				IL_54:
				this.ResetTerrainArea((Entity)cookie);
				return EatResponse.NotEaten;
			case EventId.BuildingRemovedFromBoard:
				return EatResponse.NotEaten;
			}
			goto IL_14;
		}

		public void IndexTerrainMesh(GameObject targetGameObject)
		{
			GameObject gameObject = targetGameObject.transform.Find("terrainInnerMesh").gameObject;
			this.currentTerrainMesh = gameObject.GetComponent<MeshFilter>().sharedMesh;
			this.originalVertexColors = this.currentTerrainMesh.colors32;
			this.targetColors = this.currentTerrainMesh.colors32;
			int num = (int)Mathf.Round(-23f) - 1;
			int num2 = (int)Mathf.Round(23f) + 1;
			int num3 = (int)Mathf.Round(-23f) - 1;
			int num4 = (int)Mathf.Round(23f) + 1;
			if (this.vertexTable == null)
			{
				this.vertexTableStartX = num;
				this.vertexTableStartZ = num3;
				this.vertexTableCountX = num2 - num;
				this.vertexTableCountZ = num4 - num3;
				this.vertexTable = new int[this.vertexTableCountX, this.vertexTableCountZ];
				this.vertexIndexes = new List<int>(16384);
			}
			else
			{
				this.vertexIndexes.Clear();
			}
			Vector3[] vertices = this.currentTerrainMesh.vertices;
			int num5 = 0;
			for (int i = num; i < num2; i++)
			{
				int num6 = 0;
				for (int j = num3; j < num4; j++)
				{
					this.vertexTable[num5, num6] = this.GetVerticesForPosition(i, j, gameObject, vertices);
					num6++;
				}
				num5++;
			}
		}

		private int GetVerticesForPosition(int gridLocationX, int gridLocationZ, GameObject terrainGameObject, Vector3[] targetVertices)
		{
			int count = this.vertexIndexes.Count;
			this.vertexIndexes.Add(0);
			int i = 0;
			int num = targetVertices.Length;
			while (i < num)
			{
				Vector3 vector = terrainGameObject.transform.TransformPoint(targetVertices[i]);
				float num2 = (float)(gridLocationX * 3);
				float num3 = (float)(gridLocationZ * 3);
				if (vector.x >= num2 - 3f && vector.x <= num2 + 3f && vector.z >= num3 - 3f && vector.z <= num3 + 3f)
				{
					this.vertexIndexes.Add(i);
				}
				i++;
			}
			this.vertexIndexes[count] = this.vertexIndexes.Count - count - 1;
			return count;
		}

		public void ResetTerrain()
		{
			if (this.currentTerrainMesh != null)
			{
				this.currentTerrainMesh.colors32 = this.originalVertexColors;
				this.targetColors = this.currentTerrainMesh.colors32;
			}
		}

		private void RefreshTerrainBlendingNewMap()
		{
			EntityController entityController = Service.EntityController;
			NodeList<BuildingNode> nodeList = entityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				if (buildingNode.BuildingComp.BuildingType.Type != BuildingType.Clearable && buildingNode.BuildingComp.BuildingType.Type != BuildingType.Trap)
				{
					this.PaintTerrainPlacedArea(buildingNode.Entity);
				}
			}
		}

		private void ResetTerrainArea(Entity building)
		{
			SizeComponent sizeComponent = building.Get<SizeComponent>();
			TransformComponent transformComponent = building.Get<TransformComponent>();
			this.ResetRangeOfGridPositions(transformComponent.X, transformComponent.Z, sizeComponent.Width, sizeComponent.Depth);
		}

		private void PaintTerrainPlacedArea(Entity building)
		{
			if (building != null)
			{
				SizeComponent sizeComponent = building.Get<SizeComponent>();
				TransformComponent transformComponent = building.Get<TransformComponent>();
				BuildingComponent buildingComponent = building.Get<BuildingComponent>();
				if (buildingComponent.BuildingType.Type != BuildingType.Trap && buildingComponent.BuildingType.Type != BuildingType.Clearable)
				{
					this.PaintRangeOfGridPositions(transformComponent.X, transformComponent.Z, sizeComponent.Width, sizeComponent.Depth, TerrainBlendController.TERRAIN_PLACEMENT_COLOR);
				}
			}
		}

		private int GetVertexIndexOffset(int paintLocationX, int paintLocationZ)
		{
			int result = -1;
			if (paintLocationX >= this.vertexTableStartX && paintLocationX < this.vertexTableCountX && paintLocationZ >= this.vertexTableStartZ && paintLocationZ < this.vertexTableCountZ)
			{
				int num = paintLocationX - this.vertexTableStartX;
				int num2 = paintLocationZ - this.vertexTableStartZ;
				if (num < this.vertexTable.GetLength(0) && num2 < this.vertexTable.GetLength(1))
				{
					result = this.vertexTable[num, num2];
				}
			}
			return result;
		}

		private bool PaintVerticesAtLocation(int paintLocationX, int paintLocationZ, Color32 paintColor)
		{
			int vertexIndexOffset = this.GetVertexIndexOffset(paintLocationX, paintLocationZ);
			if (vertexIndexOffset >= 0)
			{
				int num = this.vertexIndexes[vertexIndexOffset];
				if (num != 0)
				{
					Rand rand = Service.Rand;
					for (int i = 1; i <= num; i++)
					{
						int num2 = this.vertexIndexes[vertexIndexOffset + i];
						Color32 a = this.targetColors[num2];
						this.targetColors[num2] = Color32.Lerp(a, paintColor, rand.ViewRangeFloat(0.1f, 0.8f));
					}
					return true;
				}
			}
			return false;
		}

		private bool ResetVertexColorsAtLocation(int paintLocationX, int paintLocationZ)
		{
			int vertexIndexOffset = this.GetVertexIndexOffset(paintLocationX, paintLocationZ);
			if (vertexIndexOffset >= 0)
			{
				int num = this.vertexIndexes[vertexIndexOffset];
				if (num != 0)
				{
					for (int i = 1; i <= num; i++)
					{
						int num2 = this.vertexIndexes[vertexIndexOffset + i];
						this.targetColors[num2] = this.originalVertexColors[num2];
					}
					return true;
				}
			}
			return false;
		}

		private void PaintRangeOfGridPositions(int positionX, int positionZ, int width, int depth, Color32 paintColor)
		{
			if (this.currentTerrainMesh == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < depth; j++)
				{
					if (this.PaintVerticesAtLocation(i + positionX, j + positionZ, paintColor))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.currentTerrainMesh.colors32 = this.targetColors;
			}
		}

		private void ResetRangeOfGridPositions(int positionX, int positionZ, int width, int depth)
		{
			if (this.currentTerrainMesh == null)
			{
				return;
			}
			bool flag = false;
			for (int i = 0; i < width; i++)
			{
				for (int j = 0; j < depth; j++)
				{
					if (this.ResetVertexColorsAtLocation(i + positionX, j + positionZ))
					{
						flag = true;
					}
				}
			}
			if (flag)
			{
				this.currentTerrainMesh.colors32 = this.targetColors;
			}
		}
	}
}
