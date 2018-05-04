using Net.RichardLord.Ash.Core;
using StaRTS.Assets;
using StaRTS.Main.Utils.Events;
using StaRTS.Main.Views.Entities;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace StaRTS.Main.Views.World
{
	public class ShuttleAnim : IViewFrameTimeObserver
	{
		private const float FADE_FACTOR = 15f;

		private const float SCALE_FACTOR = 50f;

		private const float EFFECT_DELAY = 0.2f;

		private const float SHADOW_POS_Y = 1.1f;

		private const float SHADOW_MIN_ALPHA = 0.1f;

		private const float CONTRABAND_EFFECT_DELAY = 3f;

		private const float OUTLINE_WIDTH = 0.00125f;

		private const float OUTLINE_DURATION = 0.3f;

		public AssetHandle MainHandle;

		public AssetHandle LandingHandle;

		public AssetHandle TakeoffHandle;

		private ShuttleState state;

		private Queue<ShuttleState> states;

		private uint timerId;

		private OutlinedAsset shuttleOutline;

		private List<uint> outlineTimerIds;

		public Entity Starport
		{
			get;
			private set;
		}

		public GameObject GameObj
		{
			get;
			set;
		}

		public Animation Anim
		{
			get;
			set;
		}

		public GameObject CenterOfMass
		{
			get;
			set;
		}

		public GameObject ShadowGameObject
		{
			get;
			set;
		}

		public Material ShadowMaterial
		{
			get;
			set;
		}

		public GameObject LandingEffect
		{
			get;
			set;
		}

		public GameObject TakeOffEffect
		{
			get;
			set;
		}

		public float Percentage
		{
			get;
			set;
		}

		public bool IsContrabandShuttle
		{
			get;
			set;
		}

		public bool IsArmoryShuttle
		{
			get;
			set;
		}

		public ShuttleState State
		{
			get
			{
				return this.state;
			}
		}

		public ShuttleAnim(Entity starport)
		{
			this.Starport = starport;
			this.state = ShuttleState.None;
			this.states = new Queue<ShuttleState>();
			this.timerId = 0u;
		}

		public void EnqueueState(ShuttleState newState)
		{
			this.states.Enqueue(newState);
			if (this.timerId == 0u)
			{
				this.DequeueState();
			}
		}

		private void DequeueState()
		{
			if (this.Anim == null)
			{
				this.Stop();
				return;
			}
			if (this.states.Count != 0)
			{
				this.state = this.states.Dequeue();
				if (this.state != ShuttleState.None)
				{
					if (this.state == ShuttleState.Landing)
					{
						Service.ViewTimerManager.CreateViewTimer((!this.IsContrabandShuttle) ? 0.2f : 3f, false, new TimerDelegate(this.CallbackPlayParticle), this.LandingEffect);
					}
					else if (this.state == ShuttleState.LiftOff)
					{
						this.PlayParticle(this.TakeOffEffect);
					}
					this.Anim.Play(this.state.ToString());
					Service.EventManager.SendEvent(EventId.ShuttleAnimStateChanged, this);
					float num = 0f;
					if (this.state != ShuttleState.Idle)
					{
						IEnumerator enumerator = this.Anim.GetEnumerator();
						try
						{
							if (enumerator.MoveNext())
							{
								AnimationState animationState = (AnimationState)enumerator.Current;
								num = animationState.length;
							}
						}
						finally
						{
							IDisposable disposable = enumerator as IDisposable;
							if (disposable != null)
							{
								disposable.Dispose();
							}
						}
					}
					if (num > 0f)
					{
						Service.ViewTimeEngine.RegisterFrameTimeObserver(this);
						this.timerId = Service.ViewTimerManager.CreateViewTimer(num, false, new TimerDelegate(this.OnAnimationFinished), null);
					}
					else
					{
						this.DequeueState();
					}
				}
			}
		}

		private void CallbackPlayParticle(uint id, object cookie)
		{
			GameObject effectObject = cookie as GameObject;
			this.PlayParticle(effectObject);
		}

		private void PlayParticle(GameObject effectObject)
		{
			if (effectObject != null)
			{
				ParticleSystem component = effectObject.GetComponent<ParticleSystem>();
				if (component != null)
				{
					component.Play();
				}
			}
		}

		private void OnAnimationFinished(uint id, object cookie)
		{
			this.timerId = 0u;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
			this.DequeueState();
		}

		public void OnViewFrameTime(float dt)
		{
			if (this.CenterOfMass != null)
			{
				Transform transform = this.CenterOfMass.transform;
				Vector3 position = new Vector3(transform.position.x, 1.1f, transform.position.z);
				Quaternion rotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
				if (this.ShadowGameObject != null && this.ShadowMaterial != null)
				{
					float a = Mathf.Clamp(1f - transform.position.y / 15f, 0.1f, 1f);
					float num = 1f + (transform.position.y - 1f) / 50f;
					Transform transform2 = this.ShadowGameObject.transform;
					transform2.position = position;
					transform2.rotation = rotation;
					transform2.localScale = new Vector3(num, 1f, num);
					this.ShadowMaterial.color = new Color(0f, 0f, 0f, a);
				}
				if (this.LandingEffect != null)
				{
					Transform transform3 = this.LandingEffect.transform;
					transform3.position = position;
					transform3.rotation = rotation;
				}
				if (this.TakeOffEffect != null)
				{
					Transform transform4 = this.TakeOffEffect.transform;
					transform4.position = position;
					transform4.rotation = rotation;
				}
			}
		}

		public void Stop()
		{
			if (this.timerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timerId);
				this.timerId = 0u;
			}
			if (this.Anim != null)
			{
				this.Anim.Stop();
			}
			this.states.Clear();
			this.state = ShuttleState.None;
			Service.ViewTimeEngine.UnregisterFrameTimeObserver(this);
		}

		public void PulseOutline()
		{
			if (this.shuttleOutline == null)
			{
				this.shuttleOutline = new OutlinedAsset();
				this.shuttleOutline.Init(this.GameObj);
				this.shuttleOutline.SetOutlineWidth(0.00125f);
			}
			uint item = Service.ViewTimerManager.CreateViewTimer(0.3f, false, new TimerDelegate(this.OnOutlineTimer), null);
			if (this.outlineTimerIds == null)
			{
				this.outlineTimerIds = new List<uint>();
			}
			this.outlineTimerIds.Add(item);
		}

		private void OnOutlineTimer(uint timerId, object cookie)
		{
			this.outlineTimerIds.Remove(timerId);
			if (this.outlineTimerIds.Count == 0)
			{
				this.shuttleOutline.RemoveOutline();
				this.shuttleOutline = null;
			}
		}
	}
}
