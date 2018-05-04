using StaRTS.Main.Models;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Views.UX;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class UserWarBaseMapDataLoader : IMapDataLoader
	{
		private const WorldType worldType = WorldType.WarBase;

		private Map baseMap;

		private string squadMemberName;

		private FactionType faction;

		public void Initialize(Map baseMap, string squadMemberName, FactionType faction)
		{
			this.baseMap = baseMap;
			this.squadMemberName = squadMemberName;
			this.faction = faction;
		}

		public void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail)
		{
			onMapLoaded(this.baseMap);
		}

		public List<IAssetVO> GetPreloads()
		{
			return null;
		}

		public List<IAssetVO> GetProjectilePreloads(Map map)
		{
			return null;
		}

		public WorldType GetWorldType()
		{
			return WorldType.WarBase;
		}

		public string GetWorldName()
		{
			return this.squadMemberName;
		}

		public string GetFactionAssetName()
		{
			return UXUtils.GetIconNameFromFactionType(this.faction);
		}

		public PlanetVO GetPlanetData()
		{
			return this.baseMap.Planet;
		}
	}
}
