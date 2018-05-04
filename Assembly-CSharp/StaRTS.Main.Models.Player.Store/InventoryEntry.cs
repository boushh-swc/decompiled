using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace StaRTS.Main.Models.Player.Store
{
	public class InventoryEntry : ISerializable
	{
		public const int UNINITIALIZED_SCALE = -1;

		public int Amount;

		public int Capacity;

		public int Scale = -1;

		public string ToJson()
		{
			return Serializer.Start().Add<int>("amount", this.Amount).Add<int>("capacity", this.Capacity).Add<int>("scale", 0).End().ToString();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Amount = Convert.ToInt32(dictionary["amount"]);
			this.Capacity = Convert.ToInt32(dictionary["capacity"]);
			return this;
		}

		public void AddString(StringBuilder sb, bool skipScale)
		{
			sb.Append(this.Amount).Append("|").Append(this.Capacity).Append("|").Append((!skipScale) ? this.Scale.ToString() : string.Empty).Append("\n");
		}
	}
}
