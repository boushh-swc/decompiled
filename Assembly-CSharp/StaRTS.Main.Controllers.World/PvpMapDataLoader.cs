using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Commands.Pvp;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class PvpMapDataLoader : IMapDataLoader
	{
		private const WorldType worldType = WorldType.Opponent;

		private BattleInitializationData battleData;

		private PvpTarget pvpTarget;

		public PvpMapDataLoader()
		{
			Service.PvpMapDataLoader = this;
		}

		public PvpMapDataLoader Initialize(BattleInitializationData battleData)
		{
			this.battleData = battleData;
			this.pvpTarget = battleData.PvpTarget;
			return this;
		}

		public void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail)
		{
			onMapLoaded(this.pvpTarget.BaseMap);
		}

		public List<IAssetVO> GetPreloads()
		{
			return MapDataLoaderUtils.GetBattlePreloads(this.battleData);
		}

		public List<IAssetVO> GetProjectilePreloads(Map map)
		{
			return ProjectileUtils.GetBattleProjectileAssets(map, null, null, null, null, this.pvpTarget.GuildDonatedTroops, this.pvpTarget.Champions, this.battleData.AttackerEquipment, this.pvpTarget.Equipment);
		}

		public WorldType GetWorldType()
		{
			return WorldType.Opponent;
		}

		public string GetWorldName()
		{
			if (!string.IsNullOrEmpty(this.pvpTarget.PlayerName))
			{
				return this.pvpTarget.PlayerName;
			}
			string playerId = this.pvpTarget.PlayerId;
			int num = playerId.Length;
			if (num > 10)
			{
				num = 10;
			}
			return playerId.Substring(0, num);
		}

		public string GetFactionAssetName()
		{
			return UXUtils.GetIconNameFromFactionType(this.pvpTarget.PlayerFaction);
		}

		public PlanetVO GetPlanetData()
		{
			return this.pvpTarget.BaseMap.Planet;
		}
	}
}
