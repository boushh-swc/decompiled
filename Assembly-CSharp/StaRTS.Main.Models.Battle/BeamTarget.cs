using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Models.Battle
{
	public class BeamTarget
	{
		private int maxDamagePercent;

		public SmartEntity Target
		{
			get;
			private set;
		}

		public bool IsFirstHit
		{
			get;
			private set;
		}

		public bool HitThisSegment
		{
			get;
			private set;
		}

		public int TotalHitCount
		{
			get;
			private set;
		}

		public int CurDamagePercent
		{
			get;
			private set;
		}

		public BeamTarget(SmartEntity target)
		{
			this.Target = target;
			this.IsFirstHit = true;
			this.HitThisSegment = false;
			this.TotalHitCount = 0;
			this.CurDamagePercent = 0;
			this.maxDamagePercent = 0;
		}

		public void ApplyBeamDamage(int damagePercent)
		{
			if (damagePercent > this.maxDamagePercent)
			{
				this.CurDamagePercent += damagePercent - this.maxDamagePercent;
				this.maxDamagePercent = damagePercent;
			}
			this.HitThisSegment = true;
			this.TotalHitCount++;
		}

		public void OnBeamAdvance()
		{
			this.IsFirstHit = false;
			this.HitThisSegment = false;
			this.CurDamagePercent = 0;
		}
	}
}
