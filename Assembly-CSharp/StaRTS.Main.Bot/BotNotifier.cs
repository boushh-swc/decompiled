using System;

namespace StaRTS.Main.Bot
{
	public class BotNotifier : IBotNotifierParent
	{
		protected object arg;

		public BotPerformer SuccessPerformer;

		public BotPerformer FailurePerformer;

		public BotPerformer UpdatePerformer;

		public IBotNotifierParent Parent;

		public BotNotifier NextNotifier
		{
			get;
			set;
		}

		public virtual BotNotifier Init(object arg)
		{
			this.arg = arg;
			return this;
		}

		public bool IsUpdateNotifier()
		{
			return this.NextNotifier != null || this.UpdatePerformer != null;
		}

		public virtual void Start()
		{
			if (this.EvaluateUpdate())
			{
				if (this.SuccessPerformer != null)
				{
					this.SuccessPerformer.Perform();
				}
			}
			else if (this.FailurePerformer != null)
			{
				this.FailurePerformer.Perform();
			}
		}

		public void Update()
		{
			if (!this.IsUpdateNotifier())
			{
				this.Start();
				return;
			}
			if (this.EvaluateUpdate())
			{
				if (this.UpdatePerformer != null)
				{
					this.UpdatePerformer.Perform();
				}
			}
			else if (this.FailurePerformer != null)
			{
				this.FailurePerformer.Perform();
			}
			if (this.NextNotifier != null)
			{
				this.NextNotifier.Update();
			}
		}

		public virtual bool EvaluateUpdate()
		{
			return false;
		}

		public void AddNotifier(BotNotifier notifier)
		{
			if (this.NextNotifier == null)
			{
				this.NextNotifier = notifier;
				this.NextNotifier.Parent = this;
				return;
			}
			this.NextNotifier.AddNotifier(notifier);
		}
	}
}
