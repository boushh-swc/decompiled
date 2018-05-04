using Game.Behaviors;
using StaRTS.FX;
using StaRTS.Main.Models.Battle;
using StaRTS.Main.Models.ValueObjects;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities.Projectiles;
using StaRTS.Utils;
using StaRTS.Utils.Core;
using System;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class ProjectileView
	{
		private const string GAMEOBJECT_NAME = "Projectile";

		private bool isMultipleEmitter;

		private GameObject root;

		private GameObject DefaultTrackerObject;

		private TrailRenderer trailToClear;

		public ProjectileTypeVO ProjectileType
		{
			get;
			private set;
		}

		public Bullet Bullet
		{
			get;
			private set;
		}

		public bool IsOnGround
		{
			get;
			private set;
		}

		public Vector3 StartLocation
		{
			get;
			private set;
		}

		public float LifetimeSeconds
		{
			get;
			set;
		}

		public Transform EmitterTracker
		{
			get;
			private set;
		}

		public Transform MeshTracker
		{
			get;
			private set;
		}

		public Transform TargetTracker
		{
			get;
			private set;
		}

		public Transform DefaultTracker
		{
			get;
			private set;
		}

		public ParticleSystem Emitter
		{
			get;
			private set;
		}

		public Vector3 TargetLocation
		{
			get;
			private set;
		}

		public ProjectileViewPath ViewPath
		{
			get;
			private set;
		}

		public ProjectileView()
		{
			this.OnReturnToPool();
		}

		public void Init(ProjectileTypeVO projectileType, Bullet bullet, bool isOnGround)
		{
			this.ProjectileType = projectileType;
			this.Bullet = bullet;
			this.IsOnGround = isOnGround;
		}

		public void InitWithoutBullet(float lifeSeconds, Vector3 start, Vector3 target, Transform targetTracker)
		{
			this.DefaultTrackerObject = new GameObject();
			this.DefaultTracker = this.DefaultTrackerObject.transform;
			this.DefaultTracker.position = start;
			this.InternalInit(lifeSeconds, start, target, targetTracker);
		}

		public void InitWithMesh(float lifeSeconds, Vector3 start, Vector3 target, GameObject mesh, Transform targetTracker)
		{
			this.InternalInit(lifeSeconds, start, target, targetTracker);
			this.InitMeshTracker(mesh);
		}

		public void InitWithEmitter(float lifeSeconds, Vector3 start, Vector3 target, ParticleSystem emitter, Transform targetTracker)
		{
			this.InternalInit(lifeSeconds, start, target, targetTracker);
			this.InitEmitterTracker(emitter);
		}

		public void InitWithEmitters(float lifeSeconds, Vector3 start, Vector3 target, GameObject rootNode, GameObject mesh, Transform targetTracker)
		{
			this.InternalInit(lifeSeconds, start, target, targetTracker);
			if (mesh != null)
			{
				this.InitMeshTracker(mesh);
			}
			this.InitMultipleEmitterTracker(rootNode);
		}

		public void InitWithMeshAndEmitter(float lifeSeconds, Vector3 start, Vector3 target, GameObject mesh, ParticleSystem emitter, Transform targetTracker)
		{
			this.InternalInit(lifeSeconds, start, target, targetTracker);
			this.InitMeshTracker(mesh);
			this.InitEmitterTracker(emitter);
		}

		public void SetTargetLocation(Vector3 target)
		{
			this.TargetLocation = target;
		}

		public Transform GetTransform()
		{
			Transform result = null;
			if (this.root != null)
			{
				result = this.root.transform;
			}
			else if (this.MeshTracker != null)
			{
				result = this.MeshTracker;
			}
			else if (this.EmitterTracker != null)
			{
				result = this.EmitterTracker;
			}
			else if (this.DefaultTracker != null)
			{
				result = this.DefaultTracker;
			}
			return result;
		}

		private void InternalInit(float lifeSeconds, Vector3 start, Vector3 target, Transform targetTransform)
		{
			this.LifetimeSeconds = lifeSeconds;
			this.StartLocation = start;
			this.SetTargetLocation(target);
			this.TargetTracker = targetTransform;
			if (this.ProjectileType.IsMultiStage)
			{
				this.ViewPath = new ProjectileViewMultiStagePath(this);
			}
			else if (this.ProjectileType.Arcs)
			{
				this.ViewPath = new ProjectileViewArcPath(this);
			}
			else
			{
				this.ViewPath = new ProjectileViewLinearPath(this);
			}
		}

		private void InitMeshTracker(GameObject mesh)
		{
			this.MeshTracker = mesh.transform;
			this.MeshTracker.position = this.StartLocation;
			if (!this.ProjectileType.Arcs)
			{
				this.MeshTracker.LookAt(this.TargetLocation);
			}
			mesh.SetActive(true);
		}

		private void InitMultipleEmitterTracker(GameObject rootNode)
		{
			this.root = rootNode;
			this.EmitterTracker = rootNode.transform;
			this.EmitterTracker.position = this.StartLocation;
			this.EmitterTracker.LookAt(this.TargetLocation);
			this.isMultipleEmitter = true;
			this.root.SetActive(true);
			ParticleSystem[] allEmitters = MultipleEmittersPool.GetAllEmitters(this.root);
			ParticleSystem[] array = allEmitters;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem particle = array[i];
				this.PlayEmitter(particle);
				this.PlayTrails(particle);
			}
		}

		private void InitEmitterTracker(ParticleSystem emitter)
		{
			this.EmitterTracker = emitter.transform;
			this.EmitterTracker.position = this.StartLocation;
			this.EmitterTracker.LookAt(this.TargetLocation);
			this.Emitter = emitter;
			this.PlayEmitter(this.Emitter);
			this.Emitter.gameObject.SetActive(true);
			this.PlayTrails(this.Emitter);
		}

		private void PlayEmitter(ParticleSystem particle)
		{
			if (Mathf.Approximately(particle.emissionRate, 0f))
			{
				particle.startLifetime = this.LifetimeSeconds;
			}
			UnityUtils.PlayParticleEmitter(particle);
		}

		private void PlayTrails(ParticleSystem particle)
		{
			TrailRenderer component = particle.gameObject.GetComponent<TrailRenderer>();
			if (component != null)
			{
				component.enabled = true;
				component.time = -component.time;
				this.trailToClear = component;
			}
			WeaponTrail component2 = particle.gameObject.GetComponent<WeaponTrail>();
			if (component2 != null)
			{
				component2.Restart();
			}
		}

		private void ResetParticle(ParticleSystem particle)
		{
			if (particle != null)
			{
				particle.Stop(false);
				TrailRenderer component = particle.gameObject.GetComponent<TrailRenderer>();
				if (component != null)
				{
					component.enabled = false;
					if (component.time < 0f)
					{
						component.time = -component.time;
					}
				}
			}
		}

		public bool Update(float dt, out float distSquared)
		{
			this.ViewPath.AgeSeconds += dt;
			bool flag = this.ViewPath.AgeSeconds >= this.LifetimeSeconds;
			if (flag)
			{
				Service.EventManager.SendEvent(EventId.ProjectileViewPathComplete, this.Bullet);
				this.Reset();
				distSquared = 0f;
			}
			else
			{
				this.ViewPath.OnUpdate(dt);
				distSquared = (this.TargetLocation - this.ViewPath.CurrentLocation).sqrMagnitude;
				if (this.trailToClear != null && this.ViewPath.AgeSeconds > dt)
				{
					this.trailToClear.time = -this.trailToClear.time;
					this.trailToClear = null;
				}
			}
			return flag;
		}

		public void Reset()
		{
			if (this.DefaultTrackerObject != null)
			{
				this.DefaultTracker.DetachChildren();
				this.DefaultTracker = null;
				UnityEngine.Object.Destroy(this.DefaultTrackerObject);
				this.DefaultTrackerObject = null;
			}
			this.ResetParticle(this.Emitter);
			if (this.root != null)
			{
				ParticleSystem[] allEmitters = MultipleEmittersPool.GetAllEmitters(this.root);
				ParticleSystem[] array = allEmitters;
				for (int i = 0; i < array.Length; i++)
				{
					ParticleSystem particle = array[i];
					this.ResetParticle(particle);
				}
			}
			if (this.MeshTracker != null && this.MeshTracker.gameObject != null)
			{
				this.MeshTracker.gameObject.SetActive(false);
			}
			if (this.MeshTracker != null)
			{
				this.MeshTracker.DetachChildren();
			}
			if (this.EmitterTracker != null)
			{
				if (this.isMultipleEmitter)
				{
					this.root.SetActive(false);
				}
				else
				{
					this.EmitterTracker.DetachChildren();
				}
			}
			this.root = null;
		}

		public void OnReturnToPool()
		{
			this.Emitter = null;
			this.MeshTracker = null;
			this.EmitterTracker = null;
			this.DefaultTracker = null;
			this.trailToClear = null;
			this.Bullet = null;
			if (this.DefaultTrackerObject != null)
			{
				UnityEngine.Object.Destroy(this.DefaultTrackerObject);
				this.DefaultTrackerObject = null;
			}
			this.ViewPath = null;
		}
	}
}
