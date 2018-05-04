using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Controllers.Goals
{
	public class LootGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private const int CREDITS = 0;

		private const int MATERIAL = 1;

		private const int CONTRABAND = 2;

		private int itemType;

		public LootGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			string goalItem = parent.GetGoalItem(vo);
			if (goalItem != null)
			{
				if (goalItem == "credits")
				{
					this.itemType = 0;
					goto IL_92;
				}
				if (goalItem == "materials")
				{
					this.itemType = 1;
					goto IL_92;
				}
				if (goalItem == "contraband")
				{
					this.itemType = 2;
					goto IL_92;
				}
			}
			Service.Logger.ErrorFormat("Loot type not found for goal {0}", new object[]
			{
				vo.Uid
			});
			IL_92:
			Service.EventManager.RegisterObserver(this, EventId.BattleEndProcessing);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.BattleEndProcessing)
			{
				if (this.IsEventValidForGoal())
				{
					CurrentBattle currentBattle = Service.BattleController.GetCurrentBattle();
					int num = 0;
					if (this.itemType == 0)
					{
						num = currentBattle.LootCreditsEarned;
					}
					else if (this.itemType == 1)
					{
						num = currentBattle.LootMaterialsEarned;
					}
					else if (this.itemType == 2)
					{
						num = currentBattle.LootContrabandEarned;
					}
					if (num > 0)
					{
						this.parent.Progress(this, num);
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.BattleEndProcessing);
			base.Destroy();
		}
	}
}
