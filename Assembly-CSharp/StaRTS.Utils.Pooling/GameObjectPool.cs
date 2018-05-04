using System;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Utils.Pooling
{
	public class GameObjectPool
	{
		public delegate GameObject CreatePoolObjectDelegate(GameObjectPool objectPool);

		public delegate void DestroyPoolObjectDelegate(GameObject instance);

		public delegate void ActivatePoolObjectDelegate(GameObject instance);

		public delegate void DeactivatePoolObjectDelegate(GameObject instance);

		private Queue<GameObject> pool;

		private GameObjectPool.CreatePoolObjectDelegate createPoolObjectDelegate;

		private GameObjectPool.DestroyPoolObjectDelegate destroyPoolObjectDelegate;

		private GameObjectPool.DeactivatePoolObjectDelegate deactivatePoolObjectDelegate;

		private GameObjectPool.ActivatePoolObjectDelegate activatePoolObjectDelegate;

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

		public GameObjectPool(GameObjectPool.CreatePoolObjectDelegate createPoolObjectDelegate, GameObjectPool.DestroyPoolObjectDelegate destroyPoolObjectDelegate, GameObjectPool.ActivatePoolObjectDelegate activatePoolObjectDelegate, GameObjectPool.DeactivatePoolObjectDelegate deactivatePoolObjectDelegate)
		{
			this.pool = new Queue<GameObject>();
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

		protected virtual GameObject CreateNew()
		{
			this.Capacity++;
			return this.createPoolObjectDelegate(this);
		}

		public virtual void ReturnToPool(GameObject obj)
		{
			if (this.deactivatePoolObjectDelegate != null)
			{
				this.deactivatePoolObjectDelegate(obj);
			}
			this.pool.Enqueue(obj);
		}

		public virtual GameObject GetFromPool(bool allowBeyondCapacity)
		{
			if (this.pool.Count != 0)
			{
				GameObject gameObject = this.pool.Dequeue();
				if (this.activatePoolObjectDelegate != null)
				{
					this.activatePoolObjectDelegate(gameObject);
				}
				return gameObject;
			}
			if (allowBeyondCapacity)
			{
				return this.CreateNew();
			}
			throw new Exception("No more pool objects available");
		}

		public virtual GameObject GetFromPool()
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
