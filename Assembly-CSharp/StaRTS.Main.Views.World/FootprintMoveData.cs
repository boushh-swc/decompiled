using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Views.World
{
	public class FootprintMoveData
	{
		public Entity Building
		{
			get;
			private set;
		}

		public float WorldAnchorX
		{
			get;
			private set;
		}

		public float WorldAnchorZ
		{
			get;
			private set;
		}

		public bool CanOccupy
		{
			get;
			private set;
		}

		public FootprintMoveData(Entity building, float worldAnchorX, float worldAnchorZ, bool canOccupy)
		{
			this.Building = building;
			this.WorldAnchorX = worldAnchorX;
			this.WorldAnchorZ = worldAnchorZ;
			this.CanOccupy = canOccupy;
		}
	}
}
