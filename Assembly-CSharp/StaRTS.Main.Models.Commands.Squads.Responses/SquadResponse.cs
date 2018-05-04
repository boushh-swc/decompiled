using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class SquadResponse : AbstractResponse
	{
		public string SquadId
		{
			get;
			private set;
		}

		public object SquadData
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("id"))
			{
				this.SquadId = Convert.ToString(dictionary["id"]);
				this.SquadData = obj;
			}
			return this;
		}
	}
}
