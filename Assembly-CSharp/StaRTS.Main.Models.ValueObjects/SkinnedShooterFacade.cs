using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public class SkinnedShooterFacade : IShooterVO
	{
		protected IShooterVO original;

		protected SkinOverrideTypeVO skinned;

		public int[] Preference
		{
			get;
			set;
		}

		public int PreferencePercentile
		{
			get;
			set;
		}

		public int NearnessPercentile
		{
			get;
			set;
		}

		public uint ViewRange
		{
			get
			{
				return this.skinned.ViewRange;
			}
		}

		public uint MinAttackRange
		{
			get
			{
				return this.skinned.MinAttackRange;
			}
		}

		public uint MaxAttackRange
		{
			get
			{
				return this.skinned.MaxAttackRange;
			}
		}

		public int Damage
		{
			get
			{
				return this.original.Damage;
			}
		}

		public int DPS
		{
			get
			{
				return this.original.DPS;
			}
		}

		public uint WarmupDelay
		{
			get
			{
				return this.skinned.WarmupDelay;
			}
		}

		public uint AnimationDelay
		{
			get
			{
				return this.skinned.AnimationDelay;
			}
		}

		public uint ShotDelay
		{
			get
			{
				return this.skinned.ShotDelay;
			}
		}

		public uint CooldownDelay
		{
			get
			{
				return this.skinned.CooldownDelay;
			}
		}

		public uint ShotCount
		{
			get
			{
				return this.skinned.ShotCount;
			}
		}

		public ProjectileTypeVO ProjectileType
		{
			get
			{
				return this.skinned.ProjectileType;
			}
		}

		public int[] GunSequence
		{
			get
			{
				return this.skinned.GunSequence;
			}
		}

		public Dictionary<int, int> Sequences
		{
			get
			{
				return this.skinned.Sequences;
			}
		}

		public bool OverWalls
		{
			get
			{
				return this.original.OverWalls;
			}
		}

		public uint RetargetingOffset
		{
			get
			{
				return this.original.RetargetingOffset;
			}
		}

		public bool ClipRetargeting
		{
			get
			{
				return this.original.ClipRetargeting;
			}
		}

		public bool NewTargetOnReload
		{
			get
			{
				return this.original.NewTargetOnReload;
			}
		}

		public bool StrictCooldown
		{
			get
			{
				return this.original.StrictCooldown;
			}
		}

		public TroopRole TroopRole
		{
			get
			{
				if (this.skinned.TroopRole == TroopRole.None)
				{
					return this.original.TroopRole;
				}
				return this.skinned.TroopRole;
			}
		}

		public bool CrushesWalls
		{
			get
			{
				return this.skinned.CrushesWalls || this.original.CrushesWalls;
			}
		}

		public bool IgnoresWalls
		{
			get
			{
				return this.skinned.IgnoresWalls;
			}
		}

		public SkinnedShooterFacade(IShooterVO original, SkinOverrideTypeVO skinned)
		{
			this.original = original;
			this.skinned = skinned;
			this.Preference = new int[original.Preference.Length];
			Array.Copy(original.Preference, this.Preference, original.Preference.Length);
			for (int i = 0; i < this.Preference.Length; i++)
			{
				if (skinned.Preference[i] > -1)
				{
					this.Preference[i] = skinned.Preference[i];
				}
			}
			this.PreferencePercentile = original.PreferencePercentile;
			this.NearnessPercentile = original.PreferencePercentile;
			if (skinned.PreferencePercentile > -1)
			{
				this.PreferencePercentile = skinned.PreferencePercentile;
				this.NearnessPercentile = 100 - this.PreferencePercentile;
			}
		}
	}
}
