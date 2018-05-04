using StaRTS.Utils.MetaData;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Models.ValueObjects
{
	public class CrateTierVO : IGeometryVO, IValueObject
	{
		private List<string[]> allHq;

		public static int COLUMN_assetName
		{
			get;
			private set;
		}

		public static int COLUMN_hq4
		{
			get;
			private set;
		}

		public static int COLUMN_hq5
		{
			get;
			private set;
		}

		public static int COLUMN_hq6
		{
			get;
			private set;
		}

		public static int COLUMN_hq7
		{
			get;
			private set;
		}

		public static int COLUMN_hq8
		{
			get;
			private set;
		}

		public static int COLUMN_hq9
		{
			get;
			private set;
		}

		public static int COLUMN_hq10
		{
			get;
			private set;
		}

		public static int COLUMN_purchasable
		{
			get;
			private set;
		}

		public static int COLUMN_crystals
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

		public static int COLUMN_additionalCrateTier
		{
			get;
			private set;
		}

		public static int COLUMN_additionalCrateCondition
		{
			get;
			private set;
		}

		public static int COLUMN_storeVisibilityConditions
		{
			get;
			private set;
		}

		public static int COLUMN_storePurchasableConditions
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string[] hq4
		{
			get;
			set;
		}

		public string[] hq5
		{
			get;
			set;
		}

		public string[] hq6
		{
			get;
			set;
		}

		public string[] hq7
		{
			get;
			set;
		}

		public string[] hq8
		{
			get;
			set;
		}

		public string[] hq9
		{
			get;
			set;
		}

		public string[] hq10
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

		public bool Purchasable
		{
			get;
			set;
		}

		public int Crystals
		{
			get;
			set;
		}

		public string AdditionalCrateTier
		{
			get;
			set;
		}

		public string AdditionalCrateCondition
		{
			get;
			set;
		}

		public string[] StoreVisibilityConditions
		{
			get;
			set;
		}

		public string[] StorePurchasableConditions
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
			this.allHq = new List<string[]>();
			this.hq4 = row.TryGetString(CrateTierVO.COLUMN_hq4, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq4);
			this.hq5 = row.TryGetString(CrateTierVO.COLUMN_hq5, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq5);
			this.hq6 = row.TryGetString(CrateTierVO.COLUMN_hq6, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq6);
			this.hq7 = row.TryGetString(CrateTierVO.COLUMN_hq7, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq7);
			this.hq8 = row.TryGetString(CrateTierVO.COLUMN_hq8, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq8);
			this.hq9 = row.TryGetString(CrateTierVO.COLUMN_hq9, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq9);
			this.hq10 = row.TryGetString(CrateTierVO.COLUMN_hq10, string.Empty).Split(new char[]
			{
				' '
			});
			this.allHq.Add(this.hq10);
			this.AssetName = row.TryGetString(CrateTierVO.COLUMN_assetName);
			this.BundleName = row.TryGetString(CrateTierVO.COLUMN_bundleName);
			this.IconAssetName = row.TryGetString(CrateTierVO.COLUMN_iconAssetName, this.AssetName);
			this.IconBundleName = row.TryGetString(CrateTierVO.COLUMN_iconBundleName, this.BundleName);
			this.IconCameraPosition = row.TryGetVector3(CrateTierVO.COLUMN_iconCameraPosition);
			this.IconLookatPosition = row.TryGetVector3(CrateTierVO.COLUMN_iconLookatPosition);
			this.IconCloseupCameraPosition = row.TryGetVector3(CrateTierVO.COLUMN_iconCloseupCameraPosition, this.IconCameraPosition);
			this.IconCloseupLookatPosition = row.TryGetVector3(CrateTierVO.COLUMN_iconCloseupLookatPosition, this.IconLookatPosition);
			this.Purchasable = row.TryGetBool(CrateTierVO.COLUMN_purchasable);
			this.Crystals = row.TryGetInt(CrateTierVO.COLUMN_crystals, 0);
			this.AdditionalCrateTier = row.TryGetString(CrateTierVO.COLUMN_additionalCrateTier);
			this.AdditionalCrateCondition = row.TryGetString(CrateTierVO.COLUMN_additionalCrateCondition);
			this.StoreVisibilityConditions = row.TryGetStringArray(CrateTierVO.COLUMN_storeVisibilityConditions);
			this.StorePurchasableConditions = row.TryGetStringArray(CrateTierVO.COLUMN_storePurchasableConditions);
		}

		public string SupplyTableId(int hq, FactionType faction, int itemNumber)
		{
			int num = itemNumber - 1;
			string[] array = this.allHq[hq - 4];
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			if (faction == FactionType.Rebel)
			{
				return array[num * 2];
			}
			return array[num * 2 + 1];
		}
	}
}
