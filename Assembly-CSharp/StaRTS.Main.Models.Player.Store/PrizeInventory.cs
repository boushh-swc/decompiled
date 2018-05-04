using StaRTS.Externals.Manimal;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Store
{
	public class PrizeInventory : ISerializable
	{
		private const string COMPENSATION_CRATE_CONTEXT = "gsat";

		public Dictionary<string, int> Troops
		{
			get;
			private set;
		}

		public Dictionary<string, int> SpecialAttacks
		{
			get;
			private set;
		}

		public Dictionary<string, int> CurrencyResources
		{
			get;
			private set;
		}

		public InventoryCrates Crates
		{
			get;
			private set;
		}

		public PrizeInventory()
		{
			this.Crates = new InventoryCrates();
			this.Troops = new Dictionary<string, int>();
			this.SpecialAttacks = new Dictionary<string, int>();
			this.CurrencyResources = new Dictionary<string, int>();
		}

		public int GetTroopAmount(string troopID)
		{
			return this.GetAmount(this.Troops, troopID);
		}

		public int GetSpecialAttackAmount(string specialAttackID)
		{
			return this.GetAmount(this.SpecialAttacks, specialAttackID);
		}

		public int GetResourceAmount(string resourceName)
		{
			return this.GetAmount(this.CurrencyResources, resourceName);
		}

		private int GetAmount(Dictionary<string, int> dict, string key)
		{
			return (!dict.ContainsKey(key)) ? 0 : dict[key];
		}

		public int GetTotalTroopAmount()
		{
			int num = 0;
			foreach (KeyValuePair<string, int> current in this.Troops)
			{
				num += current.Value;
			}
			return num;
		}

		public int GetTotalSpecialAttackAmount()
		{
			int num = 0;
			foreach (KeyValuePair<string, int> current in this.SpecialAttacks)
			{
				num += current.Value;
			}
			return num;
		}

		public void ModifyTroopAmount(string troopID, int delta)
		{
			this.ModifyAmount(this.Troops, troopID, delta);
		}

		public void ModifySpecialAttackAmount(string specialAttackID, int delta)
		{
			this.ModifyAmount(this.SpecialAttacks, specialAttackID, delta);
		}

		public void ModifyResourceAmount(string resourceName, int delta)
		{
			this.ModifyAmount(this.CurrencyResources, resourceName, delta);
		}

		private void ModifyAmount(Dictionary<string, int> dict, string key, int delta)
		{
			if (dict.ContainsKey(key))
			{
				dict[key] += delta;
			}
			else if (delta > 0)
			{
				dict[key] = delta;
			}
		}

		public string ToJson()
		{
			return "{}";
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("troop"))
				{
					this.ObjectToDictionary(dictionary["troop"], this.Troops);
				}
				if (dictionary.ContainsKey("specialAttack"))
				{
					this.ObjectToDictionary(dictionary["specialAttack"], this.SpecialAttacks);
				}
				if (dictionary.ContainsKey("resources"))
				{
					this.ObjectToDictionary(dictionary["resources"], this.CurrencyResources);
				}
				if (dictionary.ContainsKey("crates"))
				{
					object obj2 = dictionary["crates"];
					this.Crates.FromObject(obj2);
				}
			}
			return this;
		}

		private void ObjectToDictionary(object obj, Dictionary<string, int> dict)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				foreach (KeyValuePair<string, object> current in dictionary)
				{
					dict.Add(current.Key, Convert.ToInt32(current.Value));
				}
			}
		}

		public List<CrateData> GetCompensationCrates()
		{
			uint time = ServerTime.Time;
			List<CrateData> list = new List<CrateData>();
			foreach (CrateData current in this.Crates.Available.Values)
			{
				if (current.Context == "gsat" && !current.Claimed && (!current.DoesExpire || current.ExpiresTimeStamp > time))
				{
					list.Add(current);
				}
			}
			return list;
		}
	}
}
