using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Cheats
{
	public class CheatSetTroopDonateRepRequest : PlayerIdRequest
	{
		public int TroopsDonated
		{
			get;
			set;
		}

		public CheatSetTroopDonateRepRequest(int troopsDonated)
		{
			base.PlayerId = Service.CurrentPlayer.PlayerId;
			this.TroopsDonated = troopsDonated;
		}

		public override string ToJson()
		{
			Serializer serializer = Serializer.Start();
			serializer.AddString("playerId", base.PlayerId);
			serializer.Add<int>("donationCount", this.TroopsDonated);
			return serializer.End().ToString();
		}
	}
}
