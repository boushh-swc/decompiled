using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads.War
{
	public class SquadWarData
	{
		public string WarId;

		public SquadWarSquadData[] Squads;

		public List<SquadWarBuffBaseData> BuffBases;

		public int PrepGraceStartTimeStamp;

		public int PrepEndTimeStamp;

		public int ActionGraceStartTimeStamp;

		public int ActionEndTimeStamp;

		public int CooldownEndTimeStamp;

		public int StartTimeStamp;

		public bool RewardsProcessed;

		public SquadWarData()
		{
			this.BuffBases = new List<SquadWarBuffBaseData>();
			this.Squads = new SquadWarSquadData[2];
			this.PrepGraceStartTimeStamp = 0;
			this.PrepEndTimeStamp = 0;
			this.ActionGraceStartTimeStamp = 0;
			this.ActionEndTimeStamp = 0;
			this.CooldownEndTimeStamp = 0;
			this.StartTimeStamp = 0;
			this.RewardsProcessed = false;
		}
	}
}
