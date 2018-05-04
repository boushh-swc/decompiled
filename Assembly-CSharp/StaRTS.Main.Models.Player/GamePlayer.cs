using StaRTS.Main.Models.Player.Misc;
using StaRTS.Main.Models.Player.Store;
using StaRTS.Main.Models.Player.World;
using StaRTS.Main.Models.Squads;
using StaRTS.Main.Utils;
using System;

namespace StaRTS.Main.Models.Player
{
	public class GamePlayer
	{
		public Inventory Inventory
		{
			get;
			set;
		}

		public Map Map
		{
			get;
			set;
		}

		public UnlockedLevelData UnlockedLevels
		{
			get;
			set;
		}

		public Squad Squad
		{
			get;
			set;
		}

		public bool IsContrabandUnlocked
		{
			get;
			set;
		}

		public virtual string PlayerName
		{
			get;
			set;
		}

		public virtual int AttackRating
		{
			get;
			set;
		}

		public virtual int DefenseRating
		{
			get;
			set;
		}

		public virtual int AttacksWon
		{
			get;
			set;
		}

		public virtual int DefensesWon
		{
			get;
			set;
		}

		public virtual FactionType Faction
		{
			get;
			set;
		}

		public int PlayerMedals
		{
			get
			{
				return GameUtils.CalcuateMedals(this.AttackRating, this.DefenseRating);
			}
		}

		public int CurrentXPAmount
		{
			get
			{
				return this.Inventory.GetItemAmount("xp");
			}
		}

		public int CurrentCreditsAmount
		{
			get
			{
				return this.Inventory.GetItemAmount("credits");
			}
		}

		public int MaxCreditsAmount
		{
			get
			{
				return this.Inventory.GetItemCapacity("credits");
			}
		}

		public int CurrentMaterialsAmount
		{
			get
			{
				return this.Inventory.GetItemAmount("materials");
			}
		}

		public int MaxMaterialsAmount
		{
			get
			{
				return this.Inventory.GetItemCapacity("materials");
			}
		}

		public int CurrentContrabandAmount
		{
			get
			{
				return this.Inventory.GetItemAmount("contraband");
			}
		}

		public int MaxContrabandAmount
		{
			get
			{
				return this.Inventory.GetItemCapacity("contraband");
			}
		}

		public int CurrentReputationAmount
		{
			get
			{
				return this.Inventory.GetItemAmount("reputation");
			}
		}

		public int MaxReputationAmount
		{
			get
			{
				return this.Inventory.GetItemCapacity("reputation");
			}
		}
	}
}
