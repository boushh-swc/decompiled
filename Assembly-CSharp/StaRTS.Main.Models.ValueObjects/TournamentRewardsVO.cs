using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class TournamentRewardsVO : IValueObject
	{
		public static int COLUMN_tournamentRewardsId
		{
			get;
			private set;
		}

		public static int COLUMN_tournamentTier
		{
			get;
			private set;
		}

		public static int COLUMN_crateRewardUid
		{
			get;
			private set;
		}

		public static int COLUMN_rebelGuaranteedReward
		{
			get;
			private set;
		}

		public static int COLUMN_empireGuaranteedReward
		{
			get;
			private set;
		}

		public static int COLUMN_rebelGuaranteedRewardLabel
		{
			get;
			private set;
		}

		public static int COLUMN_empireGuaranteedRewardLabel
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string TournamentRewardsId
		{
			get;
			set;
		}

		public string TournamentTier
		{
			get;
			set;
		}

		public string[] CrateRewardIds
		{
			get;
			set;
		}

		public string RebelGuaranteedReward
		{
			get;
			set;
		}

		public string EmpireGuaranteedReward
		{
			get;
			set;
		}

		public string RebelGuaranteedRewardLabel
		{
			get;
			set;
		}

		public string EmpireGuaranteedRewardlabel
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TournamentRewardsId = row.TryGetString(TournamentRewardsVO.COLUMN_tournamentRewardsId);
			this.TournamentTier = row.TryGetString(TournamentRewardsVO.COLUMN_tournamentTier);
			this.CrateRewardIds = row.TryGetStringArray(TournamentRewardsVO.COLUMN_crateRewardUid);
			this.RebelGuaranteedReward = row.TryGetString(TournamentRewardsVO.COLUMN_rebelGuaranteedReward);
			this.EmpireGuaranteedReward = row.TryGetString(TournamentRewardsVO.COLUMN_empireGuaranteedReward);
			this.RebelGuaranteedRewardLabel = row.TryGetString(TournamentRewardsVO.COLUMN_rebelGuaranteedRewardLabel);
			this.EmpireGuaranteedRewardlabel = row.TryGetString(TournamentRewardsVO.COLUMN_empireGuaranteedRewardLabel);
		}
	}
}
