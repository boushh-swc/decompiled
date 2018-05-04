using Game.Behaviors;
using StaRTS.FX;
using StaRTS.Main.Models.Entities;
using System;

namespace StaRTS.Main.Controllers
{
	public class DeployedTroop
	{
		public string Uid
		{
			get;
			private set;
		}

		public SmartEntity Entity
		{
			get;
			private set;
		}

		public uint AbilityTimer
		{
			get;
			set;
		}

		public uint CoolDownTimer
		{
			get;
			set;
		}

		public int AbilityClipCount
		{
			get;
			set;
		}

		public bool Activated
		{
			get;
			set;
		}

		public LightSaberHitEffect LightSaberHitFx
		{
			get;
			set;
		}

		public WeaponTrail WeaponTrail
		{
			get;
			set;
		}

		public float WeaponTrailActivateLifetime
		{
			get;
			set;
		}

		public float WeaponTrailDeactivateLifetime
		{
			get;
			set;
		}

		public bool EffectsSetup
		{
			get;
			set;
		}

		public DeployedTroop(string uid, SmartEntity entity)
		{
			this.Uid = uid;
			this.Entity = entity;
			this.AbilityTimer = 0u;
			this.CoolDownTimer = 0u;
			this.AbilityClipCount = 0;
			this.Activated = false;
		}
	}
}
