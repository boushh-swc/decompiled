using StaRTS.Utils.Pooling;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class MultipleEmittersPool : EmitterPool
	{
		public static int ID;

		private GameObject emitterRootProto;

		private GameObjectPool emitterRootPool;

		public MultipleEmittersPool(GameObject emitterRootNode, EmitterReturnedToPool emitterReturnedToPool) : base(emitterReturnedToPool)
		{
			this.emitterRootProto = emitterRootNode;
			this.emitterRootPool = new GameObjectPool(new GameObjectPool.CreatePoolObjectDelegate(this.CreateEmitterRootPoolObject), new GameObjectPool.DestroyPoolObjectDelegate(this.DestroyEmitterRootPoolObject), new GameObjectPool.ActivatePoolObjectDelegate(this.ActivateEmitterRootPoolObject), new GameObjectPool.DeactivatePoolObjectDelegate(this.DeactivateEmitterRootPoolObject));
		}

		public static void StaticReset()
		{
			MultipleEmittersPool.ID = 0;
		}

		private GameObject CreateEmitterRootPoolObject(GameObjectPool objectPool)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.emitterRootProto);
			gameObject.name = gameObject.name + "_" + MultipleEmittersPool.ID++;
			MultipleEmittersPool.StopChildEmitters(gameObject);
			this.ActivateEmitterRootPoolObject(gameObject);
			return gameObject;
		}

		private void DestroyEmitterRootPoolObject(GameObject emitterRootNode)
		{
			UnityEngine.Object.Destroy(emitterRootNode);
		}

		private void DeactivateEmitterRootPoolObject(GameObject emitterRootNode)
		{
			emitterRootNode.SetActive(false);
		}

		private void ActivateEmitterRootPoolObject(GameObject emitterRootNode)
		{
		}

		public GameObject GetEmitterRoot()
		{
			return this.emitterRootPool.GetFromPool();
		}

		public static ParticleSystem[] GetAllEmitters(GameObject rootNode)
		{
			return rootNode.GetComponentsInChildren<ParticleSystem>(true);
		}

		public static void StopChildEmitters(GameObject rootNode)
		{
			ParticleSystem[] allEmitters = MultipleEmittersPool.GetAllEmitters(rootNode);
			ParticleSystem[] array = allEmitters;
			for (int i = 0; i < array.Length; i++)
			{
				ParticleSystem particleSystem = array[i];
				if (!particleSystem.isStopped)
				{
					particleSystem.Stop(false);
				}
			}
		}

		public void StopEmissionAndReturnToPool(GameObject emitterRootNode, float delayPreEmitterStop, float delayPostEmitterStop)
		{
			if (emitterRootNode != null)
			{
				base.StopEmitterAndReturnToPool(emitterRootNode, delayPreEmitterStop, new EmitterStopDelegate(this.StopEmitter), delayPostEmitterStop, new EmitterStopDelegate(this.PostEmitterStop));
			}
		}

		private void PostEmitterStop(object cookie)
		{
			GameObject gameObject = (GameObject)cookie;
			if (gameObject != null)
			{
				this.emitterRootPool.ReturnToPool(gameObject);
			}
		}

		private void StopEmitter(object cookie)
		{
			GameObject gameObject = (GameObject)cookie;
			if (gameObject != null)
			{
				MultipleEmittersPool.StopChildEmitters(gameObject);
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			this.emitterRootProto = null;
			this.emitterRootPool.Destroy();
			this.emitterRootPool = null;
		}
	}
}
