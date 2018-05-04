using StaRTS.Main.Controllers;
using StaRTS.Utils.Core;
using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SkinTypeVO : IAssetVO, IAudioVO, IGeometryVO, ISpeedVO, IValueObject
	{
		public static int COLUMN_unitID
		{
			get;
			private set;
		}

		public static int COLUMN_override
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

		public static int COLUMN_audioAttack
		{
			get;
			private set;
		}

		public static int COLUMN_audioDeath
		{
			get;
			private set;
		}

		public static int COLUMN_audioPlacement
		{
			get;
			private set;
		}

		public static int COLUMN_audioImpact
		{
			get;
			private set;
		}

		public static int COLUMN_audioTrain
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

		public static int COLUMN_iconRotationSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_maxSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_rotationSpeed
		{
			get;
			private set;
		}

		public static int COLUMN_hologramUid
		{
			get;
			private set;
		}

		public static int COLUMN_textureUid
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string UnitId
		{
			get;
			set;
		}

		public SkinOverrideTypeVO Override
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

		public List<StrIntPair> AudioCharge
		{
			get;
			set;
		}

		public List<StrIntPair> AudioAttack
		{
			get;
			set;
		}

		public List<StrIntPair> AudioDeath
		{
			get;
			set;
		}

		public List<StrIntPair> AudioPlacement
		{
			get;
			set;
		}

		public List<StrIntPair> AudioMovement
		{
			get;
			set;
		}

		public List<StrIntPair> AudioMovementAway
		{
			get;
			set;
		}

		public List<StrIntPair> AudioImpact
		{
			get;
			set;
		}

		public List<StrIntPair> AudioTrain
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

		public int MaxSpeed
		{
			get;
			set;
		}

		public int RotationSpeed
		{
			get;
			set;
		}

		public MobilizationHologramVO MobilizationHologram
		{
			get;
			set;
		}

		public TextureVO CardTexture
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.UnitId = row.TryGetString(SkinTypeVO.COLUMN_unitID);
			string text = row.TryGetString(SkinTypeVO.COLUMN_override);
			if (!string.IsNullOrEmpty(text))
			{
				StaticDataController staticDataController = Service.StaticDataController;
				this.Override = staticDataController.Get<SkinOverrideTypeVO>(text);
			}
			this.AssetName = row.TryGetString(SkinTypeVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(SkinTypeVO.COLUMN_bundleName);
			ValueObjectController valueObjectController = Service.ValueObjectController;
			this.AudioAttack = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SkinTypeVO.COLUMN_audioAttack));
			this.AudioDeath = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SkinTypeVO.COLUMN_audioDeath));
			this.AudioPlacement = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SkinTypeVO.COLUMN_audioPlacement));
			this.AudioImpact = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SkinTypeVO.COLUMN_audioImpact));
			this.AudioTrain = valueObjectController.GetStrIntPairs(this.Uid, row.TryGetString(SkinTypeVO.COLUMN_audioTrain));
			this.IconCameraPosition = row.TryGetVector3(SkinTypeVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(SkinTypeVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(SkinTypeVO.COLUMN_iconCloseupCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(SkinTypeVO.COLUMN_iconCloseupLookatPosition);
			this.IconAssetName = row.TryGetString(SkinTypeVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(SkinTypeVO.COLUMN_iconBundleName, this.BundleName);
			this.IconRotationSpeed = row.TryGetFloat(SkinTypeVO.COLUMN_iconRotationSpeed);
			this.MaxSpeed = row.TryGetInt(SkinTypeVO.COLUMN_maxSpeed);
			this.RotationSpeed = row.TryGetInt(SkinTypeVO.COLUMN_rotationSpeed);
			string text2 = row.TryGetString(SkinTypeVO.COLUMN_hologramUid);
			if (!string.IsNullOrEmpty(text2))
			{
				StaticDataController staticDataController2 = Service.StaticDataController;
				this.MobilizationHologram = staticDataController2.Get<MobilizationHologramVO>(text2);
			}
			string text3 = row.TryGetString(SkinTypeVO.COLUMN_textureUid);
			if (!string.IsNullOrEmpty(text3))
			{
				StaticDataController staticDataController3 = Service.StaticDataController;
				this.CardTexture = staticDataController3.Get<TextureVO>(text3);
			}
		}
	}
}
