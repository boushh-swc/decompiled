using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.World
{
	public class MapDataLoaderUtils
	{
		public static List<IAssetVO> GetBattlePreloads(BattleInitializationData battleData)
		{
			List<IAssetVO> list = new List<IAssetVO>();
			StaticDataController staticDataController = Service.StaticDataController;
			SkinController skinController = Service.SkinController;
			BattleTypeVO battleTypeVO = (battleData == null) ? null : battleData.BattleVO;
			if (battleTypeVO == null || !battleTypeVO.OverridePlayerUnits)
			{
				Inventory inventory = Service.CurrentPlayer.Inventory;
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(inventory.Troop, list, battleData.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<SpecialAttackTypeVO>(inventory.SpecialAttack, list, battleData.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(inventory.Hero, list, battleData.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(inventory.Champion, list, battleData.AttackerEquipment, staticDataController, skinController);
			}
			if (battleTypeVO != null)
			{
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(battleTypeVO.TroopData, list, battleData.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<SpecialAttackTypeVO>(battleTypeVO.SpecialAttackData, list, battleData.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(battleTypeVO.HeroData, list, battleData.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(battleTypeVO.ChampionData, list, battleData.AttackerEquipment, staticDataController, skinController);
			}
			MapDataLoaderUtils.AddSummonableVisitorsToList(battleData, staticDataController, list, skinController);
			MapDataLoaderUtils.AddFXPreloads(list);
			return list;
		}

		private static void AddSummonableVisitorsToList(BattleInitializationData battleData, StaticDataController idc, List<IAssetVO> assets, SkinController sc)
		{
			List<string> list = new List<string>();
			List<string> list2 = new List<string>();
			List<string> list3 = new List<string>();
			if (battleData.AttackerEquipment != null)
			{
				list3.AddRange(battleData.AttackerEquipment);
			}
			if (battleData.DefenderEquipment != null)
			{
				list3.AddRange(battleData.DefenderEquipment);
			}
			int count = list3.Count;
			for (int i = 0; i < count; i++)
			{
				EquipmentVO equipmentVO = idc.Get<EquipmentVO>(list3[i]);
				int num = equipmentVO.EffectUids.Length;
				for (int j = 0; j < num; j++)
				{
					EquipmentEffectVO equipmentEffectVO = idc.Get<EquipmentEffectVO>(equipmentVO.EffectUids[j]);
					int num2 = equipmentEffectVO.BuffUids.Length;
					for (int k = 0; k < num2; k++)
					{
						BuffTypeVO buffTypeVO = idc.Get<BuffTypeVO>(equipmentEffectVO.BuffUids[k]);
						if (buffTypeVO.Modify == BuffModify.Summon)
						{
							SummonDetailsVO summonDetailsVO = idc.Get<SummonDetailsVO>(buffTypeVO.Details);
							int num3 = summonDetailsVO.VisitorUids.Length;
							for (int l = 0; l < num3; l++)
							{
								string item = summonDetailsVO.VisitorUids[l];
								List<string> list4 = (summonDetailsVO.VisitorType != VisitorType.Troop) ? list2 : list;
								if (!list4.Contains(item))
								{
									list4.Add(item);
								}
							}
						}
					}
				}
			}
			int m = 0;
			int count2 = list.Count;
			while (m < count2)
			{
				MapDataLoaderUtils.AddDeployableToList<TroopTypeVO>(list[m], 1, assets, list3, idc, sc);
				m++;
			}
			int n = 0;
			int count3 = list2.Count;
			while (n < count3)
			{
				MapDataLoaderUtils.AddDeployableToList<SpecialAttackTypeVO>(list2[n], 1, assets, list3, idc, sc);
				n++;
			}
		}

		public static List<IAssetVO> GetBattleRecordPreloads(BattleRecord battleRecord)
		{
			List<IAssetVO> list = new List<IAssetVO>();
			if (battleRecord == null)
			{
				Service.Logger.Error("Battle Record is null in MapDataLoaderUtils.GetBattleRecordPreloads.");
				return list;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			SkinController skinController = Service.SkinController;
			BattleDeploymentData attackerDeploymentData = battleRecord.AttackerDeploymentData;
			if (attackerDeploymentData != null)
			{
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(attackerDeploymentData.TroopData, list, battleRecord.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<SpecialAttackTypeVO>(attackerDeploymentData.SpecialAttackData, list, battleRecord.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(attackerDeploymentData.HeroData, list, battleRecord.AttackerEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(attackerDeploymentData.ChampionData, list, battleRecord.AttackerEquipment, staticDataController, skinController);
			}
			BattleDeploymentData defenderDeploymentData = battleRecord.DefenderDeploymentData;
			if (defenderDeploymentData != null)
			{
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(defenderDeploymentData.TroopData, list, battleRecord.DefenderEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<SpecialAttackTypeVO>(defenderDeploymentData.SpecialAttackData, list, battleRecord.DefenderEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(defenderDeploymentData.HeroData, list, battleRecord.DefenderEquipment, staticDataController, skinController);
				MapDataLoaderUtils.AddDeployablesToList<TroopTypeVO>(defenderDeploymentData.ChampionData, list, battleRecord.DefenderEquipment, staticDataController, skinController);
			}
			MapDataLoaderUtils.AddFXPreloads(list);
			return list;
		}

		private static void AddFXPreloads(List<IAssetVO> assets)
		{
			List<IAssetVO> effectAssetTypes = Service.CurrencyEffects.GetEffectAssetTypes("setupTypeLooting");
			assets.AddRange(effectAssetTypes);
			assets.AddRange(Service.ShieldEffects.GetEffectAssetTypes());
			assets.AddRange(FXUtils.GetEffectAssetTypes());
			Service.UXController.MiscElementsManager.EnsureHealthSliderPool();
		}

		private static void AddDeployablesToList<T>(InventoryStorage storage, List<IAssetVO> assets, List<string> equipment, StaticDataController dc, SkinController skinController) where T : IValueObject
		{
			Dictionary<string, InventoryEntry> internalStorage = storage.GetInternalStorage();
			foreach (KeyValuePair<string, InventoryEntry> current in internalStorage)
			{
				MapDataLoaderUtils.AddDeployableToList<T>(current.Key, current.Value.Amount, assets, equipment, dc, skinController);
			}
		}

		private static void AddDeployablesToList<T>(Dictionary<string, int> deployables, List<IAssetVO> assets, List<string> equipment, StaticDataController dc, SkinController skinController) where T : IValueObject
		{
			if (deployables != null)
			{
				foreach (KeyValuePair<string, int> current in deployables)
				{
					MapDataLoaderUtils.AddDeployableToList<T>(current.Key, current.Value, assets, equipment, dc, skinController);
				}
			}
		}

		private static void AddDeployableToList<T>(string uid, int amount, List<IAssetVO> assets, List<string> equipment, StaticDataController dc, SkinController skinController) where T : IValueObject
		{
			if (amount > 0)
			{
				IAssetVO assetVO = dc.GetOptional<T>(uid) as IAssetVO;
				if (assetVO != null)
				{
					if (assetVO is TroopTypeVO)
					{
						TroopTypeVO troopTypeVO = (TroopTypeVO)assetVO;
						MapDataLoaderUtils.AddSpawnEffect(troopTypeVO, assets, dc);
						MapDataLoaderUtils.AddPlanetAttachments(troopTypeVO, assets, dc);
						SkinTypeVO applicableSkin = skinController.GetApplicableSkin(troopTypeVO, equipment);
						if (applicableSkin != null)
						{
							assetVO = applicableSkin;
						}
					}
					for (int i = 0; i < amount; i++)
					{
						assets.Add(assetVO);
					}
				}
			}
		}

		private static void AddSpawnEffect(TroopTypeVO troopType, List<IAssetVO> assets, StaticDataController dc)
		{
			if (troopType != null)
			{
				string spawnEffectUid = troopType.SpawnEffectUid;
				if (!string.IsNullOrEmpty(spawnEffectUid))
				{
					EffectsTypeVO item = dc.Get<EffectsTypeVO>(spawnEffectUid);
					assets.Add(item);
				}
			}
		}

		private static void AddPlanetAttachments(TroopTypeVO troopType, List<IAssetVO> assets, StaticDataController dc)
		{
			if (troopType == null)
			{
				return;
			}
			if (string.IsNullOrEmpty(troopType.PlanetAttachmentId))
			{
				return;
			}
			string uid = Service.WorldTransitioner.GetMapDataLoader().GetPlanetData().Uid;
			foreach (PlanetAttachmentVO current in dc.GetAll<PlanetAttachmentVO>())
			{
				if (!(current.AttachmentId != troopType.PlanetAttachmentId))
				{
					if (current.Planets != null)
					{
						if (Array.IndexOf<string>(current.Planets, uid) != -1)
						{
							assets.Add(current);
						}
					}
				}
			}
		}
	}
}
