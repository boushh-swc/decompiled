using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Leaderboard
{
	public class LeaderboardBattleHistory
	{
		public int AttacksWon
		{
			get;
			private set;
		}

		public int DefensesWon
		{
			get;
			private set;
		}

		public LeaderboardBattleHistory(object obj)
		{
			Dictionary<string, object> dictionary = obj as Dictionary<string, object>;
			if (dictionary != null)
			{
				if (dictionary.ContainsKey("attacksWon"))
				{
					this.AttacksWon = Convert.ToInt32(dictionary["attacksWon"]);
				}
				if (dictionary.ContainsKey("defensesWon"))
				{
					this.DefensesWon = Convert.ToInt32(dictionary["defensesWon"]);
				}
			}
		}
	}
}
