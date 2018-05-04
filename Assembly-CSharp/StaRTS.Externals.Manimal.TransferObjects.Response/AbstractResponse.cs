using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Response
{
	public abstract class AbstractResponse : ISerializable
	{
		public abstract ISerializable FromObject(object obj);

		public string ToJson()
		{
			return string.Empty;
		}
	}
}
