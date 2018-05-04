using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Models.Player.Store
{
	public class InventoryStorage : ISerializable
	{
		public const int NO_LIMIT = -1;

		public const string INVENTORY_CREDITS = "credits";

		public const string INVENTORY_MATERIALS = "materials";

		public const string INVENTORY_CONTRABAND = "contraband";

		public const string INVENTORY_REPUTATION = "reputation";

		public const string INVENTORY_CRYSTALS = "crystals";

		public const string INVENTORY_DROIDS = "droids";

		public const string INVENTORY_XP = "xp";

		public const string SUBSTORAGE_TROOP = "troop";

		public const string SUBSTORAGE_SPECIALATTACK = "specialAttack";

		public const string SUBSTORAGE_HERO = "hero";

		public const string SUBSTORAGE_CHAMPION = "champion";

		public const string SUBSTORAGE_BUILDING = "building";

		public const string SUBSTORAGE_RESOURCE = "resources";

		private Dictionary<string, InventoryEntry> internalStorage;

		private Dictionary<string, InventoryStorage> subStorage;

		private int totalStorageCapacity;

		private EventId inventoryEvent;

		private Type inventoryType;

		public string Key
		{
			get;
			set;
		}

		public InventoryStorage(string key, EventId updateEvent, Type inventoryType)
		{
			this.inventoryType = inventoryType;
			this.Key = key;
			this.inventoryEvent = updateEvent;
			this.internalStorage = new Dictionary<string, InventoryEntry>();
			this.subStorage = new Dictionary<string, InventoryStorage>();
			this.totalStorageCapacity = -1;
		}

		public void CreateInventoryItem(string inventoryKey, int amount, int capacity)
		{
			InventoryEntry inventoryEntry = new InventoryEntry();
			inventoryEntry.Amount = amount;
			inventoryEntry.Capacity = capacity;
			this.internalStorage.Add(inventoryKey, inventoryEntry);
		}

		private void CalculateScale(string inventoryEntryKey)
		{
			int scale = 1;
			if (this.inventoryType != null)
			{
				IUpgradeableVO upgradeableVO = null;
				if (this.inventoryType == typeof(TroopTypeVO))
				{
					upgradeableVO = Service.StaticDataController.Get<TroopTypeVO>(inventoryEntryKey);
				}
				else if (this.inventoryType == typeof(SpecialAttackTypeVO))
				{
					upgradeableVO = Service.StaticDataController.Get<SpecialAttackTypeVO>(inventoryEntryKey);
				}
				if (upgradeableVO != null)
				{
					scale = upgradeableVO.Size;
				}
			}
			this.internalStorage[inventoryEntryKey].Scale = scale;
		}

		public void CreateInventoryItem(string inventoryKey, InventoryEntry entry)
		{
			if (this.internalStorage.ContainsKey(inventoryKey))
			{
				Service.Logger.ErrorFormat("Key {0} already exists in storage {1}.", new object[]
				{
					inventoryKey,
					this.Key
				});
				return;
			}
			this.internalStorage.Add(inventoryKey, entry);
		}

		public bool HasItem(string inventoryKey)
		{
			return this.internalStorage.ContainsKey(inventoryKey);
		}

		public void SetItemCapacity(string inventoryKey, int newCapacity)
		{
			if (!this.internalStorage.ContainsKey(inventoryKey))
			{
				Service.Logger.ErrorFormat("Key {0} not found in storage {1}.", new object[]
				{
					inventoryKey,
					this.Key
				});
				return;
			}
			this.internalStorage[inventoryKey].Capacity = newCapacity;
			Service.EventManager.SendEvent(this.inventoryEvent, inventoryKey);
		}

		public int GetItemCapacity(string inventoryKey)
		{
			if (!this.internalStorage.ContainsKey(inventoryKey))
			{
				return -1;
			}
			return this.internalStorage[inventoryKey].Capacity;
		}

		public bool ModifyItemAmount(string inventoryKey, int delta)
		{
			if (inventoryKey == null)
			{
				return false;
			}
			if (!this.internalStorage.ContainsKey(inventoryKey))
			{
				this.CreateInventoryItem(inventoryKey, 0, -1);
			}
			InventoryEntry inventoryEntry = this.internalStorage[inventoryKey];
			if (inventoryEntry.Capacity != -1 && delta > 0 && inventoryEntry.Amount + delta > inventoryEntry.Capacity)
			{
				Service.Logger.WarnFormat("Item storage exceeded: {0} + {1} > {2}. Key {3} in storage {4}.", new object[]
				{
					inventoryEntry.Amount,
					delta,
					inventoryEntry.Capacity,
					inventoryKey,
					this.Key
				});
				return false;
			}
			int num = this.GetTotalStorageCapacity();
			if (num != -1)
			{
				int totalStorageAmount = this.GetTotalStorageAmount();
				if (inventoryEntry.Scale == -1)
				{
					this.CalculateScale(inventoryKey);
				}
				int num2 = delta * inventoryEntry.Scale;
				if (num2 > 0 && totalStorageAmount + num2 > num)
				{
					Service.Logger.WarnFormat("Total storage exceeded: {0} + {1} > {2}. Key {3} in storage {4}.", new object[]
					{
						totalStorageAmount,
						num2,
						num,
						inventoryKey,
						this.Key
					});
					return false;
				}
			}
			inventoryEntry.Amount += delta;
			Service.EventManager.SendEvent(this.inventoryEvent, inventoryKey);
			return true;
		}

		public bool CanStoreAll(int delta, CurrencyType currencyType)
		{
			string key = string.Empty;
			switch (currencyType)
			{
			case CurrencyType.Credits:
				key = "credits";
				break;
			case CurrencyType.Materials:
				key = "materials";
				break;
			case CurrencyType.Contraband:
				key = "contraband";
				break;
			case CurrencyType.Reputation:
				key = "reputation";
				break;
			default:
				return false;
			}
			InventoryEntry inventoryEntry = this.internalStorage[key];
			int num = delta;
			if (inventoryEntry.Capacity != -1)
			{
				num = inventoryEntry.Capacity - inventoryEntry.Amount;
			}
			return delta <= num;
		}

		public int ModifyCredits(int delta)
		{
			if (delta == 0)
			{
				return 0;
			}
			int val = delta;
			if (this.internalStorage["credits"].Capacity != -1)
			{
				val = this.internalStorage["credits"].Capacity - this.internalStorage["credits"].Amount;
			}
			int num = Math.Min(delta, val);
			this.ModifyItemAmount("credits", num);
			return delta - num;
		}

		public int ModifyMaterials(int delta)
		{
			if (delta == 0)
			{
				return 0;
			}
			int val = delta;
			if (this.internalStorage["materials"].Capacity != -1)
			{
				val = this.internalStorage["materials"].Capacity - this.internalStorage["materials"].Amount;
			}
			int num = Math.Min(delta, val);
			this.ModifyItemAmount("materials", num);
			return delta - num;
		}

		public int ModifyContraband(int delta)
		{
			if (delta == 0)
			{
				return 0;
			}
			int val = delta;
			if (this.internalStorage["contraband"].Capacity != -1)
			{
				val = this.internalStorage["contraband"].Capacity - this.internalStorage["contraband"].Amount;
			}
			int num = Math.Min(delta, val);
			this.ModifyItemAmount("contraband", num);
			return delta - num;
		}

		public int ModifyReputation(int delta)
		{
			if (delta == 0)
			{
				return 0;
			}
			int val = delta;
			int capacity = this.internalStorage["reputation"].Capacity;
			int amount = this.internalStorage["reputation"].Amount;
			if (capacity != -1)
			{
				val = capacity - amount;
			}
			int num = Math.Max(Math.Min(delta, val), -amount);
			this.ModifyItemAmount("reputation", num);
			return delta - num;
		}

		public void ModifyCrystals(int delta)
		{
			this.ModifyItemAmount("crystals", delta);
		}

		public void ModifyDroids(int delta)
		{
			this.ModifyItemAmount("droids", delta);
			Service.EventManager.SendEvent(this.inventoryEvent, "droids");
		}

		public void ModifyXP(int delta)
		{
			this.ModifyItemAmount("xp", delta);
			Service.EventManager.SendEvent(this.inventoryEvent, "xp");
		}

		public int GetItemAmount(string inventoryKey)
		{
			if (!this.internalStorage.ContainsKey(inventoryKey))
			{
				return 0;
			}
			return this.internalStorage[inventoryKey].Amount;
		}

		public InventoryStorage Sub(string key)
		{
			if (!this.subStorage.ContainsKey(key))
			{
				Service.Logger.ErrorFormat("Substorage {0} not found in storage {1}.", new object[]
				{
					key,
					this.Key
				});
				return new InventoryStorage(key, this.inventoryEvent, null);
			}
			return this.subStorage[key];
		}

		public InventoryStorage CreateSubstorage(string key, EventId updateEvent, Type inventoryType)
		{
			InventoryStorage inventoryStorage = new InventoryStorage(key, updateEvent, inventoryType);
			this.subStorage.Add(key, inventoryStorage);
			return inventoryStorage;
		}

		public int GetTotalStorageAmount()
		{
			int num = 0;
			foreach (KeyValuePair<string, InventoryEntry> current in this.internalStorage)
			{
				if (current.Value.Scale == -1)
				{
					this.CalculateScale(current.Key);
				}
				num += current.Value.Amount * current.Value.Scale;
			}
			return num;
		}

		public void SetTotalStorageCapacity(int capacity)
		{
			this.totalStorageCapacity = capacity;
			Service.EventManager.SendEvent(this.inventoryEvent, null);
		}

		public int GetTotalStorageCapacity()
		{
			return this.totalStorageCapacity;
		}

		public void ClearItemAmount(string inventoryKey)
		{
			if (!this.internalStorage.ContainsKey(inventoryKey))
			{
				this.CreateInventoryItem(inventoryKey, 0, -1);
			}
			InventoryEntry inventoryEntry = this.internalStorage[inventoryKey];
			inventoryEntry.Amount = 0;
			Service.EventManager.SendEvent(this.inventoryEvent, inventoryKey);
		}

		public Dictionary<string, InventoryEntry> GetInternalStorage()
		{
			return this.internalStorage;
		}

		public string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.Add<int>("capacity", this.totalStorageCapacity);
			Serializer serializer2 = Serializer.Start();
			foreach (KeyValuePair<string, InventoryEntry> current in this.internalStorage)
			{
				serializer2.AddObject<InventoryEntry>(current.Key, current.Value);
			}
			serializer2.End();
			serializer.Add<string>("storage", serializer2.ToString());
			if (this.subStorage.Count > 0)
			{
				Serializer serializer3 = Serializer.Start();
				foreach (KeyValuePair<string, InventoryStorage> current2 in this.subStorage)
				{
					serializer3.AddObject<InventoryStorage>(current2.Key, current2.Value);
				}
				serializer3.End();
				serializer.Add<string>("subStorage", serializer3.ToString());
			}
			return serializer.End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.totalStorageCapacity = Convert.ToInt32(dictionary["capacity"]);
			if (dictionary.ContainsKey("storage"))
			{
				Dictionary<string, object> dictionary2 = dictionary["storage"] as Dictionary<string, object>;
				if (dictionary2 != null)
				{
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						this.CreateStorageItem(current.Key, current.Value);
					}
				}
			}
			if (!dictionary.ContainsKey("subStorage"))
			{
				return this;
			}
			dictionary = (dictionary["subStorage"] as Dictionary<string, object>);
			foreach (KeyValuePair<string, InventoryStorage> current2 in this.subStorage)
			{
				if (dictionary.ContainsKey(current2.Key))
				{
					current2.Value.FromObject(dictionary[current2.Key]);
				}
			}
			return this;
		}

		protected void CreateStorageItem(string key, object obj)
		{
			InventoryEntry inventoryEntry = new InventoryEntry();
			inventoryEntry.FromObject(obj);
			this.CreateInventoryItem(key, inventoryEntry);
		}

		public bool IsInventorySubstorageFull()
		{
			if (this.totalStorageCapacity == -1)
			{
				return false;
			}
			int num = 0;
			foreach (string current in this.subStorage.Keys)
			{
				InventoryStorage inventoryStorage = this.subStorage[current];
				num += inventoryStorage.GetTotalStorageAmount();
			}
			return num >= this.totalStorageCapacity;
		}

		public void AddString(StringBuilder sb, bool skipScale)
		{
			List<string> list = new List<string>(this.GetInternalStorage().Keys);
			list.Sort();
			foreach (string current in list)
			{
				if (current != "xp" && current != "credits" && current != "crystals" && current != "materials")
				{
					sb.Append(current).Append("|");
					this.GetInternalStorage()[current].AddString(sb, skipScale);
				}
			}
		}

		public void AddString(StringBuilder sb)
		{
			this.AddString(sb, false);
		}
	}
}
