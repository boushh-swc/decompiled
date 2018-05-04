using StaRTS.Main.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class PlanetLootEntryVO : IValueObject
	{
		public static int COLUMN_supplyDataUid
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

		public static int COLUMN_reqArmory
		{
			get;
			private set;
		}

		public static int COLUMN_badge
		{
			get;
			private set;
		}

		public static int COLUMN_showDate
		{
			get;
			private set;
		}

		public static int COLUMN_hideDate
		{
			get;
			private set;
		}

		public static int COLUMN_featureAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_featureAssetBundle
		{
			get;
			private set;
		}

		public static int COLUMN_notesString
		{
			get;
			private set;
		}

		public static int COLUMN_typeString
		{
			get;
			private set;
		}

		public string SupplyDataUid
		{
			get;
			private set;
		}

		public int MinHQ
		{
			get;
			private set;
		}

		public int MaxHQ
		{
			get;
			private set;
		}

		public bool ReqArmory
		{
			get;
			private set;
		}

		public bool Badge
		{
			get;
			private set;
		}

		public int ShowDateTimeStamp
		{
			get;
			private set;
		}

		public int HideDateTimeStamp
		{
			get;
			private set;
		}

		public string FeatureAssetName
		{
			get;
			private set;
		}

		public string FeatureAssetBundle
		{
			get;
			private set;
		}

		public string NotesString
		{
			get;
			private set;
		}

		public string TypeStringID
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.SupplyDataUid = row.TryGetString(PlanetLootEntryVO.COLUMN_supplyDataUid);
			this.MinHQ = row.TryGetInt(PlanetLootEntryVO.COLUMN_minHQ);
			this.MaxHQ = row.TryGetInt(PlanetLootEntryVO.COLUMN_maxHQ);
			this.ReqArmory = row.TryGetBool(PlanetLootEntryVO.COLUMN_reqArmory);
			this.Badge = row.TryGetBool(PlanetLootEntryVO.COLUMN_badge);
			string text = row.TryGetString(PlanetLootEntryVO.COLUMN_showDate);
			string text2 = row.TryGetString(PlanetLootEntryVO.COLUMN_hideDate);
			if (!string.IsNullOrEmpty(text))
			{
				this.ShowDateTimeStamp = TimedEventUtils.GetTimestamp(this.Uid, text);
			}
			if (!string.IsNullOrEmpty(text2))
			{
				this.HideDateTimeStamp = TimedEventUtils.GetTimestamp(this.Uid, text2);
			}
			this.FeatureAssetName = row.TryGetString(PlanetLootEntryVO.COLUMN_featureAssetName);
			this.FeatureAssetBundle = row.TryGetString(PlanetLootEntryVO.COLUMN_featureAssetBundle);
			this.NotesString = row.TryGetString(PlanetLootEntryVO.COLUMN_notesString);
			this.TypeStringID = row.TryGetString(PlanetLootEntryVO.COLUMN_typeString);
		}
	}
}
