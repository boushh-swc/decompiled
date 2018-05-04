using StaRTS.Main.Models;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class HomeMapDataLoader : IMapDataLoader
	{
		private const WorldType worldType = WorldType.Home;

		private MapLoadedDelegate onMapLoaded;

		public HomeMapDataLoader()
		{
			Service.HomeMapDataLoader = this;
		}

		public void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail)
		{
			this.DoOfflineSimulationForGenerators();
			onMapLoaded(Service.CurrentPlayer.Map);
		}

		private void DoOfflineSimulationForGenerators()
		{
			Map map = Service.CurrentPlayer.Map;
			StaticDataController staticDataController = Service.StaticDataController;
			ICurrencyController iCurrencyController = Service.ICurrencyController;
			ISupportController iSupportController = Service.ISupportController;
			foreach (Building current in map.Buildings)
			{
				BuildingTypeVO buildingTypeVO = staticDataController.Get<BuildingTypeVO>(current.Uid);
				if (buildingTypeVO.Type == BuildingType.Resource && iSupportController.FindCurrentContract(current.Key) == null)
				{
					current.AccruedCurrency = iCurrencyController.CalculateAccruedCurrency(current, buildingTypeVO);
				}
			}
		}

		public List<IAssetVO> GetPreloads()
		{
			List<IAssetVO> result = new List<IAssetVO>();
			if (Service.UXController == null)
			{
				return result;
			}
			UXController uXController = Service.UXController;
			if (uXController.MiscElementsManager != null)
			{
				uXController.MiscElementsManager.ReleaseHealthSliderPool();
			}
			return result;
		}

		public List<IAssetVO> GetProjectilePreloads(Map map)
		{
			return null;
		}

		public WorldType GetWorldType()
		{
			return WorldType.Home;
		}

		public string GetWorldName()
		{
			return string.Empty;
		}

		public string GetFactionAssetName()
		{
			FactionType faction = Service.CurrentPlayer.Faction;
			return UXUtils.GetIconNameFromFactionType(faction);
		}

		public PlanetVO GetPlanetData()
		{
			return Service.CurrentPlayer.Map.Planet;
		}
	}
}
