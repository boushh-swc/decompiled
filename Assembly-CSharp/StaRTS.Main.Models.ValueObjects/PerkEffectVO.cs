using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class PerkEffectVO : IValueObject
	{
		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_building
		{
			get;
			private set;
		}

		public static int COLUMN_currency
		{
			get;
			private set;
		}

		public static int COLUMN_generationRate
		{
			get;
			private set;
		}

		public static int COLUMN_contractTimeReduction
		{
			get;
			private set;
		}

		public static int COLUMN_contractDiscount
		{
			get;
			private set;
		}

		public static int COLUMN_relocationDiscount
		{
			get;
			private set;
		}

		public static int COLUMN_troopRequestAmount
		{
			get;
			private set;
		}

		public static int COLUMN_troopRequestTimeDiscount
		{
			get;
			private set;
		}

		public static int COLUMN_stringId
		{
			get;
			private set;
		}

		public static int COLUMN_stringModId
		{
			get;
			private set;
		}

		public static int COLUMN_stringUpgradeModId
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string Type
		{
			get;
			private set;
		}

		public BuildingType PerkBuilding
		{
			get;
			private set;
		}

		public CurrencyType Currency
		{
			get;
			private set;
		}

		public float GenerationRate
		{
			get;
			private set;
		}

		public float ContractTimeReduction
		{
			get;
			private set;
		}

		public float ContractDiscount
		{
			get;
			private set;
		}

		public int RelocationDiscount
		{
			get;
			private set;
		}

		public int TroopRequestAmount
		{
			get;
			private set;
		}

		public int TroopRequestTimeDiscount
		{
			get;
			private set;
		}

		public string StatStringId
		{
			get;
			private set;
		}

		public string StatValueFormatStringId
		{
			get;
			private set;
		}

		public string StatUpgradeValueFormatStringId
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Type = row.TryGetString(PerkEffectVO.COLUMN_type);
			this.PerkBuilding = StringUtils.ParseEnum<BuildingType>(row.TryGetString(PerkEffectVO.COLUMN_building));
			this.Currency = StringUtils.ParseEnum<CurrencyType>(row.TryGetString(PerkEffectVO.COLUMN_currency));
			this.GenerationRate = row.TryGetFloat(PerkEffectVO.COLUMN_generationRate);
			this.ContractTimeReduction = row.TryGetFloat(PerkEffectVO.COLUMN_contractTimeReduction);
			this.ContractDiscount = row.TryGetFloat(PerkEffectVO.COLUMN_contractDiscount);
			this.RelocationDiscount = row.TryGetInt(PerkEffectVO.COLUMN_relocationDiscount);
			this.TroopRequestAmount = row.TryGetInt(PerkEffectVO.COLUMN_troopRequestAmount);
			this.TroopRequestTimeDiscount = row.TryGetInt(PerkEffectVO.COLUMN_troopRequestTimeDiscount);
			this.StatStringId = row.TryGetString(PerkEffectVO.COLUMN_stringId);
			this.StatValueFormatStringId = row.TryGetString(PerkEffectVO.COLUMN_stringModId);
			this.StatUpgradeValueFormatStringId = row.TryGetString(PerkEffectVO.COLUMN_stringUpgradeModId);
		}
	}
}
