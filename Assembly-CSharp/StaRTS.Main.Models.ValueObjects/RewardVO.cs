using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class RewardVO : IValueObject
	{
		public static int COLUMN_currencyRewards
		{
			get;
			private set;
		}

		public static int COLUMN_shardRewards
		{
			get;
			private set;
		}

		public static int COLUMN_buildingUnlocks
		{
			get;
			private set;
		}

		public static int COLUMN_troopRewards
		{
			get;
			private set;
		}

		public static int COLUMN_troopUnlocks
		{
			get;
			private set;
		}

		public static int COLUMN_specAttackRewards
		{
			get;
			private set;
		}

		public static int COLUMN_specAttackUnlocks
		{
			get;
			private set;
		}

		public static int COLUMN_heroRewards
		{
			get;
			private set;
		}

		public static int COLUMN_heroUnlocks
		{
			get;
			private set;
		}

		public static int COLUMN_droids
		{
			get;
			private set;
		}

		public static int COLUMN_protectionReward
		{
			get;
			private set;
		}

		public static int COLUMN_buildingInstantRewards
		{
			get;
			private set;
		}

		public static int COLUMN_buildingInstantUpgrades
		{
			get;
			private set;
		}

		public static int COLUMN_troopResearchInstantUpgrades
		{
			get;
			private set;
		}

		public static int COLUMN_heroResearchInstantUpgrades
		{
			get;
			private set;
		}

		public static int COLUMN_specAttackResearchInstantUpgrades
		{
			get;
			private set;
		}

		public static int COLUMN_crateReward
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] CurrencyRewards
		{
			get;
			set;
		}

		public string[] ShardRewards
		{
			get;
			set;
		}

		public string[] BuildingUnlocks
		{
			get;
			set;
		}

		public string[] TroopRewards
		{
			get;
			set;
		}

		public string[] TroopUnlocks
		{
			get;
			set;
		}

		public string[] SpecialAttackRewards
		{
			get;
			set;
		}

		public string[] SpecialAttackUnlocks
		{
			get;
			set;
		}

		public string[] HeroRewards
		{
			get;
			set;
		}

		public string[] HeroUnlocks
		{
			get;
			set;
		}

		public string[] BuildingInstantRewards
		{
			get;
			set;
		}

		public string[] BuildingInstantUpgrades
		{
			get;
			set;
		}

		public string[] TroopResearchInstantUpgrades
		{
			get;
			set;
		}

		public string[] HeroResearchInstantUpgrades
		{
			get;
			set;
		}

		public string[] SpecAttackResearchInstantUpgrades
		{
			get;
			set;
		}

		public string DroidRewards
		{
			get;
			set;
		}

		public string ProtectionRewards
		{
			get;
			set;
		}

		public string CrateReward
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.CurrencyRewards = row.TryGetStringArray(RewardVO.COLUMN_currencyRewards);
			this.ShardRewards = row.TryGetStringArray(RewardVO.COLUMN_shardRewards);
			this.BuildingUnlocks = row.TryGetStringArray(RewardVO.COLUMN_buildingUnlocks);
			this.TroopRewards = row.TryGetStringArray(RewardVO.COLUMN_troopRewards);
			this.TroopUnlocks = row.TryGetStringArray(RewardVO.COLUMN_troopUnlocks);
			this.SpecialAttackRewards = row.TryGetStringArray(RewardVO.COLUMN_specAttackRewards);
			this.SpecialAttackUnlocks = row.TryGetStringArray(RewardVO.COLUMN_specAttackUnlocks);
			this.HeroRewards = row.TryGetStringArray(RewardVO.COLUMN_heroRewards);
			this.HeroUnlocks = row.TryGetStringArray(RewardVO.COLUMN_heroUnlocks);
			this.DroidRewards = row.TryGetString(RewardVO.COLUMN_droids);
			this.ProtectionRewards = row.TryGetString(RewardVO.COLUMN_protectionReward);
			this.BuildingInstantRewards = row.TryGetStringArray(RewardVO.COLUMN_buildingInstantRewards);
			this.BuildingInstantUpgrades = row.TryGetStringArray(RewardVO.COLUMN_buildingInstantUpgrades);
			this.TroopResearchInstantUpgrades = row.TryGetStringArray(RewardVO.COLUMN_troopResearchInstantUpgrades);
			this.HeroResearchInstantUpgrades = row.TryGetStringArray(RewardVO.COLUMN_heroResearchInstantUpgrades);
			this.SpecAttackResearchInstantUpgrades = row.TryGetStringArray(RewardVO.COLUMN_specAttackResearchInstantUpgrades);
			this.CrateReward = row.TryGetString(RewardVO.COLUMN_crateReward);
		}
	}
}
