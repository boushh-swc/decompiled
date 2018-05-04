using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public interface IHealthComponent
	{
		int Health
		{
			get;
			set;
		}

		int MaxHealth
		{
			get;
			set;
		}

		ArmorType ArmorType
		{
			get;
			set;
		}

		bool IsDead();
	}
}
