using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class DefenderComponent : ComponentBase
	{
		public int SpawnX
		{
			get;
			private set;
		}

		public int SpawnZ
		{
			get;
			private set;
		}

		public bool Leashed
		{
			get;
			private set;
		}

		public DamageableComponent SpawnBuilding
		{
			get;
			private set;
		}

		public int PatrolLoc
		{
			get;
			set;
		}

		public bool Patrolling
		{
			get;
			set;
		}

		public DefenderComponent(int spawnX, int spawnZ, DamageableComponent building, bool leashed, int spawnLocIndex)
		{
			this.SpawnX = spawnX;
			this.SpawnZ = spawnZ;
			this.SpawnBuilding = building;
			this.PatrolLoc = spawnLocIndex;
			this.Leashed = leashed;
			this.Patrolling = false;
		}
	}
}
