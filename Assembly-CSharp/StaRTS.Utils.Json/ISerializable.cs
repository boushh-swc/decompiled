using System;

namespace StaRTS.Utils.Json
{
	public interface ISerializable
	{
		string ToJson();

		ISerializable FromObject(object obj);
	}
}
