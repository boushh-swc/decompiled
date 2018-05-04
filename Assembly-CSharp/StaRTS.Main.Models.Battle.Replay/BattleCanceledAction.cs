using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class BattleCanceledAction : IBattleAction, ISerializable
	{
		private const string TIME_KEY = "time";

		public const string ACTION_ID = "BattleCanceled";

		public uint Time
		{
			get;
			set;
		}

		public string ActionId
		{
			get
			{
				return "BattleCanceled";
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Time = Convert.ToUInt32(dictionary["time"]);
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().AddString("actionId", this.ActionId).Add<uint>("time", this.Time).End().ToString();
		}
	}
}
