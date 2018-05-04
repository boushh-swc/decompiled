using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models
{
	public class Deployable : ISerializable
	{
		private const string AMOUNT_KEY = "amount";

		private const string UID_KEY = "uid";

		public int Amount
		{
			get;
			set;
		}

		public string Uid
		{
			get;
			set;
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Amount = Convert.ToInt32(dictionary["amount"]);
			this.Uid = (dictionary["uid"] as string);
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().Add<int>("amount", this.Amount).AddString("uid", this.Uid).End().ToString();
		}
	}
}
