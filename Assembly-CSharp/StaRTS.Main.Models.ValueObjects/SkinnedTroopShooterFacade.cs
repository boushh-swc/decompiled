using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SkinnedTroopShooterFacade : SkinnedShooterFacade, IShooterVO, ITroopShooterVO
	{
		public bool TargetLocking
		{
			get
			{
				return ((ITroopShooterVO)this.original).TargetLocking;
			}
		}

		public bool TargetSelf
		{
			get
			{
				return ((ITroopShooterVO)this.original).TargetSelf;
			}
		}

		public SkinnedTroopShooterFacade(ITroopShooterVO original, SkinOverrideTypeVO skinned) : base(original, skinned)
		{
		}
	}
}
