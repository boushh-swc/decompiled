using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IUpgradeableVO : IValueObject, IAssetVO, IGeometryVO, IUnlockableVO
	{
		string UpgradeGroup
		{
			get;
		}

		int UpgradeCredits
		{
			get;
		}

		int UpgradeMaterials
		{
			get;
		}

		int UpgradeContraband
		{
			get;
		}

		int UpgradeTime
		{
			get;
			set;
		}

		int Lvl
		{
			get;
			set;
		}

		int Order
		{
			get;
			set;
		}

		int Size
		{
			get;
			set;
		}

		string BuildingRequirement
		{
			get;
			set;
		}

		bool PlayerFacing
		{
			get;
		}

		bool UnlockedByEvent
		{
			get;
			set;
		}
	}
}
