using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using StaRTS.Utils.MeshCombiner;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.CombineMesh
{
	public abstract class AbstractCombineMeshHelper
	{
		private const string FLAG_MESH = "flagMesh";

		public abstract HashSet<BuildingType> GetEligibleBuildingTypes();

		protected abstract List<SmartEntity> GetBuildingEntityListByType(BuildingType buidlingType);

		private bool IsEntityEligible(SmartEntity smartEntity)
		{
			if (smartEntity.BuildingComp != null)
			{
				BuildingType buildingTypeFromBuilding = this.GetBuildingTypeFromBuilding(smartEntity);
				return this.IsBuildingTypeEligible(buildingTypeFromBuilding) && this.IsEntityEligibleForEligibleBuildingType(smartEntity);
			}
			return false;
		}

		protected virtual bool IsEntityEligibleForEligibleBuildingType(SmartEntity smartEntity)
		{
			return true;
		}

		private bool IsBuildingTypeEligible(BuildingType buildingType)
		{
			return this.GetEligibleBuildingTypes().Contains(buildingType);
		}

		protected BuildingType GetBuildingTypeFromBuilding(SmartEntity smartEntity)
		{
			return smartEntity.BuildingComp.BuildingType.Type;
		}

		private void InternalBuildingObjectDestroyed(BuildingType buildingType, Dictionary<BuildingType, MeshCombiner> meshCombiners, bool noFutureAddCallExpected)
		{
			if (noFutureAddCallExpected)
			{
				this.CombineMesh(buildingType, meshCombiners[buildingType]);
			}
			else
			{
				this.UncombineMesh(buildingType, meshCombiners[buildingType]);
			}
		}

		public void BuildingObjectDestroyed(BuildingType buildingType, Dictionary<BuildingType, MeshCombiner> meshCombiners, bool noFutureAddCallExpected)
		{
			if (this.IsBuildingTypeEligible(buildingType))
			{
				this.InternalBuildingObjectDestroyed(buildingType, meshCombiners, noFutureAddCallExpected);
			}
		}

		public void BuildingObjectDestroyed(SmartEntity buildingEntity, Dictionary<BuildingType, MeshCombiner> meshCombiners, bool noFutureAddCallExpected)
		{
			if (this.IsEntityEligible(buildingEntity))
			{
				BuildingType buildingTypeFromBuilding = this.GetBuildingTypeFromBuilding(buildingEntity);
				this.InternalBuildingObjectDestroyed(buildingTypeFromBuilding, meshCombiners, noFutureAddCallExpected);
			}
		}

		public void BuildingObjectAdded(SmartEntity buildingEntity, Dictionary<BuildingType, MeshCombiner> meshCombiners)
		{
			if (this.IsEntityEligible(buildingEntity))
			{
				BuildingType buildingTypeFromBuilding = this.GetBuildingTypeFromBuilding(buildingEntity);
				this.CombineMesh(buildingTypeFromBuilding, meshCombiners[buildingTypeFromBuilding]);
			}
		}

		public virtual void CombineAllMeshTypes(Dictionary<BuildingType, MeshCombiner> meshCombiners)
		{
			Dictionary<BuildingType, HashSet<Renderer>> allRenderersToCombine = this.GetAllRenderersToCombine();
			foreach (KeyValuePair<BuildingType, HashSet<Renderer>> current in allRenderersToCombine)
			{
				BuildingType key = current.Key;
				meshCombiners[key].CombineMeshes(current.Value);
			}
		}

		public virtual void UncombineAllMeshTypes(Dictionary<BuildingType, MeshCombiner> meshCombiners)
		{
			foreach (KeyValuePair<BuildingType, MeshCombiner> current in meshCombiners)
			{
				if (current.Value != null)
				{
					current.Value.UncombineMesh();
				}
			}
		}

		protected virtual void CombineMesh(BuildingType buildingType, MeshCombiner meshCombiner)
		{
			meshCombiner.CombineMeshes(this.GetRenderersToCombine(buildingType));
		}

		protected virtual void UncombineMesh(BuildingType buildingType, MeshCombiner meshCombiner)
		{
			meshCombiner.UncombineMesh();
		}

		private Dictionary<BuildingType, HashSet<Renderer>> GetAllRenderersToCombine()
		{
			EntityController entityController = Service.EntityController;
			Dictionary<BuildingType, HashSet<Renderer>> dictionary = new Dictionary<BuildingType, HashSet<Renderer>>();
			NodeList<BuildingNode> nodeList = entityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				BuildingType type = buildingNode.BuildingComp.BuildingType.Type;
				SmartEntity smartEntity = (SmartEntity)buildingNode.Entity;
				if (this.IsBuildingTypeEligible(type) && this.IsEntityEligibleForEligibleBuildingType(smartEntity))
				{
					HashSet<Renderer> hashSet;
					if (!dictionary.TryGetValue(type, out hashSet))
					{
						hashSet = new HashSet<Renderer>();
						dictionary[type] = hashSet;
					}
					this.AddRenderersFromEntity(hashSet, smartEntity);
				}
			}
			return dictionary;
		}

		private HashSet<Renderer> GetRenderersToCombine(BuildingType buildingType)
		{
			HashSet<Renderer> hashSet = new HashSet<Renderer>();
			List<SmartEntity> buildingEntityListByType = this.GetBuildingEntityListByType(buildingType);
			int i = 0;
			int count = buildingEntityListByType.Count;
			while (i < count)
			{
				SmartEntity smartEntity = buildingEntityListByType[i];
				if (this.IsEntityEligibleForEligibleBuildingType(smartEntity))
				{
					this.AddRenderersFromEntity(hashSet, smartEntity);
				}
				i++;
			}
			return hashSet;
		}

		private void AddRenderFromGameObject(HashSet<Renderer> list, GameObject gameObject)
		{
			Renderer component = gameObject.GetComponent<Renderer>();
			list.Add(component);
		}

		protected void AddRenderersFromEntity(HashSet<Renderer> renderers, SmartEntity entity)
		{
			if (entity.GameObjectViewComp != null)
			{
				GameObject mainGameObject = entity.GameObjectViewComp.MainGameObject;
				AssetMeshDataMonoBehaviour component = mainGameObject.GetComponent<AssetMeshDataMonoBehaviour>();
				if (component != null)
				{
					List<GameObject> selectableGameObjects = component.SelectableGameObjects;
					int i = 0;
					int count = selectableGameObjects.Count;
					while (i < count)
					{
						GameObject gameObject = selectableGameObjects[i];
						if (!gameObject.name.StartsWith("flagMesh"))
						{
							this.AddRenderFromGameObject(renderers, gameObject);
						}
						i++;
					}
					this.AddRenderFromGameObject(renderers, component.ShadowGameObject);
				}
			}
		}
	}
}
