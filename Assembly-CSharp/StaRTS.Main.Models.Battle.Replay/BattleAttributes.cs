using StaRTS.Main.Models.Entities;
using StaRTS.Main.Models.Entities.Components;
using StaRTS.Main.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class BattleAttributes : ISerializable
	{
		private const string BATTLE_ENDED_AT_KEY = "battleEndedAt";

		private const string DAMAGE_PERCENTAGE_AT_KEY = "damagePercentage";

		private const string TIME_LEFT_KEY = "timeLeft";

		private const string DEVICE_INFO_KEY = "deviceInfo";

		private const string DEATH_LOG_DELIM = "_";

		private const string DEATH_LOG_UNKNOWN = "unknown";

		private List<string> deathLogUids;

		private List<uint> deathLogTimes;

		public uint BattleEndedAt
		{
			get;
			set;
		}

		public int DamagePercentage
		{
			get;
			set;
		}

		public int TimeLeft
		{
			get;
			set;
		}

		public int LootCreditsEarned
		{
			get;
			set;
		}

		public int LootMaterialsEarned
		{
			get;
			set;
		}

		public int LootContrabandEarned
		{
			get;
			set;
		}

		public string DeviceInfo
		{
			get;
			private set;
		}

		public int DeathLogCount
		{
			get;
			private set;
		}

		public BattleAttributes()
		{
			this.BattleEndedAt = 0u;
			this.DamagePercentage = 0;
			this.TimeLeft = 0;
			this.LootCreditsEarned = 0;
			this.LootMaterialsEarned = 0;
			this.LootContrabandEarned = 0;
			this.DeviceInfo = null;
			this.deathLogUids = null;
			this.deathLogTimes = null;
			this.DeathLogCount = 0;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("battleEndedAt"))
			{
				this.BattleEndedAt = Convert.ToUInt32(dictionary["battleEndedAt"]);
			}
			if (dictionary.ContainsKey("damagePercentage"))
			{
				this.DamagePercentage = Convert.ToInt32(dictionary["damagePercentage"]);
			}
			if (dictionary.ContainsKey("timeLeft"))
			{
				this.TimeLeft = Convert.ToInt32(dictionary["timeLeft"]);
			}
			if (dictionary.ContainsKey("lootCreditsEarned"))
			{
				this.LootCreditsEarned = Convert.ToInt32(dictionary["lootCreditsEarned"]);
			}
			if (dictionary.ContainsKey("lootMaterialsEarned"))
			{
				this.LootMaterialsEarned = Convert.ToInt32(dictionary["lootMaterialsEarned"]);
			}
			if (dictionary.ContainsKey("lootContrabandEarned"))
			{
				this.LootContrabandEarned = Convert.ToInt32(dictionary["lootContrabandEarned"]);
			}
			if (dictionary.ContainsKey("deviceInfo"))
			{
				this.DeviceInfo = (dictionary["deviceInfo"] as string);
			}
			if (dictionary.ContainsKey("deathLog"))
			{
				this.DeathLogCount = 0;
				this.deathLogUids = null;
				this.deathLogTimes = null;
				List<object> list = dictionary["deathLog"] as List<object>;
				if (list != null)
				{
					int i = 0;
					int count = list.Count;
					while (i < count)
					{
						string text = list[i] as string;
						if (text != null)
						{
							int num = text.LastIndexOf("_");
							if (num >= 0)
							{
								string uid = text.Substring(0, num);
								uint time = Convert.ToUInt32(text.Substring(num + 1));
								this.InternalAddToDeathLog(uid, time);
							}
						}
						i++;
					}
				}
			}
			return this;
		}

		public string ToJson()
		{
			List<string> list = new List<string>();
			for (int i = 0; i < this.DeathLogCount; i++)
			{
				string item = string.Format("{0}{1}{2}", this.deathLogUids[i], "_", this.deathLogTimes[i]);
				list.Add(item);
			}
			return Serializer.Start().Add<uint>("battleEndedAt", this.BattleEndedAt).Add<int>("damagePercentage", this.DamagePercentage).Add<int>("timeLeft", this.TimeLeft).Add<int>("lootCreditsEarned", this.LootCreditsEarned).Add<int>("lootMaterialsEarned", this.LootMaterialsEarned).Add<int>("lootContrabandEarned", this.LootContrabandEarned).AddArrayOfPrimitives<string>("deathLog", list).AddString("deviceInfo", this.DeviceInfo).End().ToString();
		}

		public static bool Equals(BattleAttributes attr1, BattleAttributes attr2)
		{
			if (object.ReferenceEquals(attr1, attr2))
			{
				return true;
			}
			if (attr1 == null || attr2 == null)
			{
				return false;
			}
			if (attr1.BattleEndedAt != attr2.BattleEndedAt || attr1.DamagePercentage != attr2.DamagePercentage || attr1.TimeLeft != attr2.TimeLeft || attr1.LootCreditsEarned != attr2.LootCreditsEarned || attr1.LootMaterialsEarned != attr2.LootMaterialsEarned || attr1.LootContrabandEarned != attr2.LootContrabandEarned || attr1.DeathLogCount != attr2.DeathLogCount)
			{
				return false;
			}
			int i = 0;
			int deathLogCount = attr1.DeathLogCount;
			while (i < deathLogCount)
			{
				if (attr1.deathLogUids[i] != attr2.deathLogUids[i])
				{
					return false;
				}
				if (attr1.deathLogTimes[i] != attr2.deathLogTimes[i])
				{
					return false;
				}
				i++;
			}
			return true;
		}

		private void InternalAddToDeathLog(string uid, uint time)
		{
			if (this.DeathLogCount == 0)
			{
				this.deathLogUids = new List<string>();
				this.deathLogTimes = new List<uint>();
			}
			this.deathLogUids.Add(uid);
			this.deathLogTimes.Add(time);
			this.DeathLogCount++;
		}

		public void AddToDeathLog(SmartEntity entity, uint time)
		{
			string uid = "unknown";
			if (entity != null)
			{
				BuildingComponent buildingComp = entity.BuildingComp;
				if (buildingComp != null)
				{
					uid = buildingComp.BuildingType.Uid;
				}
				else
				{
					TroopComponent troopComp = entity.TroopComp;
					if (troopComp != null)
					{
						uid = troopComp.TroopType.Uid;
					}
				}
			}
			this.InternalAddToDeathLog(uid, time);
		}

		public void AddDeviceInfo()
		{
			this.DeviceInfo = GameUtils.GetDeviceInfo();
		}
	}
}
