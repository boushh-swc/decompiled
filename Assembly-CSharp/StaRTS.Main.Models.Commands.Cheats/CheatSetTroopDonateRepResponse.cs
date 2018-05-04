using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetTroopDonateRepResponse : AbstractResponse
	{
		public override ISerializable FromObject(object rawDonateData)
		{
			Service.CurrentPlayer.SetTroopDonationProgress(rawDonateData);
			return this;
		}
	}
}
