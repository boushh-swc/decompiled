using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models
{
	public class OperationProgress : ISerializable
	{
		public string Uid;

		public string ToJson()
		{
			return string.Empty;
		}

		public ISerializable FromObject(object obj)
		{
			return this;
		}
	}
}
