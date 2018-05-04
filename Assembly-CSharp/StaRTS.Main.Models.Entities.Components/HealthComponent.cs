using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class HealthComponent : ComponentBase, IHealthComponent
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

		public HealthComponent(int health, ArmorType armorType)
		{
			this.MaxHealth = health;
			this.Health = this.MaxHealth;
			this.ArmorType = armorType;
		}

		public HealthComponent()
		{
			this.Health = 0;
		}

		public bool IsDead()
		{
			return this.Health <= 0;
		}
	}
}
