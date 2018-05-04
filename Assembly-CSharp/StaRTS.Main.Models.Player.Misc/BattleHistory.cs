using StaRTS.Main.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Player.Misc
{
	public class BattleHistory : ISerializable
	{
		private List<BattleEntry> battles;

		public BattleHistory()
		{
			this.battles = new List<BattleEntry>();
		}

		public void AddBattle(BattleEntry battle)
		{
			BattleEntry battleEntry = battle.Clone();
			battleEntry.SetupExpendedTroops();
			this.battles.Add(battleEntry);
		}

		public string ToJson()
		{
			return Serializer.Start().AddArray<BattleEntry>("battleLogs", this.battles).End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			List<object> list = obj as List<object>;
			this.battles = new List<BattleEntry>();
			int count = list.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = new BattleEntry();
				battleEntry.FromObject(list[i]);
				this.battles.Add(battleEntry);
			}
			return this;
		}

		public void SetupBattles()
		{
			int count = this.battles.Count;
			for (int i = 0; i < count; i++)
			{
				this.battles[i].SetupExpendedTroops();
			}
		}

		public List<BattleEntry> GetBattleHistory()
		{
			return this.battles;
		}

		public int GetTotalPvpCreditsLooted()
		{
			int count = this.battles.Count;
			int num = 0;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = this.battles[i];
				if (battleEntry.IsPvP())
				{
					num += battleEntry.LootCreditsEarned;
				}
			}
			return num;
		}

		public int GetTotalPvpMaterialLooted()
		{
			int num = 0;
			int count = this.battles.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = this.battles[i];
				if (battleEntry.IsPvP())
				{
					num += battleEntry.LootMaterialsEarned;
				}
			}
			return num;
		}

		public int GetTotalPvpContrabandLooted()
		{
			int num = 0;
			int count = this.battles.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = this.battles[i];
				if (battleEntry.IsPvP())
				{
					num += battleEntry.LootContrabandEarned;
				}
			}
			return num;
		}

		public int GetTotalPvpWins()
		{
			int num = 0;
			int count = this.battles.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = this.battles[i];
				if (battleEntry.IsPvP() && battleEntry.Won)
				{
					num++;
				}
			}
			return num;
		}

		public BattleEntry GetBattleEntryById(string id)
		{
			BattleEntry result = null;
			int count = this.battles.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = this.battles[i];
				if (battleEntry.RecordID == id)
				{
					if (GameUtils.IsBattleVersionSupported(battleEntry.CmsVersion, battleEntry.BattleVersion))
					{
						result = battleEntry;
					}
					break;
				}
			}
			return result;
		}

		public BattleEntry GetLatestValidPvPBattle()
		{
			BattleEntry result = null;
			uint num = 0u;
			int count = this.battles.Count;
			for (int i = 0; i < count; i++)
			{
				BattleEntry battleEntry = this.battles[i];
				if (GameUtils.IsBattleVersionSupported(battleEntry.CmsVersion, battleEntry.BattleVersion) && battleEntry.IsPvP() && battleEntry.EndBattleServerTime > num)
				{
					num = battleEntry.EndBattleServerTime;
					result = battleEntry;
				}
			}
			return result;
		}
	}
}
