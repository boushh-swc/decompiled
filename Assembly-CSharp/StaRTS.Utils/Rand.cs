using StaRTS.Utils.Core;
using System;

namespace StaRTS.Utils
{
	public class Rand
	{
		private Random viewRandom;

		private RandSimSeed simSeed = default(RandSimSeed);

		public RandSimSeed SimSeed
		{
			get
			{
				return this.simSeed;
			}
			set
			{
				this.simSeed = value;
				this.ValidateSimSeed(ref this.simSeed.SimSeedA);
				this.ValidateSimSeed(ref this.simSeed.SimSeedB);
			}
		}

		public float ViewValue
		{
			get
			{
				return (float)this.viewRandom.NextDouble();
			}
		}

		public Rand()
		{
			Service.Rand = this;
			this.viewRandom = new Random((int)DateTime.Now.Ticks);
			this.RandomizeSimSeed();
		}

		public RandSimSeed RandomizeSimSeed()
		{
			this.simSeed.SimSeedA = (uint)(this.viewRandom.Next(65536) << 16 | this.viewRandom.Next(65536));
			this.simSeed.SimSeedB = (uint)(this.viewRandom.Next(65536) << 16 | this.viewRandom.Next(65536));
			this.ValidateSimSeed(ref this.simSeed.SimSeedA);
			this.ValidateSimSeed(ref this.simSeed.SimSeedB);
			return this.simSeed;
		}

		private uint SimValue()
		{
			this.simSeed.SimSeedA = 36969u * (this.simSeed.SimSeedA & 65535u) + (this.simSeed.SimSeedA >> 16);
			this.simSeed.SimSeedB = 18000u * (this.simSeed.SimSeedB & 65535u) + (this.simSeed.SimSeedB >> 16);
			return (this.simSeed.SimSeedA << 16) + this.simSeed.SimSeedB;
		}

		private void ValidateSimSeed(ref uint seed)
		{
			if (seed == 0u)
			{
				seed = 1u;
			}
		}

		public int ViewRangeInt(int lowInclusive, int highExclusive)
		{
			return this.viewRandom.Next(lowInclusive, highExclusive);
		}

		public float ViewRangeFloat(float lowInclusive, float highExclusive)
		{
			double num = this.viewRandom.NextDouble();
			double num2 = (double)(highExclusive - lowInclusive);
			return lowInclusive + (float)(num * num2);
		}

		public int SimRange(int lowInclusive, int highExclusive)
		{
			uint num = this.SimValue();
			uint num2 = (uint)(highExclusive - lowInclusive);
			return lowInclusive + (int)(num % num2);
		}
	}
}
