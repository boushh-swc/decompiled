using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads
{
	public class SqmWarParticipantData
	{
		public List<string> Participants;

		public bool AllowSameFactionMatchMaking;

		public SqmWarParticipantData()
		{
			this.Participants = new List<string>();
		}
	}
}
