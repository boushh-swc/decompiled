using StaRTS.Main.Models.Squads;
using StaRTS.Utils.Core;
using StaRTS.Utils.Scheduling;
using System;
using System.Collections.Generic;

namespace StaRTS.Main.Controllers.Squads
{
	public abstract class AbstractSquadServerAdapter
	{
		protected SquadController.SquadMsgsCallback callback;

		private uint timerId;

		protected float pollFrequency;

		protected List<SquadMsg> list;

		public bool Enabled
		{
			get;
			private set;
		}

		public AbstractSquadServerAdapter()
		{
			this.timerId = 0u;
			this.list = new List<SquadMsg>();
			this.Enabled = false;
		}

		public virtual void EnableAndPoll(SquadController.SquadMsgsCallback callback, float pollFrequency)
		{
			if (pollFrequency <= 0f)
			{
				return;
			}
			this.Enable(callback);
			if (this.Enabled)
			{
				this.pollFrequency = pollFrequency;
				this.Poll();
			}
		}

		public virtual void Enable(SquadController.SquadMsgsCallback callback)
		{
			if (this.Enabled || callback == null)
			{
				return;
			}
			this.callback = callback;
			this.Enabled = true;
		}

		public void AdjustPollFrequency(float pollFrequency)
		{
			if (pollFrequency > 0f)
			{
				this.pollFrequency = pollFrequency;
				this.ResetPollTimer();
			}
		}

		protected void OnPollFinished(object response)
		{
			this.list.Clear();
			this.PopulateSquadMsgsReceived(response);
			if (this.callback != null)
			{
				this.callback(this.list);
			}
			if (this.timerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timerId);
				this.timerId = 0u;
			}
			this.timerId = Service.ViewTimerManager.CreateViewTimer(this.pollFrequency, false, new TimerDelegate(this.OnPollTimer), null);
		}

		private void OnPollTimer(uint timerId, object cookie)
		{
			this.timerId = 0u;
			this.Poll();
		}

		public void ResetPollTimer()
		{
			if (this.timerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timerId);
				this.timerId = Service.ViewTimerManager.CreateViewTimer(this.pollFrequency, false, new TimerDelegate(this.OnPollTimer), null);
			}
		}

		public void DisableTimer()
		{
			if (this.timerId != 0u)
			{
				Service.ViewTimerManager.KillViewTimer(this.timerId);
				this.timerId = 0u;
			}
			this.Enabled = false;
		}

		public virtual void Disable()
		{
			this.callback = null;
			this.list.Clear();
			this.DisableTimer();
		}

		protected abstract void Poll();

		protected abstract void PopulateSquadMsgsReceived(object response);
	}
}
