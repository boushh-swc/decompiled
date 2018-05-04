using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands
{
	public class BattleEndRequest : PlayerIdChecksumRequest
	{
		protected CurrentBattle battle;

		protected Dictionary<string, int> seededTroopsDeployed;

		protected Dictionary<string, int> defendingUnitsKilled;

		protected Dictionary<string, int> attackingUnitsKilled;

		protected Dictionary<string, int> defenderGuildUnitsSpent;

		protected Dictionary<string, int> attackerGuildUnitsSpent;

		protected Dictionary<string, int> lootEarned;

		protected Dictionary<string, int> buildingHealthMap;

		protected Dictionary<string, string> buildingUids;

		protected List<string> unarmedTraps;

		protected BattleRecord replayData;

		protected string planetId;

		public BattleEndRequest(CurrentBattle battle, Dictionary<string, int> seededTroopsDeployed, Dictionary<string, int> defendingUnitsKilled, Dictionary<string, int> attackingUnitsKilled, Dictionary<string, int> defenderGuildUnitsSpent, Dictionary<string, int> attackerGuildUnitsSpent, Dictionary<string, int> lootEarned, Dictionary<string, int> buildingHealthMap, Dictionary<string, string> buildingUids, List<string> unarmedTraps, BattleRecord replayData)
		{
			this.battle = battle;
			this.seededTroopsDeployed = seededTroopsDeployed;
			this.defendingUnitsKilled = defendingUnitsKilled;
			this.attackingUnitsKilled = attackingUnitsKilled;
			this.defenderGuildUnitsSpent = ((defenderGuildUnitsSpent == null) ? new Dictionary<string, int>() : defenderGuildUnitsSpent);
			this.attackerGuildUnitsSpent = ((attackerGuildUnitsSpent == null) ? new Dictionary<string, int>() : attackerGuildUnitsSpent);
			this.lootEarned = ((lootEarned == null) ? new Dictionary<string, int>() : lootEarned);
			this.buildingHealthMap = buildingHealthMap;
			this.buildingUids = buildingUids;
			this.unarmedTraps = ((unarmedTraps == null) ? new List<string>() : unarmedTraps);
			this.replayData = replayData;
			this.planetId = Service.CurrentPlayer.PlanetId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("cmsVersion", this.replayData.CmsVersion);
			startedSerializer.AddString("battleVersion", this.replayData.BattleVersion);
			startedSerializer.AddString("battleId", this.battle.RecordID);
			startedSerializer.AddString("battleUid", this.battle.BattleUid);
			startedSerializer.AddDictionary<int>("seededTroopsDeployed", this.seededTroopsDeployed);
			startedSerializer.AddDictionary<int>("defendingUnitsKilled", this.defendingUnitsKilled);
			startedSerializer.AddDictionary<int>("attackingUnitsKilled", this.attackingUnitsKilled);
			startedSerializer.AddDictionary<int>("defenderGuildTroopsSpent", this.defenderGuildUnitsSpent);
			startedSerializer.AddDictionary<int>("attackerGuildTroopsSpent", this.attackerGuildUnitsSpent);
			startedSerializer.AddDictionary<int>("loot", this.lootEarned);
			startedSerializer.AddDictionary<int>("damagedBuildings", this.buildingHealthMap);
			if (this.buildingUids != null)
			{
				startedSerializer.AddDictionary<string>("buildingUids", this.buildingUids);
			}
			startedSerializer.AddArrayOfPrimitives<string>("unarmedTraps", this.unarmedTraps);
			startedSerializer.AddObject<BattleRecord>("replayData", this.replayData);
			startedSerializer.Add<int>("baseDamagePercent", this.battle.DamagePercent);
			startedSerializer.AddDictionary<int>("numVisitors", this.battle.NumVisitors);
			startedSerializer.Add<int>("stars", this.battle.EarnedStars);
			startedSerializer.AddBool("isUserEnded", this.battle.Canceled);
			startedSerializer.AddString("planetId", this.planetId);
			return startedSerializer.End().ToString();
		}
	}
}
