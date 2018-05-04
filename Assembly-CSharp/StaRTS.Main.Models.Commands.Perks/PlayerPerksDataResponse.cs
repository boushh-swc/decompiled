using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerksDataResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			Service.PerkManager.UpdatePlayerPerksData(obj);
			return this;
		}
	}
}
