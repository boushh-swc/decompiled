using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class AddPatchRequest : PlayerIdRequest
	{
		public string Patch
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			if (!string.IsNullOrEmpty(this.Patch))
			{
				serializer.AddString("patch", this.Patch);
			}
			return serializer.End().ToString();
		}
	}
}
