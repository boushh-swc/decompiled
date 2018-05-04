using StaRTS.Main.Controllers;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class UnlockedLevelData : ISerializable
	{
		public LevelMap Troops;

		public LevelMap Starships;

		public LevelMap Buildings;

		public LevelMap Equipment;

		public UnlockedLevelData()
		{
			this.Troops = new LevelMap();
			this.Starships = new LevelMap();
			this.Buildings = new LevelMap();
			this.Equipment = new LevelMap();
		}

		public void UpgradeTroopsOrStarships(Contract contract)
		{
			bool isStarship = contract.DeliveryType == DeliveryType.UpgradeStarship;
			this.UpgradeTroopsOrStarships(contract.ProductUid, isStarship);
		}

		public void UpgradeTroopsOrStarships(string uid, bool isStarship)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			IUpgradeableVO upgradeableVO;
			InventoryStorage storage;
			if (isStarship)
			{
				upgradeableVO = staticDataController.Get<SpecialAttackTypeVO>(uid);
				this.Starships.SetLevel(upgradeableVO);
				storage = GameUtils.GetWorldOwner().Inventory.SpecialAttack;
			}
			else
			{
				upgradeableVO = staticDataController.Get<TroopTypeVO>(uid);
				this.Troops.SetLevel(upgradeableVO);
				TroopType type = ((TroopTypeVO)upgradeableVO).Type;
				if (type != TroopType.Hero)
				{
					if (type != TroopType.Champion)
					{
						storage = GameUtils.GetWorldOwner().Inventory.Troop;
					}
					else
					{
						storage = GameUtils.GetWorldOwner().Inventory.Champion;
					}
				}
				else
				{
					storage = GameUtils.GetWorldOwner().Inventory.Hero;
				}
			}
			UnlockedLevelData.UpgradeTroopsOrStarshipsInventory(storage, isStarship, upgradeableVO.UpgradeGroup, upgradeableVO.Uid);
		}

		public static void UpgradeTroopsOrStarshipsInventory(InventoryStorage storage, bool isStarship, string productUpgradeGroup, string productUid)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			Dictionary<string, InventoryEntry> internalStorage = storage.GetInternalStorage();
			int num = 0;
			foreach (string current in internalStorage.Keys)
			{
				IUpgradeableVO upgradeableVO = (!isStarship) ? staticDataController.Get<TroopTypeVO>(current) : staticDataController.Get<SpecialAttackTypeVO>(current);
				if (upgradeableVO.UpgradeGroup == productUpgradeGroup)
				{
					num += internalStorage[current].Amount;
					storage.ClearItemAmount(current);
				}
			}
			storage.ModifyItemAmount(productUid, num);
		}

		public void UpgradeEquipmentLevel(Contract contract)
		{
			StaticDataController staticDataController = Service.StaticDataController;
			IUpgradeableVO level = staticDataController.Get<EquipmentVO>(contract.ProductUid);
			this.Equipment.SetLevel(level);
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			return serializer.End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("troop"))
			{
				this.Troops.FromObject(dictionary["troop"]);
			}
			if (dictionary.ContainsKey("specialAttack"))
			{
				this.Starships.FromObject(dictionary["specialAttack"]);
			}
			if (dictionary.ContainsKey("building"))
			{
				this.Buildings.FromObject(dictionary["building"]);
			}
			if (dictionary.ContainsKey("equipment"))
			{
				this.Equipment.FromObject(dictionary["equipment"]);
			}
			return this;
		}
	}
}
