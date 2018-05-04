using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TroopShieldHealthComponent : ComponentBase, IHealthComponent
	{
		public const int HEALTH_QUANTITY_DEATH = 0;

		public int Health
		{
			get;
			set;
		}

		public int MaxHealth
		{
			get;
			set;
		}

		public ArmorType ArmorType
		{
			get;
			set;
		}

		public TroopShieldHealthComponent(int health, ArmorType armorType)
		{
			this.MaxHealth = health;
			this.Health = this.MaxHealth;
			this.ArmorType = armorType;
		}

		public TroopShieldHealthComponent()
		{
			this.Health = 0;
		}

		public bool IsDead()
		{
			return this.Health <= 0;
		}
	}
}
