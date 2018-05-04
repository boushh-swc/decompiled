using StaRTS.Utils;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Globalization;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class LimitedEditionItemVO : IGeometryVO, ILimitedEditionItemVO, IValueObject
	{
		public static int COLUMN_faction
		{
			get;
			private set;
		}

		public static int COLUMN_startDate
		{
			get;
			private set;
		}

		public static int COLUMN_endDate
		{
			get;
			private set;
		}

		public static int COLUMN_credits
		{
			get;
			private set;
		}

		public static int COLUMN_materials
		{
			get;
			private set;
		}

		public static int COLUMN_contraband
		{
			get;
			private set;
		}

		public static int COLUMN_crystals
		{
			get;
			private set;
		}

		public static int COLUMN_crateId
		{
			get;
			private set;
		}

		public static int COLUMN_storeTab
		{
			get;
			private set;
		}

		public static int COLUMN_description
		{
			get;
			private set;
		}

		public static int COLUMN_iconAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_iconBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_iconCameraPosition
		{
			get;
			private set;
		}

		public static int COLUMN_iconLookatPosition
		{
			get;
			private set;
		}

		public static int COLUMN_audienceConditions
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public FactionType Faction
		{
			get;
			set;
		}

		public int StartTime
		{
			get;
			set;
		}

		public int EndTime
		{
			get;
			set;
		}

		public string CrateId
		{
			get;
			set;
		}

		public int Credits
		{
			get;
			set;
		}

		public int Materials
		{
			get;
			set;
		}

		public int Contraband
		{
			get;
			set;
		}

		public int Crystals
		{
			get;
			set;
		}

		public StoreTab StoreTab
		{
			get;
			set;
		}

		public string Description
		{
			get;
			set;
		}

		public Vector3 IconCameraPosition
		{
			get;
			set;
		}

		public Vector3 IconLookatPosition
		{
			get;
			set;
		}

		public Vector3 IconCloseupCameraPosition
		{
			get;
			set;
		}

		public Vector3 IconCloseupLookatPosition
		{
			get;
			set;
		}

		public string IconBundleName
		{
			get;
			set;
		}

		public string IconAssetName
		{
			get;
			set;
		}

		public float IconRotationSpeed
		{
			get;
			set;
		}

		public string[] AudienceConditions
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.Faction = StringUtils.ParseEnum<FactionType>(row.TryGetString(LimitedEditionItemVO.COLUMN_faction));
			string text = row.TryGetString(LimitedEditionItemVO.COLUMN_startDate);
			if (!string.IsNullOrEmpty(text))
			{
				try
				{
					DateTime date = DateTime.ParseExact(text, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
					this.StartTime = DateUtils.GetSecondsFromEpoch(date);
				}
				catch (Exception)
				{
					this.StartTime = 0;
					Service.Logger.Warn("LimitedEditionItemVO:: LEI CMS Start Date Format Error: " + this.Uid);
				}
			}
			else
			{
				this.StartTime = 0;
				Service.Logger.Warn("LimitedEditionItemVO:: LEI CMS Start Date Not Specified For: " + this.Uid);
			}
			string text2 = row.TryGetString(LimitedEditionItemVO.COLUMN_endDate);
			if (!string.IsNullOrEmpty(text2))
			{
				try
				{
					DateTime date2 = DateTime.ParseExact(text2, "HH:mm,dd-MM-yyyy", CultureInfo.InvariantCulture);
					this.EndTime = DateUtils.GetSecondsFromEpoch(date2);
				}
				catch (Exception)
				{
					this.EndTime = 2147483647;
					Service.Logger.Warn("LimitedEditionItemVO:: LEI CMS End Date Format Error: " + this.Uid);
				}
			}
			else
			{
				this.EndTime = 2147483647;
			}
			this.CrateId = row.TryGetString(LimitedEditionItemVO.COLUMN_crateId, string.Empty);
			this.Credits = row.TryGetInt(LimitedEditionItemVO.COLUMN_credits, 0);
			this.Materials = row.TryGetInt(LimitedEditionItemVO.COLUMN_materials, 0);
			this.Contraband = row.TryGetInt(LimitedEditionItemVO.COLUMN_contraband, 0);
			this.Crystals = row.TryGetInt(LimitedEditionItemVO.COLUMN_crystals, 0);
			this.StoreTab = StringUtils.ParseEnum<StoreTab>(row.TryGetString(LimitedEditionItemVO.COLUMN_storeTab));
			this.Description = row.TryGetString(LimitedEditionItemVO.COLUMN_description, string.Empty);
			this.IconAssetName = row.TryGetString(LimitedEditionItemVO.COLUMN_iconAssetName, string.Empty);
			this.IconBundleName = row.TryGetString(LimitedEditionItemVO.COLUMN_iconBundleName, string.Empty);
			this.IconCameraPosition = row.TryGetVector3(LimitedEditionItemVO.COLUMN_iconCameraPosition, Vector3.one);
			this.IconLookatPosition = row.TryGetVector3(LimitedEditionItemVO.COLUMN_iconLookatPosition, Vector3.zero);
			this.IconCloseupCameraPosition = this.IconCameraPosition;
			this.IconCloseupLookatPosition = this.IconCloseupLookatPosition;
			this.IconRotationSpeed = 0f;
			this.AudienceConditions = row.TryGetStringArray(LimitedEditionItemVO.COLUMN_audienceConditions);
		}
	}
}
