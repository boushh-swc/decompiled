using Net.RichardLord.Ash.Core;
using StaRTS.GameBoard.Components;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Utils;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.World;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Controllers
{
	public class EditBaseController : IEventObserver
	{
		private const string LOWERED_FOOTPRINT_NAME = "Lowered Footprint";

		private string LIFTED_FOOTPRINT_NAME = "Lifted Footprint";

		private Footprint loweredFootprint;

		private Dictionary<Entity, Footprint> liftedFootprints;

		public EditBaseController()
		{
			Service.EditBaseController = this;
			this.liftedFootprints = new Dictionary<Entity, Footprint>();
		}

		public void Enable(bool enable)
		{
			EventManager eventManager = Service.EventManager;
			if (enable)
			{
				eventManager.RegisterObserver(this, EventId.UserLiftedBuilding, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.UserLoweredBuilding, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.UserMovedLiftedBuilding, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.BuildingRemovedFromBoard, EventPriority.Default);
				eventManager.RegisterObserver(this, EventId.BuildingSelectedFromStore, EventPriority.Default);
				this.AddAllLoweredFootprints();
			}
			else
			{
				this.RemoveAllLoweredFootprints();
				this.RemoveAllLiftedFootprints();
				eventManager.UnregisterObserver(this, EventId.UserLiftedBuilding);
				eventManager.UnregisterObserver(this, EventId.UserLoweredBuilding);
				eventManager.UnregisterObserver(this, EventId.UserMovedLiftedBuilding);
				eventManager.UnregisterObserver(this, EventId.BuildingRemovedFromBoard);
				eventManager.UnregisterObserver(this, EventId.BuildingSelectedFromStore);
			}
		}

		private void AddAllLoweredFootprints()
		{
			if (this.loweredFootprint == null)
			{
				this.loweredFootprint = new Footprint("Lowered Footprint", 3f);
				BuildingController buildingController = Service.BuildingController;
				NodeList<BuildingNode> nodeList = Service.EntityController.GetNodeList<BuildingNode>();
				for (BuildingNode buildingNode = nodeList.Head; buildingNode != null; buildingNode = buildingNode.Next)
				{
					SmartEntity smartEntity = (SmartEntity)buildingNode.Entity;
					if (!buildingController.IsLifted(smartEntity) && !Service.BaseLayoutToolController.IsBuildingStashed(smartEntity))
					{
						this.AddLoweredFootprint(smartEntity);
					}
				}
				if (buildingController.IsPurchasing && !buildingController.IsLifted(buildingController.PurchasingBuilding))
				{
					this.AddLoweredFootprint((SmartEntity)buildingController.PurchasingBuilding);
				}
				bool allowErrorThrown = true;
				if (Service.BaseLayoutToolController.IsBaseLayoutModeActive)
				{
					allowErrorThrown = false;
				}
				this.loweredFootprint.GenerateMesh(true, false, allowErrorThrown);
			}
		}

		private void AddLoweredFootprint(SmartEntity building)
		{
			TransformComponent transformComp = building.TransformComp;
			Vector3 vector = new Vector3(Units.BoardToWorldX(transformComp.X), 0f, Units.BoardToWorldZ(transformComp.Z));
			SizeComponent sizeComp = building.SizeComp;
			this.loweredFootprint.AddTiles(vector.x, vector.z, sizeComp.Width, sizeComp.Depth, building.WallComp != null);
		}

		private void RemoveAllLoweredFootprints()
		{
			if (this.loweredFootprint != null)
			{
				this.loweredFootprint.DestroyFootprint();
				this.loweredFootprint = null;
			}
		}

		private void ResetLoweredFootprints()
		{
			this.RemoveAllLoweredFootprints();
			this.AddAllLoweredFootprints();
		}

		private void AddLiftedFootprint(SmartEntity building)
		{
			if (!this.liftedFootprints.ContainsKey(building))
			{
				SizeComponent sizeComp = building.SizeComp;
				Footprint footprint = new Footprint(this.LIFTED_FOOTPRINT_NAME, 3f);
				footprint.AddTiles(0f, 0f, sizeComp.Width, sizeComp.Depth, building.WallComp != null);
				footprint.GenerateMesh(true, true);
				this.liftedFootprints.Add(building, footprint);
			}
		}

		private void RemoveLiftedFootprint(SmartEntity building)
		{
			if (building != null && this.liftedFootprints.ContainsKey(building))
			{
				this.liftedFootprints[building].DestroyFootprint();
				this.liftedFootprints.Remove(building);
			}
		}

		private void RemoveAllLiftedFootprints()
		{
			foreach (Footprint current in this.liftedFootprints.Values)
			{
				current.DestroyFootprint();
			}
			this.liftedFootprints.Clear();
		}

		private void MoveLiftedFootprint(FootprintMoveData moveData)
		{
			Entity building = moveData.Building;
			if (this.liftedFootprints.ContainsKey(building))
			{
				Footprint footprint = this.liftedFootprints[building];
				if (footprint.MoveTiles(moveData.WorldAnchorX, moveData.WorldAnchorZ, moveData.CanOccupy, true) && Service.BuildingController.SelectedBuilding == building)
				{
					Service.EventManager.SendEvent(EventId.UserGridMovedBuildingAudio, null);
				}
			}
		}

		public static void BuildingBoardToWorld(Entity building, int boardX, int boardZ, out float worldX, out float worldZ)
		{
			SizeComponent sizeComp = ((SmartEntity)building).SizeComp;
			BuildingComponent buildingComp = ((SmartEntity)building).BuildingComp;
			int num = 0;
			if (buildingComp.BuildingType.Type != BuildingType.Blocker && sizeComp.Width > 1 && sizeComp.Depth > 1)
			{
				num = 1;
			}
			worldX = Units.BoardToWorldX((float)boardX + (float)(sizeComp.Width - num) * 0.5f);
			worldZ = Units.BoardToWorldZ((float)boardZ + (float)(sizeComp.Depth - num) * 0.5f);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.UserLiftedBuilding:
				this.AddLiftedFootprint((SmartEntity)cookie);
				this.ResetLoweredFootprints();
				return EatResponse.NotEaten;
			case EventId.UserLiftedBuildingAudio:
			case EventId.UserGridMovedBuildingAudio:
				IL_22:
				if (id != EventId.BuildingRemovedFromBoard && id != EventId.BuildingSelectedFromStore)
				{
					return EatResponse.NotEaten;
				}
				this.ResetLoweredFootprints();
				return EatResponse.NotEaten;
			case EventId.UserMovedLiftedBuilding:
				this.MoveLiftedFootprint((FootprintMoveData)cookie);
				return EatResponse.NotEaten;
			case EventId.UserLoweredBuilding:
				this.RemoveLiftedFootprint((SmartEntity)cookie);
				this.ResetLoweredFootprints();
				return EatResponse.NotEaten;
			}
			goto IL_22;
		}
	}
}
