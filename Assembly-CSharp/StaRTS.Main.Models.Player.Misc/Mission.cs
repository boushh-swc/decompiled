using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class Mission : ISerializable
	{
		public string Uid
		{
			get;
			set;
		}

		public string CampaignUid
		{
			get;
			set;
		}

		public int EarnedStars
		{
			get;
			set;
		}

		public MissionStatus Status
		{
			get;
			private set;
		}

		public int GrindMissionRetries
		{
			get;
			set;
		}

		public Dictionary<string, int> Counters
		{
			get;
			set;
		}

		public int[] LootRemaining
		{
			get;
			set;
		}

		public bool Locked
		{
			get;
			set;
		}

		public bool Activated
		{
			get;
			set;
		}

		public bool Completed
		{
			get;
			set;
		}

		public bool Collected
		{
			get;
			set;
		}

		public static Mission CreateFromCampaignMissionVO(CampaignMissionVO missionVO)
		{
			return new Mission
			{
				Uid = missionVO.Uid,
				CampaignUid = missionVO.CampaignUid,
				Locked = false,
				Activated = false,
				Completed = false,
				EarnedStars = 0,
				Counters = null,
				LootRemaining = null,
				GrindMissionRetries = 0
			};
		}

		private void EnsureCounters()
		{
			if (this.Counters == null)
			{
				this.Counters = new Dictionary<string, int>();
			}
		}

		private void EnsureLootRemaining()
		{
			if (this.LootRemaining == null)
			{
				int num = 6;
				this.LootRemaining = new int[num];
				for (int i = 0; i < num; i++)
				{
					this.LootRemaining[i] = -1;
				}
			}
		}

		public void AddToCounter(string counterKey, int delta)
		{
			this.EnsureCounters();
			if (this.Counters.ContainsKey(counterKey))
			{
				Dictionary<string, int> counters;
				Dictionary<string, int> expr_1D = counters = this.Counters;
				int num = counters[counterKey];
				expr_1D[counterKey] = num + delta;
			}
			else
			{
				this.Counters.Add(counterKey, delta);
			}
		}

		public void SetLootRemaining(int credits, int materials, int contraband)
		{
			this.EnsureLootRemaining();
			this.LootRemaining[1] = ((credits <= 0) ? 0 : credits);
			this.LootRemaining[2] = ((materials <= 0) ? 0 : materials);
			this.LootRemaining[3] = ((contraband <= 0) ? 0 : contraband);
		}

		public string ToJson()
		{
			return "{}";
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Uid = dictionary["uid"].ToString();
			this.CampaignUid = dictionary["campaignUid"].ToString();
			this.EarnedStars = Convert.ToInt32(dictionary["earnedStars"]);
			this.Counters = null;
			if (dictionary.ContainsKey("grindMissionRetries"))
			{
				this.GrindMissionRetries = Convert.ToInt32(dictionary["grindMissionRetries"]);
			}
			if (dictionary.ContainsKey("counters"))
			{
				Dictionary<string, object> dictionary2 = dictionary["counters"] as Dictionary<string, object>;
				if (dictionary2 != null && dictionary2.Count != 0)
				{
					this.EnsureCounters();
					foreach (KeyValuePair<string, object> current in dictionary2)
					{
						this.Counters.Add(current.Key, Convert.ToInt32(current.Value));
					}
				}
			}
			this.LootRemaining = null;
			if (dictionary.ContainsKey("lootRemaining"))
			{
				Dictionary<string, object> dictionary3 = dictionary["lootRemaining"] as Dictionary<string, object>;
				if (dictionary3 != null && dictionary3.Count != 0)
				{
					this.EnsureLootRemaining();
					foreach (KeyValuePair<string, object> current2 in dictionary3)
					{
						CurrencyType currencyType = StringUtils.ParseEnum<CurrencyType>(current2.Key);
						this.LootRemaining[(int)currencyType] = Convert.ToInt32(current2.Value);
					}
				}
			}
			if (dictionary.ContainsKey("status"))
			{
				this.Status = StringUtils.ParseEnum<MissionStatus>(dictionary["status"] as string);
				if (this.Status == MissionStatus.Claimed)
				{
					this.Collected = true;
					this.Activated = true;
				}
				if (this.Status == MissionStatus.Completed || this.Status == MissionStatus.Claimed)
				{
					this.Completed = true;
					this.Activated = true;
				}
				else if (this.Status != MissionStatus.Default)
				{
					this.Activated = true;
				}
				this.Locked = false;
			}
			else
			{
				this.Locked = true;
				this.Completed = false;
				this.Collected = false;
			}
			return this;
		}
	}
}
