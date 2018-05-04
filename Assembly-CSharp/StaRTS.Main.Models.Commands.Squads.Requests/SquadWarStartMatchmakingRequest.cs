using StaRTS.Main.Models.Squads;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Requests
{
	public class SquadWarStartMatchmakingRequest : PlayerIdChecksumRequest
	{
		public List<string> Participants;

		public bool AllowSameFactionMatchMaking;

		public SquadWarStartMatchmakingRequest(SqmWarParticipantData participantData)
		{
			this.Participants = new List<string>();
			int i = 0;
			int count = participantData.Participants.Count;
			while (i < count)
			{
				this.Participants.Add(participantData.Participants[i]);
				i++;
			}
			this.AllowSameFactionMatchMaking = participantData.AllowSameFactionMatchMaking;
		}

		public override string ToJson()
		{
			Serializer startedSerializer = base.GetStartedSerializer();
			startedSerializer.AddArrayOfPrimitives<string>("participantIds", this.Participants);
			startedSerializer.AddBool("isSameFactionWarAllowed", this.AllowSameFactionMatchMaking);
			return startedSerializer.End().ToString();
		}
	}
}
