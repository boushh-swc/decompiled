using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Perks
{
	public class CurrencyPerkEffectDataTO
	{
		public float RateBonus
		{
			get;
			private set;
		}

		public uint StartTime
		{
			get;
			private set;
		}

		public uint EndTime
		{
			get;
			private set;
		}

		public int Duration
		{
			get;
			private set;
		}

		public CurrencyPerkEffectDataTO(float rate, uint start, uint end)
		{
			this.RateBonus = rate;
			this.StartTime = start;
			this.EndTime = end;
			if (start > end)
			{
				Service.Logger.Error(string.Concat(new object[]
				{
					"Bad CurrencyPerkEffectDataTO time data: End ",
					end,
					" - Start ",
					start
				}));
			}
			this.Duration = (int)(end - start);
		}

		public void AdjustRateBonus(float delta)
		{
			this.RateBonus += delta;
		}
	}
}
