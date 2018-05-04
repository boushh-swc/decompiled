using StaRTS.Main.Controllers.GameStates;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Views.World;
using StaRTS.Utils.Core;
using StaRTS.Utils.MeshCombiner;
using StaRTS.Utils.State;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers.CombineMesh
{
	public class HomeBaseCombineMeshHelper : AbstractCombineMeshHelper
	{
		private readonly HashSet<BuildingType> HOME_SUPPORTED_BUILDING_TYPES = new HashSet<BuildingType>(new BuildingType[]
		{
			BuildingType.Starport,
			BuildingType.Wall,
			BuildingType.Storage,
			BuildingType.ShieldGenerator,
			BuildingType.Clearable
		});

		public override HashSet<BuildingType> GetEligibleBuildingTypes()
		{
			return this.HOME_SUPPORTED_BUILDING_TYPES;
		}

		private bool IsHomeCombiningEligible()
		{
			IState currentState = Service.GameStateMachine.CurrentState;
			return !(currentState is EditBaseState) && !(currentState is BaseLayoutToolState);
		}

		public override void CombineAllMeshTypes(Dictionary<BuildingType, MeshCombiner> meshCombiners)
		{
			if (this.IsHomeCombiningEligible())
			{
				SmartEntity selectedBuilding = null;
				Vector3 zero = Vector3.zero;
				BuildingMover buildingMoverForCombineMeshManager = Service.BuildingController.GetBuildingMoverForCombineMeshManager();
				buildingMoverForCombineMeshManager.DeselectBuildingBeforeCombiningMesh(out selectedBuilding, out zero);
				base.CombineAllMeshTypes(meshCombiners);
				buildingMoverForCombineMeshManager.SelectBuildingAfterCombiningMesh(selectedBuilding, zero);
			}
		}

		protected override void CombineMesh(BuildingType buildingType, MeshCombiner meshCombiner)
		{
			if (this.IsHomeCombiningEligible())
			{
				BuildingController buildingController = Service.BuildingController;
				SmartEntity selectedBuilding = buildingController.SelectedBuilding;
				if (selectedBuilding != null)
				{
					BuildingType buildingTypeFromBuilding = base.GetBuildingTypeFromBuilding(selectedBuilding);
					BuildingMover buildingMoverForCombineMeshManager = Service.BuildingController.GetBuildingMoverForCombineMeshManager();
					Vector3 zero = Vector3.zero;
					if (buildingTypeFromBuilding == buildingType)
					{
						buildingMoverForCombineMeshManager.DeselectBuildingBeforeCombiningMesh(out selectedBuilding, out zero);
					}
					base.CombineMesh(buildingType, meshCombiner);
					if (buildingTypeFromBuilding == buildingType)
					{
						buildingMoverForCombineMeshManager.SelectBuildingAfterCombiningMesh(selectedBuilding, zero);
					}
				}
				else
				{
					base.CombineMesh(buildingType, meshCombiner);
				}
			}
		}

		protected override void UncombineMesh(BuildingType buildingType, MeshCombiner meshCombiner)
		{
			if (this.IsHomeCombiningEligible())
			{
				base.UncombineMesh(buildingType, meshCombiner);
			}
		}

		protected override bool IsEntityEligibleForEligibleBuildingType(SmartEntity smartEntity)
		{
			bool result = true;
			BuildingType buildingTypeFromBuilding = base.GetBuildingTypeFromBuilding(smartEntity);
			if (buildingTypeFromBuilding == BuildingType.Storage)
			{
				result = this.IsStorageBuildingEligibleForHomeCombineMesh(smartEntity);
			}
			return result;
		}

		protected override List<SmartEntity> GetBuildingEntityListByType(BuildingType buildingType)
		{
			return Service.BuildingLookupController.GetBuildingListByType(buildingType);
		}

		private bool IsStorageBuildingEligibleForHomeCombineMesh(SmartEntity entity)
		{
			return entity.BuildingComp.BuildingType.Currency != CurrencyType.Credits;
		}
	}
}
