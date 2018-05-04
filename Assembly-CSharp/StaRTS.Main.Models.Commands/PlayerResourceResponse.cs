using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands
{
	public class PlayerResourceResponse : AbstractResponse
	{
		public int CrystalsDelta
		{
			get;
			protected set;
		}

		public override ISerializable FromObject(object obj)
		{
			return this;
		}
	}
}
