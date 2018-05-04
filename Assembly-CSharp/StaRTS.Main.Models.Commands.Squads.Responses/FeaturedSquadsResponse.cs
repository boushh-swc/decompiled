using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class FeaturedSquadsResponse : AbstractResponse
	{
		public List<object> SquadData
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			this.SquadData = new List<object>();
			List<object> list = obj as List<object>;
			if (list == null)
			{
				return this;
			}
			int i = 0;
			int count = list.Count;
			while (i < count)
			{
				this.SquadData.Add(list[i]);
				i++;
			}
			return this;
		}
	}
}
