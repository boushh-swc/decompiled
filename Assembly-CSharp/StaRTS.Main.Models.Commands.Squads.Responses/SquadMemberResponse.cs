using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class SquadMemberResponse : AbstractResponse
	{
		public object SquadMemberData
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
			this.SquadMemberData = obj;
			return this;
		}
	}
}
