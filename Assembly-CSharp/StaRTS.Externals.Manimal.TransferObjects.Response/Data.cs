using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Externals.Manimal.TransferObjects.Response
{
	public class Data : AbstractResponse
	{
		public uint RequestId
		{
			get;
			protected set;
		}

		public Dictionary<string, object> Messages
		{
			get;
			protected set;
		}

		public object Result
		{
			get;
			protected set;
		}

		public uint Status
		{
			get;
			protected set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.RequestId = Convert.ToUInt32(dictionary["requestId"]);
			this.Messages = (dictionary["messages"] as Dictionary<string, object>);
			this.Result = dictionary["result"];
			this.Status = Convert.ToUInt32(dictionary["status"]);
			return this;
		}
	}
}
