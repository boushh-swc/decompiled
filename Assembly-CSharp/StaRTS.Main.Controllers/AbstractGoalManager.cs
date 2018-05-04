using StaRTS.Main.Controllers.Goals;
using StaRTS.Main.Models;
using StaRTS.Main.Models.Commands.Crates;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers
{
	public class AbstractGoalManager
	{
		protected const int MAX_RETRIES = 3;

		protected bool refreshing;

		protected int retryCount;

		public AbstractGoalManager()
		{
			this.Login();
		}

		protected virtual void Login()
		{
			this.VerifyCurrentGoalsAgainstMeta();
			this.FillProcessorMap();
		}

		protected virtual void VerifyCurrentGoalsAgainstMeta()
		{
		}

		public virtual void Update()
		{
		}

		public void Relocate()
		{
			this.ClearProcessorMap(false);
			this.FillProcessorMap();
		}

		public void Progress(BaseGoalProcessor processor, int amount)
		{
			this.Progress(processor, amount, null);
		}

		public virtual void Progress(BaseGoalProcessor processor, int amount, object cookie)
		{
		}

		public void ClaimCallback(CrateDataResponse response, object cookie)
		{
			if (response.CrateDataTO != null)
			{
				CrateData crateDataTO = response.CrateDataTO;
				List<string> resolvedSupplyIdList = GameUtils.GetResolvedSupplyIdList(crateDataTO);
				Service.InventoryCrateRewardController.GrantInventoryCrateReward(resolvedSupplyIdList, response.CrateDataTO);
			}
		}

		protected virtual void FillProcessorMap()
		{
		}

		protected virtual void ClearProcessorMap(bool sendExpirationEvent)
		{
		}

		public void RefreshFromServer()
		{
			this.retryCount = 0;
			this.AttemptRefreshFromServer();
		}

		protected virtual void AttemptRefreshFromServer()
		{
		}

		public virtual string GetGoalItem(IValueObject goalVO)
		{
			return null;
		}

		public virtual GoalType GetGoalType(IValueObject goalVO)
		{
			return GoalType.Invalid;
		}

		public virtual bool GetGoalAllowPvE(IValueObject goalVO)
		{
			return true;
		}
	}
}
