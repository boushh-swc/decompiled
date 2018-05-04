using StaRTS.Utils.Core;
using StaRTS.Utils.Json;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Squads.War
{
	public class SquadWarParticipantState : ISerializable
	{
		public string SquadMemberName;

		public string SquadMemberId;

		public int TurnsLeft;

		public int Score;

		public int VictoryPointsLeft;

		public int HQLevel = 1;

		public int AttacksWon;

		public int DefensesWon;

		public uint DefendingAttackExpirationDate;

		public ISerializable FromObject(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary.ContainsKey("id"))
			{
				this.SquadMemberId = Convert.ToString(dictionary["id"]);
			}
			if (dictionary.ContainsKey("name"))
			{
				this.SquadMemberName = Convert.ToString(dictionary["name"]);
			}
			if (dictionary.ContainsKey("turns"))
			{
				this.TurnsLeft = Convert.ToInt32(dictionary["turns"]);
			}
			if (dictionary.ContainsKey("victoryPoints"))
			{
				this.VictoryPointsLeft = Convert.ToInt32(dictionary["victoryPoints"]);
			}
			if (dictionary.ContainsKey("attacksWon"))
			{
				this.AttacksWon = Convert.ToInt32(dictionary["attacksWon"]);
			}
			if (dictionary.ContainsKey("defensesWon"))
			{
				this.DefensesWon = Convert.ToInt32(dictionary["defensesWon"]);
			}
			if (dictionary.ContainsKey("score"))
			{
				this.Score = Convert.ToInt32(dictionary["score"]);
			}
			if (dictionary.ContainsKey("currentlyDefending"))
			{
				Dictionary<string, object> dictionary2 = dictionary["currentlyDefending"] as Dictionary<string, object>;
				if (dictionary2 != null && dictionary2.ContainsKey("expiration"))
				{
					this.DefendingAttackExpirationDate = Convert.ToUInt32(dictionary2["expiration"]);
				}
			}
			if (dictionary.ContainsKey("level"))
			{
				this.HQLevel = Convert.ToInt32(dictionary["level"]);
				if (this.HQLevel <= 0)
				{
					this.HQLevel = 1;
					Service.Logger.WarnFormat("War participant has no HQ level: {0}", new object[]
					{
						this.SquadMemberId
					});
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
