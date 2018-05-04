using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class HeroAbilityAction : IBattleAction, ISerializable
	{
		private const string TIME_KEY = "time";

		private const string TROOP_ID_KEY = "troopId";

		public const string ACTION_ID = "HeroAbilityActivated";

		public uint Time
		{
			get;
			set;
		}

		public string TroopUid
		{
			get;
			set;
		}

		public string ActionId
		{
			get
			{
				return "HeroAbilityActivated";
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Time = Convert.ToUInt32(dictionary["time"]);
			this.TroopUid = (dictionary["troopId"] as string);
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().AddString("actionId", this.ActionId).Add<uint>("time", this.Time).AddString("troopId", this.TroopUid).End().ToString();
		}
	}
}
