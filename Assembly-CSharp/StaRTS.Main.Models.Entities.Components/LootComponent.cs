using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class LootComponent : ComponentBase
	{
		public int[] LootQuantities
		{
			get;
			protected set;
		}

		public int AttackCount
		{
			get;
			protected set;
		}

		public LootComponent()
		{
			int num = 6;
			this.LootQuantities = new int[num];
			for (int i = 0; i < num; i++)
			{
				this.LootQuantities[i] = 0;
			}
		}

		public void SetLootQuantity(CurrencyType type, int quantity)
		{
			this.LootQuantities[(int)type] = quantity;
		}

		public void IncParticleCount()
		{
			this.AttackCount++;
		}
	}
}
