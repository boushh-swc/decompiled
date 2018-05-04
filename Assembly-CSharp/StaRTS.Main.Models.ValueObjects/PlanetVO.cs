using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class PlanetVO : IAssetVO, IGeometryVO, IValueObject
	{
		public static int COLUMN_ambientMusic
		{
			get;
			private set;
		}

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_bundleName
		{
			get;
			private set;
		}

		public static int COLUMN_loaderAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_loaderBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_loaderDisplayName
		{
			get;
			private set;
		}

		public static int COLUMN_planetaryLighting
		{
			get;
			private set;
		}

		public static int COLUMN_planetaryFX
		{
			get;
			private set;
		}

		public static int COLUMN_position
		{
			get;
			private set;
		}

		public static int COLUMN_galaxyAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_galaxyBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_footerTexture
		{
			get;
			private set;
		}

		public static int COLUMN_footerConflictTexture
		{
			get;
			private set;
		}

		public static int COLUMN_playerFacing
		{
			get;
			private set;
		}

		public static int COLUMN_order
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

		public static int COLUMN_iconCloseupCameraPosition
		{
			get;
			private set;
		}

		public static int COLUMN_iconCloseupLookatPosition
		{
			get;
			private set;
		}

		public static int COLUMN_currencyType
		{
			get;
			private set;
		}

		public static int COLUMN_currencyAmount
		{
			get;
			private set;
		}

		public static int COLUMN_leaderboardAssetBundle
		{
			get;
			private set;
		}

		public static int COLUMN_leaderboardTileTexture
		{
			get;
			private set;
		}

		public static int COLUMN_leaderboardButtonTexture
		{
			get;
			private set;
		}

		public static int COLUMN_abbreviation
		{
			get;
			private set;
		}

		public static int COLUMN_medalIconName
		{
			get;
			private set;
		}

		public static int COLUMN_planetBIName
		{
			get;
			private set;
		}

		public static int COLUMN_rebelMusic
		{
			get;
			private set;
		}

		public static int COLUMN_empireMusic
		{
			get;
			private set;
		}

		public static int COLUMN_battleMusic
		{
			get;
			private set;
		}

		public static int COLUMN_introStoryAction
		{
			get;
			private set;
		}

		public static int COLUMN_nightDuration
		{
			get;
			private set;
		}

		public static int COLUMN_sunriseDuration
		{
			get;
			private set;
		}

		public static int COLUMN_midDayDuration
		{
			get;
			private set;
		}

		public static int COLUMN_sunsetDuration
		{
			get;
			private set;
		}

		public static int COLUMN_cyclesPerDay
		{
			get;
			private set;
		}

		public static int COLUMN_sunriseMidnightOffset
		{
			get;
			private set;
		}

		public static int COLUMN_timeOfDayAssetBundle
		{
			get;
			private set;
		}

		public static int COLUMN_warBoardLightingAssetBundle
		{
			get;
			private set;
		}

		public static int COLUMN_warBoardAssetName
		{
			get;
			private set;
		}

		public static int COLUMN_warBoardBundleName
		{
			get;
			private set;
		}

		public static int COLUMN_perkBuildingHighlightColor
		{
			get;
			private set;
		}

		public static int COLUMN_planetLootUid
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string AmbientMusic
		{
			get;
			set;
		}

		public string AssetName
		{
			get;
			set;
		}

		public string BundleName
		{
			get;
			set;
		}

		public string LoadingScreenAssetName
		{
			get;
			set;
		}

		public string LoadingScreenBundleName
		{
			get;
			set;
		}

		public string LoadingScreenText
		{
			get;
			set;
		}

		public string PlanetaryLighting
		{
			get;
			set;
		}

		public string PlanetaryFX
		{
			get;
			set;
		}

		public int Population
		{
			get;
			set;
		}

		public float Angle
		{
			get;
			set;
		}

		public float Radius
		{
			get;
			set;
		}

		public float HeightOffset
		{
			get;
			set;
		}

		public string GalaxyAssetName
		{
			get;
			set;
		}

		public string GalaxyBundleName
		{
			get;
			set;
		}

		public string FooterTexture
		{
			get;
			set;
		}

		public string FooterConflictTexture
		{
			get;
			set;
		}

		public bool PlayerFacing
		{
			get;
			set;
		}

		public int Order
		{
			get;
			set;
		}

		public string IconAssetName
		{
			get;
			set;
		}

		public string IconBundleName
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

		public float IconRotationSpeed
		{
			get;
			set;
		}

		public CurrencyType CurrencyType
		{
			get;
			set;
		}

		public int CurrencyAmount
		{
			get;
			set;
		}

		public string LeaderboardAssetBundle
		{
			get;
			set;
		}

		public string LeaderboardTileTexture
		{
			get;
			set;
		}

		public string LeaderboardButtonTexture
		{
			get;
			set;
		}

		public string Abbreviation
		{
			get;
			set;
		}

		public string MedalIconName
		{
			get;
			set;
		}

		public string PlanetBIName
		{
			get;
			set;
		}

		public string RebelMusic
		{
			get;
			set;
		}

		public string EmpireMusic
		{
			get;
			set;
		}

		public string BattleMusic
		{
			get;
			set;
		}

		public string IntroStoryAction
		{
			get;
			set;
		}

		public float NightDuration
		{
			get;
			set;
		}

		public float SunriseDuration
		{
			get;
			set;
		}

		public float MidDayDuration
		{
			get;
			set;
		}

		public float SunsetDuration
		{
			get;
			set;
		}

		public float CyclesPerDay
		{
			get;
			set;
		}

		public float SunriseMidnightOffset
		{
			get;
			set;
		}

		public string TimeOfDayAsset
		{
			get;
			set;
		}

		public string WarBoardLightingAsset
		{
			get;
			set;
		}

		public string WarBoardAssetName
		{
			get;
			set;
		}

		public string WarBoardBundleName
		{
			get;
			set;
		}

		public Color PlanetPerkShaderColor
		{
			get;
			private set;
		}

		public string PlanetLootUid
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AmbientMusic = row.TryGetString(PlanetVO.COLUMN_ambientMusic);
			this.AssetName = row.TryGetString(PlanetVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(PlanetVO.COLUMN_bundleName);
			this.LoadingScreenAssetName = row.TryGetString(PlanetVO.COLUMN_loaderAssetName);
			this.LoadingScreenBundleName = row.TryGetString(PlanetVO.COLUMN_loaderBundleName);
			this.LoadingScreenText = row.TryGetString(PlanetVO.COLUMN_loaderDisplayName);
			this.PlanetaryLighting = row.TryGetString(PlanetVO.COLUMN_planetaryLighting);
			this.PlanetaryFX = row.TryGetString(PlanetVO.COLUMN_planetaryFX);
			Vector3 vector = row.TryGetVector3(PlanetVO.COLUMN_position);
			this.Angle = vector[0];
			this.Radius = vector[1];
			this.HeightOffset = vector[2];
			this.GalaxyAssetName = row.TryGetString(PlanetVO.COLUMN_galaxyAssetName);
			this.GalaxyBundleName = row.TryGetString(PlanetVO.COLUMN_galaxyBundleName);
			this.FooterTexture = row.TryGetString(PlanetVO.COLUMN_footerTexture);
			this.FooterConflictTexture = row.TryGetString(PlanetVO.COLUMN_footerConflictTexture);
			this.PlayerFacing = row.TryGetBool(PlanetVO.COLUMN_playerFacing);
			this.Order = row.TryGetInt(PlanetVO.COLUMN_order);
			this.IconAssetName = row.TryGetString(PlanetVO.COLUMN_iconAssetName);
			this.IconBundleName = row.TryGetString(PlanetVO.COLUMN_iconBundleName);
			this.IconCameraPosition = row.TryGetVector3(PlanetVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(PlanetVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(PlanetVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(PlanetVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.CurrencyType = StringUtils.ParseEnum<CurrencyType>(row.TryGetString(PlanetVO.COLUMN_currencyType));
			this.CurrencyAmount = row.TryGetInt(PlanetVO.COLUMN_currencyAmount);
			this.LeaderboardAssetBundle = row.TryGetString(PlanetVO.COLUMN_leaderboardAssetBundle);
			this.LeaderboardTileTexture = row.TryGetString(PlanetVO.COLUMN_leaderboardTileTexture);
			this.LeaderboardButtonTexture = row.TryGetString(PlanetVO.COLUMN_leaderboardButtonTexture);
			this.Abbreviation = row.TryGetString(PlanetVO.COLUMN_abbreviation);
			this.MedalIconName = row.TryGetString(PlanetVO.COLUMN_medalIconName);
			this.PlanetBIName = row.TryGetString(PlanetVO.COLUMN_planetBIName);
			this.RebelMusic = row.TryGetString(PlanetVO.COLUMN_rebelMusic);
			this.EmpireMusic = row.TryGetString(PlanetVO.COLUMN_empireMusic);
			this.BattleMusic = row.TryGetString(PlanetVO.COLUMN_battleMusic);
			this.IntroStoryAction = row.TryGetString(PlanetVO.COLUMN_introStoryAction);
			this.NightDuration = row.TryGetFloat(PlanetVO.COLUMN_nightDuration);
			this.SunriseDuration = row.TryGetFloat(PlanetVO.COLUMN_sunriseDuration);
			this.MidDayDuration = row.TryGetFloat(PlanetVO.COLUMN_midDayDuration);
			this.SunsetDuration = row.TryGetFloat(PlanetVO.COLUMN_sunsetDuration);
			this.CyclesPerDay = row.TryGetFloat(PlanetVO.COLUMN_cyclesPerDay);
			this.SunriseMidnightOffset = row.TryGetFloat(PlanetVO.COLUMN_sunriseMidnightOffset);
			this.TimeOfDayAsset = row.TryGetString(PlanetVO.COLUMN_timeOfDayAssetBundle);
			this.WarBoardLightingAsset = row.TryGetString(PlanetVO.COLUMN_warBoardLightingAssetBundle);
			this.WarBoardAssetName = row.TryGetString(PlanetVO.COLUMN_warBoardAssetName);
			this.WarBoardBundleName = row.TryGetString(PlanetVO.COLUMN_warBoardBundleName);
			float[] array = row.TryGetFloatArray(PlanetVO.COLUMN_perkBuildingHighlightColor);
			if (array != null)
			{
				this.PlanetPerkShaderColor = new Color(array[0], array[1], array[2], array[3]);
			}
			else
			{
				this.PlanetPerkShaderColor = new Color(1f, 1f, 1f, 1f);
			}
			this.PlanetLootUid = row.TryGetString(PlanetVO.COLUMN_planetLootUid);
		}

		public long GetSunriseTimestamp(int timeStamp)
		{
			DateTime date = DateUtils.DateFromSeconds(timeStamp);
			TimeSpan timeSpanSinceStartOfDate = DateUtils.GetTimeSpanSinceStartOfDate(date);
			return (long)DateUtils.GetSecondsFromEpoch(date.Subtract(timeSpanSinceStartOfDate).AddHours((double)this.SunriseMidnightOffset));
		}

		public Vector3 GetGalaxyPositionAsVec3()
		{
			return new Vector3(this.Angle, this.Radius, this.HeightOffset);
		}

		public void SetGalaxyPositionFromVec3(Vector3 pos)
		{
			this.Angle = pos.x;
			this.Radius = pos.y;
			this.HeightOffset = pos.z;
		}
	}
}
