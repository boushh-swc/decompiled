using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class GetSquadWarStatusResponse : AbstractResponse
	{
		public string Id
		{
			get;
			private set;
		}

		public object Squad1Data
		{
			get;
			private set;
		}

		public object Squad2Data
		{
			get;
			private set;
		}

		public object BuffBaseData
		{
			get;
			private set;
		}

		public int PrepGraceStartTimeStamp
		{
			get;
			private set;
		}

		public int PrepEndTimeStamp
		{
			get;
			private set;
		}

		public int ActionGraceStartTimeStamp
		{
			get;
			private set;
		}

		public int ActionEndTimeStamp
		{
			get;
			private set;
		}

		public int CooldownEndTimeStamp
		{
			get;
			private set;
		}

		public int StartTimeStamp
		{
			get;
			private set;
		}

		public bool RewardsProcessed
		{
			get;
			private set;
		}

		public override ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary == null)
			{
				return this;
			}
			if (dictionary.ContainsKey("id"))
			{
				this.Id = Convert.ToString(dictionary["id"]);
			}
			if (dictionary.ContainsKey("guild"))
			{
				this.Squad1Data = dictionary["guild"];
			}
			if (dictionary.ContainsKey("rival"))
			{
				this.Squad2Data = dictionary["rival"];
			}
			if (dictionary.ContainsKey("buffBases"))
			{
				this.BuffBaseData = dictionary["buffBases"];
			}
			if (dictionary.ContainsKey("prepGraceStartTime"))
			{
				this.PrepGraceStartTimeStamp = Convert.ToInt32(dictionary["prepGraceStartTime"]);
			}
			if (dictionary.ContainsKey("prepEndTime"))
			{
				this.PrepEndTimeStamp = Convert.ToInt32(dictionary["prepEndTime"]);
			}
			if (dictionary.ContainsKey("actionGraceStartTime"))
			{
				this.ActionGraceStartTimeStamp = Convert.ToInt32(dictionary["actionGraceStartTime"]);
			}
			if (dictionary.ContainsKey("actionEndTime"))
			{
				this.ActionEndTimeStamp = Convert.ToInt32(dictionary["actionEndTime"]);
			}
			if (dictionary.ContainsKey("cooldownEndTime"))
			{
				this.CooldownEndTimeStamp = Convert.ToInt32(dictionary["cooldownEndTime"]);
			}
			if (dictionary.ContainsKey("startTime"))
			{
				this.StartTimeStamp = Convert.ToInt32(dictionary["startTime"]);
			}
			else
			{
				this.StartTimeStamp = this.PrepEndTimeStamp;
			}
			if (dictionary.ContainsKey("rewardsProcessed"))
			{
				this.RewardsProcessed = Convert.ToBoolean(dictionary["rewardsProcessed"]);
			}
			return this;
		}
	}
}
