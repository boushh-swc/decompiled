using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Externals.Manimal.TransferObjects.Response
{
	public class Response : AbstractResponse
	{
		public uint ProtocolVersion
		{
			get;
			protected set;
		}

		public string ServerTime
		{
			get;
			protected set;
		}

		public uint ServerTimestamp
		{
			get;
			protected set;
		}

		public List<Data> DataList
		{
			get;
			protected set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.ProtocolVersion = Convert.ToUInt32(dictionary["protocolVersion"]);
			this.ServerTime = Convert.ToString(dictionary["serverTime"]);
			this.ServerTimestamp = Convert.ToUInt32(dictionary["serverTimestamp"]);
			List<object> list = dictionary["data"] as List<object>;
			this.DataList = new List<Data>();
			for (int i = 0; i < list.Count; i++)
			{
				this.DataList.Add(new Data().FromObject(list[i]) as Data);
			}
			return this;
		}
	}
}
