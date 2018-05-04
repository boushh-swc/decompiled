using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class SupplyData : ISerializable
	{
		public string SupplyId
		{
			get;
			private set;
		}

		public string SupplyPoolId
		{
			get;
			private set;
		}

		public string ToJson()
		{
			return "{}";
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("supplyId"))
				{
					this.SupplyId = Convert.ToString(dictionary["supplyId"]);
				}
				if (dictionary.ContainsKey("supplyPoolId"))
				{
					this.SupplyPoolId = Convert.ToString(dictionary["supplyPoolId"]);
				}
			}
			return this;
		}
	}
}
