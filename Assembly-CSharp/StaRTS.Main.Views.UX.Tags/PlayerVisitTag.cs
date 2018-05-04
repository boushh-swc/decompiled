using System;

namespace StaRTS.Main.Views.UX.Tags
{
	public class PlayerVisitTag
	{
		public bool IsSquadMate
		{
			get;
			private set;
		}

		public bool IsFriend
		{
			get;
			private set;
		}

		public string TabName
		{
			get;
			private set;
		}

		public string PlayerId
		{
			get;
			private set;
		}

		public PlayerVisitTag(bool isSquadMate, bool isFriend, string tabName, string playerId)
		{
			this.IsSquadMate = isSquadMate;
			this.IsFriend = isFriend;
			this.TabName = tabName;
			this.PlayerId = playerId;
		}
	}
}
