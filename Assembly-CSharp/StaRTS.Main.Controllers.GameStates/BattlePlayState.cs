using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class BattlePlayState : IGameState, IState
	{
		public void OnEnter()
		{
			BattleController battleController = Service.BattleController;
			battleController.StartBattle();
			Service.BattleRecordController.StartRecord();
			HudConfig hudConfig = null;
			BattleType type = battleController.GetCurrentBattle().Type;
			switch (type)
			{
			case BattleType.Pvp:
			case BattleType.PveAttack:
			case BattleType.PveFue:
			case BattleType.ClientBattle:
				hudConfig = new HudConfig(new string[]
				{
					"Currency",
					"OpponentInfo",
					"ButtonEndBattle",
					"LabelTimeLeft",
					"DamageStars",
					"LabelBaseNameOpponent",
					"TroopsGrid",
					"LabelDeployInstructions",
					"LabelCurrencyValueOpponent"
				});
				break;
			case BattleType.PveDefend:
				hudConfig = new HudConfig(new string[]
				{
					"Currency",
					"PlayerInfo",
					"ButtonEndBattle",
					"LabelTimeLeft",
					"DamageStars",
					"LabelBaseNameOpponent",
					"TroopsGrid",
					"LabelDeployInstructions",
					"LabelCurrencyValueOpponent"
				});
				break;
			case BattleType.PveBuffBase:
			case BattleType.PvpAttackSquadWar:
				hudConfig = new HudConfig(new string[]
				{
					"OpponentInfo",
					"BuffsYoursSquadWars",
					"ButtonEndBattle",
					"LabelTimeLeft",
					"DamageStars",
					"LabelBaseNameOpponent",
					"TroopsGrid",
					"WarAttackStarted"
				});
				if (type != BattleType.PveBuffBase)
				{
					hudConfig.Add("BuffsOpponentsSquadWars");
				}
				break;
			}
			Service.UXController.HUD.ConfigureControls(hudConfig);
			Service.BILoggingController.SchedulePerformanceLogging(false);
		}

		public void OnExit(IState nextState)
		{
			Service.BILoggingController.UnschedulePerformanceLogging();
			Service.DeployerController.ExitAllDeployModes();
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
