using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands
{
	public class BattleIdResponse : AbstractResponse
	{
		public string BattleId
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return null;
			}
			if (dictionary.ContainsKey("battleId"))
			{
				this.BattleId = (string)dictionary["battleId"];
			}
			return this;
		}
	}
}
