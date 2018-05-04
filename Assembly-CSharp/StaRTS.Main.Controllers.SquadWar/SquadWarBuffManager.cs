using StaRTS.Main.Controllers.Squads;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.Battle.Replay;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.SquadWar
{
	public class SquadWarBuffManager : IEventObserver
	{
		private SquadController controller;

		public SquadWarBuffManager(SquadController controller)
		{
			this.controller = controller;
			EventManager eventManager = Service.EventManager;
			eventManager.RegisterObserver(this, EventId.BattleLoadStart, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BattleReplaySetup, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BattleEndFullyProcessed, EventPriority.Default);
			eventManager.RegisterObserver(this, EventId.BattleLeftBeforeStarting, EventPriority.Default);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			switch (id)
			{
			case EventId.BattleReplaySetup:
			{
				BattleRecord battleRecord = (BattleRecord)cookie;
				SquadWarBuffManager.AddWarBuffs(battleRecord.BattleType, battleRecord.AttackerWarBuffs, battleRecord.DefenderWarBuffs);
				return EatResponse.NotEaten;
			}
			case EventId.BattleRecordRetrieved:
				IL_18:
				if (id == EventId.BattleLoadStart)
				{
					if (this.controller.WarManager.CurrentSquadWar != null)
					{
						CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
						SquadWarBuffManager.AddWarBuffs(currentBattle.Type, currentBattle.AttackerWarBuffs, currentBattle.DefenderWarBuffs);
					}
					return EatResponse.NotEaten;
				}
				if (id != EventId.BattleEndFullyProcessed)
				{
					return EatResponse.NotEaten;
				}
				goto IL_92;
			case EventId.BattleLeftBeforeStarting:
				goto IL_92;
			}
			goto IL_18;
			IL_92:
			Service.BuffController.ClearWarBuffs();
			return EatResponse.NotEaten;
		}

		public static void AddWarBuffs(BattleType battleType, List<string> attackerBuffs, List<string> defenderBuffs)
		{
			BuffController buffController = Service.BuffController;
			buffController.ClearWarBuffs();
			if (battleType != BattleType.PvpAttackSquadWar && battleType != BattleType.PveBuffBase)
			{
				return;
			}
			StaticDataController staticDataController = Service.StaticDataController;
			if (attackerBuffs != null)
			{
				int i = 0;
				int count = attackerBuffs.Count;
				while (i < count)
				{
					WarBuffVO warBuff = staticDataController.Get<WarBuffVO>(attackerBuffs[i]);
					buffController.AddAttackerWarBuff(warBuff);
					i++;
				}
			}
			if (battleType != BattleType.PveBuffBase && defenderBuffs != null)
			{
				int j = 0;
				int count2 = defenderBuffs.Count;
				while (j < count2)
				{
					WarBuffVO warBuff2 = staticDataController.Get<WarBuffVO>(defenderBuffs[j]);
					buffController.AddDefenderWarBuff(warBuff2);
					j++;
				}
			}
		}
	}
}
