using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Goals
{
	public class ReceiveSquadUnitGoalProcessor : BaseGoalProcessor, IEventObserver
	{
		public ReceiveSquadUnitGoalProcessor(IValueObject vo, AbstractGoalManager parent) : base(vo, parent)
		{
			Service.EventManager.RegisterObserver(this, EventId.SquadTroopsReceived);
		}

		public EatResponse OnEvent(EventId id, object cookie)
		{
			if (id == EventId.SquadTroopsReceived)
			{
				KeyValuePair<string, int> keyValuePair = (KeyValuePair<string, int>)cookie;
				string key = keyValuePair.Key;
				int value = keyValuePair.Value;
				this.CheckUnusedPveFlag();
				StaticDataController staticDataController = Service.StaticDataController;
				TroopTypeVO troopTypeVO = staticDataController.Get<TroopTypeVO>(key);
				this.parent.Progress(this, troopTypeVO.Size * value);
			}
			return EatResponse.NotEaten;
		}

		public override void Destroy()
		{
			Service.EventManager.UnregisterObserver(this, EventId.SquadTroopsReceived);
			base.Destroy();
		}
	}
}
