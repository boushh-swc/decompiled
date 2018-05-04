using StaRTS.Externals.Manimal.TransferObjects.Response;
using StaRTS.Main.Utils.Events;
using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Commands.Squads.Responses
{
	public class LeaderboardResponse : AbstractResponse
	{
		public List<object> TopData
		{
			get;
			private set;
		}

		public List<object> SurroundingData
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
			if (dictionary.ContainsKey("leaders"))
			{
				this.TopData = (dictionary["leaders"] as List<object>);
			}
			if (dictionary.ContainsKey("surrounding"))
			{
				this.SurroundingData = (dictionary["surrounding"] as List<object>);
			}
			Service.EventManager.SendEvent(EventId.SquadLeaderboardUpdated, null);
			return this;
		}
	}
}
