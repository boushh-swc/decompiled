using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Battle.Replay
{
	public class SpecialAttackDeployedAction : IBattleAction, ISerializable
	{
		private const string TIME_KEY = "time";

		private const string SPECIAL_ATTACK_ID_KEY = "specialAttackId";

		private const string BOARD_X_KEY = "boardX";

		private const string BOARD_Z_KEY = "boardZ";

		private const string TEAM_TYPE = "teamType";

		public const string ACTION_ID = "SpecialAttackDeployed";

		public uint Time
		{
			get;
			set;
		}

		public string SpecialAttackId
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
				return "SpecialAttackDeployed";
			}
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			this.Time = Convert.ToUInt32(dictionary["time"]);
			this.SpecialAttackId = (dictionary["specialAttackId"] as string);
			this.BoardX = Convert.ToInt32(dictionary["boardX"]);
			this.BoardZ = Convert.ToInt32(dictionary["boardZ"]);
			this.TeamType = StringUtils.ParseEnum<TeamType>(dictionary["teamType"] as string);
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().AddString("actionId", this.ActionId).Add<uint>("time", this.Time).AddString("specialAttackId", this.SpecialAttackId).Add<int>("boardX", this.BoardX).Add<int>("boardZ", this.BoardZ).AddString("teamType", this.TeamType.ToString()).End().ToString();
		}
	}
}
