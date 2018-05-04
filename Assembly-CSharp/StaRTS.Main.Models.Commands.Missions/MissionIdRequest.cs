using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Missions
{
	public class MissionIdRequest : PlayerIdChecksumRequest
	{
		public string MissionUid
		{
			get;
			private set;
		}

		public string BattleUid
		{
			get;
			private set;
		}

		public MissionIdRequest(string missionUid)
		{
			this.MissionUid = missionUid;
		}

		public MissionIdRequest(string missionUid, string battleUid)
		{
			this.MissionUid = missionUid;
			this.BattleUid = battleUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("missionUid", this.MissionUid);
			if (this.BattleUid != null)
			{
				startedSerializer.AddString("battleUid", this.BattleUid);
			}
			return startedSerializer.End().ToString();
		}
	}
}
