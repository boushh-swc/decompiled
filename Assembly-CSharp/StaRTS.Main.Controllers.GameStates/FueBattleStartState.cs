using StaRTS.Main.Models.Battle;
using System;

namespace StaRTS.Main.Controllers.GameStates
{
	public class FueBattleStartState : BattleStartState
	{
		public FueBattleStartState(string battleVOId)
		{
			BattleInitializationData data = BattleInitializationData.CreateFromBattleTypeVO(battleVOId);
			base.Setup(data, null);
		}

		public override void OnEnter()
		{
		}
	}
}
