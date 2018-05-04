using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface ISpeedVO
	{
		int MaxSpeed
		{
			get;
			set;
		}

		int RotationSpeed
		{
			get;
			set;
		}
	}
}
