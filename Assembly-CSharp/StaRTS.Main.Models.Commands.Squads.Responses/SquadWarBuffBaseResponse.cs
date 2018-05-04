using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Squads.War;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class SquadWarBuffBaseResponse : AbstractResponse
	{
		public SquadWarBuffBaseData SquadWarBuffBaseData
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			if (!(obj is Dictionary<string, object>))
			{
				return this;
			}
			this.SquadWarBuffBaseData = new SquadWarBuffBaseData();
			this.SquadWarBuffBaseData.FromObject(obj);
			return this;
		}
	}
}
