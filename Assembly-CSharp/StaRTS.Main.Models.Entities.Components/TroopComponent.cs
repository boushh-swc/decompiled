using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TroopComponent : WalkerComponent
	{
		public ITroopDeployableVO TroopType;

		public ITroopShooterVO TroopShooterVO
		{
			get;
			set;
		}

		public ITroopShooterVO OriginalTroopShooterVO
		{
			get;
			set;
		}

		public IAudioVO AudioVO
		{
			get;
			set;
		}

		public TroopAbilityVO AbilityVO
		{
			get;
			private set;
		}

		public int TargetCount
		{
			get;
			set;
		}

		public bool UpdateWallAttackerTroop
		{
			get;
			set;
		}

		public bool IsAbilityModeActive
		{
			get;
			set;
		}

		public TroopComponent(TroopTypeVO troopType) : base(troopType.AssetName, troopType)
		{
			this.TroopType = troopType;
			this.TroopShooterVO = troopType;
			this.OriginalTroopShooterVO = troopType;
			this.AudioVO = troopType;
			if (!string.IsNullOrEmpty(troopType.Ability))
			{
				this.AbilityVO = Service.StaticDataController.Get<TroopAbilityVO>(troopType.Ability);
			}
			base.SetVOData(troopType);
			this.TargetCount = 0;
			this.UpdateWallAttackerTroop = false;
			this.IsAbilityModeActive = false;
		}

		public void SetVOData(ITroopShooterVO troopVO, ISpeedVO speedVO)
		{
			this.TroopShooterVO = troopVO;
			base.SetVOData(speedVO);
		}
	}
}
