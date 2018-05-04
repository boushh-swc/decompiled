using StaRTS.Main.Models;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class BaseGoalProcessor
	{
		protected AbstractGoalManager parent;

		protected IValueObject goalVO;

		public BaseGoalProcessor(IValueObject goalVO, AbstractGoalManager parent)
		{
			this.goalVO = goalVO;
			this.parent = parent;
		}

		protected void RecordProgress(int amount)
		{
			this.parent.Progress(this, amount);
		}

		public virtual void Destroy()
		{
		}

		public string GetGoalUid()
		{
			return this.goalVO.Uid;
		}

		protected virtual bool IsEventValidForGoal()
		{
			CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
			if (currentBattle != null)
			{
				if (currentBattle.Type == BattleType.ClientBattle || currentBattle.Type == BattleType.PveFue)
				{
					return false;
				}
				if (!this.parent.GetGoalAllowPvE(this.goalVO) && (currentBattle.Type == BattleType.PveDefend || currentBattle.Type == BattleType.PveAttack || currentBattle.Type == BattleType.PveBuffBase || currentBattle.Type == BattleType.PvpAttackSquadWar))
				{
					return false;
				}
				if (currentBattle.IsReplay)
				{
					return false;
				}
			}
			return true;
		}

		protected virtual void CheckUnusedPveFlag()
		{
			if (this.parent.GetGoalAllowPvE(this.goalVO))
			{
				Service.Logger.Warn(string.Concat(new object[]
				{
					"AllowPvE is not supported for GoalType: ",
					this.parent.GetGoalType(this.goalVO),
					" id: ",
					this.goalVO.Uid
				}));
			}
		}
	}
}
