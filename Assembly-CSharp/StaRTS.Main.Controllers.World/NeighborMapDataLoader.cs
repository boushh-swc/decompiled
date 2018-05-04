using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Player;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class NeighborMapDataLoader : IMapDataLoader
	{
		private const WorldType worldType = WorldType.Neighbor;

		private VisitNeighborResponse response;

		public NeighborMapDataLoader(VisitNeighborResponse response)
		{
			this.response = response;
		}

		public void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail)
		{
			onMapLoaded(this.response.MapData);
		}

		public WorldType GetWorldType()
		{
			return WorldType.Neighbor;
		}

		public string GetWorldName()
		{
			return this.response.Name;
		}

		public List<IAssetVO> GetPreloads()
		{
			return null;
		}

		public List<IAssetVO> GetProjectilePreloads(Map map)
		{
			return null;
		}

		public string GetFactionAssetName()
		{
			FactionType faction = this.response.Faction;
			return UXUtils.GetIconNameFromFactionType(faction);
		}

		public PlanetVO GetPlanetData()
		{
			return this.response.MapData.Planet;
		}
	}
}
