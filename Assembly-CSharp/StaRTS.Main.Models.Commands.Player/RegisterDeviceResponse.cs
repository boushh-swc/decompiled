using StaRTS.Main.Models.Player;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class RegisterDeviceResponse : PlayerResourceResponse
	{
		public override ISerializable FromObject(object obj)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("crystals"))
			{
				base.CrystalsDelta = Convert.ToInt32(dictionary["crystals"]);
				currentPlayer.Inventory.ModifyCrystals(base.CrystalsDelta);
			}
			if (base.CrystalsDelta > 0)
			{
				currentPlayer.IsPushIncentivized = true;
			}
			return this;
		}
	}
}
