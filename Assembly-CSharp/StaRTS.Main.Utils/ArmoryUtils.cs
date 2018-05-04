using Net.RichardLord.Ash.Core;
using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Entities.Nodes;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Static;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Utils
{
	public static class ArmoryUtils
	{
		public static List<string> GetValidEquipment(CurrentPlayer player, StaticDataController dc, string planetId)
		{
			List<string> list = null;
			List<string> equipment = player.ActiveArmory.Equipment;
			if (equipment != null)
			{
				int i = 0;
				int count = equipment.Count;
				while (i < count)
				{
					EquipmentVO equipmentVO = dc.Get<EquipmentVO>(equipment[i]);
					if (ArmoryUtils.IsEquipmentValidForPlanet(equipmentVO, planetId))
					{
						if (list == null)
						{
							list = new List<string>();
						}
						list.Add(equipmentVO.Uid);
					}
					i++;
				}
			}
			return list;
		}

		public static bool IsEquipmentValidForPlanet(EquipmentVO equipment, string planetId)
		{
			string[] planetIDs = equipment.PlanetIDs;
			int i = 0;
			int num = planetIDs.Length;
			while (i < num)
			{
				if (planetId == planetIDs[i])
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public static EquipmentVO GetCurrentEquipmentDataByID(string equipmentId)
		{
			int level = Service.CurrentPlayer.UnlockedLevels.Equipment.GetLevel(equipmentId);
			return ArmoryUtils.GetEquipmentDataByID(equipmentId, level);
		}

		public static EquipmentVO GetEquipmentDataByID(string equipmentID, int level)
		{
			EquipmentUpgradeCatalog equipmentUpgradeCatalog = Service.EquipmentUpgradeCatalog;
			return equipmentUpgradeCatalog.GetByLevel(equipmentID, level);
		}

		public static int GetCurrentActiveEquipmentCapacity(ActiveArmory playerArmory)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			int num = 0;
			int i = 0;
			int count = playerArmory.Equipment.Count;
			while (i < count)
			{
				EquipmentVO equipmentVO = staticDataController.Get<EquipmentVO>(playerArmory.Equipment[i]);
				num += equipmentVO.Size;
				i++;
			}
			return num;
		}

		public static bool HasEnoughCapacityToActivateEquipment(ActiveArmory armory, EquipmentVO equipment)
		{
			return ArmoryUtils.GetCurrentActiveEquipmentCapacity(armory) + equipment.Size <= armory.MaxCapacity;
		}

		public static bool IsEquipmentOnValidPlanet(CurrentPlayer player, EquipmentVO equipment)
		{
			if (equipment.PlanetIDs == null)
			{
				StringBuilder stringBuilder = new StringBuilder("CMS ERROR: ");
				stringBuilder.AppendFormat("{0} has no valid planets", equipment.Uid);
				Service.Logger.Error(stringBuilder.ToString());
				return false;
			}
			int i = 0;
			int num = equipment.PlanetIDs.Length;
			while (i < num)
			{
				if (player.PlanetId == equipment.PlanetIDs[i])
				{
					return true;
				}
				i++;
			}
			return false;
		}

		public static bool PlayerHasArmory()
		{
			NodeList<ArmoryNode> armoryNodeList = Service.BuildingLookupController.ArmoryNodeList;
			return armoryNodeList.CalculateCount() > 0;
		}

		public static bool IsBuildingRequirementMet(EquipmentVO equipment)
		{
			UnlockController unlockController = Service.UnlockController;
			return unlockController.IsUpgradeableUnlocked(equipment);
		}

		public static bool IsEquipmentActive(CurrentPlayer currentPlayer, EquipmentVO equipment)
		{
			return currentPlayer.ActiveArmory.Equipment.Contains(equipment.Uid);
		}

		public static bool CanAffordEquipment(CurrentPlayer currentPlayer, EquipmentVO equipment)
		{
			string equipmentID = equipment.EquipmentID;
			Dictionary<string, int> shards = currentPlayer.Shards;
			int num = (!shards.ContainsKey(equipmentID)) ? 0 : shards[equipmentID];
			return num >= equipment.UpgradeShards;
		}

		public static bool IsAnyEquipmentActive(ActiveArmory armory)
		{
			return ArmoryUtils.GetCurrentActiveEquipmentCapacity(armory) > 0;
		}

		public static bool IsEquipmentOwned(CurrentPlayer currentPlayer, EquipmentVO equipmentVO)
		{
			return currentPlayer.UnlockedLevels.Equipment.Has(equipmentVO);
		}

		public static bool HasReachedMaxEquipmentShards(CurrentPlayer player, EquipmentUpgradeCatalog catalog, string equipmentID)
		{
			int level = player.UnlockedLevels.Equipment.GetLevel(equipmentID);
			EquipmentVO maxLevel = catalog.GetMaxLevel(equipmentID);
			int num = (maxLevel == null) ? 0 : maxLevel.Lvl;
			if (level >= num)
			{
				return true;
			}
			int numEquipmentShardsToReachLevel = ArmoryUtils.GetNumEquipmentShardsToReachLevel(catalog, equipmentID, level, num);
			return player.Shards.ContainsKey(equipmentID) && player.Shards[equipmentID] >= numEquipmentShardsToReachLevel;
		}

		public static int GetNumEquipmentShardsToReachLevel(EquipmentUpgradeCatalog catalog, string equipmentID, int initialUnlockLevel, int maxLevel)
		{
			if (initialUnlockLevel >= maxLevel)
			{
				return 0;
			}
			int num = 0;
			for (int i = initialUnlockLevel + 1; i <= maxLevel; i++)
			{
				EquipmentVO byLevel = catalog.GetByLevel(equipmentID, i);
				num += byLevel.UpgradeShards;
			}
			return num;
		}

		public static bool IsArmoryFull(CurrentPlayer currentPlayer)
		{
			int currentActiveEquipmentCapacity = ArmoryUtils.GetCurrentActiveEquipmentCapacity(currentPlayer.ActiveArmory);
			int maxCapacity = currentPlayer.ActiveArmory.MaxCapacity;
			return currentActiveEquipmentCapacity >= maxCapacity;
		}

		public static bool IsArmoryEmpty(CurrentPlayer currentPlayer)
		{
			int currentActiveEquipmentCapacity = ArmoryUtils.GetCurrentActiveEquipmentCapacity(currentPlayer.ActiveArmory);
			return currentActiveEquipmentCapacity == 0;
		}

		public static int GetShardsRequiredForNextUpgrade(CurrentPlayer currentPlayer, EquipmentUpgradeCatalog equipmentCatalog, EquipmentVO equipmentVO)
		{
			if (!ArmoryUtils.IsEquipmentOwned(currentPlayer, equipmentVO))
			{
				EquipmentVO minLevel = equipmentCatalog.GetMinLevel(equipmentVO.EquipmentID);
				return minLevel.UpgradeShards;
			}
			EquipmentVO nextLevel = equipmentCatalog.GetNextLevel(equipmentVO);
			if (nextLevel == null)
			{
				return -1;
			}
			return nextLevel.UpgradeShards;
		}

		public static bool IsAtMaxLevel(EquipmentUpgradeCatalog equipmentUpgradeCatalog, EquipmentVO equipmentVO)
		{
			EquipmentVO maxLevel = equipmentUpgradeCatalog.GetMaxLevel(equipmentVO.EquipmentID);
			return maxLevel.Lvl == equipmentVO.Lvl;
		}
	}
}
