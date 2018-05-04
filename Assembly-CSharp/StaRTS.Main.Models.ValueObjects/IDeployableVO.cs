using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IDeployableVO : IUpgradeableVO, IValueObject, IAssetVO, IGeometryVO, IUnlockableVO
	{
		FactionType Faction
		{
			get;
			set;
		}

		int Credits
		{
			get;
			set;
		}

		int Materials
		{
			get;
			set;
		}

		int Contraband
		{
			get;
			set;
		}

		string EventFeaturesString
		{
			get;
			set;
		}

		string EventButtonAction
		{
			get;
			set;
		}

		string EventButtonData
		{
			get;
			set;
		}

		string EventButtonString
		{
			get;
			set;
		}

		string UpgradeShardUid
		{
			get;
			set;
		}

		int UpgradeShardCount
		{
			get;
			set;
		}
	}
}
