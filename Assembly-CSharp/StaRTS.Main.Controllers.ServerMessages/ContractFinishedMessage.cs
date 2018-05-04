using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.ServerMessages
{
	public class ContractFinishedMessage : AbstractMessage
	{
		private List<ContractTO> FinishedContracts;

		public override object MessageCookie
		{
			get
			{
				return this.FinishedContracts;
			}
		}

		public override EventId MessageEventId
		{
			get
			{
				return EventId.ContractsCompletedWhileOffline;
			}
		}

		public override ISerializable FromObject(object obj)
		{
			this.FinishedContracts = new List<ContractTO>();
			List<object> list = (List<object>)obj;
			for (int i = 0; i < list.Count; i++)
			{
				Dictionary<string, object> dictionary = list[i] as Dictionary<string, object>;
				List<object> list2 = dictionary["message"] as List<object>;
				for (int j = 0; j < list2.Count; j++)
				{
					ContractTO item = new ContractTO().FromObject(list2[j]) as ContractTO;
					this.FinishedContracts.Add(item);
				}
			}
			return this;
		}
	}
}
