using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public interface IMapDataLoader
	{
		void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail);

		WorldType GetWorldType();

		string GetWorldName();

		string GetFactionAssetName();

		List<IAssetVO> GetPreloads();

		List<IAssetVO> GetProjectilePreloads(Map map);

		PlanetVO GetPlanetData();
	}
}
