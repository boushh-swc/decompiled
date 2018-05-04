using StaRTS.Main.Models.Entities;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Battle
{
	public class HealthFragment
	{
		public HealthType Type
		{
			get;
			private set;
		}

		public int Quantity
		{
			get;
			private set;
		}

		public int SplashQuantity
		{
			get;
			private set;
		}

		public HealthFragment(SmartEntity source, HealthType type, int quantity)
		{
			this.Type = type;
			int splashQuantity = quantity;
			if (type != HealthType.Healing && source != null)
			{
				int modifyValueMax = quantity;
				Service.BuffController.ApplyActiveBuffs(source, BuffModify.Damage, ref quantity, modifyValueMax);
				splashQuantity = quantity;
				Service.BuffController.ApplyActiveBuffs(source, BuffModify.SplashDamage, ref splashQuantity, modifyValueMax);
			}
			this.Quantity = quantity;
			this.SplashQuantity = splashQuantity;
		}
	}
}
