using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Response
{
	public class DefaultResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			return this;
		}
	}
}
