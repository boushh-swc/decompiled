using Net.RichardLord.Ash.Core;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Utils.Core;
using System;

namespace StaRTS.Main.Models.Entities.Components
{
	public class TrackingComponent : ComponentBase
	{
		public const float DEFAULT_VELOCITY = 0.25f;

		public const float IDLE_VELOCITY = 0.75f;

		public const float MAX_PITCH_ANGLE = 0.2617994f;

		private Entity targetEntity;

		public float MaxVelocity
		{
			get;
			set;
		}

		public TransformComponent TargetTransform
		{
			get;
			set;
		}

		public int TargetX
		{
			get;
			set;
		}

		public int TargetZ
		{
			get;
			set;
		}

		public float TargetYaw
		{
			get;
			set;
		}

		public float TargetPitch
		{
			get;
			set;
		}

		public float Yaw
		{
			get;
			set;
		}

		public float Pitch
		{
			get;
			set;
		}

		public bool TrackPitch
		{
			get;
			private set;
		}

		public TrackingMode Mode
		{
			get;
			set;
		}

		public float IdleFastForwardTimeRemaining
		{
			get;
			set;
		}

		public Entity TargetEntity
		{
			get
			{
				return this.targetEntity;
			}
			set
			{
				this.targetEntity = value;
				if (this.targetEntity == null)
				{
					this.TargetTransform = null;
				}
				else
				{
					this.TargetTransform = this.targetEntity.Get<TransformComponent>();
				}
			}
		}

		public TrackingComponent(TurretTypeVO turrentType, bool trackPitch)
		{
			this.MaxVelocity = 0.25f;
			float num = (float)Service.Rand.SimRange(0, 360) * 3.14159274f / 180f;
			this.TargetEntity = null;
			this.TargetTransform = null;
			this.TargetX = 0;
			this.TargetZ = 0;
			this.TargetYaw = num;
			this.Mode = TrackingMode.Disabled;
			this.Yaw = num;
			this.TrackPitch = trackPitch;
			if (trackPitch)
			{
				this.TargetPitch = 0f;
				this.Pitch = 0f;
			}
		}
	}
}
