using System;

namespace StaRTS.Utils.Animation.Anims
{
	public class Anim
	{
		private Action<float> optimizedUpdate;

		private Action<Anim, float> onUpdateCallback;

		public float Age
		{
			get;
			set;
		}

		public float Delay
		{
			get;
			set;
		}

		public uint DelayTimer
		{
			get;
			set;
		}

		public float Duration
		{
			get;
			set;
		}

		public object Tag
		{
			get;
			set;
		}

		public bool Playing
		{
			get;
			protected set;
		}

		public Action<Anim> OnCompleteCallback
		{
			get;
			set;
		}

		public Action<Anim> OnBeginCallback
		{
			get;
			set;
		}

		public Action<Anim, float> OnUpdateCallback
		{
			get
			{
				return this.onUpdateCallback;
			}
			set
			{
				this.onUpdateCallback = value;
				if (value == null)
				{
					this.optimizedUpdate = new Action<float>(this.UpdateWithoutCallback);
				}
				else
				{
					this.optimizedUpdate = new Action<float>(this.UpdateWithCallback);
				}
			}
		}

		public virtual Easing.EasingDelegate EaseFunction
		{
			get;
			set;
		}

		public Anim()
		{
			this.EaseFunction = new Easing.EasingDelegate(Easing.Linear);
			this.optimizedUpdate = new Action<float>(this.UpdateWithoutCallback);
		}

		public virtual void OnBegin()
		{
		}

		public virtual void OnUpdate(float dt)
		{
		}

		public virtual void OnComplete()
		{
		}

		public void Begin()
		{
			this.Playing = true;
			this.OnBegin();
			if (this.OnBeginCallback != null)
			{
				this.OnBeginCallback(this);
			}
		}

		public void Update(float dt)
		{
			this.optimizedUpdate(dt);
		}

		private void UpdateWithCallback(float dt)
		{
			this.OnUpdate(dt);
			this.OnUpdateCallback(this, dt);
		}

		private void UpdateWithoutCallback(float dt)
		{
			this.OnUpdate(dt);
		}

		public void Complete()
		{
			this.Age = 0f;
			this.Playing = false;
			this.OnComplete();
			if (this.OnCompleteCallback != null)
			{
				this.OnCompleteCallback(this);
			}
		}

		public void Tick(float dt)
		{
			this.Age += dt;
		}

		public void Cleanup()
		{
			this.Tag = null;
			this.OnCompleteCallback = null;
			this.OnBeginCallback = null;
			this.optimizedUpdate = null;
			this.onUpdateCallback = null;
			this.OnUpdateCallback = null;
		}
	}
}
