using StaRTS.Utils.Json;
using System;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class SquadWarParticipantIdRequest : PlayerIdChecksumRequest
	{
		public string ParticipantId
		{
			get;
			set;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddString("participantId", this.ParticipantId);
			return startedSerializer.End().ToString();
		}
	}
}
