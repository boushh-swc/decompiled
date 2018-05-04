using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public abstract class AbstractRequest : ISerializable
	{
		public abstract string ToJson();

		public ISerializable FromObject(object obj)
		{
			return null;
		}
	}
}
