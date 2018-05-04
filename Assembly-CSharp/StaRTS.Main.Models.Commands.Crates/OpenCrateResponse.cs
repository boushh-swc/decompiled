using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class OpenCrateResponse : AbstractResponse
	{
		public List<string> SupplyIDs
		{
			get;
			private set;
		}

		public OpenCrateResponse()
		{
			this.SupplyIDs = new List<string>();
		}

		public override ISerializable FromObject(object obj)
		{
			List<object> list = obj as List<object>;
			if (list != null)
			{
				this.SupplyIDs.Clear();
				int count = list.Count;
				for (int i = 0; i < count; i++)
				{
					this.SupplyIDs.Add(Convert.ToString(list[i]));
				}
			}
			return this;
		}
	}
}
