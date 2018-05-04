using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Models.Player;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Player
{
	public class ExternalCurrencySyncResponse : AbstractResponse
	{
		public object Result
		{
			get;
			protected set;
		}

		public uint Status
		{
			get;
			protected set;
		}

		public List<Data> DataList
		{
			get;
			protected set;
		}

		protected virtual void LogResults(KeyValuePair<string, object> entry, int diff)
		{
		}

		public override ISerializable FromObject(object obj)
		{
			CurrentPlayer currentPlayer = Service.CurrentPlayer;
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (!dictionary.ContainsKey("playerModel"))
			{
				return this;
			}
			Dictionary<string, object> dictionary2 = dictionary["playerModel"] as Dictionary<string, object>;
			if (!dictionary2.ContainsKey("inventory"))
			{
				return this;
			}
			Dictionary<string, object> dictionary3 = dictionary2["inventory"] as Dictionary<string, object>;
			if (!dictionary3.ContainsKey("storage"))
			{
				return this;
			}
			Dictionary<string, object> dictionary4 = dictionary3["storage"] as Dictionary<string, object>;
			InventoryEntry inventoryEntry = new InventoryEntry();
			foreach (KeyValuePair<string, object> current in dictionary4)
			{
				inventoryEntry.FromObject(current.Value);
				int itemAmount = currentPlayer.Inventory.GetItemAmount(current.Key);
				int amount = inventoryEntry.Amount;
				int num = amount - itemAmount;
				Service.Logger.Debug(current.Key + ":" + num);
				if (num != 0)
				{
					currentPlayer.Inventory.ModifyItemAmount(current.Key, num);
				}
				this.LogResults(current, num);
			}
			return this;
		}
	}
}
