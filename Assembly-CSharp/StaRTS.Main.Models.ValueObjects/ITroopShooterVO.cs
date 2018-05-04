using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface ITroopShooterVO : IShooterVO
	{
		bool TargetLocking
		{
			get;
		}

		bool TargetSelf
		{
			get;
		}
	}
}
