using StaRTS.Main.Models;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class BattleEndPlaybackState : BattleEndState
	{
		public BattleEndPlaybackState(bool isSquadWarBattle) : base(isSquadWarBattle)
		{
		}

		public override void OnEnter()
		{
			base.ShowBattleEndScreen(true);
			Service.UXController.HUD.ConfigureControls(new HudConfig(new string[0]));
		}
	}
}
