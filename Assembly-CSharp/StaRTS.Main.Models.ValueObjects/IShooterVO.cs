using System;
using System.Collections.Generic;

namespace StaRTS.Main.Models.ValueObjects
{
	public interface IShooterVO
	{
		int[] Preference
		{
			get;
		}

		int PreferencePercentile
		{
			get;
		}

		int NearnessPercentile
		{
			get;
		}

		uint ViewRange
		{
			get;
		}

		uint MinAttackRange
		{
			get;
		}

		uint MaxAttackRange
		{
			get;
		}

		int Damage
		{
			get;
		}

		int DPS
		{
			get;
		}

		uint WarmupDelay
		{
			get;
		}

		uint AnimationDelay
		{
			get;
		}

		uint ShotDelay
		{
			get;
		}

		uint CooldownDelay
		{
			get;
		}

		uint ShotCount
		{
			get;
		}

		ProjectileTypeVO ProjectileType
		{
			get;
		}

		int[] GunSequence
		{
			get;
		}

		Dictionary<int, int> Sequences
		{
			get;
		}

		bool OverWalls
		{
			get;
		}

		uint RetargetingOffset
		{
			get;
		}

		bool ClipRetargeting
		{
			get;
		}

		bool NewTargetOnReload
		{
			get;
		}

		bool StrictCooldown
		{
			get;
		}

		TroopRole TroopRole
		{
			get;
		}

		bool CrushesWalls
		{
			get;
		}

		bool IgnoresWalls
		{
			get;
		}
	}
}
