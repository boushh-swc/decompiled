using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CrateSupplyVO : IValueObject
	{
		public static int COLUMN_crateSupplyPoolUid
		{
			get;
			private set;
		}

		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_planet
		{
			get;
			private set;
		}

		public static int COLUMN_feature
		{
			get;
			private set;
		}

		public static int COLUMN_minHQ
		{
			get;
			private set;
		}

		public static int COLUMN_maxHQ
		{
			get;
			private set;
		}

		public static int COLUMN_rewardType
		{
			get;
			private set;
		}

		public static int COLUMN_rewardUid
		{
			get;
			private set;
		}

		public static int COLUMN_amount
		{
			get;
			private set;
		}

		public static int COLUMN_scalingUid
		{
			get;
			private set;
		}

		public static int COLUMN_dataCard
		{
			get;
			private set;
		}

		public static int COLUMN_rewardTierSfx
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] CrateSupplyPoolUid
		{
			get;
			private set;
		}

		public FactionType Faction
		{
			get;
			private set;
		}

		public string[] PlanetIds
		{
			get;
			private set;
		}

		public FeatureContextType[] FeatureContexts
		{
			get;
			private set;
		}

		public int MinHQLevel
		{
			get;
			private set;
		}

		public int MaxHQLevel
		{
			get;
			private set;
		}

		public SupplyType Type
		{
			get;
			private set;
		}

		public string RewardUid
		{
			get;
			private set;
		}

		public int Amount
		{
			get;
			private set;
		}

		public string ScalingUid
		{
			get;
			private set;
		}

		public string DataCardId
		{
			get;
			private set;
		}

		public string RewardAnimationTierSfx
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.CrateSupplyPoolUid = row.TryGetStringArray(CrateSupplyVO.COLUMN_crateSupplyPoolUid);
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(CrateSupplyVO.COLUMN_faction));
			this.PlanetIds = row.TryGetStringArray(CrateSupplyVO.COLUMN_planet);
			string[] array = row.TryGetStringArray(CrateSupplyVO.COLUMN_feature);
			if (array != null)
			{
				int num = array.Length;
				this.FeatureContexts = new FeatureContextType[num];
				for (int i = 0; i < num; i++)
				{
					this.FeatureContexts[i] = StringUtils.ParseEnum<FeatureContextType>(array[i]);
				}
			}
			this.MinHQLevel = row.TryGetInt(CrateSupplyVO.COLUMN_minHQ);
			this.MaxHQLevel = row.TryGetInt(CrateSupplyVO.COLUMN_maxHQ);
			this.Type = StringUtils.ParseEnum<SupplyType>(row.TryGetString(CrateSupplyVO.COLUMN_rewardType));
			this.RewardUid = row.TryGetString(CrateSupplyVO.COLUMN_rewardUid);
			this.Amount = row.TryGetInt(CrateSupplyVO.COLUMN_amount);
			this.ScalingUid = row.TryGetString(CrateSupplyVO.COLUMN_scalingUid);
			this.DataCardId = row.TryGetString(CrateSupplyVO.COLUMN_dataCard);
			this.RewardAnimationTierSfx = row.TryGetString(CrateSupplyVO.COLUMN_rewardTierSfx);
		}
	}
}
