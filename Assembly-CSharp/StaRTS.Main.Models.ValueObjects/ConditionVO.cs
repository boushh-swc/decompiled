using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class ConditionVO : IValueObject
	{
		public static int COLUMN_condition_type
		{
			get;
			private set;
		}

		public static int COLUMN_prepare_string
		{
			get;
			private set;
		}

		public static int COLUMN_end_on_fail
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string ConditionType
		{
			get;
			set;
		}

		public string PrepareString
		{
			get;
			set;
		}

		public bool EndOnFail
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.ConditionType = row.TryGetString(ConditionVO.COLUMN_condition_type);
			this.PrepareString = row.TryGetString(ConditionVO.COLUMN_prepare_string);
			this.EndOnFail = row.TryGetBool(ConditionVO.COLUMN_end_on_fail);
		}

		public bool IsPvpType()
		{
			return this.ConditionType == "PvpStart" || this.ConditionType == "PvpWin";
		}
	}
}
