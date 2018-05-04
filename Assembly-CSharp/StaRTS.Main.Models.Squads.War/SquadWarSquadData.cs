using StaRTS.Utils;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.Squads.War
{
	public class SquadWarSquadData : ISerializable
	{
		public string SquadId;

		public string SquadName;

		public FactionType Faction;

		public List<SquadWarParticipantState> Participants;

		public SquadWarSquadData()
		{
			this.Participants = new List<SquadWarParticipantState>();
		}

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("guildId"))
				{
					this.SquadId = (dictionary["guildId"] as string);
				}
				if (dictionary.ContainsKey("name"))
				{
					this.SquadName = WWW.UnEscapeURL(Convert.ToString(dictionary["name"]));
				}
				if (dictionary.ContainsKey("faction"))
				{
					this.Faction = StringUtils.ParseEnum<FactionType>(dictionary["faction"] as string);
				}
				if (dictionary.ContainsKey("participants"))
				{
					List<object> list = dictionary["participants"] as List<object>;
					if (list != null)
					{
						foreach (object current in list)
						{
							SquadWarParticipantState squadWarParticipantState = new SquadWarParticipantState();
							squadWarParticipantState.FromObject(current);
							this.Participants.Add(squadWarParticipantState);
						}
					}
				}
			}
			return this;
		}

		public string ToJson()
		{
			return Serializer.Start().End().ToString();
		}
	}
}
