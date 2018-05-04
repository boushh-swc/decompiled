using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using System;

namespace StaRTS.Main.Models.Player.Store
{
	public class Inventory : InventoryStorage
	{
		private const string PLAYER_INVENTORY = "playerInventory";

		public InventoryStorage Troop
		{
			get;
			set;
		}

		public InventoryStorage SpecialAttack
		{
			get;
			set;
		}

		public InventoryStorage Hero
		{
			get;
			set;
		}

		public InventoryStorage Champion
		{
			get;
			set;
		}

		public Inventory() : base("playerInventory", EventId.InventoryResourceUpdated, null)
		{
			this.Troop = base.CreateSubstorage("troop", EventId.InventoryTroopUpdated, typeof(TroopTypeVO));
			this.SpecialAttack = base.CreateSubstorage("specialAttack", EventId.InventorySpecialAttackUpdated, typeof(SpecialAttackTypeVO));
			this.Hero = base.CreateSubstorage("hero", EventId.InventoryHeroUpdated, typeof(TroopTypeVO));
			this.Champion = base.CreateSubstorage("champion", EventId.InventoryChampionUpdated, typeof(TroopTypeVO));
		}
	}
}
