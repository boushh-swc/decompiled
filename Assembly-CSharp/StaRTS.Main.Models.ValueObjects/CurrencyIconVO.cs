using StaRTS.Utils.MetaData;
using System;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CurrencyIconVO : IGeometryVO, IValueObject
	{
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

		public string Uid
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

		public float IconRotationSpeed
		{
			get;
			set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.AssetName = row.TryGetString(CurrencyIconVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(CurrencyIconVO.COLUMN_bundleName);
			this.IconAssetName = row.TryGetString(CurrencyIconVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(CurrencyIconVO.COLUMN_iconBundleName, this.BundleName);
			this.IconCameraPosition = row.TryGetVector3(CurrencyIconVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(CurrencyIconVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(CurrencyIconVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(CurrencyIconVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.IconRotationSpeed = row.TryGetFloat(CurrencyIconVO.COLUMN_iconRotationSpeed, 0f);
		}
	}
}
