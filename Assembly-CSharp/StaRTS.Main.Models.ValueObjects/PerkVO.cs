using StaRTS.Utils.MetaData;
using System;

namespace StaRTS.Main.Models.ValueObjects
{
	public class PerkVO : IValueObject
	{
		public static int COLUMN_perkGroup
		{
			get;
			private set;
		}

		public static int COLUMN_perkTier
		{
			get;
			private set;
		}

		public static int COLUMN_squadLevelUnlock
		{
			get;
			private set;
		}

		public static int COLUMN_repCost
		{
			get;
			private set;
		}

		public static int COLUMN_activationCost
		{
			get;
			private set;
		}

		public static int COLUMN_perkEffects
		{
			get;
			private set;
		}

		public static int COLUMN_activeDuration
		{
			get;
			private set;
		}

		public static int COLUMN_cooldownDuration
		{
			get;
			private set;
		}

		public static int COLUMN_sortOrder
		{
			get;
			private set;
		}

		public static int COLUMN_sortTabs
		{
			get;
			private set;
		}

		public static int COLUMN_textureIdRebel
		{
			get;
			private set;
		}

		public static int COLUMN_textureIdEmpire
		{
			get;
			private set;
		}

		public string Uid
		{
			get;
			set;
		}

		public string PerkGroup
		{
			get;
			private set;
		}

		public int PerkTier
		{
			get;
			private set;
		}

		public int SquadLevelUnlock
		{
			get;
			private set;
		}

		public int ReputationCost
		{
			get;
			private set;
		}

		public string[] ActivationCost
		{
			get;
			private set;
		}

		public string[] PerkEffects
		{
			get;
			private set;
		}

		public string[] FilterTabs
		{
			get;
			private set;
		}

		public int ActiveDurationMinutes
		{
			get;
			private set;
		}

		public int CooldownDurationMinutes
		{
			get;
			private set;
		}

		public int SortOrder
		{
			get;
			private set;
		}

		public string TextureIdRebel
		{
			get;
			private set;
		}

		public string TextureIdEmpire
		{
			get;
			private set;
		}

		public void ReadRow(Row row)
		{
			this.Uid = row.Uid;
			this.PerkGroup = row.TryGetString(PerkVO.COLUMN_perkGroup);
			this.PerkTier = row.TryGetInt(PerkVO.COLUMN_perkTier);
			this.SquadLevelUnlock = row.TryGetInt(PerkVO.COLUMN_squadLevelUnlock);
			this.ReputationCost = row.TryGetInt(PerkVO.COLUMN_repCost);
			this.ActivationCost = row.TryGetStringArray(PerkVO.COLUMN_activationCost);
			this.FilterTabs = row.TryGetStringArray(PerkVO.COLUMN_sortTabs);
			this.PerkEffects = row.TryGetStringArray(PerkVO.COLUMN_perkEffects);
			this.ActiveDurationMinutes = row.TryGetInt(PerkVO.COLUMN_activeDuration);
			this.CooldownDurationMinutes = row.TryGetInt(PerkVO.COLUMN_cooldownDuration);
			this.SortOrder = row.TryGetInt(PerkVO.COLUMN_sortOrder);
			this.TextureIdRebel = row.TryGetString(PerkVO.COLUMN_textureIdRebel);
			this.TextureIdEmpire = row.TryGetString(PerkVO.COLUMN_textureIdEmpire);
		}
	}
}
