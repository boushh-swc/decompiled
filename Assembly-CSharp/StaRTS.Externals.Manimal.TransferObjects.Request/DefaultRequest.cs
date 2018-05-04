using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public class DefaultRequest : AbstractRequest
	{
		public override string ToJson()
		{
			return Serializer.Start().End().ToString();
		}
	}
}
