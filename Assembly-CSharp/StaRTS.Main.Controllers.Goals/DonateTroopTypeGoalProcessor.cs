using StaRTS.Main.Models;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Goals
{
	public class DonateTroopTypeGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private TroopType troopType;

		public DonateTroopTypeGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			string goalItem = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(goalItem))
			{
				Service.Logger.ErrorFormat("Troop type not found for goal {0}", new object[]
				{
					vo.Uid
				});
			}
			else
			{
				this.troopType = StringUtils.ParseEnum<TroopType>(goalItem);
			}
			Service.EventManager.RegisterObserver(this, EventId.SquadTroopsDonatedByCurrentPlayer);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SquadTroopsDonatedByCurrentPlayer)
			{
				Dictionary<string, int> dictionary = (Dictionary<string, int>)cookie;
				StaticDataController staticDataController = Service.StaticDataController;
				foreach (KeyValuePair<string, int> current in dictionary)
				{
					TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(current.Key);
					if (troopTypeVO.Type == this.troopType)
					{
						this.parent.Progress(this, current.Value * troopTypeVO.Size);
					}
				}
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.SquadTroopsDonatedByCurrentPlayer);
			base.Destroy();
		}
	}
}
