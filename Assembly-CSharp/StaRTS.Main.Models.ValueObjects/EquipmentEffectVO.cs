using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class EquipmentEffectVO : IValueObject
	{
		public static int COLUMN_affectedTroopIds
		{
			get;
			private set;
		}

		public static int COLUMN_affectedSpecialAttackIds
		{
			get;
			private set;
		}

		public static int COLUMN_affectedBuildingIds
		{
			get;
			private set;
		}

		public static int COLUMN_buffUids
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] AffectedTroopIds
		{
			get;
			set;
		}

		public string[] AffectedSpecialAttackIds
		{
			get;
			set;
		}

		public string[] AffectedBuildingIds
		{
			get;
			set;
		}

		public string[] BuffUids
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AffectedTroopIds = row.TryGetStringArray(EquipmentEffectVO.COLUMN_affectedTroopIds);
			this.AffectedSpecialAttackIds = row.TryGetStringArray(EquipmentEffectVO.COLUMN_affectedSpecialAttackIds);
			this.AffectedBuildingIds = row.TryGetStringArray(EquipmentEffectVO.COLUMN_affectedBuildingIds);
			this.BuffUids = row.TryGetStringArray(EquipmentEffectVO.COLUMN_buffUids);
		}
	}
}
