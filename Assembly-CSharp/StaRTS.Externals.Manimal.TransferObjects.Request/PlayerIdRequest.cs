using StaRTS.Utils.Json;
using System;

namespace StaRTS.Externals.Manimal.TransferObjects.Request
{
	public class PlayerIdRequest : AbstractRequest
	{
		public string PlayerId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", this.PlayerId);
			return serializer.End().ToString();
		}
	}
}
