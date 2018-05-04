using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Player.Raids
{
	public class RaidDefenseCompleteRequest : BattleEndRequest
	{
		private string lastWaveId;

		public RaidDefenseCompleteRequest(EndPvEBattleTO endBattleTO, string finalWaveId) : base(endBattleTO.Battle, endBattleTO.SeededTroopsDeployed, endBattleTO.DefendingUnitsKilled, endBattleTO.AttackingUnitsKilled, endBattleTO.DefenderGuildUnitsSpent, endBattleTO.AttackerGuildUnitsSpent, endBattleTO.LootEarned, endBattleTO.BuildingHealthMap, endBattleTO.BuildingUids, endBattleTO.UnarmedTraps, endBattleTO.ReplayData)
		{
			this.lastWaveId = finalWaveId;
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
			startedSerializer.AddString("waveId", this.lastWaveId);
			return startedSerializer.End().ToString();
		}
	}
}
