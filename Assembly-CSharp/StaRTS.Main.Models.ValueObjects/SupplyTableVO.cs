using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SupplyTableVO : IValueObject
	{
		public static int COLUMN_crateTier
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

		public static int COLUMN_supplyType
		{
			get;
			private set;
		}

		public static int COLUMN_item
		{
			get;
			private set;
		}

		public static int COLUMN_scalingUid
		{
			get;
			private set;
		}

		public static int COLUMN_weight
		{
			get;
			private set;
		}

		public static int COLUMN_amount
		{
			get;
			private set;
		}

		public static int COLUMN_dataCard
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string crateTier
		{
			get;
			set;
		}

		public string faction
		{
			get;
			set;
		}

		public string planet
		{
			get;
			set;
		}

		public int minHQ
		{
			get;
			set;
		}

		public int maxHQ
		{
			get;
			set;
		}

		public SupplyType supplyType
		{
			get;
			set;
		}

		public string item
		{
			get;
			set;
		}

		public string scalingUid
		{
			get;
			set;
		}

		public int weight
		{
			get;
			set;
		}

		public int amount
		{
			get;
			set;
		}

		public string dataCard
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.crateTier = row.TryGetString(SupplyTableVO.COLUMN_crateTier);
			this.faction = row.TryGetString(SupplyTableVO.COLUMN_faction);
			this.planet = row.TryGetString(SupplyTableVO.COLUMN_planet);
			this.minHQ = row.TryGetInt(SupplyTableVO.COLUMN_minHQ);
			this.maxHQ = row.TryGetInt(SupplyTableVO.COLUMN_maxHQ);
			this.supplyType = StringUtils.ParseEnum<SupplyType>(row.TryGetString(SupplyTableVO.COLUMN_supplyType));
			this.item = row.TryGetString(SupplyTableVO.COLUMN_item);
			this.scalingUid = row.TryGetString(SupplyTableVO.COLUMN_scalingUid);
			this.weight = row.TryGetInt(SupplyTableVO.COLUMN_weight);
			this.amount = row.TryGetInt(SupplyTableVO.COLUMN_amount);
			this.dataCard = row.TryGetString(SupplyTableVO.COLUMN_dataCard);
		}
	}
}
