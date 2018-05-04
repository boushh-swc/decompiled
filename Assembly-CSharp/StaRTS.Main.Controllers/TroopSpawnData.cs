using StaRTS.DataStructures;
using StaRTS.Main.Models.ValueObjects;
using System;

namespace StaRTS.Main.Controllers
{
	public struct TroopSpawnData
	{
		public TroopTypeVO TroopType;

		public IntPosition BoardPosition;

		public int Amount;

		public TroopSpawnMode SpawnMode;

		public TroopSpawnData(TroopTypeVO troop, IntPosition position, TroopSpawnMode mode, int amount)
		{
			this.TroopType = troop;
			this.BoardPosition = position;
			this.SpawnMode = mode;
			this.Amount = amount;
		}
	}
}
