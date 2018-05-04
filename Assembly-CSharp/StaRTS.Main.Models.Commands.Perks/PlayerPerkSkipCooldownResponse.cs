using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Perks
{
	public class PlayerPerkSkipCooldownResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			Service.PerkManager.PurchaseCooldownSkipResponse(obj);
			return this;
		}
	}
}
