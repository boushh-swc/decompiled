using Net.RichardLord.Ash.Core;
using System;

namespace StaRTS.Main.Controllers.CombatTriggers
{
	public struct DefenderTroopDeployedData
	{
		public Entity TroopEntity;

		public Entity OwnerEntity;

		public DefenderTroopDeployedData(Entity troop, Entity owner)
		{
			this.TroopEntity = troop;
			this.OwnerEntity = owner;
		}
	}
}
