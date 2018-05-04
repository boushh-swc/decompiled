using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Pvp
{
	public class PvpBattleStartRequest : PlayerIdChecksumRequest
	{
		private string planetId;

		public string TargetId
		{
			get;
			private set;
		}

		public string CmsVersion
		{
			get;
			private set;
		}

		public string PvpMissionUid
		{
			get;
			private set;
		}

		public PvpBattleStartRequest(string targetId, string cmsVersion, string pvpMissionUid)
		{
			this.TargetId = targetId;
			this.CmsVersion = cmsVersion;
			this.PvpMissionUid = pvpMissionUid;
			this.planetId = Service.CurrentPlayer.PlanetId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("targetId", this.TargetId);
			startedSerializer.AddString("cmsVersion", this.CmsVersion);
			startedSerializer.AddString("battleVersion", "30.0");
			startedSerializer.AddString("simSeedA", Service.BattleController.SimSeed.SimSeedA.ToString());
			startedSerializer.AddString("simSeedB", Service.BattleController.SimSeed.SimSeedB.ToString());
			startedSerializer.AddString("planetId", this.planetId);
			if (this.PvpMissionUid != null)
			{
				startedSerializer.AddString("missionUid", this.PvpMissionUid);
			}
			return startedSerializer.End().ToString();
		}
	}
}
