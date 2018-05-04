using System;

namespace StaRTS.Main.Models.Battle
{
	public class LootData
	{
		public int Credits
		{
			get;
			set;
		}

		public int Materials
		{
			get;
			set;
		}

		public int Contraband
		{
			get;
			set;
		}

		public LootData(int credits, int materials, int contraband)
		{
			this.Credits = credits;
			this.Materials = materials;
			this.Contraband = contraband;
		}
	}
}
