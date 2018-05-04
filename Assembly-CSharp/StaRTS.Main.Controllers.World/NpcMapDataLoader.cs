using StaRTS.Assets;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Cee.Serializables;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Main.Views.UX;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class NpcMapDataLoader : IMapDataLoader
	{
		private const WorldType worldType = WorldType.NPC;

		private BattleInitializationData battleData;

		private BattleTypeVO battle;

		private MapLoadedDelegate onMapLoaded;

		private MapLoadFailDelegate onMapLoadFail;

		private AssetHandle assetHandle;

		private bool isPveBuffBase;

		public NpcMapDataLoader()
		{
			Service.NpcMapDataLoader = this;
		}

		public NpcMapDataLoader Initialize(BattleTypeVO battle)
		{
			this.battle = battle;
			return this;
		}

		public NpcMapDataLoader Initialize(BattleInitializationData battleData, bool isPveBuffBase)
		{
			this.battleData = battleData;
			this.battle = battleData.BattleVO;
			this.isPveBuffBase = isPveBuffBase;
			return this;
		}

		public void LoadMapData(MapLoadedDelegate onMapLoaded, MapLoadFailDelegate onMapLoadFail)
		{
			this.onMapLoaded = onMapLoaded;
			this.onMapLoadFail = onMapLoadFail;
			Service.AssetManager.Load(ref this.assetHandle, this.battle.AssetName, new AssetSuccessDelegate(this.OnBattleFileLoaded), new AssetFailureDelegate(this.OnBattleFileLoadFailed), null);
		}

		private void UnloadAsset()
		{
			if (this.assetHandle != AssetHandle.Invalid)
			{
				Service.AssetManager.Unload(this.assetHandle);
				this.assetHandle = AssetHandle.Invalid;
			}
		}

		public List<IAssetVO> GetPreloads()
		{
			return MapDataLoaderUtils.GetBattlePreloads(this.battleData);
		}

		public List<IAssetVO> GetProjectilePreloads(Map map)
		{
			List<string> attackerWarBuffs = null;
			List<string> defenderWarBuffs = null;
			if (this.isPveBuffBase)
			{
				Squad currentSquad = Service.SquadController.StateManager.GetCurrentSquad();
				if (currentSquad != null)
				{
					SquadWarManager warManager = Service.SquadController.WarManager;
					attackerWarBuffs = warManager.GetBuffBasesOwnedBySquad(currentSquad.SquadID);
				}
			}
			List<string> attackerEquipment = null;
			List<string> defenderEquipment = null;
			if (this.battleData != null)
			{
				attackerEquipment = this.battleData.AttackerEquipment;
				defenderEquipment = this.battleData.DefenderEquipment;
			}
			return ProjectileUtils.GetBattleProjectileAssets(map, this.battle, null, attackerWarBuffs, defenderWarBuffs, null, null, attackerEquipment, defenderEquipment);
		}

		public WorldType GetWorldType()
		{
			return WorldType.NPC;
		}

		public string GetWorldName()
		{
			if (this.isPveBuffBase)
			{
				return LangUtils.GetBattleOnPlanetName(this.battle);
			}
			return LangUtils.GetBattleName(this.battle);
		}

		private void OnBattleFileLoaded(object asset, object cookie)
		{
			object obj = new JsonParser(asset as string).Parse();
			CombatEncounter combatEncounter = new CombatEncounter().FromObject(obj) as CombatEncounter;
			this.UnloadAsset();
			if (!string.IsNullOrEmpty(this.battle.Planet))
			{
				combatEncounter.map.Planet = Service.StaticDataController.Get<PlanetVO>(this.battle.Planet);
			}
			if (this.onMapLoaded != null)
			{
				this.onMapLoaded(combatEncounter.map);
			}
		}

		private void OnBattleFileLoadFailed(object cookie)
		{
			this.UnloadAsset();
			Service.Logger.ErrorFormat("Failed to load map data {0} for battle {1}", new object[]
			{
				this.battle.AssetName,
				this.battle.Uid
			});
			this.onMapLoadFail();
		}

		public string GetFactionAssetName()
		{
			if (this.isPveBuffBase)
			{
				return UXUtils.GetIconNameFromFactionType(this.battleData.Defender.PlayerFaction);
			}
			return this.battle.DefenderId;
		}

		public PlanetVO GetPlanetData()
		{
			return Service.StaticDataController.Get<PlanetVO>(this.battle.Planet);
		}
	}
}
