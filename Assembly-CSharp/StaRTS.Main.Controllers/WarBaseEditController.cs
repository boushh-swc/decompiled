using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player.Building.Move;
using StaRTS.Main.Models.Commands.TransferObjects;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class WarBaseEditController
	{
		public Map mapData;

		public WarBaseEditController()
		{
			Service.WarBaseEditController = this;
		}

		public void EnterWarBaseEditing(Map warBaseMap)
		{
			this.mapData = warBaseMap;
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			baseLayoutToolController.EnterBaseLayoutTool();
		}

		public void ExitWarBaseEditing()
		{
			this.mapData = null;
			BaseLayoutToolController baseLayoutToolController = Service.BaseLayoutToolController;
			baseLayoutToolController.ExitBaseLayoutTool();
		}

		public void CheckForNewBuildings()
		{
			if (this.mapData == null)
			{
				Service.Logger.Warn("No war base data found, not adding new buildings");
				return;
			}
			List<Building> buildings = Service.CurrentPlayer.Map.Buildings;
			Dictionary<string, string> dictionary = new Dictionary<string, string>();
			if (this.mapData.Buildings != null)
			{
				int i = 0;
				int count = this.mapData.Buildings.Count;
				while (i < count)
				{
					dictionary[this.mapData.Buildings[i].Key] = this.mapData.Buildings[i].Uid;
					i++;
				}
			}
			bool flag = false;
			int j = 0;
			int count2 = buildings.Count;
			while (j < count2)
			{
				string key = buildings[j].Key;
				if (!dictionary.ContainsKey(key))
				{
					BuildingTypeVO buildingTypeVO = Service.StaticDataController.Get<BuildingTypeVO>(buildings[j].Uid);
					if (buildingTypeVO.Type != BuildingType.Clearable)
					{
						if (!ContractUtils.IsBuildingConstructing(key))
						{
							Building building = buildings[j].Clone();
							this.mapData.Buildings.Add(building);
							Entity entity = Service.EntityFactory.CreateBuildingEntity(building, false, true, false);
							Service.WorldController.AddEntityToWorld(entity);
							Service.BaseLayoutToolController.StashBuilding(entity, false);
							flag = true;
						}
					}
				}
				j++;
			}
			if (flag)
			{
				Service.UXController.HUD.BaseLayoutToolView.RefreshWholeStashTray();
			}
		}

		public void SaveWarBaseMap(PositionMap diffMap)
		{
			WarBaseSaveCommand command = new WarBaseSaveCommand(new WarBaseSaveRequest
			{
				PlayerId = Service.CurrentPlayer.PlayerId,
				PositionMap = diffMap
			});
			Service.ServerAPI.Enqueue(command);
		}
	}
}
