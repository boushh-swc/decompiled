using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.CombineMesh
{
	public class BattleBaseCombineMeshHelper : AbstractCombineMeshHelper
	{
		private readonly HashSet<BuildingType> BATTLE_SUPPORTED_BUILDING_TYPES = new HashSet<BuildingType>(new BuildingType[]
		{
			BuildingType.Barracks,
			BuildingType.Starport,
			BuildingType.Resource,
			BuildingType.Storage,
			BuildingType.ShieldGenerator,
			BuildingType.Clearable,
			BuildingType.Factory,
			BuildingType.Blocker
		});

		public override HashSet<BuildingType> GetEligibleBuildingTypes()
		{
			return this.BATTLE_SUPPORTED_BUILDING_TYPES;
		}

		protected override List<Entity> GetBuildingEntityListByType(BuildingType buildingType)
		{
			EntityController entityController = Service.EntityController;
			List<Entity> list = new List<Entity>();
			NodeList<BuildingNode> nodeList = entityController.GetNodeList<BuildingNode>();
			for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
			{
				BuildingType type = buildingNode.BuildingComp.BuildingType.Type;
				if (buildingType == type)
				{
					list.Add(buildingNode.Entity);
				}
			}
			return list;
		}

		protected override bool IsEntityEligibleForEligibleBuildingType(SmartEntity smartEntity)
		{
			bool result = true;
			BuildingType buildingTypeFromBuilding = base.GetBuildingTypeFromBuilding(smartEntity);
			if (buildingTypeFromBuilding == BuildingType.ShieldGenerator)
			{
				result = this.IsShieldGeneratorBuildingEligibleForBattleCombineMesh(smartEntity);
			}
			return result;
		}

		private bool IsShieldGeneratorBuildingEligibleForBattleCombineMesh(SmartEntity entity)
		{
			return entity.BuildingComp.BuildingType.SubType != BuildingSubType.OutpostDefenseGenerator;
		}
	}
}
