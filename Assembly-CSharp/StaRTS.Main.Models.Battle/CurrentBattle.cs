using StaRTS.Main.Models.Player.Misc;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle
{
	public class CurrentBattle : BattleEntry
	{
		public uint TimePassed
		{
			get;
			set;
		}

		public int InitialHealth
		{
			get;
			set;
		}

		public int CurrentHealth
		{
			get;
			set;
		}

		public bool Canceled
		{
			get;
			set;
		}

		public BattleDeploymentData AttackerDeployableData
		{
			get;
			set;
		}

		public BattleDeploymentData DefenderDeployableData
		{
			get;
			set;
		}

		public BattleDeploymentData SeededPlayerDeployableData
		{
			get;
			set;
		}

		public Dictionary<string, int> BuildingLootCreditsMap
		{
			get;
			set;
		}

		public Dictionary<string, int> BuildingLootMaterialsMap
		{
			get;
			set;
		}

		public Dictionary<string, int> BuildingLootContrabandMap
		{
			get;
			set;
		}

		public int LootCreditsDiscarded
		{
			get;
			set;
		}

		public int LootMaterialsDiscarded
		{
			get;
			set;
		}

		public int LootContrabandDiscarded
		{
			get;
			set;
		}

		public string BattleMusic
		{
			get;
			set;
		}

		public string AmbientMusic
		{
			get;
			set;
		}

		public Dictionary<string, string> DeadBuildingKeys
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

		public string BattleUid
		{
			get;
			set;
		}

		public BattleType Type
		{
			get;
			set;
		}

		public int TimeLeft
		{
			get;
			set;
		}

		public bool MultipleHeroDeploysAllowed
		{
			get;
			set;
		}

		public bool IsReplay
		{
			get;
			set;
		}

		public int DefenderBaseScore
		{
			get;
			set;
		}

		public bool PlayerDeployedGuildTroops
		{
			get;
			set;
		}

		public Dictionary<string, int> DefenderGuildTroopsAvailable
		{
			get;
			set;
		}

		public Dictionary<string, int> DefenderGuildTroopsDestroyed
		{
			get;
			set;
		}

		public Dictionary<string, int> AttackerGuildTroopsAvailable
		{
			get;
			set;
		}

		public Dictionary<string, int> AttackerGuildTroopsDeployed
		{
			get;
			set;
		}

		public Dictionary<string, int> DefenderChampionsAvailable
		{
			get;
			set;
		}

		public List<string> DisabledBuildings
		{
			get;
			set;
		}

		public List<string> UnarmedTraps
		{
			get;
			set;
		}

		public string PvpMissionUid
		{
			get;
			set;
		}

		public Dictionary<string, int> NumVisitors
		{
			get;
			set;
		}
	}
}
