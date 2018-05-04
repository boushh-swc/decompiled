using System;
using UnityEngine;

namespace StaRTS.Utils
{
	public class DampedSpring
	{
		public const float ZERO = 1E-08f;

		private Vector3 anchor;

		private Vector3 position;

		private Vector3 velocity;

		private bool moving;

		private float mass;

		private float spring_k;

		private float viscosity;

		private Vector3 force;

		private float speedLimit;

		public float SpeedLimit
		{
			get
			{
				return this.speedLimit;
			}
			set
			{
				this.speedLimit = value;
			}
		}

		public Vector3 Velocity
		{
			get
			{
				return this.velocity;
			}
			set
			{
			}
		}

		public Vector3 Anchor
		{
			get
			{
				return this.anchor;
			}
			set
			{
				this.anchor = value;
				this.moving = true;
			}
		}

		public Vector3 Position
		{
			get
			{
				return this.position;
			}
			set
			{
				this.position = value;
				this.moving = true;
			}
		}

		public DampedSpring(float m, float k)
		{
			this.anchor = Vector3.zero;
			this.position = Vector3.zero;
			this.velocity = Vector3.zero;
			this.moving = false;
			this.SetSpring(m, k);
			this.force = Vector3.zero;
			this.speedLimit = 1E+10f;
		}

		public void SetSpring(float m, float k)
		{
			this.mass = m;
			this.spring_k = k;
			this.viscosity = 2f * Mathf.Sqrt(this.mass * this.spring_k);
		}

		public void StopMoving()
		{
			this.anchor = this.position;
			this.velocity = Vector3.zero;
			this.moving = false;
		}

		public bool IsStillMoving()
		{
			return this.moving;
		}

		public void ApplyForce(Vector3 f)
		{
			this.force = f;
		}

		public void Move(float dtSeconds)
		{
			Vector3 vector = this.anchor - this.position;
			float magnitude = vector.magnitude;
			Vector3 b = Vector3.Normalize(vector) * (magnitude / this.mass) * this.spring_k;
			Vector3 a = -this.viscosity / this.mass * this.velocity;
			this.velocity += dtSeconds * (a + b + this.force);
			if (this.velocity.magnitude > this.speedLimit)
			{
				this.velocity = Vector3.Normalize(this.velocity) * this.speedLimit;
			}
			vector = this.velocity * dtSeconds;
			this.moving = (vector.sqrMagnitude > 1E-08f);
			if (this.moving)
			{
				this.position += vector;
			}
			else
			{
				this.position = this.anchor;
				this.velocity = Vector3.zero;
			}
		}
	}
}
