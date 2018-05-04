using StaRTS.Main.Utils;
using StaRTS.Utils;
using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class StickerVO : ILimitedEditionItemVO, IValueObject
	{
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

		public static int COLUMN_type
		{
			get;
			private set;
		}

		public static int COLUMN_iconAsset
		{
			get;
			private set;
		}

		public static int COLUMN_labelKey
		{
			get;
			private set;
		}

		public static int COLUMN_labelColor
		{
			get;
			private set;
		}

		public static int COLUMN_gradientColor
		{
			get;
			private set;
		}

		public static int COLUMN_priority
		{
			get;
			private set;
		}

		public static int COLUMN_audienceConditions
		{
			get;
			private set;
		}

		public static int COLUMN_mainColor
		{
			get;
			private set;
		}

		public static int COLUMN_textureOverrideRebel
		{
			get;
			private set;
		}

		public static int COLUMN_textureOverrideRebelAssetBundle
		{
			get;
			private set;
		}

		public static int COLUMN_textureOverrideEmpire
		{
			get;
			private set;
		}

		public static int COLUMN_textureOverrideEmpireAssetBundle
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public StickerType Type
		{
			get;
			set;
		}

		public string IconAsset
		{
			get;
			set;
		}

		public string LabelText
		{
			get;
			set;
		}

		public string LabelColor
		{
			get;
			set;
		}

		public string GradientColor
		{
			get;
			set;
		}

		public int Priority
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

		public string MainColor
		{
			get;
			set;
		}

		public string TextureOverrideAssetNameRebel
		{
			get;
			set;
		}

		public string TextureOverrideAssetBundleRebel
		{
			get;
			set;
		}

		public string TextureOverrideAssetNameEmpire
		{
			get;
			set;
		}

		public string TextureOverrideAssetBundleEmpire
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
			this.StartTime = TimedEventUtils.GetTimestamp(this.Uid, row.TryGetString(StickerVO.COLUMN_startDate));
			this.EndTime = TimedEventUtils.GetTimestamp(this.Uid, row.TryGetString(StickerVO.COLUMN_endDate));
			this.Type = StringUtils.ParseEnum<StickerType>(row.TryGetString(StickerVO.COLUMN_type));
			this.IconAsset = row.TryGetString(StickerVO.COLUMN_iconAsset);
			this.LabelText = row.TryGetString(StickerVO.COLUMN_labelKey);
			this.LabelColor = row.TryGetHexValueString(StickerVO.COLUMN_labelColor);
			this.GradientColor = row.TryGetHexValueString(StickerVO.COLUMN_gradientColor);
			this.Priority = row.TryGetInt(StickerVO.COLUMN_priority);
			this.AudienceConditions = row.TryGetStringArray(StickerVO.COLUMN_audienceConditions);
			this.MainColor = row.TryGetHexValueString(StickerVO.COLUMN_mainColor);
			this.TextureOverrideAssetNameRebel = row.TryGetString(StickerVO.COLUMN_textureOverrideRebel);
			this.TextureOverrideAssetBundleRebel = row.TryGetString(StickerVO.COLUMN_textureOverrideRebelAssetBundle);
			this.TextureOverrideAssetNameEmpire = row.TryGetString(StickerVO.COLUMN_textureOverrideEmpire);
			this.TextureOverrideAssetBundleEmpire = row.TryGetString(StickerVO.COLUMN_textureOverrideEmpireAssetBundle);
		}
	}
}
