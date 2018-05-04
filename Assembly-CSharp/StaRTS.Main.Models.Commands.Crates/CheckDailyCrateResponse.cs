using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Crates
{
	public class CheckDailyCrateResponse : AbstractResponse
	{
		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("crates"))
			{
				InventoryCrates crates = Service.CurrentPlayer.Prizes.Crates;
				crates.UpdateAndBadgeFromServerObject(dictionary["crates"]);
			}
			return this;
		}
	}
}
