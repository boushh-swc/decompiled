using StaRTS.Main.Views.World.Targeting;
using System;
using System.Collections.Generic;

namespace StaRTS.Utils.Pooling
{
	public class TargetReticlePool
	{
		public delegate TargetReticle CreatePoolObjectDelegate(TargetReticlePool objectPool);

		public delegate void DestroyPoolObjectDelegate(TargetReticle instance);

		public delegate void ActivatePoolObjectDelegate(TargetReticle instance);

		public delegate void DeactivatePoolObjectDelegate(TargetReticle instance);

		private Queue<TargetReticle> pool;

		private TargetReticlePool.CreatePoolObjectDelegate createPoolObjectDelegate;

		private TargetReticlePool.DestroyPoolObjectDelegate destroyPoolObjectDelegate;

		private TargetReticlePool.DeactivatePoolObjectDelegate deactivatePoolObjectDelegate;

		private TargetReticlePool.ActivatePoolObjectDelegate activatePoolObjectDelegate;

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

		public TargetReticlePool(TargetReticlePool.CreatePoolObjectDelegate createPoolObjectDelegate, TargetReticlePool.DestroyPoolObjectDelegate destroyPoolObjectDelegate, TargetReticlePool.ActivatePoolObjectDelegate activatePoolObjectDelegate, TargetReticlePool.DeactivatePoolObjectDelegate deactivatePoolObjectDelegate)
		{
			this.pool = new Queue<TargetReticle>();
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

		protected virtual TargetReticle CreateNew()
		{
			this.Capacity++;
			return this.createPoolObjectDelegate(this);
		}

		public virtual void ReturnToPool(TargetReticle obj)
		{
			if (this.deactivatePoolObjectDelegate != null)
			{
				this.deactivatePoolObjectDelegate(obj);
			}
			this.pool.Enqueue(obj);
		}

		public virtual TargetReticle GetFromPool(bool allowBeyondCapacity)
		{
			if (this.pool.Count != 0)
			{
				TargetReticle targetReticle = this.pool.Dequeue();
				if (this.activatePoolObjectDelegate != null)
				{
					this.activatePoolObjectDelegate(targetReticle);
				}
				return targetReticle;
			}
			if (allowBeyondCapacity)
			{
				return this.CreateNew();
			}
			throw new Exception("No more pool objects available");
		}

		public virtual TargetReticle GetFromPool()
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
