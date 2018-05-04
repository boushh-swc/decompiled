using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ObjectiveVO : IValueObject
	{
		public static int COLUMN_testSet
		{
			get;
			private set;
		}

		public static int COLUMN_objBucket
		{
			get;
			private set;
		}

		public static int COLUMN_faction
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

		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_item
		{
			get;
			private set;
		}

		public static int COLUMN_objString
		{
			get;
			private set;
		}

		public static int COLUMN_hq4
		{
			get;
			private set;
		}

		public static int COLUMN_hq5
		{
			get;
			private set;
		}

		public static int COLUMN_hq6
		{
			get;
			private set;
		}

		public static int COLUMN_hq7
		{
			get;
			private set;
		}

		public static int COLUMN_hq8
		{
			get;
			private set;
		}

		public static int COLUMN_hq9
		{
			get;
			private set;
		}

		public static int COLUMN_hq10
		{
			get;
			private set;
		}

		public static int COLUMN_crateRewardUid
		{
			get;
			private set;
		}

		public static int COLUMN_allowPvE
		{
			get;
			private set;
		}

		public static int COLUMN_weight
		{
			get;
			private set;
		}

		public static int COLUMN_objIcon
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public int TestSet
		{
			get;
			set;
		}

		public string ObjectiveBucket
		{
			get;
			set;
		}

		public FactionType faction
		{
			get;
			set;
		}

		public int MinHQ
		{
			get;
			set;
		}

		public int MaxHQ
		{
			get;
			set;
		}

		public GoalType ObjectiveType
		{
			get;
			set;
		}

		public string Item
		{
			get;
			set;
		}

		public string ObjString
		{
			get;
			set;
		}

		public int HQ4
		{
			get;
			set;
		}

		public int HQ5
		{
			get;
			set;
		}

		public int HQ6
		{
			get;
			set;
		}

		public int HQ7
		{
			get;
			set;
		}

		public int HQ8
		{
			get;
			set;
		}

		public int HQ9
		{
			get;
			set;
		}

		public int HQ10
		{
			get;
			set;
		}

		public string CrateRewardUid
		{
			get;
			set;
		}

		public bool AllowPvE
		{
			get;
			set;
		}

		public int Weight
		{
			get;
			set;
		}

		public string ObjIcon
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.TestSet = row.TryGetInt(ObjectiveVO.COLUMN_testSet);
			this.ObjectiveBucket = row.TryGetString(ObjectiveVO.COLUMN_objBucket);
			this.faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(ObjectiveVO.COLUMN_faction));
			this.MinHQ = row.TryGetInt(ObjectiveVO.COLUMN_minHQ, -1);
			this.MaxHQ = row.TryGetInt(ObjectiveVO.COLUMN_maxHQ, -1);
			this.ObjectiveType = StringUtils.ParseEnum<GoalType>(row.TryGetString(ObjectiveVO.COLUMN_type));
			this.Item = row.TryGetString(ObjectiveVO.COLUMN_item);
			this.ObjString = row.TryGetString(ObjectiveVO.COLUMN_objString);
			this.HQ4 = row.TryGetInt(ObjectiveVO.COLUMN_hq4);
			this.HQ5 = row.TryGetInt(ObjectiveVO.COLUMN_hq5);
			this.HQ6 = row.TryGetInt(ObjectiveVO.COLUMN_hq6);
			this.HQ7 = row.TryGetInt(ObjectiveVO.COLUMN_hq7);
			this.HQ8 = row.TryGetInt(ObjectiveVO.COLUMN_hq8);
			this.HQ9 = row.TryGetInt(ObjectiveVO.COLUMN_hq9);
			this.HQ10 = row.TryGetInt(ObjectiveVO.COLUMN_hq10);
			this.CrateRewardUid = row.TryGetString(ObjectiveVO.COLUMN_crateRewardUid);
			this.AllowPvE = row.TryGetBool(ObjectiveVO.COLUMN_allowPvE);
			this.Weight = row.TryGetInt(ObjectiveVO.COLUMN_weight, 0);
			this.ObjIcon = row.TryGetString(ObjectiveVO.COLUMN_objIcon);
		}
	}
}
