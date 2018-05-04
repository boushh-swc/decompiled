using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands
{
	public class GetEndpointsResponse : AbstractResponse
	{
		public string Event2BiLogging
		{
			get;
			private set;
		}

		public string Event2NoProxyBiLogging
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("event2BiLogging"))
			{
				this.Event2BiLogging = (string)dictionary["event2BiLogging"];
				this.Event2NoProxyBiLogging = (string)dictionary["event2NoProxyBiLogging"];
			}
			return this;
		}
	}
}
