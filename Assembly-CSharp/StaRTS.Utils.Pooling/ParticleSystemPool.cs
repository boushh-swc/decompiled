using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Utils.Pooling
{
	public class ParticleSystemPool
	{
		public delegate ParticleSystem CreatePoolObjectDelegate(ParticleSystemPool objectPool);

		public delegate void DestroyPoolObjectDelegate(ParticleSystem instance);

		public delegate void ActivatePoolObjectDelegate(ParticleSystem instance);

		public delegate void DeactivatePoolObjectDelegate(ParticleSystem instance);

		private Queue<ParticleSystem> pool;

		private ParticleSystemPool.CreatePoolObjectDelegate createPoolObjectDelegate;

		private ParticleSystemPool.DestroyPoolObjectDelegate destroyPoolObjectDelegate;

		private ParticleSystemPool.DeactivatePoolObjectDelegate deactivatePoolObjectDelegate;

		private ParticleSystemPool.ActivatePoolObjectDelegate activatePoolObjectDelegate;

		public int Count
		{
			get
			{
				return this.pool.Count;
			}
		}

		public int Capacity
		{
			get;
			protected set;
		}

		public ParticleSystemPool(ParticleSystemPool.CreatePoolObjectDelegate createPoolObjectDelegate, ParticleSystemPool.DestroyPoolObjectDelegate destroyPoolObjectDelegate, ParticleSystemPool.ActivatePoolObjectDelegate activatePoolObjectDelegate, ParticleSystemPool.DeactivatePoolObjectDelegate deactivatePoolObjectDelegate)
		{
			this.pool = new Queue<ParticleSystem>();
			this.Capacity = 0;
			this.createPoolObjectDelegate = createPoolObjectDelegate;
			this.destroyPoolObjectDelegate = destroyPoolObjectDelegate;
			this.activatePoolObjectDelegate = activatePoolObjectDelegate;
			this.deactivatePoolObjectDelegate = deactivatePoolObjectDelegate;
		}

		public void EnsurePoolCapacity(int n)
		{
			for (int i = this.Capacity; i < n; i++)
			{
				this.ReturnToPool(this.CreateNew());
			}
		}

		protected virtual ParticleSystem CreateNew()
		{
			this.Capacity++;
			return this.createPoolObjectDelegate(this);
		}

		public virtual void ReturnToPool(ParticleSystem obj)
		{
			if (this.deactivatePoolObjectDelegate != null)
			{
				this.deactivatePoolObjectDelegate(obj);
			}
			this.pool.Enqueue(obj);
		}

		public virtual ParticleSystem GetFromPool(bool allowBeyondCapacity)
		{
			if (this.pool.Count != 0)
			{
				ParticleSystem particleSystem = this.pool.Dequeue();
				if (this.activatePoolObjectDelegate != null)
				{
					this.activatePoolObjectDelegate(particleSystem);
				}
				return particleSystem;
			}
			if (allowBeyondCapacity)
			{
				return this.CreateNew();
			}
			throw new Exception("No more pool objects available");
		}

		public virtual ParticleSystem GetFromPool()
		{
			return this.GetFromPool(true);
		}

		public void ClearOutPool()
		{
			if (this.destroyPoolObjectDelegate != null)
			{
				while (this.pool.Count > 0)
				{
					this.destroyPoolObjectDelegate(this.pool.Dequeue());
				}
			}
		}

		public void Destroy()
		{
			this.ClearOutPool();
			this.pool = null;
			this.createPoolObjectDelegate = null;
			this.destroyPoolObjectDelegate = null;
			this.deactivatePoolObjectDelegate = null;
			this.activatePoolObjectDelegate = null;
		}
	}
}
