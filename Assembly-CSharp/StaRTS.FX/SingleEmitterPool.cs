using StaRTS.Utils.Pooling;
using System;
using UnityEngine;

namespace StaRTS.FX
{
	public class SingleEmitterPool : EmitterPool
	{
		private ParticleSystem emitterProto;

		private ParticleSystemPool emitterPool;

		public SingleEmitterPool(ParticleSystem emitter, EmitterReturnedToPool emitterReturnedToPool) : base(emitterReturnedToPool)
		{
			this.emitterProto = emitter;
			this.emitterPool = new ParticleSystemPool(new ParticleSystemPool.CreatePoolObjectDelegate(this.CreateEmitterPoolObject), new ParticleSystemPool.DestroyPoolObjectDelegate(this.DestroyEmitterPoolObject), new ParticleSystemPool.ActivatePoolObjectDelegate(this.ActivateEmitterPoolObject), new ParticleSystemPool.DeactivatePoolObjectDelegate(this.DeactivateEmitterPoolObject));
		}

		private ParticleSystem CreateEmitterPoolObject(ParticleSystemPool objectPool)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.emitterProto.gameObject);
			ParticleSystem component = gameObject.GetComponent<ParticleSystem>();
			this.ActivateEmitterPoolObject(component);
			return component;
		}

		private void DestroyEmitterPoolObject(ParticleSystem emitter)
		{
			UnityEngine.Object.Destroy(emitter.gameObject);
		}

		private void DeactivateEmitterPoolObject(ParticleSystem emitter)
		{
			emitter.gameObject.SetActive(false);
		}

		private void ActivateEmitterPoolObject(ParticleSystem emitter)
		{
			emitter.gameObject.SetActive(true);
		}

		public ParticleSystem GetEmitter()
		{
			return this.emitterPool.GetFromPool();
		}

		public void StopEmissionAndReturnToPool(ParticleSystem emitter, float delayPreEmitterStop, float delayPostEmitterStop)
		{
			if (emitter != null)
			{
				base.StopEmitterAndReturnToPool(emitter, delayPreEmitterStop, new EmitterStopDelegate(this.StopEmitter), delayPostEmitterStop, new EmitterStopDelegate(this.PostEmitterStop));
			}
		}

		private void PostEmitterStop(object cookie)
		{
			ParticleSystem particleSystem = (ParticleSystem)cookie;
			if (particleSystem != null)
			{
				this.emitterPool.ReturnToPool(particleSystem);
			}
		}

		private void StopEmitter(object cookie)
		{
			ParticleSystem particleSystem = (ParticleSystem)cookie;
			if (particleSystem != null && !particleSystem.isStopped)
			{
				particleSystem.Stop(false);
			}
		}

		public override void Destroy()
		{
			base.Destroy();
			this.emitterProto = null;
			this.emitterPool.Destroy();
			this.emitterPool = null;
		}
	}
}
