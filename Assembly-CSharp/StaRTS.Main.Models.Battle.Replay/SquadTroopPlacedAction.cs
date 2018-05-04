using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class SquadTroopPlacedAction : IBattleAction, ISerializable
	{
		private const string TIME_KEY = "time";

		private const string BOARD_X_KEY = "boardX";

		private const string BOARD_Z_KEY = "boardZ";

		public const string ACTION_ID = "SquadTroopPlaced";

		public uint Time
		{
			get;
			set;
		}

		public int BoardX
		{
			get;
			set;
		}

		public int BoardZ
		{
			get;
			set;
		}

		public string ActionId
		{
			get
			{
				return "SquadTroopPlaced";
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Time = Convert.ToUInt32(dictionary["time"]);
			this.BoardX = Convert.ToInt32(dictionary["boardX"]);
			this.BoardZ = Convert.ToInt32(dictionary["boardZ"]);
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().AddString("actionId", this.ActionId).Add<uint>("time", this.Time).Add<int>("boardX", this.BoardX).Add<int>("boardZ", this.BoardZ).End().ToString();
		}
	}
}
