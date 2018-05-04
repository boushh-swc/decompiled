using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class SquadWarAttackPlayerStartRequest : PlayerIdChecksumRequest
	{
		private string opponentId;

		public SquadWarAttackPlayerStartRequest(string opponentId)
		{
			this.opponentId = opponentId;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("opponentId", this.opponentId);
			startedSerializer.AddString("cmsVersion", Service.ContentManager.GetFileVersion("patches/base.json").ToString());
			startedSerializer.AddString("battleVersion", "30.0");
			startedSerializer.AddString("simSeedA", Service.BattleController.SimSeed.SimSeedA.ToString());
			startedSerializer.AddString("simSeedB", Service.BattleController.SimSeed.SimSeedB.ToString());
			return startedSerializer.End().ToString();
		}
	}
}
