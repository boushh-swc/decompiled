using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class TroopPlacedAction : IBattleAction, ISerializable
	{
		private const string TIME_KEY = "time";

		private const string TROOP_ID_KEY = "troopId";

		private const string BOARD_X_KEY = "boardX";

		private const string BOARD_Z_KEY = "boardZ";

		private const string TEAM_TYPE = "teamType";

		public const string ACTION_ID = "TroopPlaced";

		public uint Time
		{
			get;
			set;
		}

		public string TroopId
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

		public TeamType TeamType
		{
			get;
			set;
		}

		public string ActionId
		{
			get
			{
				return "TroopPlaced";
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Time = Convert.ToUInt32(dictionary["time"]);
			this.TroopId = (dictionary["troopId"] as string);
			this.BoardX = Convert.ToInt32(dictionary["boardX"]);
			this.BoardZ = Convert.ToInt32(dictionary["boardZ"]);
			this.TeamType = StringUtils.ParseEnum<TeamType>(dictionary["teamType"] as string);
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().AddString("actionId", this.ActionId).Add<uint>("time", this.Time).AddString("troopId", this.TroopId).Add<int>("boardX", this.BoardX).Add<int>("boardZ", this.BoardZ).AddString("teamType", this.TeamType.ToString()).End().ToString();
		}
	}
}
