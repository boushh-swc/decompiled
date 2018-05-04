using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.Leaderboard
{
	public class LeaderboardList<T>
	{
		public List<T> List
		{
			get;
			private set;
		}

		public bool AlwaysRefresh
		{
			get;
			set;
		}

		public uint LastRefreshTime
		{
			get;
			set;
		}

		public LeaderboardList()
		{
			this.List = new List<T>();
			this.AlwaysRefresh = false;
			this.LastRefreshTime = 0u;
		}
	}
}
