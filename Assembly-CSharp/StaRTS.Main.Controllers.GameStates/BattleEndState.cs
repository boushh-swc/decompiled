using StaRTS.Main.Models;
using StaRTS.Main.Views.UX.Screens;
using StaRTS.Utils.Core;
using StaRTS.Utils.State;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class BattleEndState : IGameState, IState
	{
		private bool isSquadWarBattle;

		public BattleEndState(bool isSquadWarBattle)
		{
			this.isSquadWarBattle = isSquadWarBattle;
		}

		public virtual void OnEnter()
		{
			Service.BattleRecordController.EndRecord();
			AlertScreen highestLevelScreen = Service.ScreenController.GetHighestLevelScreen<AlertScreen>();
			if (highestLevelScreen != null && !highestLevelScreen.IsFatal && !highestLevelScreen.IsAlwaysOnTop)
			{
				highestLevelScreen.Close(null);
			}
			this.ShowBattleEndScreen(false);
			Service.UXController.HUD.ConfigureControls(new HudConfig(new string[0]));
			if (Service.CurrentPlayer.CampaignProgress.FueInProgress)
			{
				Service.UXController.MiscElementsManager.HideFingerAnimation();
			}
		}

		public void OnExit(IState nextState)
		{
			Service.BattleController.Clear();
		}

		protected void ShowBattleEndScreen(bool isReplay)
		{
			if (this.isSquadWarBattle)
			{
				Service.ScreenController.AddScreen(new SquadWarBattleEndScreen(isReplay), true, false);
			}
			else
			{
				Service.ScreenController.AddScreen(new BattleEndScreen(isReplay), true, false);
			}
		}

		public bool CanUpdateHomeContracts()
		{
			return false;
		}
	}
}
