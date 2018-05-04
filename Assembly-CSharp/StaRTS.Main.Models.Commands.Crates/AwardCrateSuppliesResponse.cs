using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class AwardCrateSuppliesResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			currentPlayer.Prizes.Crates.FromObject(obj);
			Service.EventManager.SendEvent(EventId.InventoryCrateOpenedAndGranted, null);
			return this;
		}
	}
}
