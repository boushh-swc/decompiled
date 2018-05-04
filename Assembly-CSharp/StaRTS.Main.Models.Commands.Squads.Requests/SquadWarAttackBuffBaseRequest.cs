using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class SquadWarAttackBuffBaseRequest : PlayerIdChecksumRequest
	{
		private string buffBaseUid;

		public SquadWarAttackBuffBaseRequest(string buffBaseUid)
		{
			this.buffBaseUid = buffBaseUid;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("buffBaseUid", this.buffBaseUid);
			startedSerializer.AddString("cmsVersion", Service.ContentManager.GetFileVersion("patches/base.json").ToString());
			startedSerializer.AddString("battleVersion", "30.0");
			return startedSerializer.End().ToString();
		}
	}
}
