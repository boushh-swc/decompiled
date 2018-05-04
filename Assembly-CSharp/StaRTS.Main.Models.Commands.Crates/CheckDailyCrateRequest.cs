using StaRTS.Externals.Manimal.TransferObjects.Request;
using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class CheckDailyCrateRequest : PlayerIdRequest
	{
		public CheckDailyCrateRequest()
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			base.PlayerId = currentPlayer.PlayerId;
		}
	}
}
