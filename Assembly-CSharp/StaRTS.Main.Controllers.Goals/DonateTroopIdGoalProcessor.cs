using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Goals
{
	public class DonateTroopIdGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		private string troopId;

		public DonateTroopIdGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			this.troopId = parent.GetGoalItem(vo);
			if (string.IsNullOrEmpty(this.troopId))
			{
				Service.Logger.ErrorFormat("Troop ID not found for goal {0}", new object[]
				{
					vo.Uid
				});
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
					if (troopTypeVO.TroopID == this.troopId)
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
