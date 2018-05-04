using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class EndPvEBattleTO
	{
		public CurrentBattle Battle
		{
			get;
			set;
		}

		public Dictionary<string, int> SeededTroopsDeployed
		{
			get;
			set;
		}

		public Dictionary<string, int> DefendingUnitsKilled
		{
			get;
			set;
		}

		public Dictionary<string, int> AttackingUnitsKilled
		{
			get;
			set;
		}

		public Dictionary<string, int> DefenderGuildUnitsSpent
		{
			get;
			set;
		}

		public Dictionary<string, int> AttackerGuildUnitsSpent
		{
			get;
			set;
		}

		public Dictionary<string, int> LootEarned
		{
			get;
			set;
		}

		public Dictionary<string, int> BuildingHealthMap
		{
			get;
			set;
		}

		public Dictionary<string, string> BuildingUids
		{
			get;
			set;
		}

		public List<string> UnarmedTraps
		{
			get;
			set;
		}

		public BattleRecord ReplayData
		{
			get;
			set;
		}
	}
}
